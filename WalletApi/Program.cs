using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using StackExchange.Redis;
using WalletApi.Exceptions.Handlers;
using WalletApi.MapperProfiles;
using WalletApi.Services;
using WalletApi.Services.Impl;
using WalletEFDAL.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAzureKeyVaultService, AzureKeyVaultService>();
builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services.AddDbContext<WalletDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteDefault"));
});

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddSecretClient(new Uri(builder.Configuration.GetValue<string>("AzureKeyVault:AzureKeyVaultURL")!));
    clientBuilder.UseCredential(
        new ClientSecretCredential(
            builder.Configuration.GetValue<string>("AzureKeyVault:AzureClientTenantId"),
            builder.Configuration.GetValue<string>("AzureKeyVault:AzureClientId"),
            builder.Configuration.GetValue<string>("AzureKeyVault:AzureClientSecret")
        ));
});

builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var options = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("RedisDatabase"));
    options.ReconnectRetryPolicy = new LinearRetry(10000); // waits 10 seconds between retries
    options.ConnectRetry = 5; // retry 5 times
    options.ConnectTimeout = 5000; // 5 seconds timeout
    return ConnectionMultiplexer.Connect(options);
});

builder.Services.AddAutoMapper(typeof(TokenProfile));
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
