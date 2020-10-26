using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroservice.Models.Model
{
    public class RegisterModel
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string passport { get; set; }
        public string birthday { get; set; }
    }
}
