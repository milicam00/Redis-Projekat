using Redis.OM.Modeling;

namespace Redis.OM.Skeleton.Model;


[Document(StorageType = StorageType.Json, Prefixes = new []{"Objekat"})]
public class Objekat
{
    [RedisIdField] [Indexed]public string Id { get; set; }

    [Indexed] public string Naziv {get; set;}

    [Indexed] public List<string> HotProizvodi {get; set;}
 
}