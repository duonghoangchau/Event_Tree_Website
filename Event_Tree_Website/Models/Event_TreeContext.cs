using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Event_Tree_Website.Models
{
    public partial class Event_TreeContext : DbContext
    {
        public Event_TreeContext()
        {
        }

        public Event_TreeContext(DbContextOptions<Event_TreeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<Contribution> Contributions { get; set; } = null!;
        public virtual DbSet<DetailEvent> DetailEvents { get; set; } = null!;
        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<PersonalEvent> PersonalEvents { get; set; } = null!;
        public virtual DbSet<Premium> Premiums { get; set; } = null!;
        public virtual DbSet<Slider> Sliders { get; set; } = null!;
        public virtual DbSet<Tree> Trees { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=FUYANA;Database=Event_Tree;Trusted_Connection=True;TrustServerCertificate=\n true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.IdCategory)
                    .HasName("PK_catolory");

                entity.ToTable("category");

                entity.Property(e => e.IdCategory).HasColumnName("id_category");

                entity.Property(e => e.Hide).HasColumnName("hide");

                entity.Property(e => e.IdParent)
                    .HasMaxLength(255)
                    .HasColumnName("id_parent");

                entity.Property(e => e.Link)
                    .HasMaxLength(255)
                    .HasColumnName("link");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Order).HasColumnName("order");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comments");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.CmtContent).HasColumnName("cmt_content");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.Likes).HasColumnName("likes");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_comments_events");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_comments_users");
            });

            modelBuilder.Entity<Contribution>(entity =>
            {
                entity.ToTable("contributions");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Approval).HasColumnName("approval");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("date_time");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.EventName).HasColumnName("event_name");

                entity.Property(e => e.ImageCode).HasColumnName("image_code");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Contributions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_contributions_users");
            });

            modelBuilder.Entity<DetailEvent>(entity =>
            {
                entity.ToTable("detail_event");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EventId).HasColumnName("event_id");

                entity.Property(e => e.TreeId).HasColumnName("tree_id");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.DetailEvents)
                    .HasForeignKey(d => d.EventId)
                    .HasConstraintName("FK_detail_event_events");

                entity.HasOne(d => d.Tree)
                    .WithMany(p => p.DetailEvents)
                    .HasForeignKey(d => d.TreeId)
                    .HasConstraintName("FK_detail_event_trees");
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("date_time");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Detail).HasColumnName("detail");

                entity.Property(e => e.Hide).HasColumnName("hide");

                entity.Property(e => e.IdCategory).HasColumnName("id_category");

                entity.Property(e => e.ImageCode)
                    .HasMaxLength(255)
                    .HasColumnName("image_code");

                entity.Property(e => e.Link)
                    .HasMaxLength(255)
                    .HasColumnName("link");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .HasColumnName("status")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdCategoryNavigation)
                    .WithMany(p => p.Events)
                    .HasForeignKey(d => d.IdCategory)
                    .HasConstraintName("FK_events_catolory");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.ImageCode })
                    .HasName("PK_images_1");

                entity.ToTable("images");

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.ImageCode)
                    .HasMaxLength(20)
                    .HasColumnName("image_code");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Url).HasColumnName("url");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("menu");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Hide).HasColumnName("hide");

                entity.Property(e => e.Link).HasColumnName("link");

                entity.Property(e => e.MenuOrder).HasColumnName("menu_order");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("orders");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.OrderCode)
                    .HasMaxLength(20)
                    .HasColumnName("order_code");

                entity.Property(e => e.PremiumId).HasColumnName("premium_id");

                entity.Property(e => e.Total).HasColumnName("total");

                entity.Property(e => e.TypePayment).HasColumnName("type_payment");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Premium)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PremiumId)
                    .HasConstraintName("FK_orders_premiums");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_orders_users");
            });

            modelBuilder.Entity<PersonalEvent>(entity =>
            {
                entity.ToTable("personalEvent");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DateTime)
                    .HasColumnType("datetime")
                    .HasColumnName("date_time");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Detail).HasColumnName("detail");

                entity.Property(e => e.Hide).HasColumnName("hide");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.ImageCode)
                    .HasMaxLength(255)
                    .HasColumnName("image_code");

                entity.Property(e => e.Link)
                    .HasMaxLength(255)
                    .HasColumnName("link");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("updated_at");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.PersonalEvents)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_personalEvent_users");
            });

            modelBuilder.Entity<Premium>(entity =>
            {
                entity.ToTable("premiums");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.PremiumLevel).HasColumnName("premium_level");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.ValidityPeriod).HasColumnName("validity_period");
            });

            modelBuilder.Entity<Slider>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("SLIDER");

                entity.Property(e => e.Hide).HasColumnName("HIDE");

                entity.Property(e => e.IdSlide)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID_SLIDE");

                entity.Property(e => e.Img)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("IMG");

                entity.Property(e => e.Link)
                    .HasMaxLength(255)
                    .HasColumnName("LINK");

                entity.Property(e => e.Order).HasColumnName("ORDER");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("TITLE");
            });

            modelBuilder.Entity<Tree>(entity =>
            {
                entity.ToTable("trees");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.ImageCode)
                    .HasMaxLength(20)
                    .HasColumnName("image_code");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name")
                    .IsFixedLength();

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Trees)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_trees_users");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Avatar)
                    .HasMaxLength(20)
                    .HasColumnName("avatar");

                entity.Property(e => e.Birthday)
                    .HasColumnType("date")
                    .HasColumnName("birthday");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("created_at");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deleted_at");

                entity.Property(e => e.Email)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .HasMaxLength(100)
                    .HasColumnName("fullname");

                entity.Property(e => e.Gender).HasColumnName("gender");

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.PremiumDate)
                    .HasColumnType("datetime")
                    .HasColumnName("premium_date");

                entity.Property(e => e.PremiumId).HasColumnName("premium_id");

                entity.Property(e => e.Provide).HasColumnName("provide");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.Username)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("username");

                entity.HasOne(d => d.Premium)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.PremiumId)
                    .HasConstraintName("FK_users_premiums");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
