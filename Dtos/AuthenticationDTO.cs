using System.ComponentModel.DataAnnotations;
namespace TodoApp.Dtos
{
    public class REGISTER_USER_REQUEST_DTO
    {
        [Required]
        [MaxLength(20)]
        public string Username { get; set; }
        [Required]
        [MaxLength(18)]
        public string Password { get; set; } 
        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }        
    }

    public class ACCESS_USER_REQUEST_DTO
    {
        [Required]
        [MaxLength(20)]
        public string Username { get; set; }
        [Required]
        [MaxLength(18)]
        public string Password { get; set; } 
    }

    public class ACCESS_USER_RESPONSE_DTO
    {
        public string Token { get; set; }
        public System.DateTime Expiration { get; set; } 
    }
}