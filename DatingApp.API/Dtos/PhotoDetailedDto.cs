using System;

namespace DatingApp.API.Dtos
{
    public class PhotoDetailedDto
    {
        public int Id {get;set;}

        public string Url {get;set;}

        public string Desctiption {get;set;}

        public DateTime DateAdded {get;set;}

        public bool isMain {get;set;}

    }
}