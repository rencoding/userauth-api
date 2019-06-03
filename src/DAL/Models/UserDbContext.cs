using Microsoft.EntityFrameworkCore;

namespace GenesisTest.DAL.Models
{
    /// <summary>
    /// UserDbContext 
    /// </summary>
    public class UserDbContext: DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options):base(options)
        {
        }

        /// <summary>
        /// Users DbSet
        /// </summary>
        public virtual DbSet<User> Users { get; set; }

        /// <summary>
        /// UserTelephones DbSet
        /// </summary>
        public virtual DbSet<UserTelephone> UserTelephones { get; set; }

    }
}
