using TransactionApi.Services;
using TransactionApi.Services.Impl;
using TransactionEFDAL.DataAccess;
using Microsoft.EntityFrameworkCore;
using TransactionApi.HostedServices;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using TransactionApi.Exceptions.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TransactionDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteDefault"));
}, ServiceLifetime.Singleton);

builder.Services.AddSingleton<ITransactionService, TransactionService>();
builder.Services.AddSingleton<TransactionPoolHostedService>();
builder.Services.AddHostedService<TransactionPoolHostedService>(p => p.GetRequiredService<TransactionPoolHostedService>());

builder.Services.AddHttpClient("WalletApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiUrls:WalletApi"));
});

builder.Services.AddHttpClient("TokenApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiUrls:TokenApi"));
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
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
    var db = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
    db.Database.Migrate();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
