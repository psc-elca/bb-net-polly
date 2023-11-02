namespace Service.Interfaces;

public interface ITokenProvider
{
    Task<string> GetToken();
    Task RefreshToken();
}