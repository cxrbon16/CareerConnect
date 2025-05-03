using System.ComponentModel.DataAnnotations;

namespace CareerConnect.Models.DTOs
{
    public class RegisterRequest
    {
        [Required]
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public UserType UserType { get; set; }
    }
}
