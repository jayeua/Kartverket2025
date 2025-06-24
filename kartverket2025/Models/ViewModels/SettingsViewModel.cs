using System.ComponentModel.DataAnnotations;

namespace kartverket2025.Models.ViewModels
{
    public class SettingsViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}