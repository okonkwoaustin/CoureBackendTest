using CoureBackendTest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoureBackendTest.Controllers;

[ApiController]
[Route("api/countries")]
public class CountriesController(AppDbContext db) : ControllerBase
{
    [HttpGet("PhoneNumberLookup")]
    public async Task<IActionResult> Lookup([FromQuery] string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return BadRequest(new
            {
                message = "Phone number is required."
            });
        }

        // Keep digits only, allowing inputs such as +234 803-343-2323.
        string normalizedNumber = new(
            phoneNumber.Where(char.IsDigit).ToArray()
        );

        if (normalizedNumber.Length == 0)
        {
            return BadRequest(new
            {
                message = "Please provide a valid phone number."
            });
        }

        var countries = await db.Countries
            .Include(c => c.CountryDetails)
            .ToListAsync();

        // Longest prefix first, so overlapping calling codes
        // would be handled correctly.
        var country = countries
            .OrderByDescending(c => c.CountryCode.Length)
            .FirstOrDefault(c =>
                normalizedNumber.StartsWith(c.CountryCode));

        if (country is null)
        {
            return NotFound(new
            {
                message = "No country was found for this country calling code."
            });
        }

        return Ok(new
        {
            number = normalizedNumber,
            country = new
            {
                countryCode = country.CountryCode,
                name = country.Name,
                countryIso = country.CountryIso,
                countryDetails = country.CountryDetails.Select(detail => new
                {
                    @operator = detail.Operator,
                    operatorCode = detail.OperatorCode
                })
            }
        });
    }
}