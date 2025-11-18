using EventFlow.Api.Hubs;
using EventFlow.Data;
using EventFlow.Services;
using Microsoft.EntityFrameworkCore;
using EventFlow.Api.Services;

var builder = WebApplication.CreateBuilder(args);

//MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Registrar servicios
builder.Services.AddScoped<DashboardService>();

// Registrar el Consumer de RabbitMQ
builder.Services.AddHostedService<RabbitMQConsumerService>();

// Configurar SignalR
builder.Services.AddSignalR();

//  CONFIGURAR CORS CON IPs ESPECÍFICAS DEL GRUPO 7
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGrupo7", policy =>
    {
        policy.WithOrigins(
    
    "http://26.20.226.32:5173",

    "http://localhost:5173",
    "http://127.0.0.1:5173",

    // Para tu HTML de prueba
    "http://localhost:5218"
)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();  
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


app.UseCors("AllowGrupo7");

app.UseAuthorization();
app.MapControllers();

// Mapear el Hub de SignalR
app.MapHub<DashboardHub>("/dashboardHub");

app.Run();