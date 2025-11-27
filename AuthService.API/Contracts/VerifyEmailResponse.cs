namespace AuthService.API.Contracts;
public record VerifyEmailResponse(bool IsSuccess, string Message) 
{
    public VerifyEmailResponse() : this(true, string.Empty) { }
};