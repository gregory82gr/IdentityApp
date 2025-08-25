using Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Api.Data
{
    public class Context :IdentityDbContext<User>
    {
        public Context(DbContextOptions options) : base(options) {
        
        }
    }
}
