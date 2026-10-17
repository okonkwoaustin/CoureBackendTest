using CoureBackendTest.Data;
using CoureBackendTest.Models;
using CoureBackendTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoureBackendTest.Controllers;

[ApiController]
[Route("api/countries")]
public class CountriesController : ControllerBase
{
    private readonly IPhoneLookupService _service;

    public CountriesController(IPhoneLookupService service) => _service = service;



    /// <summary>Detects the country (and its operators) from a phone number's country code.</summary>
    /// <param name="phoneNumber">e.g. 2348033432323 (a leading + or 00 is accepted).</param>
    [HttpGet("PhoneNumbersLookup")]
    [ProducesResponseType(typeof(PhoneLookupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromQuery] string? phoneNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) ||
            !PhoneLookupService.IsValid(PhoneLookupService.Normalize(phoneNumber)))
        {
            return Problem(
                title: "Invalid phone number",
                detail: "Provide 7-15 digits including the country code, e.g. 2348033432323.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var result = await _service.LookupAsync(phoneNumber, ct);

        return result is null
            ? Problem(
                title: "Country not supported",
                detail: "No country matches the code in this phone number.",
                statusCode: StatusCodes.Status404NotFound)
            : Ok(result);
    }
}