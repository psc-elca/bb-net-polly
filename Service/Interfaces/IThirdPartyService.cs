namespace Service.Interfaces;

public interface IThirdPartyService
{
    Task<string> Get(string uri, CancellationToken cancellationToken);
}