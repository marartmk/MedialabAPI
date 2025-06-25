using System;
using System.Collections.Generic;
using MediaLabAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaLabAPI.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<C_ANA_Company> C_ANA_Companies { get; set; }
    public virtual DbSet<SysAdmin> SysAdmins { get; set; }
    public virtual DbSet<SysUser> SysUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.3.20;Database=MedialabNexttest;User Id=sa;Password=4PCgKYB3yyj5hE78;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<C_ANA_Company>(entity =>
        {
            // 🔹 CONFIGURAZIONE CHIAVE PRIMARIA ESPLICITA
            entity.HasKey(e => e.Id);
            entity.ToTable("C_ANA_Companies"); // Nome tabella esplicito

            // 🔹 CONFIGURAZIONE PROPRIETÀ ID
            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .IsRequired()
                .ValueGeneratedNever(); // Il GUID viene generato nel codice

            // 🔹 CONFIGURAZIONE IdSeq (IDENTITY)
            entity.Property(e => e.IdSeq)
                .HasColumnName("IdSeq")
                .ValueGeneratedOnAdd(); // IDENTITY sul database

            // 🔹 CONFIGURAZIONE CAMPI RICHIESTI
            entity.Property(e => e.RagioneSociale)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("RagioneSociale");

            entity.Property(e => e.Cognome)
                  .HasMaxLength(100)
                  .HasColumnName("Cognome");

            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("Nome");

            // 🔹 CONFIGURAZIONE CAMPI BOOL NON NULLABLE (dalla struttura DB)
            entity.Property(e => e.EnabledFE)
                .HasColumnName("EnabledFE")
                .IsRequired(); // NOT NULL nel DB

            entity.Property(e => e.IsVendolo)
                .HasColumnName("IsVendolo")
                .IsRequired(); // NOT NULL nel DB

            entity.Property(e => e.IsVendoloFE)
                .HasColumnName("IsVendoloFE")
                .IsRequired(); // NOT NULL nel DB

            // 🔹 CONFIGURAZIONE CAMPI DATETIME
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime2");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime2");

            // 🔹 CONFIGURAZIONE ALTRI CAMPI IMPORTANTI
            entity.Property(e => e.ApiKey).HasMaxLength(100);
            entity.Property(e => e.Banca).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Cap).HasMaxLength(10);
            entity.Property(e => e.Citta).HasMaxLength(200);
            entity.Property(e => e.Civico).HasMaxLength(50);
            entity.Property(e => e.CodiceLooc).HasMaxLength(50);
            entity.Property(e => e.CodiceSdi).HasMaxLength(20);
            entity.Property(e => e.CreatedBy).HasMaxLength(200);
            entity.Property(e => e.DatevCode).HasMaxLength(100);
            entity.Property(e => e.DeletedBy).HasMaxLength(200);
            entity.Property(e => e.EmailAziendale).HasMaxLength(50);
            entity.Property(e => e.EmailPec).HasMaxLength(50);
            entity.Property(e => e.Fax).HasMaxLength(50);
            entity.Property(e => e.FiscalCode).HasMaxLength(20);
            entity.Property(e => e.IBAN).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.IdRegimeFiscale).HasMaxLength(6);
            entity.Property(e => e.Indirizzo).HasMaxLength(200);
            entity.Property(e => e.InternalCode).HasMaxLength(200).IsUnicode(false);
            entity.Property(e => e.ModifiedBy).HasMaxLength(450);
            entity.Property(e => e.Nazione).HasMaxLength(200);
            entity.Property(e => e.PIva).HasMaxLength(20);
            entity.Property(e => e.PagheCodAzienda).HasMaxLength(4).IsUnicode(false);
            entity.Property(e => e.PagheCodFiliale).HasMaxLength(2).IsUnicode(false);
            entity.Property(e => e.Provincia).HasMaxLength(200);
            entity.Property(e => e.Regione).HasMaxLength(200);
            entity.Property(e => e.SitoWeb).HasMaxLength(200);
            entity.Property(e => e.Telefono).HasMaxLength(50);
            entity.Property(e => e.Tipologia).HasMaxLength(1);
            entity.Property(e => e.UpdatedBy).HasMaxLength(200);
            entity.Property(e => e.apiKeyScontrino).HasMaxLength(100);
            entity.Property(e => e.apiPwdScontrino).HasMaxLength(250);
            entity.Property(e => e.codiceAziendaTS).HasMaxLength(4);
            entity.Property(e => e.istatCitta).HasMaxLength(50);
            entity.Property(e => e.passwordGPSServer).HasMaxLength(100);
            entity.Property(e => e.pinAde).HasMaxLength(250);
            entity.Property(e => e.pswAde).HasMaxLength(250);
            entity.Property(e => e.smtpProvider).HasMaxLength(100);
            entity.Property(e => e.usernameAde).HasMaxLength(50);
            entity.Property(e => e.usernameGPSServer).HasMaxLength(100);
        });

        modelBuilder.Entity<SysAdmin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SYS_Admi__3214EC07A68B7A23");

            entity.ToTable("SYS_Admins");

            entity.HasIndex(e => e.Username, "UQ__SYS_Admi__536C85E4DE0CA509").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.IdCompany)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.IsSuperAdmin).HasDefaultValue(false);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<SysUser>(entity =>
        {
            entity.ToTable("SYS_Users");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            entity.Property(e => e.IdWhr)
                .HasColumnName("IdWhr");

            entity.Property(e => e.IdCompany)
                .IsRequired()
                .HasColumnName("IdCompany");

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Username");

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnName("PasswordHash");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("Email");

            entity.Property(e => e.IsAdmin)
                .IsRequired()
                .HasColumnName("IsAdmin");

            entity.Property(e => e.IsEnabled)
                .IsRequired()
                .HasColumnName("IsEnabled");

            entity.Property(e => e.AccessLevel)
                .HasMaxLength(50)
                .HasColumnName("AccessLevel");

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("CreatedAt");

            entity.Property(e => e.LastLogin).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
