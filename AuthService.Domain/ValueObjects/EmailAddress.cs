using System.Text.RegularExpressions;

public sealed record EmailAddress
{

    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }
    public static EmailAddress Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email address cannot be null or empty.", nameof(email));

        // Simple email format validation (Placeholder for full RFC validation)
        if (!Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
        {
            throw new ArgumentException("Invalid email address format.", nameof(email));
        }
        return new EmailAddress(email);
    }
    public override string ToString() => Value;
}