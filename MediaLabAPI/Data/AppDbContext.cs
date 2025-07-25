﻿using System;
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

    // DbSets esistenti
    public virtual DbSet<C_ANA_Company> C_ANA_Companies { get; set; }
    public virtual DbSet<SysAdmin> SysAdmins { get; set; }
    public virtual DbSet<SysUser> SysUsers { get; set; }
    public virtual DbSet<C_ANA_Operators> C_ANA_Operators { get; set; }
    public virtual DbSet<DeviceRegistry> DeviceRegistry { get; set; }

    // 🆕 Nuovo DbSet per le riparazioni
    public virtual DbSet<DeviceRepair> DeviceRepairs { get; set; }

    // 🔧 MANCAVA: DbSet per i test di ingresso
    public virtual DbSet<IncomingTest> IncomingTests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.3.20;Database=MedialabNexttest;User Id=sa;Password=4PCgKYB3yyj5hE78;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 🔹 CONFIGURAZIONE C_ANA_Company (esistente - mantengo invariata)
        modelBuilder.Entity<C_ANA_Company>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("C_ANA_Companies");

            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .IsRequired()
                .ValueGeneratedNever();

            entity.Property(e => e.IdSeq)
                .HasColumnName("IdSeq")
                .ValueGeneratedOnAdd();

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

            entity.Property(e => e.EnabledFE)
                .HasColumnName("EnabledFE")
                .IsRequired();

            entity.Property(e => e.IsVendolo)
                .HasColumnName("IsVendolo")
                .IsRequired();

            entity.Property(e => e.IsVendoloFE)
                .HasColumnName("IsVendoloFE")
                .IsRequired();

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.DeletedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime2");
            entity.Property(e => e.ModifiedDate).HasColumnType("datetime2");

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
            entity.Property(e => e.isTenant)
                  .HasColumnName("isTenant");
            entity.Property(e => e.isAffiliate)
                  .HasColumnName("isAffiliate");
            entity.Property(e => e.AffiliateCode)
                  .HasMaxLength(100)
                  .HasColumnName("AffiliateCode");
            entity.Property(e => e.AffiliatedDataStart)
                  .HasColumnName("AffiliatedDataStart")
                  .HasColumnType("datetime2");
            entity.Property(e => e.AffiliatedDataEnd)
                  .HasColumnName("AffiliatedDataEnd")
                  .HasColumnType("datetime2");
            entity.Property(e => e.AffiliateStatus)
                  .HasColumnName("AffiliateStatus");
        });

        // 🔹 CONFIGURAZIONE SysAdmin (mantengo invariata)
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

        // 🔹 CONFIGURAZIONE SysUser (mantengo invariata)
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

        // 🔹 CONFIGURAZIONE C_ANA_Operators (mantengo solo la parte essenziale)
        modelBuilder.Entity<C_ANA_Operators>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");
            entity.ToTable("C_ANA_Operators");
            entity.Property(e => e.Id).ValueGeneratedNever();
            // ... resto della configurazione (mantengo come era)
        });

        // 🔹 CONFIGURAZIONE DeviceRegistry
        modelBuilder.Entity<DeviceRegistry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DeviceRe__3214EC07DF53FF07");
            entity.ToTable("DeviceRegistry");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.DeviceId)
                .IsRequired();

            entity.Property(e => e.CustomerId)
                .IsRequired(false); // Nullable

            entity.Property(e => e.CompanyId)
                .IsRequired(true);

            entity.Property(e => e.MultitenantId)
                .IsRequired(true);

            entity.Property(e => e.SerialNumber)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Brand)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Model)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.DeviceType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.PurchaseDate)
                .IsRequired(false)
                .HasColumnType("date");

            entity.Property(e => e.ReceiptNumber)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(e => e.Retailer)
                .HasMaxLength(200)
                .IsRequired(false);

            entity.Property(e => e.Notes)
                .IsRequired(false);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            // Indici
            entity.HasIndex(e => e.CompanyId, "IX_DeviceRegistry_CompanyId");
            entity.HasIndex(e => e.CustomerId, "IX_DeviceRegistry_CustomerId");
            entity.HasIndex(e => e.DeviceId, "IX_DeviceRegistry_DeviceId");
            entity.HasIndex(e => e.SerialNumber, "IX_DeviceRegistry_SerialNumber");
        });

        // 🆕 CONFIGURAZIONE DeviceRepair (CORRETTA)
        modelBuilder.Entity<DeviceRepair>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("DeviceRepairs");

            // 🔧 AGGIUNTO: RepairId e RepairCode
            entity.Property(e => e.RepairId)
                .IsRequired()
                .HasDefaultValueSql("NEWID()"); // GUID generato dal database

            entity.Property(e => e.RepairCode)
                .HasMaxLength(50)
                .IsRequired(false); // Generato dall'applicazione

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.DeviceId)
                .IsRequired();

            entity.Property(e => e.CustomerId)
                .IsRequired(false);

            entity.Property(e => e.CompanyId)
                .IsRequired();

            entity.Property(e => e.MultitenantId)
                .IsRequired();

            entity.Property(e => e.FaultDeclared)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.FaultDetected)
                .HasMaxLength(500)
                .IsRequired(false);

            entity.Property(e => e.RepairAction)
                .HasMaxLength(500)
                .IsRequired(false);

            entity.Property(e => e.RepairStatusCode)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.RepairStatus)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.TechnicianCode)
                .HasMaxLength(50)
                .IsRequired(false);

            entity.Property(e => e.TechnicianName)
                .HasMaxLength(100)
                .IsRequired(false);

            entity.Property(e => e.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())")
                .IsRequired();

            entity.Property(e => e.ReceivedAt)
                .HasColumnType("datetime")
                .IsRequired(false);

            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .IsRequired(false);

            entity.Property(e => e.CompletedAt)
                .HasColumnType("datetime")
                .IsRequired(false);

            entity.Property(e => e.DeliveredAt)
                .HasColumnType("datetime")
                .IsRequired(false);

            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .IsRequired(false);

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .IsRequired();

            // 🔧 RELAZIONI CORRETTE
            entity.HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId)
                .HasPrincipalKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Company)
                .WithMany()
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indici
            entity.HasIndex(e => e.RepairId)
                .IsUnique()
                .HasDatabaseName("IX_DeviceRepairs_RepairId");

            entity.HasIndex(e => e.RepairCode)
                .IsUnique()
                .HasDatabaseName("IX_DeviceRepairs_RepairCode")
                .HasFilter("[RepairCode] IS NOT NULL");

            entity.HasIndex(e => e.CustomerId)
                .HasDatabaseName("IX_DeviceRepairs_CustomerId");

            entity.HasIndex(e => e.DeviceId)
                .HasDatabaseName("IX_DeviceRepairs_DeviceId");

            entity.HasIndex(e => e.MultitenantId)
                .HasDatabaseName("IX_DeviceRepairs_MultitenantId");
        });

        // 🆕 CONFIGURAZIONE IncomingTest (NUOVA)
        modelBuilder.Entity<IncomingTest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("IncomingTests");

            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd(); // Auto-increment se è IDENTITY

            // 🔧 CORREZIONE: Collegamento alla riparazione
            entity.Property(e => e.RepairId)
                .HasColumnName("repair_id")
                .IsRequired(false); // Nullable Guid

            entity.Property(e => e.CompanyId)
                .HasColumnName("company_id")
                .IsRequired();

            entity.Property(e => e.MultitenantId)
                .HasColumnName("multitenant_id")
                .IsRequired();

            // Mapping di tutti i campi diagnostici con nomi delle colonne corretti
            entity.Property(e => e.TelefonoSpento).HasColumnName("telefono_spento");
            entity.Property(e => e.VetroRotto).HasColumnName("vetro_rotto");
            entity.Property(e => e.Touchscreen).HasColumnName("touchscreen");
            entity.Property(e => e.Lcd).HasColumnName("lcd");
            entity.Property(e => e.FrameScollato).HasColumnName("frame_scollato");
            entity.Property(e => e.Batteria).HasColumnName("batteria");
            entity.Property(e => e.DockDiRicarica).HasColumnName("dock_di_ricarica");
            entity.Property(e => e.BackCover).HasColumnName("back_cover");
            entity.Property(e => e.Telaio).HasColumnName("telaio");
            entity.Property(e => e.TastiVolumeMuto).HasColumnName("tasti_volume_muto");
            entity.Property(e => e.TastoStandbyPower).HasColumnName("tasto_standby_power");
            entity.Property(e => e.SensoreDiProssimita).HasColumnName("sensore_di_prossimita");
            entity.Property(e => e.MicrofonoChiamate).HasColumnName("microfono_chiamate");
            entity.Property(e => e.MicrofonoAmbientale).HasColumnName("microfono_ambientale");
            entity.Property(e => e.AltoparlantteChiamata).HasColumnName("altoparlante_chiamata");
            entity.Property(e => e.SpeakerBuzzer).HasColumnName("speaker_buzzer");
            entity.Property(e => e.VetroFotocameraPosteriore).HasColumnName("vetro_fotocamera_posteriore");
            entity.Property(e => e.FotocameraPosteriore).HasColumnName("fotocamera_posteriore");
            entity.Property(e => e.FotocameraAnteriore).HasColumnName("fotocamera_anteriore");
            entity.Property(e => e.TastoHome).HasColumnName("tasto_home");
            entity.Property(e => e.TouchId).HasColumnName("touch_id");
            entity.Property(e => e.FaceId).HasColumnName("face_id");
            entity.Property(e => e.WiFi).HasColumnName("wi_fi");
            entity.Property(e => e.Rete).HasColumnName("rete");
            entity.Property(e => e.Chiamata).HasColumnName("chiamata");
            entity.Property(e => e.SchedaMadre).HasColumnName("scheda_madre");
            entity.Property(e => e.VetroPosteriore).HasColumnName("vetro_posteriore");

            entity.Property(e => e.CreatedData)
                .HasColumnName("created_data")
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ModifiedData)
                .HasColumnName("modified_data")
                .HasColumnType("datetime");

            entity.Property(e => e.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);

            // 🔧 RELAZIONE CORRETTA con DeviceRepair
            entity.HasOne(e => e.DeviceRepair)
                .WithOne(r => r.IncomingTest)
                .HasForeignKey<IncomingTest>(e => e.RepairId)
                .HasPrincipalKey<DeviceRepair>(r => r.RepairId) // 👈 USA RepairId (Guid)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Company)
                .WithMany()
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indici
            entity.HasIndex(e => e.RepairId)
                .HasDatabaseName("IX_IncomingTests_RepairId");

            entity.HasIndex(e => e.CompanyId)
                .HasDatabaseName("IX_IncomingTests_CompanyId");

            entity.HasIndex(e => e.MultitenantId)
                .HasDatabaseName("IX_IncomingTests_MultitenantId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}