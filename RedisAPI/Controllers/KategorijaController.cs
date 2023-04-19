using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.Controllers;

[ApiController]
[Route("[controller]")]
public class KategorijaController : ControllerBase
{
    private RedisCollection<Kategorija> _kategorija;
    private RedisConnectionProvider _provider;
    public KategorijaController(RedisConnectionProvider provider)
    {
        _provider = provider;
        _kategorija = (RedisCollection<Kategorija>)provider.RedisCollection<Kategorija>();
    }
    [HttpPost("Add"), Authorize(Roles = "vlasnik")]
    public IActionResult AddKategorija ([FromBody] Kategorija kategorija)
    {   
        var _objkat = (RedisCollection<Objekat>)_provider.RedisCollection<Objekat>();
        var obj = _objkat.FindById(kategorija.ObjekatId);
        if(obj != null)
        {
            _kategorija.Insert(kategorija);
            return Ok(kategorija);
        }
        return BadRequest("Ne postoji taj objekat");   
    }

    [HttpGet("GetAll")]
    public IActionResult GetAllKategorija()
    {
        try
        {
            var pom = _kategorija.ToList();
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

    [HttpGet("GetKategorije/{id_objekta}")]
    public IActionResult GetKategorijeObjekat([FromRoute] string id_objekta)
    {
        try
        {
            var pom = _kategorija.ToList().Where(x => x.ObjekatId == id_objekta);
            if(pom != null){
                return Ok(pom);
            }
            return BadRequest("Ne postoej kategorije za taj objekat");
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }
    }

    [HttpPut("UpdateName/{id}/{naziv}"), Authorize(Roles = "vlasnik")]
    public IActionResult UpdateName([FromRoute] string id, [FromRoute] string naziv)
    {
        try{
            var kategorija = _kategorija.FindById(id);
            if (kategorija != null)
            {
                kategorija.Naziv = naziv;
            }    
            _kategorija.Save();
            return Ok(kategorija);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }

    [HttpDelete("Delete/{id}"), Authorize(Roles = "vlasnik")]
    public IActionResult DeleteKategorija([FromRoute] string id)
    {
        try
        {
            var _proizvodi = (RedisCollection<Proizvod>)_provider.RedisCollection<Proizvod>();
            foreach(Proizvod pro in _proizvodi)
            {
                if(pro.KategorijaId == id)
                {
                    _provider.Connection.Unlink($"Proizvod:{pro.Id}");
                }
            }
            _provider.Connection.Unlink($"Kategorija:{id}");
            return Ok(id);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}