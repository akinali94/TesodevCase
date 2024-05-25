using Confluent.Kafka;
using ConsumerAuditService.Models;
using MongoDB.Bson;

namespace ConsumerAuditService.Services;

public class ConsumerService
{
    private readonly IConsumer<string, string> _consumer;
    
    public ConsumerService(IConfiguration configuration)
    {
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "OrderConsumerGroup",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        _consumer.Subscribe("order-log");
        
    }

    public void ProcessKafkaMessage(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = _consumer.Consume(stoppingToken);
            var message = consumeResult.Message.Value;
            var bsonMessage = BsonDocument.Parse(message);
            var denemeModel = new DenemeModel();

            //_database.AuditLogs.InsertOne(denemeModel);
        }
    }
}