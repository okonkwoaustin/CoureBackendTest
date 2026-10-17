using CoureBackendTest.Data;
using CoureBackendTest.Models;
using Microsoft.EntityFrameworkCore;

namespace CoureBackendTest.Services;

public interface IPhoneLookupService
{
    /// <summary>Returns null when no supported country code matches the number.</summary>
    Task<PhoneLookupResponse?> LookupAsync(string phoneNumber, CancellationToken ct = default);
}

public class PhoneLookupService : IPhoneLookupService
{
    private readonly AppDbContext _db;

    public PhoneLookupService(AppDbContext db) => _db = db;

    public async Task<PhoneLookupResponse?> LookupAsync(string phoneNumber, CancellationToken ct = default)
    {
        var number = Normalize(phoneNumber);

        var countries = await _db.Countries
            .AsNoTracking()
            .Include(c => c.CountryDetails)
            .ToListAsync(ct);

        // Longest matching code wins, so it stays correct if codes of different lengths are added.
        var country = countries
            .Where(c => number.StartsWith(c.CountryCode, StringComparison.Ordinal))
            .OrderByDescending(c => c.CountryCode.Length)
            .FirstOrDefault();

        if (country is null)
        {
            return null;
        }

        var operators = country.CountryDetails
            .OrderBy(d => d.Id)
            .Select(d => new OperatorDto(d.Operator, d.OperatorCode))
            .ToList();

        return new PhoneLookupResponse(
            number,
            new CountryDto(country.CountryCode, country.Name, country.CountryIso, operators));
    }

    /// <summary>Keeps digits only and drops a leading international prefix "00" (e.g. +234..., 00234...).</summary>
    public static string Normalize(string input)
    {
        var digits = new string(input.Where(char.IsDigit).ToArray());
        return digits.StartsWith("00", StringComparison.Ordinal) ? digits[2..] : digits;
    }

    /// <summary>E.164 numbers have at most 15 digits.</summary>
    public static bool IsValid(string normalized) =>
        normalized.Length is >= 7 and <= 15;
}