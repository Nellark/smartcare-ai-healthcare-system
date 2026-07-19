namespace SmartCare.API.Auth;

public interface ITokenService
{
    LoginResponse CreateToken(string email, string role);
}
