using System.ComponentModel.DataAnnotations;
using Redis.OM.Modeling;
using Redis.OM.Skeleton.Model;

[Document(Prefixes = new []{"Dostavljac"})]
public class Dostavljac
{
    [RedisIdField] [Indexed] public string Id { get; set; }

    [Indexed] public string prezime {get; set;} = String.Empty;

    //mocam ti se na Redis mora ko string jaka baza dosta
    [Indexed] public string ProsecnaOcena {get; set;} = "0";

    [Indexed] public int BrOcena {get; set;} = 0;

    [Indexed] public string Ime {get; set;} = String.Empty;

    [Indexed] public string Userime {get; set;} = String.Empty;

    [Indexed] public string Passwordhash {get; set;} = String.Empty;

    [Indexed] public string Passwordsalt {get; set;} = String.Empty;
}