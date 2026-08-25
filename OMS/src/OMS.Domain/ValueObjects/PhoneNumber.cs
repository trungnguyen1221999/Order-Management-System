namespace OMS.Domain.ValueObjects
{
    public sealed record PhoneNumber(string Value)
    {
        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty.", nameof(phoneNumber));
            // Basic validation for phone number format (you can customize this regex as needed)
            var phoneRegex = new System.Text.RegularExpressions.Regex(@"^\+?[1-9]\d{1,14}$");
            if (!phoneRegex.IsMatch(phoneNumber))
                throw new ArgumentException("Invalid phone number format.", nameof(phoneNumber));
            return new PhoneNumber(phoneNumber.Trim());
        }
    }
}
