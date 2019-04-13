using Microsoft.EntityFrameworkCore;
using Netsphere.Database.Game;

namespace Netsphere.Database
{
    public class GameContext : DbContext
    {
        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<PlayerCharacterEntity> PlayerCharacters { get; set; }
        public DbSet<PlayerDenyEntity> PlayerIgnores { get; set; }
        public DbSet<PlayerItemEntity> PlayerItems { get; set; }
        public DbSet<PlayerMailEntity> PlayerMails { get; set; }
        public DbSet<PlayerSettingEntity> PlayerSettings { get; set; }
        public DbSet<ShopEffectGroupEntity> EffectGroups { get; set; }
        public DbSet<ShopEffectEntity> Effects { get; set; }
        public DbSet<ShopPriceGroupEntity> PriceGroups { get; set; }
        public DbSet<ShopPriceEntity> Prices { get; set; }
        public DbSet<ShopItemEntity> Items { get; set; }
        public DbSet<ShopItemInfoEntity> ItemInfos { get; set; }
        public DbSet<ShopVersionEntity> ShopVersion { get; set; }
        public DbSet<StartItemEntity> StartItems { get; set; }
        public DbSet<LevelRewardEntity> LevelRewards { get; set; }

        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopEffectGroupEntity>()
                .HasIndex(x => x.Name).IsUnique();

            modelBuilder.Entity<ShopPriceGroupEntity>()
                .HasIndex(x => x.Name).IsUnique();

            modelBuilder.Entity<PlayerDenyEntity>()
                .HasOne(x => x.Player)
                .WithMany(x => x.Ignores);

            modelBuilder.Entity<PlayerDenyEntity>()
                .HasOne(x => x.DenyPlayer);

            modelBuilder.Entity<PlayerMailEntity>()
                .HasOne(x => x.Player)
                .WithMany(x => x.Inbox);

            modelBuilder.Entity<PlayerMailEntity>()
                .HasOne(x => x.SenderPlayer);
        }
    }
}
