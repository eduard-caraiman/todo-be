using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using todo_be.Categories.Repositories;
using todo_be.Categories.Services;
using todo_be.Database;
using todo_be.Documents.Messaging;
using todo_be.Todos.Repositories;
using todo_be.Todos.Service;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddSingleton<IConnection>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var rabbitMq = configuration.GetRequiredSection("RabbitMq");

    var factory = new ConnectionFactory
    {
        HostName = rabbitMq["HostName"]
                   ?? throw new InvalidOperationException("RabbitMQ HostName lipseÈ™te."),
        Port = rabbitMq.GetValue<int>("Port"),
        UserName = rabbitMq["UserName"]
                   ?? throw new InvalidOperationException("RabbitMQ UserName lipseÈ™te."),
        Password = rabbitMq["Password"]
                   ?? throw new InvalidOperationException("RabbitMQ Password lipseÈ™te."),
        AutomaticRecoveryEnabled = true
    };

    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddScoped<IDocumentUploadPublisher, RabbitMqDocumentUploadPublisher>();
builder.Services.AddScoped<IDocumentDeletePublisher, RabbitMqDocumentDeletePublisher>();
builder.Services.AddHostedService<DocumentCreatedConsumer>();
builder.Services.AddHostedService<DocumentDeletedConsumer>();

builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITodoDocumentRepository, TodoDocumentRepository>();
builder.Services.AddScoped<ITodoDocumentService, TodoDocumentService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    SeedData.Seed(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("Frontend");

app.MapControllers();
app.Run();

