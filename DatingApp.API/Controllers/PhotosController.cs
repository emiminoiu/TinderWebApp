using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{

    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        private Cloudinary _cloudinary;
        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
       {
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;
            
            Account acc = new Account (
                _cloudinaryConfig.Value.CloudName,
                 cloudinaryConfig.Value.ApiKey,
                 cloudinaryConfig.Value.ApiSecret

            );
            _cloudinary = new Cloudinary(acc);
        }
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, PhotoCreationDto photoCreation)
        {
            bool success = false;
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userRepo = await _repo.GetUser(userId);
            var file = photoCreation.file;
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0)
            {
                using(var stream = file.OpenReadStream()) {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")                     
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoCreation.Url = uploadResult.Uri.ToString();
            photoCreation.PublicId = uploadResult.PublicId;
            var photo = _mapper.Map<Photo>(photoCreation);
            if(!userRepo.Photos.Any(u => u.isMain))    
                photo.isMain = true;
            userRepo.Photos.Add(photo);
            _repo.SaveAll();
            success = true;
            if(success){
            return Ok();
            }
            return BadRequest("Could not add the photo");
        }
    }
}