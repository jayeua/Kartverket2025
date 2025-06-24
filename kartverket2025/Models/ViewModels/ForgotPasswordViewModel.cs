using System.ComponentModel.DataAnnotations;

namespace kartverket2025.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
