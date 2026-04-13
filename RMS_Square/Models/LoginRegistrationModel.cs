using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RMS_Square.Areas.SA.Models.BEL;

namespace RMS_Square.Models
{
    public class LoginRegistrationModel : UserInRoleBEL
    {

        public string UserID { get; set; }
        public string Password { get; set; }



    
    }
}