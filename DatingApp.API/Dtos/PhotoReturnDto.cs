using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Dtos
{
    public class PhotoReturnDto
    {
       
        public int Id {get;set;}

        public string Url {get;set;}

        public string Desctiption {get;set;}
        public string PublicId {get;set;}

        public DateTime DateAdded {get;set;}

        public bool isMain {get;set;}
        

    }
}