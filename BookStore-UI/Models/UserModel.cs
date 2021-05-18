using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Models
{
    public class RegistrationModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email Address")]
        public string EmailAddress { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage = "Password should be {2} to {1} chars", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name ="Confirm Password")]
        [StringLength(15, ErrorMessage = "Password should be {2} to {1} chars", MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
    }
    public class UserModel
    {

    }
}
