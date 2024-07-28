using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RESTfulAPI.Models;

public partial class RestdatabaseContext : DbContext
{
    public RestdatabaseContext()
    {
    }

    public RestdatabaseContext(DbContextOptions<RestdatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Owner> Owners { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=TUF293;Database=RESTDatabase;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>(entity =>
        {
            entity.ToTable("Member");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.Password)
                .HasMaxLength(4000)
                .IsFixedLength();
            entity.Property(e => e.Username)
                .HasMaxLength(15)
                .IsFixedLength();

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Member)
                .HasForeignKey<Member>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Owner_Member");
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.ToTable("Owner");

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("ID");
            entity.Property(e => e.Fullname)
                .HasMaxLength(25)
                .IsFixedLength();
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Patent);

            entity.ToTable("Vehicle");

            entity.Property(e => e.Patent)
                .HasMaxLength(6)
                .IsFixedLength();
            entity.Property(e => e.Brand)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.Driver)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Model)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.Type)
                .HasMaxLength(15)
                .IsFixedLength();

            entity.HasOne(d => d.DriverNavigation).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.Driver)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Owner_Vehicle");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
