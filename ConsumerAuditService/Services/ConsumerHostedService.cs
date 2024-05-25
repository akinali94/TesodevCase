namespace ConsumerAuditService.Services;

public class ConsumerHostedService : BackgroundService
{
    private readonly ConsumerService _consumer;

    public ConsumerHostedService(ConsumerService consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => _consumer.ProcessKafkaMessage(stoppingToken), stoppingToken);
    }
}

/*
 *
 *     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
       while (!stoppingToken.IsCancellationRequested)
       {
           ProcessKafkaMessage(stoppingToken);
           await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
       }

       _consumer.Close();
   }
*/