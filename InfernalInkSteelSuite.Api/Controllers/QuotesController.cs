using InfernalInkSteelSuite.Api.Services;
using InfernalInkSteelSuite.Domain;
using Microsoft.AspNetCore.Mvc;

namespace InfernalInkSteelSuite.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotesController(QuoteService service) : ControllerBase
{
    private readonly QuoteService _service = service;

    [HttpPost("preview")]
    public IActionResult Calculate([FromBody] QuoteInput input)
    {
        if (input == null) return BadRequest();
        var estimate = _service.CalculateQuote(input);
        return Ok(estimate);
    }

    [HttpPost]
    public IActionResult Create([FromBody] QuoteInput input)
    {
        if (input == null) return BadRequest();
        var estimate = _service.CalculateQuote(input);
        var quote = _service.CreateQuote(input, estimate);
        return CreatedAtAction(nameof(Get), new { id = quote.Id }, quote);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_service.GetAllQuotes());
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var quote = _service.GetQuote(id);
        if (quote == null) return NotFound();
        return Ok(quote);
    }
}
