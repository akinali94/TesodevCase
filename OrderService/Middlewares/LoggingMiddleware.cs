using OrderService.Configs;
using OrderService.Helpers;

namespace OrderService.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;
    private readonly KafkaProducerConfig _producer;
    
    public LoggingMiddleware(RequestDelegate next, KafkaProducerConfig producer, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _producer = producer;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        await LogRequest(context);
        
        var originalBodyStream = context.Response.Body;
        using (var responseBody = new MemoryStream())
        {
            context.Response.Body = responseBody;

            await _next(context);

            await LogResponse(context);

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }

    private async Task LogRequest(HttpContext context)
    {
        var request = context.Request;

        var requestLog = new RequestLog
        {
            Timestamp = DateTime.Now,
            Level = "Info",
            Message = $"HTTP {request.Method} : {request.Path}",
            Source = "Request",
            Host = $"{request.Host}",
            User = $"{context.User.Identity?.Name}"
        };
        
        _logger.LogInformation("Request Details: {requestLog}", requestLog);
        await _producer.ProduceAsync("order-log", requestLog);
    }

    private async Task LogResponse(HttpContext context)
    {
        var response = context.Response;

        var responseLog = new ResponseLog
        {
            Timestamp = DateTime.Now,
            Level = "Info",
            Message = $"HTTP {response.StatusCode}",
            Source = "Response",
            ContentType = $"{response.ContentType}",

        };

        if (response.StatusCode >= 500)
        {
            responseLog.Level = "Error";
            _logger.LogError($"Response Details: {responseLog}", responseLog);
        }
        else
        {
            _logger.LogInformation($"Response Details: {responseLog}", responseLog);
            
        }
        
        await _producer.ProduceAsync("order-log", responseLog);
    }
}

