using Microsoft.EntityFrameworkCore;
using Netsphere.Database.Auth;

namespace Netsphere.Database
{
    public class AuthContext : DbContext
    {
        public DbSet<AccountEntity> Accounts { get; set; }
        public DbSet<BanEntity> Bans { get; set; }
        public DbSet<NicknameHistoryEntity> Nicknames { get; set; }
        public DbSet<LoginHistoryEntity> LoginHistory { get; set; }

        public AuthContext(DbContextOptions<AuthContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountEntity>()
                .HasIndex(x => x.Username).IsUnique();

            modelBuilder.Entity<AccountEntity>()
                .HasIndex(x => x.Nickname).IsUnique();
        }
    }
}
