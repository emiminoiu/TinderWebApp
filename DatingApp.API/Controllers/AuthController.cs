using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repository, IConfiguration config, IMapper mapper)
        {
            this.repository = repository;
            this.config = config;
            _mapper = mapper;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {
            userDto.Username = userDto.Username.ToLower();
            if (await repository.UserExist(userDto.Username))
            {
                return BadRequest("Username already taken!");
            }

            var userToCreate = _mapper.Map<User>(userDto);

            var createdUser = await repository.Register(userToCreate, userDto.Password);

            var userToReturn = _mapper.Map<UserDetailedDto>(createdUser);
           // return CreatedAtAction("GetUser", userToReturn);
            return CreatedAtAction("GetUser","Users", new {id = createdUser.Id}, userToReturn);
       //   return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.Id}, userToReturn);

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
                 Expires = DateTime.Now.AddDays(100),
                 SigningCredentials = creds
             };

             var tokenHandler = new JwtSecurityTokenHandler();
             var token = tokenHandler.CreateToken(tokenDescriptor);

             var user = _mapper.Map<UserListDto>(userRepo);
             return Ok(new{
                 token = tokenHandler.WriteToken(token),
                 user
             });
        }

    }
}