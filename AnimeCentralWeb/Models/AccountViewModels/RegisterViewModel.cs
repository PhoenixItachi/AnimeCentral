using AnimeCentralWeb.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AnimeCentralWeb.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = AnimeUtils.FormErrorRequiredMessage)]
        [StringLength(100, ErrorMessage = "{0} trebuie sa fie intre {2} si {1} caractere.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        [RegularExpression(".+[0-9].+", ErrorMessage = "Parola trebuie sa contina cel putin un numar.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirma Parola")]
        [Compare("Password", ErrorMessage = "Parolele trebuie sa fie identice.")]
        public string ConfirmPassword { get; set; }

        public IFormFile Image { get; set; }
    }
}
