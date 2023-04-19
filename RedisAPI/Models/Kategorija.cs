using Redis.OM.Modeling;
using Redis.OM.Searching;

namespace Redis.OM.Skeleton.Model;


[Document(StorageType = StorageType.Json, Prefixes = new []{"Kategorija"})]
public class Kategorija
{
    [RedisIdField] [Indexed] public string Id { get; set; }

    [Indexed] public string Naziv {get; set;}

    [Indexed] public string ObjekatId {get; set;}
    
}