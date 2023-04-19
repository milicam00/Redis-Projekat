using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.Controllers;

[ApiController]
[Route("[controller]")]
public class DostavljacController : ControllerBase
{
    private RedisCollection<Dostavljac> _dostavljac;
    private IConfiguration _configuration;
    private RedisConnectionProvider _provider;
    public DostavljacController(RedisConnectionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _dostavljac = (RedisCollection<Dostavljac>)provider.RedisCollection<Dostavljac>();
        _configuration = configuration;
    }
    // [HttpPost]
    // public async Task<Dostavljac> AddPerson([FromBody] Dostavljac dostavljac)
    // {
    //     await _dostavljac.InsertAsync(dostavljac);
    //     return dostavljac;
    // }
    
    [HttpGet("GetAll")]
    public IActionResult GetAllDostavljac()
    {
        try{
            var pom = _dostavljac.ToList();
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

    [HttpGet("GetMe/{userime}")]
    public IActionResult GetMe([FromRoute] string userime)
    {
        try
        {
            var pom = _dostavljac.Where(x=> x.Userime == userime).FirstOrDefault();
            if(pom != null){
                return Ok(pom);
            }
            return BadRequest("Ne postoji taj dostavljac");
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }
    }

    [HttpPut("GiveGrade/{id}/{ocena}"), Authorize(Roles = "korisnik")]
    public IActionResult GiveGrade([FromRoute] string id, [FromRoute] int ocena)
    {
        try{
            var dostavljac =_dostavljac.FindById(id);
            if (dostavljac != null)
            {
                var broj = float.Parse(dostavljac.ProsecnaOcena);
                broj = (broj * dostavljac.BrOcena + ocena) / (++dostavljac.BrOcena);
                dostavljac.ProsecnaOcena = broj.ToString();
            }    
            _dostavljac.Save();
            return Ok(dostavljac);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }

    [HttpPut("Update/{id}/{ime}/{prez}"), Authorize(Roles = "dostavljac")]
    public IActionResult Update([FromRoute] string id, [FromRoute] string ime,  [FromRoute] string prez)
    {
        try{
            var dostavljac =_dostavljac.FindById(id);
            if (dostavljac != null)
            {
                dostavljac.Ime = ime;
                dostavljac.prezime = prez;
            }    
            _dostavljac.Save();
            return Ok(dostavljac);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }

    [HttpDelete("DeleteDostava/{id}/{id_dostava}"),  Authorize(Roles = "dostavljac")]
    public IActionResult DeleteDostava([FromRoute] string id, [FromRoute] string id_dostava)
    {
        try{
            var _dostava = (RedisCollection<Dostava>)_provider.RedisCollection<Dostava>();
            var dost = _dostava.FindById(id_dostava);
            if(dost == null){
                return BadRequest("Ne postoji ta dostava");
            }
            if(dost.DostavljacId != id){
                return BadRequest("Ne mozete da obrisete tu dostavu");
            }
            
            _provider.Connection.Unlink($"Dostava:{id_dostava}");
            return Ok(id_dostava);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        } 
    }

    [HttpDelete("Delete/{id}"), Authorize(Roles = "dostavljac")]
    public IActionResult DeleteDostavljac([FromRoute] string id)
    {
        try{
            _provider.Connection.Unlink($"Dostavljac:{id}");
            return Ok(id);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        } 
    }
}