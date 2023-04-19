using Redis.OM.Modeling;

namespace Redis.OM.Skeleton.Model;


[Document(StorageType = StorageType.Json, Prefixes = new []{"Vlasnik"})]
public class Vlasnik
{
    [RedisIdField] [Indexed]public string Id { get; set; }

    [Indexed] public string Prezime {get; set;} = String.Empty;
    
    [Indexed] public string Email {get; set;} = String.Empty;

    [Indexed] public string Brtelefona {get; set;} = String.Empty;

    [Indexed] public List<string> ObjektiId {get; set;}

    [Indexed] public string Ime {get; set;} = String.Empty;

    [Indexed] public string Userime {get; set;} = String.Empty;

    [Indexed] public string Passwordhash {get; set;} = String.Empty;

    [Indexed] public string Passwordsalt {get; set;} = String.Empty;
}