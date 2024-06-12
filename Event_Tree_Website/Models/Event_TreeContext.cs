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
        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<PersonalEvent> PersonalEvents { get; set; } = null!;
     
        public virtual DbSet<Slider> Sliders { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-13VMKLD;Database=Event_Tree;Trusted_Connection=True;TrustServerCertificate=\n true;");
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
                    .HasConstraintName("FK_personalEvent_users");
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

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

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

                entity.Property(e => e.Password)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Role).HasColumnName("role");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_at");

                entity.Property(e => e.Username)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("username");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
