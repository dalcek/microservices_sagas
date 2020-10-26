using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UsersMicroservice.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersMicroservice.Models.ContextData
{
    public class ApplicationUsersContext : IdentityDbContext<User, Role, string>
    {
        public ApplicationUsersContext(DbContextOptions<ApplicationUsersContext> options) : base(options)
        {

        }

        public DbSet<User> ApplicationUsers { get; set; }
    }
}
