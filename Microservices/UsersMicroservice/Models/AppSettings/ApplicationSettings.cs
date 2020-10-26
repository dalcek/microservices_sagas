using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroservice.Models.AppSettings
{
    public class ApplicationSettings
    {
        public string JWT_Secret { get; set; }
        public string Client_URL { get; set; }
    }
}
