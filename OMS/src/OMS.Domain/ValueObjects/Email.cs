using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OMS.Domain.ValueObjects
{
    public sealed record Email(string Value)
    {
        private static readonly Regex EmailRegex = new(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled
        );

        public static Email Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
                throw new ArgumentException("Invalid email format.", nameof(email));
            return new Email(email.Trim());
        }
    }
}