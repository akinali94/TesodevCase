using CustomerService.Helpers;
using CustomerService.Services;
using CustomerService.V1.Models.RequestModels;
using CustomerService.V1.Models.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.V1.Controllers;


[ApiController]
[Route("api/v1/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _service;

    public CustomerController(ICustomerService service)
    {
        _service = service;
    }

    [HttpPost("/Create")]
    public async Task<IActionResult> Create([FromBody] CustomerCreateModel createModel)
    {
        Guid id = await _service.Create(createModel);

        return Created(nameof(Create), $"Created ID: {id}");
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchById(Guid id, [FromBody] CustomerPatchModel patchModel)
    {
        if (patchModel == null)
            return BadRequest("Update Model is empty");

        var success = await _service.Update(id, patchModel);
        if (!success)
            throw new AppException("Update is not successful");

        return Ok($"Customer {id} is updated");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _service.Delete(id);
        
        return Ok($"Customer {id} is deleted");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerGetModel>> Get([FromRoute] Guid id)
    {
        var getCustomer = await _service.GetById(id);
        return Ok(getCustomer);
    }
    
    [HttpGet("/GetAll")]
    public async Task<ActionResult<IEnumerable<CustomerGetModel>>> GetAll()
    {
        var customers = await _service.GetAll();
        return Ok(customers);

    }

    [HttpPost("{id}")]
    public async Task<ActionResult> Validate([FromRoute] Guid id)
    {
        var validate = await _service.Validate(id);
        if (!validate)
            return BadRequest("Customer could not be validated");

        return Ok($"{id} is Valid");
    }
}