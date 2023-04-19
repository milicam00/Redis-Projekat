using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Redis.OM.Searching;
using Redis.OM.Skeleton.Model;

namespace Redis.OM.Skeleton.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private RedisCollection<Korisnik> _korisnik;
    private RedisCollection<Vlasnik> _vlasnik;
    private RedisCollection<Dostavljac> _dostavljac;
    private RedisConnectionProvider _provider;
    private IConfiguration _configuration;

    public AuthController(RedisConnectionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration;
        _korisnik = (RedisCollection<Korisnik>)provider.RedisCollection<Korisnik>();
        _vlasnik = (RedisCollection<Vlasnik>)provider.RedisCollection<Vlasnik>();
        _dostavljac = (RedisCollection<Dostavljac>)provider.RedisCollection<Dostavljac>();
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] CovekDto request)
    {
        CreatePasswordHash(request.Password, out byte[] passHash, out byte[] passSalt);
        var str = Convert.ToBase64String(passHash);
        var pom = _korisnik.Where(x => x.Userime == request.UserName).FirstOrDefault();
        if (pom != null){
            return BadRequest("Vec postoji");
        }
        
        if (request.tip == 1){
            var osoba = new Korisnik();
            osoba.Userime = request.UserName;
            osoba.Passwordhash = Convert.ToBase64String(passHash);
            osoba.Passwordsalt = Convert.ToBase64String(passSalt);
            _korisnik.InsertAsync((Korisnik)osoba);
            return Ok(osoba);
        }
        else if (request.tip == 2){
            var osoba = new Vlasnik();
            osoba.Userime = request.UserName;
            osoba.Passwordhash = Convert.ToBase64String(passHash);
            osoba.Passwordsalt = Convert.ToBase64String(passSalt);
            _vlasnik.InsertAsync(osoba);
            return Ok(osoba);
        }
        else if (request.tip == 3){
            var osoba = new Dostavljac();
            osoba.Userime = request.UserName;
            osoba.Passwordhash = Convert.ToBase64String(passHash);
            osoba.Passwordsalt = Convert.ToBase64String(passSalt);
            _dostavljac.InsertAsync(osoba);
            return Ok(osoba);
        }
        return BadRequest("Ne odgovarajuci tip");
    }


    [HttpPost("Login")]
    public IActionResult Login([FromBody] CovekDto request)
    {
        try 
        {
            if (request.tip == 1){
                var osoba = new Korisnik();
                osoba = _korisnik.Where( x => x.Userime == request.UserName).FirstOrDefault();
                if (!VerifyPasswordHash(request.Password, osoba.Passwordhash, osoba.Passwordsalt))
                {
                    return BadRequest("Pogresana sifra ili username");
                }
                string token = CreateClaimsKorisnik(osoba);
                return Ok(token);
            }
            else if (request.tip == 2){
                var osoba = new Vlasnik();
                osoba = _vlasnik.Where( x => x.Userime == request.UserName).FirstOrDefault();
                 if (!VerifyPasswordHash(request.Password, osoba.Passwordhash, osoba.Passwordsalt))
                {
                    return BadRequest("Pogresana sifra");
                }
                string token = CreateClaimsVlasnik(osoba);
                return Ok(token);
            }
            else if (request.tip == 3){
                var osoba = new Dostavljac();
                osoba = _dostavljac.Where( x => x.Userime == request.UserName).FirstOrDefault();
                if (!VerifyPasswordHash(request.Password, osoba.Passwordhash, osoba.Passwordsalt))
                {
                    return BadRequest("Pogresana sifra");
                }
                string token = CreateClaimsDostavljac(osoba);
                return Ok(token);
            }
            else{
                return BadRequest("Ne postoji osoba s tim imenom");
            }
        }
        catch(Exception e)
        {
            return Ok(e.Message);
        }
    }

    private string CreateClaimsKorisnik(Korisnik osoba)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, osoba.Userime),
            new Claim(ClaimTypes.Role, "korisnik")
        };
        return CreateToken(claims);
    }

    private string CreateClaimsVlasnik(Vlasnik osoba)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, osoba.Userime),
            new Claim(ClaimTypes.Role, "vlasnik")
        };
        return CreateToken(claims);
    }
    private string CreateClaimsDostavljac(Dostavljac osoba)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, osoba.Userime),
            new Claim(ClaimTypes.Role, "dostavljac")
        };
        return CreateToken(claims);
    }
    private string CreateToken(List<Claim> claims)
    {
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(10),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
    }
    private  bool VerifyPasswordHash(string pass, string passHash, string passSalt)
    {
        using(var hmac = new HMACSHA512(Convert.FromBase64String(passSalt)))
        {
            var ComputeHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(pass)));
            if (ComputeHash == passHash){
                return true;
            } 
            else{
                return false;
            }
        }
    }
}