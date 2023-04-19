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
public class KorisnikController : ControllerBase
{
    private RedisCollection<Korisnik> _korisnik;
    private RedisConnectionProvider _provider;
    private IConfiguration _configuration;

    public KorisnikController(RedisConnectionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
          _configuration = configuration;
        _korisnik = (RedisCollection<Korisnik>)provider.RedisCollection<Korisnik>();
    }
    // [HttpPost]
    // public async Task<Korisnik> AddKorisnik ([FromBody] Korisnik kori)
    // {   
    //     await _korisnik.InsertAsync(kori);
    //     return kori;
    // }

    [HttpGet("GetAll")]
    public IActionResult GetAllKorisnik()
    {
        try
        {
            var pom = _korisnik.ToList();
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
            var pom = _korisnik.Where(x=> x.Userime == userime).FirstOrDefault();
            if(pom != null){
                return Ok(pom);
            }
            return BadRequest("Ne postoji taj korisnik");
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }
    }

    [HttpGet("Get/{id}"), Authorize(Roles = "korisnik")]
    public IActionResult GetDostave([FromRoute] string id)
    {
        try
        {
            var _dostave = (RedisCollection<Dostava>)_provider.RedisCollection<Dostava>();
            var dost = _dostave.ToList().Where(x => x.KorisnikId == id);
            if(dost != null){
                return Ok(dost);
            }
            return BadRequest("Nemate niti jednu aktivnu dostavu");
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }
    }

    [HttpPut("Update/{id}/{ime}/{prezime}/{email}/{adresa}/{brtelefona}"), Authorize(Roles = "korisnik")]
    public IActionResult Update([FromRoute] string id, [FromRoute] string ime, [FromRoute] string prezime, [FromRoute] string email, [FromRoute] string adresa, [FromRoute] string brtelefona )
    {
        try{
            var korisnik = _korisnik.FindById(id);
            if (korisnik != null)
            {
                korisnik.Ime = ime;
                korisnik.Prezime = prezime;
                korisnik.Email = email;
                korisnik.Adressa = adresa;
                korisnik.BrTelefona = brtelefona;
            }    
            _korisnik.Save();
            return Ok(korisnik);
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }   
    }


    [HttpDelete("Delete/{id}"), Authorize(Roles = "korisnik")]
    public IActionResult DeleteKorisnik([FromRoute] string id)
    {
        try
        {   
            _provider.Connection.Unlink($"Korisnik:{id}");
            return Ok(id);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}