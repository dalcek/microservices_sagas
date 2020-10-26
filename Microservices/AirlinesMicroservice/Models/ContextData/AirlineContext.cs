using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AirlinesMicroservice.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirlinesMicroservice.Models.ContextData
{
    public class AirlineContext : DbContext
    {
        public AirlineContext(DbContextOptions<AirlineContext> options) : base(options)
        {

        }

        public DbSet<Airline> AirlineCompanies { get; set; }
    }
}
