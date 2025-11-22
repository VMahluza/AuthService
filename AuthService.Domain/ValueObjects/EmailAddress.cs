using System.Text.RegularExpressions;

/// <summary>
/// Represents a validated and immutable email address value object.
/// This type ensures that any email stored in the domain is valid by construction.
/// </summary>
public sealed record EmailAddress
{
    /// <summary>
    /// Gets the raw, validated email address string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the EmailAddress class.
    /// </summary>
    /// <param name="value">The validated email address string.</param>
    private EmailAddress(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Factory method to create a new EmailAddress instance after validating the format and presence of the input.
    /// </summary>
    /// <param name="email">The email string provided by the user.</param>
    /// <returns>A new, validated EmailAddress object.</returns>
    /// <exception cref="ArgumentException">Thrown if the email is null, empty, or fails format validation.</exception>
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

    /// <summary>
    /// Returns the underlying email address string.
    /// </summary>
    /// <returns>The raw email address string.</returns>
    public override string ToString() => Value;
}