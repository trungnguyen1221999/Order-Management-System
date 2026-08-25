using OMS.Domain.Common;

namespace OMS.Domain.ValueObjects
{
    public sealed record Address(
        string Street,
        string City,
        string Province,
        string Country,
        string? PostalCode = null
    )
    {
        public static Address Create(
            string street,
            string city,
            string province,
            string country,
            string? postalCode = null
        )
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new DomainException("Street can not be empty");

            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("City can not be empty");

            if (string.IsNullOrWhiteSpace(country) || country.Trim().Length != 2)
                throw new DomainException("Country code must have 2 letters (ex: VN, US, FI).");

            return new Address(
                street.Trim(),
                city.Trim(),
                province.Trim(),
                country.ToUpperInvariant().Trim(),
                postalCode?.Trim()
            );
        }

        public bool IsDomestic() => Country == "FI";

        public string ToFormattedString() =>
            PostalCode is null
                ? $"{Street}, {City}, {Province}, {Country}"
                : $"{Street}, {City}, {Province} {PostalCode}, {Country}";

        public override string ToString() => ToFormattedString();
    }
}
