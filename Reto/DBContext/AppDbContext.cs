using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Reto.Models;

namespace Reto.DBContext;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actividad> Actividads { get; set; }

    public virtual DbSet<Debilidad> Debilidads { get; set; }

    public virtual DbSet<Habilidad> Habilidads { get; set; }

    public virtual DbSet<Heroe> Heroes { get; set; }

    public virtual DbSet<Lucha> Luchas { get; set; }

    public virtual DbSet<Patrocinador> Patrocinadors { get; set; }

    public virtual DbSet<Relacion> Relacions { get; set; }

    public virtual DbSet<Villano> Villanos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-4I9M249D;Initial Catalog=The_Guardians_of_the_Globe;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actividad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Activida__3214EC07A2874BC1");

            entity.HasOne(d => d.Heroe).WithMany(p => p.Actividads).HasConstraintName("FK__Actividad__Heroe__4AB81AF0");
        });

        modelBuilder.Entity<Debilidad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Debilida__3214EC07C85BF27B");
        });

        modelBuilder.Entity<Habilidad>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Habilida__3214EC072854AD98");
        });

        modelBuilder.Entity<Heroe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Heroe__3214EC07484FF4B0");

            entity.HasMany(d => d.Debilidads).WithMany(p => p.Heroes)
                .UsingEntity<Dictionary<string, object>>(
                    "HeroeDebilidad",
                    r => r.HasOne<Debilidad>().WithMany()
                        .HasForeignKey("DebilidadId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Heroe_Deb__Debil__4222D4EF"),
                    l => l.HasOne<Heroe>().WithMany()
                        .HasForeignKey("HeroeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Heroe_Deb__Heroe__412EB0B6"),
                    j =>
                    {
                        j.HasKey("HeroeId", "DebilidadId").HasName("PK__Heroe_De__B400AE382BFDFF1F");
                        j.ToTable("Heroe_Debilidad");
                    });

            entity.HasMany(d => d.Habilidads).WithMany(p => p.Heroes)
                .UsingEntity<Dictionary<string, object>>(
                    "HeroeHabilidad",
                    r => r.HasOne<Habilidad>().WithMany()
                        .HasForeignKey("HabilidadId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Heroe_Hab__Habil__3E52440B"),
                    l => l.HasOne<Heroe>().WithMany()
                        .HasForeignKey("HeroeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Heroe_Hab__Heroe__3D5E1FD2"),
                    j =>
                    {
                        j.HasKey("HeroeId", "HabilidadId").HasName("PK__Heroe_Ha__0E63132DA32CD0F2");
                        j.ToTable("Heroe_Habilidad");
                    });

            entity.HasMany(d => d.Patrocinadors).WithMany(p => p.Heroes)
                .UsingEntity<Dictionary<string, object>>(
                    "HeroePatrocinador",
                    r => r.HasOne<Patrocinador>().WithMany()
                        .HasForeignKey("PatrocinadorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Heroe_Pat__Patro__5070F446"),
                    l => l.HasOne<Heroe>().WithMany()
                        .HasForeignKey("HeroeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Heroe_Pat__Heroe__4F7CD00D"),
                    j =>
                    {
                        j.HasKey("HeroeId", "PatrocinadorId").HasName("PK__Heroe_Pa__1FFDE02B81096C5A");
                        j.ToTable("Heroe_Patrocinador");
                    });

            entity.HasMany(d => d.Relacions).WithMany(p => p.Heroes)
                .UsingEntity<Dictionary<string, object>>(
                    "HeroeRelacion",
                    r => r.HasOne<Relacion>().WithMany()
                        .HasForeignKey("RelacionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Heroe_Rel__Relac__59FA5E80"),
                    l => l.HasOne<Heroe>().WithMany()
                        .HasForeignKey("HeroeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Heroe_Rel__Heroe__59063A47"),
                    j =>
                    {
                        j.HasKey("HeroeId", "RelacionId").HasName("PK__Heroe_Re__5D8313E4DBE8A92D");
                        j.ToTable("Heroe_Relacion");
                    });
        });

        modelBuilder.Entity<Lucha>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lucha__3214EC07B913D485");

            entity.HasOne(d => d.Heroe).WithMany(p => p.Luchas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lucha__HeroeId__534D60F1");

            entity.HasOne(d => d.Villano).WithMany(p => p.Luchas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Lucha__VillanoId__5441852A");
        });

        modelBuilder.Entity<Patrocinador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Patrocin__3214EC07A562559A");
        });

        modelBuilder.Entity<Relacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Relacion__3214EC07CC22729C");
        });

        modelBuilder.Entity<Villano>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Villano__3214EC0752FC37A8");

            entity.HasMany(d => d.Habilidads).WithMany(p => p.Villanos)
                .UsingEntity<Dictionary<string, object>>(
                    "VillanoHabilidad",
                    r => r.HasOne<Habilidad>().WithMany()
                        .HasForeignKey("HabilidadId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Villano_H__Habil__47DBAE45"),
                    l => l.HasOne<Villano>().WithMany()
                        .HasForeignKey("VillanoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Villano_H__Villa__46E78A0C"),
                    j =>
                    {
                        j.HasKey("VillanoId", "HabilidadId").HasName("PK__Villano___114B810DEE0A9BE2");
                        j.ToTable("Villano_Habilidad");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
