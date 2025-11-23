using AuthService.Domain.Interfaces;
namespace AuthService.Domain.ValueObjects;
public sealed record PasswordHash
{

    public string Value { get; }

    private PasswordHash(string value)
    {
        Value = value;
    }
    public static PasswordHash Create(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));

        // Additional validation to check hash format/length could go here.
        return new PasswordHash(passwordHash);
    }
    public bool Verify(string rawPassword, IPasswordHasher hasher) =>  hasher.Verify(rawPassword, this);
    public override string ToString() => Value;
}