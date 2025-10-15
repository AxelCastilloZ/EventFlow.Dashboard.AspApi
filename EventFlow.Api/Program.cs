using EventFlow.Api.Hubs;
using EventFlow.Data;
using EventFlow.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configurar Entity Framework con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar DashboardService
builder.Services.AddScoped<DashboardService>();

// Configurar SignalR
builder.Services.AddSignalR();

// Configurar CORS para permitir conexiones desde el frontend
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Servir archivos estáticos (DEBE IR ANTES DE HTTPS REDIRECTION)
app.UseDefaultFiles();
app.UseStaticFiles();

// Comentar temporalmente UseHttpsRedirection
// app.UseHttpsRedirection();

// Usar CORS
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Mapear el Hub de SignalR
app.MapHub<DashboardHub>("/dashboardHub");

app.Run();