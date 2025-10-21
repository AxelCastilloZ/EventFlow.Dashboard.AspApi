using EventFlow.Api.Hubs;
using EventFlow.Data;
using EventFlow.Services;
using Microsoft.EntityFrameworkCore;
using EventFlow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar Entity Framework con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar servicios
builder.Services.AddScoped<DashboardService>();

//  Registrar el Consumer de RabbitMQ (del proyecto EventFlow.Services)
//builder.Services.AddHostedService<RabbitMQConsumerService>();

// Configurar SignalR
builder.Services.AddSignalR();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Mapear el Hub de SignalR con la interfaz
app.MapHub<DashboardHub>("/dashboardHub");

app.Run();