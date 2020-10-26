using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroservice.Models.Model
{
    public class LogInModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IdToken { get; set; }
    }

    public class SocialLogInModel
    {
        public string UserId { get; set; }
        public string Provider { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PictureUrl { get; set; }
        public string IdToken { get; set; }
        public string AuthToken { get; set; }
    }
}
