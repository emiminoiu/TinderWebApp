using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Dtos
{
    public class PhotoCreationDto
    {
        public string Url {get;set;}
        public IFormFile file {get;set;}

        public string Description { get; set; }
        public string PublicId { get; set; }

        public DateTime DateAdded { get; set; }
        
        public PhotoCreationDto()
        {
            DateAdded = DateTime.Now;
        }
        

    }
}