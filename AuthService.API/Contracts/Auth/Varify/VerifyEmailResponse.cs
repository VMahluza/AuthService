namespace AuthService.API.Contracts.Auth.Varify;
public record VerifyEmailResponse(bool IsSuccess, string Message) 
{
    public VerifyEmailResponse() : this(true, string.Empty) { }
};