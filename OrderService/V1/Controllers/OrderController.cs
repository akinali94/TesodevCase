using Microsoft.AspNetCore.Mvc;
using OrderService.Commands;
using OrderService.Models;
using OrderService.Queries;
using OrderService.V1.Models.CommandModels;
using OrderService.V1.Models.QueryModels;

namespace OrderService.V1.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class OrderController : ControllerBase
{
    private readonly CreateCommandHandler _createCommandHandler;
    private readonly UpdateCommandHandler _updateCommandHandler;
    private readonly DeleteCommandHandler _deleteCommandHandler;
    private readonly ChangeStatusCommandHandler _changeStatusCommandHandler;
    private readonly GetAllQueryHandler _getAllQueryHandler;
    private readonly GetByIdQueryHandler _getByIdQueryHandler;
    private readonly GetByCustomerIdQueryHandler _getByCustomerIdQueryHandlerHandler;
    private readonly HttpClient _httpClient;
    
    public OrderController(CreateCommandHandler createCommandHandler, GetAllQueryHandler gelAllQueryHandler, GetByIdQueryHandler getByIdQueryHandler, HttpClient httpClient, UpdateCommandHandler updateCommandHandler, DeleteCommandHandler deleteCommandHandler, GetByCustomerIdQueryHandler getByCustomerIdQueryHandlerHandler, ChangeStatusCommandHandler changeStatusCommandHandler)
    {
        _createCommandHandler = createCommandHandler;
        _updateCommandHandler = updateCommandHandler;
        _deleteCommandHandler = deleteCommandHandler;
        _changeStatusCommandHandler = changeStatusCommandHandler;
        
        _getAllQueryHandler = gelAllQueryHandler;
        _getByIdQueryHandler = getByIdQueryHandler;
        _getByCustomerIdQueryHandlerHandler = getByCustomerIdQueryHandlerHandler;
        
        _httpClient = httpClient;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateCommand command)
    {
        var customerResponse =
            await _httpClient.GetAsync($"http://localhost:5236/api/v1/Customer/Validate/{command.CustomerId}");
        if (!customerResponse.IsSuccessStatusCode)
            return BadRequest("Customer Id is not valid");

        var orderId = await _createCommandHandler.Handle(command);
        
        return Created(nameof(Create), $"Created ID: {orderId}");
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCommand command)
    {
        var result = await _updateCommandHandler.Handle(id, command);
        if (!result)
            return NotFound("Update is not successful");

        return Ok($"Order {id} is updated");
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
    public async Task<ActionResult<Order>> GetByCustomerId(string id)
    {
        var customerResponse =
            await _httpClient.GetAsync($"http://localhost:5236/api/v1/Customer/Validate/{id}");
        if (!customerResponse.IsSuccessStatusCode)
            return BadRequest("Customer Id is not valid");
        
        var order = _getByCustomerIdQueryHandlerHandler.Handle(new GetByCustomerIdQuery(id));
        
        if (order == null)
            return NotFound("This customer has no order!");

        return Ok(order);
    }

    [HttpPatch("ChangeStatus")]
    public async Task<ActionResult<bool>> ChangeStatus([FromBody] ChangeStatusCommand command)
    {
        var result = await _changeStatusCommandHandler.Handle(command);

        return Ok($"Order Status of {command.OrderId} is changed");
    }
}