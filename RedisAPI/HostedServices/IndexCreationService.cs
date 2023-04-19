using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.HostedServices;

public class IndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;
    public IndexCreationService(RedisConnectionProvider provider)
    {
        _provider = provider;
    }
    
    /// <summary>
    /// Checks redis to see if the index already exists, if it doesn't create a new index
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var info = (await _provider.Connection.ExecuteAsync("FT._LIST")).ToArray().Select(x=>x.ToString());
        await _provider.Connection.CreateIndexAsync(typeof(Dostava));     
        await _provider.Connection.CreateIndexAsync(typeof(Dostavljac)); 
        await _provider.Connection.CreateIndexAsync(typeof(Kategorija)); 
        await _provider.Connection.CreateIndexAsync(typeof(Korisnik)); 
        await _provider.Connection.CreateIndexAsync(typeof(Objekat)); 
        await _provider.Connection.CreateIndexAsync(typeof(Proizvod)); 
        await _provider.Connection.CreateIndexAsync(typeof(Vlasnik)); 
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}