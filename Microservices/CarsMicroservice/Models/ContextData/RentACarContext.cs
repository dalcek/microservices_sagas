using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarsMicroservice.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarsMicroservice.Models.ContextData
{
    public class RentACarContext : DbContext
    {
        public RentACarContext(DbContextOptions<RentACarContext> options) : base(options)
        {

        }

        public DbSet<RentACar> RentACarCompanies { get; set; }
    }
}
