using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Reporting.Common.Models {
    public class LoginRequest {
        [Required]
        public string Username { get; set; }

        [Required]
        public string UserID { get; set; }//We have created predefined users for simplicity, so we use it instead of hashed password in demo purposes

        [Required]
        public string Password { get; set; }
    }
}
