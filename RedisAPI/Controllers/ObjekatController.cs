using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.Controllers;

[ApiController]
[Route("[controller]")]
public class ObjekatController : ControllerBase
{
    private RedisCollection<Objekat> _objekat;
    private RedisConnectionProvider _provider;
    public ObjekatController(RedisConnectionProvider provider)
    {
        _provider = provider;
        _objekat = (RedisCollection<Objekat>)provider.RedisCollection<Objekat>();
    }
    [HttpPost("Add"), Authorize(Roles = "vlasnik")]
    public async Task<Objekat> AddObjekat ([FromBody] Objekat obj)
    {   
        await _objekat.InsertAsync(obj);
        return obj;
    }

    [HttpGet("GetAll")]
    public IActionResult GetAllObjekat()
    {
        try
        {
            var pom = _objekat.ToList();
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
    [HttpPut("UpdateName/{id}/{naziv}"), Authorize(Roles = "vlasnik")]
    public IActionResult UpdateName([FromRoute] string id, [FromRoute] string naziv)
    {
        try{
            var objekat = _objekat.FindById(id);
            if (objekat != null)
            {
                objekat.Naziv = naziv;
            }    
            _objekat.Save();
            return Ok(objekat);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }


    [HttpDelete("Delete{id}"), Authorize(Roles = "vlasnik")]
    public IActionResult DeleteObjekat([FromRoute] string id)
    {
        try
        {   
            var _kategorija = (RedisCollection<Kategorija>)_provider.RedisCollection<Kategorija>();
            foreach(Kategorija kat in _kategorija)
            {
                if(kat.ObjekatId == id)
                {
                    var _proizvodi = (RedisCollection<Proizvod>)_provider.RedisCollection<Proizvod>();
                    foreach(Proizvod pro in _proizvodi)
                    {
                        if(pro.KategorijaId == kat.Id)
                        {
                            _provider.Connection.Unlink($"Proizvod:{pro.Id}");
                        }
                    }
                    _provider.Connection.Unlink($"Kategorija:{kat.Id}");
                }
            }
            _provider.Connection.Unlink($"Objekat:{id}");
            return Ok(id);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}