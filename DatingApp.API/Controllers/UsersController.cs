using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
      
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;

        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
                     
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
  
            var userRepo = await _repo.GetUser(currentUserId);

            userParams.UserId = currentUserId;

            if(string.IsNullOrEmpty(userParams.Gender)) {
                userParams.Gender = userRepo.Gender == "male" ? "female" : "male";
            }

            var users = await _repo.GetUsers(userParams);
             
            var userToReturn = _mapper.Map<IEnumerable<UserDetailedDto>>(users);

            Response.AddPagination(users.CurrentPage,
             users.PageSize, users.TotalCount, users.TotalPages);
            
            return Ok(userToReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);

            var userToReturn = _mapper.Map<UserDetailedDto>(user);

            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userUpdate)
        {
            bool success = false;
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userRepo = await _repo.GetUser(id);
            _mapper.Map(userUpdate, userRepo);
            _repo.SaveAll();
            success = true;
            if(success) {
            return NoContent();
            }
            throw new Exception("Updating User Failed!");
        }
        [HttpPost("{id}/like/{recipientId}")]

        public async Task<IActionResult> LikeUser(int id, int recipientId) 
        {
            Like newlike = new Like();
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var like = await _repo.GetLike(id, recipientId);

            if(like != null) {
                return BadRequest("You already liked this photo!");
            }

            if(await _repo.GetUser(recipientId) == null) {
                return NotFound();
            }

            newlike.LikerId = id;
            newlike.LikeeId = recipientId;
            _repo.Add<Like>(newlike);
            _repo.SaveAll();
            return Ok();
        }
    }
}