using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OrderService.Commands;
using OrderService.Configs;
using OrderService.Configs.HttpConfig;
using OrderService.Helpers;
using OrderService.Models;
using OrderService.Queries;
using OrderService.V1.Models.CommandModels;
using OrderService.V1.Models.QueryModels;
using OrderService.V1.Models.Validators;

namespace OrderService.V1.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class OrderController : ControllerBase
{
    
    private readonly ICommandHandler<CreateCommand, string> _createCommandHandler;
    private readonly ICommandHandler<UpdateCommand, bool> _updateCommandHandler;
    private readonly ICommandHandler<DeleteCommand, bool> _deleteCommandHandler;
    private readonly ICommandHandler<ChangeStatusCommand, bool> _changeStatusCommandHandler;
    private readonly IQueryHandler<GetAllQuery, IEnumerable<Order>> _getAllQueryHandler;
    private readonly IQueryHandler<GetByIdQuery, Order> _getByIdQueryHandler;
    private readonly IQueryHandler<GetByCustomerIdQuery, IEnumerable<Order>> _getByCustomerIdQueryHandlerHandler;
    private readonly IHttpHandler _httpHandler;
    private readonly IValidator<CreateCommand> _validatorCreate;
    private readonly IValidator<UpdateCommand> _validatorUpdate;
    private readonly IValidator<ChangeStatusCommand> _validatorStatus;
    
    public OrderController(ICommandHandler<CreateCommand, string> createCommandHandler, 
        ICommandHandler<UpdateCommand, bool> updateCommandHandler, ICommandHandler<DeleteCommand, bool> deleteCommandHandler, 
        ICommandHandler<ChangeStatusCommand, bool> changeStatusCommandHandler, IQueryHandler<GetAllQuery, IEnumerable<Order>> getAllQueryHandler, 
        IQueryHandler<GetByIdQuery, Order> getByIdQueryHandler, IQueryHandler<GetByCustomerIdQuery, 
            IEnumerable<Order>> getByCustomerIdQueryHandlerHandler, IHttpHandler httpHandler, IValidator<CreateCommand> validatorCreate, 
        IValidator<UpdateCommand> validatorUpdate, IValidator<ChangeStatusCommand> validatorStatus)
    {
        _createCommandHandler = createCommandHandler;
        _updateCommandHandler = updateCommandHandler;
        _deleteCommandHandler = deleteCommandHandler;
        _changeStatusCommandHandler = changeStatusCommandHandler;
        _getAllQueryHandler = getAllQueryHandler;
        _getByIdQueryHandler = getByIdQueryHandler;
        _getByCustomerIdQueryHandlerHandler = getByCustomerIdQueryHandlerHandler;
        
        _httpHandler = httpHandler;
        
        _validatorCreate = validatorCreate;
        _validatorUpdate = validatorUpdate;
        _validatorStatus = validatorStatus;
    }


    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateCommand command)
    {
        var customerResponse =
            await _httpHandler.GetAsync($"http://localhost:5236/api/v1/Customer/Validate/{command.CustomerId}");
        //private readonly HttpClient _httpClient; --> tan aliyorduk, sonrasinda IHttpClient tanimladik.
        //var customerResponse = await _httpClient.GetAsync($"http://localhost:5236/api/v1/Customer/Validate/{command.CustomerId}");
        if (!customerResponse.IsSuccessStatusCode)
            return BadRequest("Customer Id is not valid");
        
        var validationResult = await _validatorCreate.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            throw new CustomException($"{validationResult.Errors[0].ErrorMessage}");
            //return StatusCode(StatusCodes.Status400BadRequest, validationResult.Errors);
        }
        var orderId = await _createCommandHandler.Handle(command);
        
        return Created(nameof(Create), $"Created ID: {orderId}");
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateCommand command)
    {
        var validationResult = await _validatorUpdate.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            throw new CustomException($"{validationResult.Errors[0].ErrorMessage}");
        }
        var result = await _updateCommandHandler.Handle(command);
        if (!result)
            return NotFound("Update is not successful");

        return Ok($"Order {command.Id} is updated");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] string id)
    {
        var command = new DeleteCommand(id);
        
        var result = await _deleteCommandHandler.Handle(command);
        if (!result)
            return BadRequest("Order can not be deleted");
        
        return Ok($"Order {id} is deleted");
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Order>>> GetAll()
    {
        var orders = await _getAllQueryHandler.Handle(new GetAllQuery());
        if (!orders.Any())
            throw new CustomException("There are no Orders");

        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetById(string id)
    {
        var order = await _getByIdQueryHandler.Handle(new GetByIdQuery(id));
        if (order == null)
            return NotFound("Order is Not Found");

        return Ok(order);
    }
    
    [HttpGet("Customer/{id}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetByCustomerId(string id)
    {
        var customerResponse =
            await _httpHandler.GetAsync($"http://localhost:5236/api/v1/Customer/Validate/{id}");
        if (!customerResponse.IsSuccessStatusCode)
            return BadRequest("Customer Id is not valid");
        
        var order = await _getByCustomerIdQueryHandlerHandler.Handle(new GetByCustomerIdQuery(id));
        
        if (order == null)
            return NotFound("This customer has no order!");

        return Ok(order);
    }

    [HttpPatch("ChangeStatus")]
    public async Task<ActionResult<bool>> ChangeStatus([FromBody] ChangeStatusCommand command)
    {
        var validationResult = await _validatorStatus.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            throw new CustomException($"{validationResult.Errors[0].ErrorMessage}");
        }
        
        var result = await _changeStatusCommandHandler.Handle(command);

        return Ok($"Order Status of {command.OrderId} is changed");
    }
}