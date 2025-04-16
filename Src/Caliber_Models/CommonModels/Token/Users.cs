using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caliber_Components.Authentication.Token
{
   //Represents a model for login user credentials.
    public class LoginUserModel
    {
        //Gets or sets the user name
       
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Login UserId")]
        [DisplayName("User Id")]
        public string? UserID { get; set; }

        // Gets or sets the password.
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Login Password")]
        [DisplayName("Password")]
        public string? Passsword { get; set; }
    }
    public class Users
    {
        public static List<LoginUserModel> UsersList { get; set; } = new();
    }
}
