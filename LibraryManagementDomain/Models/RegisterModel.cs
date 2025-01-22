using System.ComponentModel.DataAnnotations;

namespace LibraryManagementDomain.Models
{
    public class RegisterModel
    {
        [Required]
        //[PasswordStrength(6)] // Ensure password is at least 6 characters long with one special character
        public string Password { get; set; }
        public int RoleId { get; set; } 
    }
}
