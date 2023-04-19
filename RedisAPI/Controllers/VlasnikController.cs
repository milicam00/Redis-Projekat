

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
public class VlasnikController : ControllerBase
{
    private RedisCollection<Vlasnik> _vlasnik;
    private RedisConnectionProvider _provider;
    private readonly IConfiguration _configuration;

    public VlasnikController(RedisConnectionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
        _vlasnik = (RedisCollection<Vlasnik>)provider.RedisCollection<Vlasnik>();
    }
    // [HttpPost]
    // public IActionResult AddVlasnik ([FromBody] Vlasnik vlasnik)
    // {   
    //     _vlasnik.InsertAsync(vlasnik);
    //     return Ok(vlasnik);
    // }

    [HttpGet("GetAll")]
    public IActionResult GetAllVlasnik()
    {
        try
        {
            var pom = _vlasnik.ToList();
            if(pom != null){
                return Ok(pom);
            }
            return BadRequest("Ne postoje vlasnici");
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
            var pom = _vlasnik.Where(x=> x.Userime == userime).FirstOrDefault();
            if(pom != null){
                return Ok(pom);
            }
            return BadRequest("Ne postoje vlasnici");
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }
    }
    [HttpPut("Update/{id}"), Authorize(Roles = "vlasnik")]
    public IActionResult Update([FromRoute] string id, [FromRoute] string ime, [FromRoute] string prezime, [FromRoute] string email, [FromRoute] string brtelefona )
    {
        try{
            var vlasnik = _vlasnik.FindById(id);
            if (vlasnik != null)
            {
                vlasnik.Ime = ime;
                vlasnik.Prezime = prezime;
                vlasnik.Email = email;
                vlasnik.Brtelefona = brtelefona;
            }    
            _vlasnik.Save();
            return Ok(vlasnik);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }

    [HttpDelete("Delete/{id}"), Authorize(Roles = "vlasnik")]
    public IActionResult DeeleteVlasnik([FromRoute] string id)
    {
        try
        {   
            Vlasnik vlasnik = _vlasnik.FindById(id);
            if(vlasnik == null){
                return BadRequest("Ne postoji taj vlasnik");
            }
            var _objekat = (RedisCollection<Objekat>)_provider.RedisCollection<Objekat>();
            var _kategorija = (RedisCollection<Kategorija>)_provider.RedisCollection<Kategorija>();
            var _proizvodi = (RedisCollection<Proizvod>)_provider.RedisCollection<Proizvod>();

            foreach(string obj_id in vlasnik.ObjektiId){
                var objekat = _objekat.FindById(obj_id);
                foreach(Kategorija kat in _kategorija)
                {
                    if(kat.ObjekatId == objekat.Id)
                    {      
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
                _provider.Connection.Unlink($"Objekat:{obj_id}");   
            }      
            _provider.Connection.Unlink($"Vlasnik:{id}");
            return Ok(id);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}