using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.Controllers;

[ApiController]
[Route("[controller]")]
public class ProizvodController : ControllerBase
{
    private RedisCollection<Proizvod> _proizvodi;
    private RedisConnectionProvider _provider;

    public ProizvodController(RedisConnectionProvider provider)
    {
        _provider = provider;
        _proizvodi = (RedisCollection<Proizvod>)provider.RedisCollection<Proizvod>();
    }
    [HttpPost, Authorize(Roles = "vlasnik")]
    public IActionResult AddProizvod ([FromBody] Proizvod proizvod)
    {
        var _kategorije = (RedisCollection<Kategorija>)_provider.RedisCollection<Kategorija>();
        var kat = _kategorije.FindById(proizvod.KategorijaId);
        if(kat != null){
            _proizvodi.Insert(proizvod);
            return Ok(proizvod);
        } 
        else{
            return BadRequest("Ne postoji ta kategorija");
        }
        
    }

    [HttpGet("GetAll")]
    public IActionResult GetAllProizvod()
    {
        try{
            var pom = _proizvodi.ToList();
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

    [HttpGet("Get/{id_kategorije}")]
    public IActionResult GetProizvod([FromRoute] string id_kategorije)
    {
        try{
            var pom2 = _proizvodi.ToList().Where(x => x.KategorijaId == id_kategorije);
            if(pom2 != null){
                 return Ok(pom2);
            }
            return BadRequest("Ne postoje proizvodi u toj kategoriji");
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("Order/{id}"), Authorize(Roles = "korisnik")]
    public IActionResult OrderProizvod([FromRoute] string id)
    {
        try{
            var proizvod =_proizvodi.FindById(id);
            if (proizvod != null)
            {
                proizvod.BrPorudzbina++;
            }
            else{
                return BadRequest("Ne postoji proizvod s tim ID");
            }    
            _proizvodi.Save();
            return Ok(proizvod);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }

    [HttpPut("UpdatePrice/{id}/{cena}"), Authorize(Roles = "vlasnik")]
    public IActionResult UpdatePrice([FromRoute] string id, [FromRoute] int cena)
    {
        try{
            var proizvod =_proizvodi.FindById(id);
            if (proizvod != null)
            {
                proizvod.Cena = cena;
            }    
            _proizvodi.Save();
            return Ok(proizvod);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }

    [HttpPut("UpdateDescription/{id}/{opis}"), Authorize(Roles = "vlasnik")]
    public IActionResult UpdateDescription([FromRoute] string id, [FromRoute] string opis)
    {
        try{
            var proizvod =_proizvodi.FindById(id);
            if (proizvod != null)
            {
                proizvod.Opis = opis;
            }    
            _proizvodi.Save();
            return Ok(proizvod);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }

    [HttpPut("UpadteName/{id}/{naziv}"), Authorize(Roles = "vlasnik")]
    public IActionResult UpadteName([FromRoute] string id, [FromRoute] string naziv)
    {
        try{
            var proizvod =_proizvodi.FindById(id);
            if (proizvod != null)
            {
                proizvod.Naziv = naziv;
            }    
            _proizvodi.Save();
            return Ok(proizvod);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }

    [HttpDelete("Delete/{id}"), Authorize(Roles = "vlasnik")]
    public IActionResult DeleteProizvod([FromRoute] string id)
    {
        try
        {
            var pro =_proizvodi.FindById(id);
            if(pro != null){
                _provider.Connection.Unlink($"Proizvod:{id}");
                return Ok(id); 
            }
            return BadRequest("Ne postoji taj proizvod");        
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }  
    }
}