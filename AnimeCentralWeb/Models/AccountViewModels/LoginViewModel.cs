using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username obligatoriu.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Parola obligatorie.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Retine?")]
        public bool RememberMe { get; set; }
    }
}
