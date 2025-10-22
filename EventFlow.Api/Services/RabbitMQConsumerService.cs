using EventFlow.Api.Hubs;
using EventFlow.DTOs;
using EventFlow.Services;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EventFlow.Api.Services;

public class RabbitMQConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMQConsumerService> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMQConsumerService(
        IServiceScopeFactory scopeFactory,
        ILogger<RabbitMQConsumerService> logger,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation(" Iniciando RabbitMQ Consumer Service...");

            var rabbitConfig = _configuration.GetSection("RabbitMQ");
            var hostName = rabbitConfig["HostName"] ?? "localhost";
            var userName = rabbitConfig["UserName"] ?? "guest";
            var password = rabbitConfig["Password"] ?? "guest";
            var queueName = rabbitConfig["QueueName"] ?? "pagos.dashboard";

            _logger.LogInformation(" Configuración RabbitMQ:");
            _logger.LogInformation("   Host: {Host}", hostName);
            _logger.LogInformation("   User: {User}", userName);
            _logger.LogInformation("   Queue: {Queue}", queueName);

            var factory = new ConnectionFactory
            {
                HostName = hostName,
                UserName = userName,
                Password = password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken
            );

            _logger.LogInformation(" Conectado a RabbitMQ");

            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var mensaje = Encoding.UTF8.GetString(body);

                _logger.LogInformation(" Evento recibido: {Mensaje}", mensaje);

                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var evento = JsonSerializer.Deserialize<EventoPagoDto>(mensaje, options);

                    if (evento == null)
                    {
                        _logger.LogWarning(" No se pudo deserializar el evento");
                        return;
                    }

                    _logger.LogInformation(" Evento parseado - ID: {IdCompra}, Estado: {Estado}",
                        evento.IdCompra, evento.Estado);

                    if (evento.Estado?.ToLower() != "exitoso")
                    {
                        _logger.LogWarning(" Pago no exitoso, se ignora");
                        return;
                    }

                    using var scope = _scopeFactory.CreateScope();

                    var dashboardService = scope.ServiceProvider
                        .GetRequiredService<DashboardService>();

                    var dashboardData = await dashboardService.GetDashboardDataAsync();

                    _logger.LogInformation(" Stats - Compras: {Total}, Valor: ${Valor}",
                        dashboardData.TotalCompras, dashboardData.ValorTotalTransaccionado);

                    var hubContext = scope.ServiceProvider
                        .GetRequiredService<IHubContext<DashboardHub>>();

                    await hubContext.Clients.All
                        .SendAsync("ReceiveDashboardUpdate", dashboardData, stoppingToken);

                    _logger.LogInformation(" Dashboard actualizado vía SignalR");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error procesando evento");
                }

                await Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            _logger.LogInformation(" Escuchando en queue '{Queue}'", queueName);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " Error fatal en Consumer");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(" Deteniendo Consumer...");

        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            await _connection.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}