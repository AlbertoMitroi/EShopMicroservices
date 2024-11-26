using Discount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BuildingBlocks.Messaging.MassTransit;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Application Services
var assembly = typeof(Program).Assembly;
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

//Data Services
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, RedisCacheRepository>();

var redisConfig = builder.Configuration.GetSection("Redis"); 
var redisHost = redisConfig.GetValue<string>("RedisUrl"); 
var redisPort = redisConfig.GetValue<string>("Port"); 
var redisPassword = redisConfig.GetValue<string>("Password"); 
var redisSsl = redisConfig.GetValue<bool>("SSL");
var redisAbortOnConnectFail = redisConfig.GetValue<bool>("AbortOnConnectFail");

var configOptions = new ConfigurationOptions
{
    Password = redisPassword,
    Ssl = redisSsl,
    AbortOnConnectFail = redisAbortOnConnectFail,
    ConnectTimeout = 10000
};

configOptions.EndPoints.Add(redisHost!, int.Parse(redisPort!));

try
{
    var multiplexer = ConnectionMultiplexer.Connect(configOptions);
    builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
}
catch (Exception ex)
{
    Console.WriteLine($"Error connecting to Redis: {ex.Message}");
}

builder.Logging.AddFilter("StackExchange.Redis", LogLevel.Debug);

//Grpc Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return handler;
});

//Async Communication Services
builder.Services.AddMessageBroker(builder.Configuration);

//Cross-Cutting Services
builder.Services.AddSingleton<CustomExceptionHandler>(); // instead of: builder.Services.AddExceptionHandler<CustomExceptionHandler>(); 

builder.Logging.AddFilter("Npgsql", LogLevel.Warning); // set Npgsql to warning

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis($"{redisHost}:{redisPort},password={redisPassword},ssl=True,abortConnect=False");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapCarter();
app.UseMiddleware<CustomExceptionMiddleware>(); // instead of: app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();