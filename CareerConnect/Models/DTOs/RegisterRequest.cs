using System.ComponentModel.DataAnnotations;

namespace CareerConnect.Models.DTOs
{
    public class RegisterRequest
    {
        public required string Name { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        public required UserType UserType { get; set; }
    }
}
