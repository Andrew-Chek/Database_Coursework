using Microsoft.EntityFrameworkCore;
namespace RepoCode
{
    public partial class courseWorkdbContext : DbContext
    {
        public courseWorkdbContext()
        {
        }

        public courseWorkdbContext(DbContextOptions<courseWorkdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Brand> brands { get; set; }
        public virtual DbSet<Category> categories { get; set; }
        public virtual DbSet<Item> items { get; set; }
        public virtual DbSet<Moderator> moderators { get; set; }
        public string connectionString { get; set; }
        public courseWorkdbContext(string connectionString)
        {
            this.connectionString = connectionString;
            Database.EnsureCreated();
        }
        public void CreateSubscription()
        {
            var result = -1;
            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM pg_subscription";
                this.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }
                }
                this.Database.CloseConnection();
            }
            if (result == 0)
            {
                this.Database.ExecuteSqlRaw("CREATE SUBSCRIPTION logical_sub\n" +
                "CONNECTION 'host=localhost port=5432 user=postgres password=2003Lipovetc dbname=courseWorkdb'\n" +
                "PUBLICATION logical_pub\n" +
                "WITH(create_slot = false, slot_name = 'logical_slot');");
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(connectionString);
            }
        }
        public void CheckPublication()
        {
            var result = -1;
            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM pg_publication";
                this.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                    if (reader.Read())
                        result = reader.GetInt32(0);
                this.Database.CloseConnection();
            }

            if (result == 0)
                this.Database.ExecuteSqlRaw("CREATE PUBLICATION logical_pub FOR ALL TABLES;");

            using (var command = this.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT COUNT(*) FROM pg_replication_slots";
                this.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                    if (reader.Read())
                        result = reader.GetInt32(0);
                this.Database.CloseConnection();
            }

            if (result == 0)
                this.Database.ExecuteSqlRaw("SELECT * FROM pg_create_logical_replication_slot('logical_slot', 'pgoutput');");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.brand_id)
                    .HasName("brends_pkey");

                entity.ToTable("brands");

                entity.Property(e => e.brand_id)
                    .HasColumnName("brand_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.brand)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("brand");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.category_id)
                    .HasName("categories_pkey");

                entity.ToTable("categories");

                entity.Property(e => e.category_id)
                    .HasColumnName("category_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.category)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("category");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.item_id)
                    .HasName("items_pkey");

                entity.ToTable("items");

                entity.Property(e => e.item_id)
                    .HasColumnName("item_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.brand_id).HasColumnName("brand_id");

                entity.Property(e => e.category_id).HasColumnName("category_id");

                entity.Property(e => e.cost).HasColumnName("cost");

                entity.Property(e => e.createYear).HasColumnName("createYear");

                entity.Property(e => e.name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.HasOne(d => d.brand)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.brand_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("brand_id");

                entity.HasOne(d => d.category)
                    .WithMany(p => p.items)
                    .HasForeignKey(d => d.category_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("category_id");
            });

            modelBuilder.Entity<Moderator>(entity =>
            {
                entity.HasKey(e => e.mod_id)
                    .HasName("moderators_pkey");

                entity.ToTable("moderators");

                entity.Property(e => e.mod_id)
                    .HasColumnName("mod_id")
                    .UseIdentityAlwaysColumn();

                entity.Property(e => e.name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.password)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("password");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
