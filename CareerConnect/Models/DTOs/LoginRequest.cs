using System.ComponentModel.DataAnnotations;

namespace CareerConnect.Models.DTOs
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
