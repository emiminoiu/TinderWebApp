using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repository;
        private readonly IConfiguration config;
        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            this.repository = repository;
            this.config = config;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {
            userDto.Username = userDto.Username.ToLower();
            if (await repository.UserExist(userDto.Username))
            {
                return BadRequest("Username already taken!");
            }

            var userToCreate = new User
            {
                Username = userDto.Username
            };

            var createdUser = await repository.Register(userToCreate, userDto.Password);

            return StatusCode(201);

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userDto)
        {
             var userRepo = await repository.Login(userDto.Username, userDto.Password);
             if(userRepo == null)
             {
                 return Unauthorized();
             }
             var claims = new[]
             {
                 new Claim(ClaimTypes.NameIdentifier, userRepo.Id.ToString()),
                 new Claim(ClaimTypes.Name, userRepo.Username)
             };
             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));

             var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

             var tokenDescriptor = new SecurityTokenDescriptor{
                 Subject = new ClaimsIdentity(claims),
                 Expires = DateTime.Now.AddDays(1),
                 SigningCredentials = creds
             };

             var tokenHandler = new JwtSecurityTokenHandler();
             var token = tokenHandler.CreateToken(tokenDescriptor);
             return Ok(new{
                 token = tokenHandler.WriteToken(token)
             });
        }

    }
}