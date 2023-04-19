using Redis.OM.Modeling;

namespace Redis.OM.Skeleton.Model;


[Document(StorageType = StorageType.Json, Prefixes = new []{"Proizvod"})]
public class Proizvod
{
    [RedisIdField] [Indexed]public string Id { get; set; }

    [Indexed] public string Naziv {get; set;}

    [Indexed] public int Cena {get; set;}

    [Indexed] public string Opis {get; set;}
    [Indexed] public string KategorijaId {get; set;}
    [Indexed] public int BrPorudzbina {get; set;}
 
}