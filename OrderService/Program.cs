using FluentValidation;
using OrderService.Commands;
using OrderService.Configs;
using OrderService.Configs.HttpConfig;
using OrderService.Middlewares;
using OrderService.Models;
using OrderService.Queries;
using OrderService.V1.Models.CommandModels;
using OrderService.V1.Models.QueryModels;
using OrderService.V1.Models.Validators;
using HttpClientHandler = OrderService.Configs.HttpConfig.HttpClientHandler;

namespace OrderService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        
        //MongoDb settings
        builder.Services.Configure<MongoDbSettings>(
            builder.Configuration.GetSection("OrderDatabase"));
        
        
        //DENEME
        builder.Services.AddScoped<DbContext>();
        builder.Services.AddScoped<ICommandHandler<ChangeStatusCommand, bool>, ChangeStatusCommandHandler>();
        builder.Services.AddScoped<ICommandHandler<CreateCommand, string>, CreateCommandHandler>();
        builder.Services.AddScoped<ICommandHandler<DeleteCommand, bool>, DeleteCommandHandler>();
        builder.Services.AddScoped<ICommandHandler<UpdateCommand, bool>, UpdateCommandHandler>();
        builder.Services.AddScoped<IQueryHandler<GetAllQuery, IEnumerable<Order>>, GetAllQueryHandler>();
        builder.Services.AddScoped<IQueryHandler<GetByCustomerIdQuery, IEnumerable<Order>>, GetByCustomerIdQueryHandler>();
        builder.Services.AddScoped<IQueryHandler<GetByIdQuery, Order>, GetByIdQueryHandler>();
        builder.Services.AddSingleton<IKafkaProducerConfig,KafkaProducerConfig>();
        builder.Services.AddSingleton<IHttpHandler, HttpClientHandler>();
        
        //Fluent Validation
        builder.Services.AddScoped<IValidator<ChangeStatusCommand>, OrderStatusUpdateValidator>();
        builder.Services.AddScoped<IValidator<CreateCommand>, OrderCreateValidator>();
        builder.Services.AddScoped<IValidator<UpdateCommand>, OrderUpdateValidator>();
        
        builder.Services.AddControllers();
        
        //HttpClient for Microservices
        builder.Services.AddHttpClient();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        //Custom Exception
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<LoggingMiddleware>();

        app.MapControllers();

        app.Run();
    }
}