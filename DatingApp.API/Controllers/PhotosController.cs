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

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                 cloudinaryConfig.Value.ApiKey,
                 cloudinaryConfig.Value.ApiSecret

            );
            _cloudinary = new Cloudinary(acc);
        }


        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoReturnDto>(photoRepo);

            return Ok(photo);
        }


        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoCreationDto photoCreation)
        {
            bool success = false;
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userRepo = await _repo.GetUser(userId);
            var file = photoCreation.file;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
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
            if (!userRepo.Photos.Any(u => u.isMain))
                photo.isMain = true;
            userRepo.Photos.Add(photo);

            _repo.SaveAll();
            success = true;
            if (success)
            {
                var photoReturn = _mapper.Map<PhotoReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoReturn);
            }
            return BadRequest("Could not add the photo");
        }


        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }
            var photoRepo = await _repo.GetPhoto(id);

            if (photoRepo.isMain)
                return BadRequest("Already main photo");

            var currentMainPhoto = await _repo.GetMainPhoto(userId);

            currentMainPhoto.isMain = false;

            photoRepo.isMain = true;

            _repo.SaveAll();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }
            var photoRepo = await _repo.GetPhoto(id);

            if (photoRepo.isMain)
                return BadRequest("You cannot delete your main photo");
            if (photoRepo.PublicId != null)
            {

                var deleteParams = new DeletionParams(photoRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if (result.Result == "ok")
                {
                    _repo.Delete(photoRepo);
                }
            }
            if (photoRepo.PublicId == null)
            {
                _repo.Delete(photoRepo);
            }

            _repo.SaveAll();

            return Ok();
        }

    }
}

