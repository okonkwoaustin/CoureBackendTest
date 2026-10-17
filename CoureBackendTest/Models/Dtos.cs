namespace CoureBackendTest.Models;

public record OperatorDto(string Operator, string OperatorCode);

public record CountryDto(
    string CountryCode,
    string Name,
    string CountryIso,
    IReadOnlyList<OperatorDto> CountryDetails);

public record PhoneLookupResponse(string Number, CountryDto Country);
