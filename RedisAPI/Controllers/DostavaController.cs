using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.Controllers;

[ApiController]
[Route("[controller]")]
public class DostavaController : ControllerBase
{
    private RedisCollection<Dostava> _dostava;
    private RedisConnectionProvider _provider;
    public DostavaController(RedisConnectionProvider provider)
    {
        _provider = provider;
        _dostava = (RedisCollection<Dostava>)provider.RedisCollection<Dostava>();
    }
    [HttpPost("AddDostava"), Authorize(Roles = "dostavljac")]
    public IActionResult AddDostava ([FromBody] Dostava dostava)
    {   
        var _proizvod = (RedisCollection<Proizvod>)_provider.RedisCollection<Proizvod>();
        var _korisnik = (RedisCollection<Korisnik>)_provider.RedisCollection<Korisnik>();
        var _dostavljac = (RedisCollection<Dostavljac>)_provider.RedisCollection<Dostavljac>();

        var ko = _korisnik.FindById(dostava.KorisnikId);
        var dost = _dostava.FindById(dostava.DostavljacId);
        
        if(ko == null){
            return BadRequest("Ne postoji taj korisnik");
        }
        if(dost == null){
            return BadRequest("Ne postoji taj dostavljac");
        }
        var suma = 0;
        foreach(string st in dostava.ProizvodiID){
            var pom = _proizvod.FindById(st);
           if(pom == null){
                return BadRequest("Ne postoji taj proizvod: " + st);
           }
           suma += pom.Cena;
        }
        dostava.Cena = suma;
        _dostava.InsertAsync(dostava);
        return Ok(dostava);
    }

    [HttpGet("GetAll")]
    public IActionResult GetAllDostava()
    {
        try
        {
            var pom = _dostava.ToList();
            if(pom != null){
                return Ok(pom);
            }
            return BadRequest(3);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }
    }

    [HttpDelete("Delete/{id}"), Authorize(Roles = "dostavljac")]
    public IActionResult DeleteDostava([FromRoute] string id)
    {
        try
        {   
            _provider.Connection.Unlink($"Dostava:{id}");
            return Ok(id);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}