using FileStorage.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FileStorage.Domain
{
    public class DataDbContext : IdentityDbContext<ApplicationUser>
    {
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {

        }
    }
}
