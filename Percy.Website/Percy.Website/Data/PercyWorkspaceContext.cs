
using Microsoft.EntityFrameworkCore;

namespace Percy.Website.Data
{
    public class PercyWorkspaceContext : DbContext
    {
        public PercyWorkspaceContext(
            DbContextOptions<PercyWorkspaceContext> options)
            : base(options)
        {
        }

        // Animal Crossing museum catalog.
        public DbSet<MuseumItem> MuseumItems
            => Set<MuseumItem>();

        // Player and town museum donations.
        public DbSet<MuseumDonation> MuseumDonations
            => Set<MuseumDonation>();

        // Fishing locations.
        public DbSet<SpawnLocation> SpawnLocations
            => Set<SpawnLocation>();

        // Fish-to-location relationships.
        public DbSet<FishSpawnLocation> FishSpawnLocations
            => Set<FishSpawnLocation>();

        // Fish monthly and hourly availability.
        public DbSet<FishAvailability> FishAvailabilities
            => Set<FishAvailability>();

        // Insect monthly and hourly availability.
        public DbSet<InsectAvailability> InsectAvailabilities
            => Set<InsectAvailability>();

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Museum catalog.
            modelBuilder.Entity<MuseumItem>(entity =>
            {
                entity.ToTable("MuseumItems", "ac");

                entity.HasKey(e => e.MuseumItemID);

                entity.Property(e => e.ItemName)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasColumnType("nvarchar(max)");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            });

            // Museum donations.
            modelBuilder.Entity<MuseumDonation>(entity =>
            {
                entity.ToTable("MuseumDonations", "ac");

                entity.HasKey(e => e.DonationID);

                entity.Property(e => e.DonationDate)
                    .HasColumnType("date");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime2(0)")
                    .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasIndex(e => new
                {
                    e.TownID,
                    e.MuseumItemID
                }).IsUnique();
            });

            // Fishing location reference table.
            modelBuilder.Entity<SpawnLocation>(entity =>
            {
                entity.ToTable("SpawnLocations", "ac");

                entity.HasKey(e => e.LocationID);

                entity.Property(e => e.LocationName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            });

            // Fish spawning location junction table.
            modelBuilder.Entity<FishSpawnLocation>(entity =>
            {
                entity.ToTable("FishSpawnLocations", "ac");

                entity.HasKey(e => new
                {
                    e.MuseumItemID,
                    e.LocationID
                });

                entity.HasOne<MuseumItem>()
                    .WithMany()
                    .HasForeignKey(e => e.MuseumItemID);

                entity.HasOne<SpawnLocation>()
                    .WithMany()
                    .HasForeignKey(e => e.LocationID);
            });

            // Fish availability.
            modelBuilder.Entity<FishAvailability>(entity =>
            {
                entity.ToTable("FishAvailability", "ac");

                entity.HasKey(e => e.AvailabilityID);

                entity.Property(e => e.AvailabilityID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.StartTime)
                    .HasColumnType("time");

                entity.Property(e => e.EndTime)
                    .HasColumnType("time");

                entity.Property(e => e.WeatherCondition)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.LastUpdated)
                    .HasColumnType("datetime2");

                entity.HasOne<MuseumItem>()
                    .WithMany()
                    .HasForeignKey(e => e.MuseumItemID);
            });

            // Insect availability, including island-only species.
            modelBuilder.Entity<InsectAvailability>(entity =>
            {
                entity.ToTable("InsectAvailability", "ac");

                entity.HasKey(e => e.AvailabilityID);

                entity.Property(e => e.AvailabilityID)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.StartTime)
                    .HasColumnType("time");

                entity.Property(e => e.EndTime)
                    .HasColumnType("time");

                entity.Property(e => e.WeatherCondition)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.LastUpdated)
                    .HasColumnType("datetime2");

                entity.HasOne<MuseumItem>()
                    .WithMany()
                    .HasForeignKey(e => e.MuseumItemID);

                entity.HasOne<SpawnLocation>()
                    .WithMany()
                    .HasForeignKey(e => e.LocationID);
            });
        }
    }

    public class MuseumItem
    {
        public int MuseumItemID { get; set; }

        public int GameID { get; set; }

        public int CategoryID { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? SellPrice { get; set; }

        public bool IsActive { get; set; }

        public int? FossilGroupID { get; set; }

        // Fish-specific attribute.
        public int? ShadowSize { get; set; }
    }

    public class MuseumDonation
    {
        public int DonationID { get; set; }

        public int TownID { get; set; }

        public int? PlayerID { get; set; }

        public int MuseumItemID { get; set; }

        public DateTime? DonationDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class SpawnLocation
    {
        public int LocationID { get; set; }

        public string LocationName { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }

    public class FishSpawnLocation
    {
        public int MuseumItemID { get; set; }

        public int LocationID { get; set; }
    }

    public class FishAvailability
    {
        public int AvailabilityID { get; set; }

        public int MuseumItemID { get; set; }

        public byte Month { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsAllDay { get; set; }

        public string WeatherCondition { get; set; }
            = string.Empty;

        public int? SourceID { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class InsectAvailability
    {
        public int AvailabilityID { get; set; }

        public int MuseumItemID { get; set; }

        public int LocationID { get; set; }

        public byte Month { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public bool IsAllDay { get; set; }

        public string WeatherCondition { get; set; }
            = string.Empty;

        public int? SourceID { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsIslandOnly { get; set; }
    }
}
