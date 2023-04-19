using Redis.OM.Modeling;

namespace Redis.OM.Skeleton.Model;


[Document(StorageType = StorageType.Json, Prefixes = new []{"Dostava"})]
public class Dostava
{
    [RedisIdField] [Indexed]public string Id { get; set; }

    [Indexed] public List<string> ProizvodiID {get; set;}

    [Indexed] public int Cena {get; set;}

    [Indexed] public string KorisnikId {get; set;}
    [Indexed] public string DostavljacId {get; set;}
}