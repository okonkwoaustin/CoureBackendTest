namespace CoureBackendTest.Models
{
    public class Country
    {
        public int Id { get; set; }

        public string CountryCode { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string CountryIso { get; set; } = string.Empty;

        public List<CountryDetail> CountryDetails { get; set; } = [];
    }
}
