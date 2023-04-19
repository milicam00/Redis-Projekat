using Redis.OM.Modeling;

namespace Redis.OM.Skeleton.Model;


[Document(StorageType = StorageType.Json, Prefixes = new []{"Korisnik"})]
public class Korisnik
{
    [RedisIdField] [Indexed]public string Id { get; set; }

    [Indexed] public string Prezime {get; set;} = String.Empty;

    [Indexed] public string Email {get; set;} = String.Empty;

    [Indexed] public string Adressa {get; set;} = String.Empty;

    [Indexed] public string BrTelefona {get; set;} = String.Empty;

    [Indexed] public string Ime {get; set;} = String.Empty;

    [Indexed] public string Userime {get; set;} = String.Empty;

    [Indexed] public string Passwordhash {get; set;} = String.Empty;

    [Indexed] public string Passwordsalt {get; set;} = String.Empty;
    

    //nacin placanja
}