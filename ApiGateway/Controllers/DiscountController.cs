using ApiGateway.Model;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DiscountController : ControllerBase
{

    private readonly IDiscountService _discountService;
    public DiscountController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet("GetByCode")]
    public async Task<IActionResult> Get(string code)
    {
        var result = await _discountService.GetDiscountByCode(code);
        return Ok(result);
    }

    [HttpGet("GetById")]

    public async Task<IActionResult> GetById(string Id)
    {
        var result = await _discountService.GetDiscountById(Id);
        return Ok(result);
    }

    [HttpPut("UseDiscount")]
    public async Task<IActionResult> Put(string code)
    {
        var result = await _discountService.UseDiscountBy(code);
        return Ok(result);
    }
}