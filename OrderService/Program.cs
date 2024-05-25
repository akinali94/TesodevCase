using FluentValidation;
using OrderService.Commands;
using OrderService.Configs;
using OrderService.Middlewares;
using OrderService.Queries;
using OrderService.V1.Models.CommandModels;
using OrderService.V1.Models.Validators;

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
        builder.Services.AddScoped<ChangeStatusCommandHandler>();
        builder.Services.AddScoped<CreateCommandHandler>();
        builder.Services.AddScoped<DeleteCommandHandler>();
        builder.Services.AddScoped<UpdateCommandHandler>();
        builder.Services.AddScoped<GetAllQueryHandler>();
        builder.Services.AddScoped<GetByCustomerIdQueryHandler>();
        builder.Services.AddScoped<GetByIdQueryHandler>();
        builder.Services.AddSingleton<KafkaProducerConfig>();
        
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