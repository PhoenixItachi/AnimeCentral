using System.ComponentModel.DataAnnotations;

namespace AnimeCentralWeb.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username obligatoriu.")]
        [Display(Prompt = "Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Parola obligatorie.")]
        [DataType(DataType.Password)]
        [Display(Prompt = "Parola")]
        public string Password { get; set; }

        [Display(Name = "Retine?")]
        public bool RememberMe { get; set; }
    }
}
