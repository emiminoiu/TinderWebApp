using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserRegisterDto
    {
        [Required]
      
        public string Username {get;set;}

        [Required]
        [StringLength(9, MinimumLength = 5,ErrorMessage="You must specify a passoword between 5 and 9 characters!")]
        public string Password {get;set;}
    }
}