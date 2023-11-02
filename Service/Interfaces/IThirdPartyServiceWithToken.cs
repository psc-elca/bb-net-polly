namespace Service.Interfaces;

public interface IThirdPartyServiceWithToken
{
    Task<string> Get(string uri, CancellationToken cancellationToken);
}