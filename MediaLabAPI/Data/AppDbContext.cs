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
    public virtual DbSet<C_ANA_Operators> C_ANA_Operators { get; set; }
    public virtual DbSet<DeviceRegistry> DeviceRegistry { get; set; }

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

        modelBuilder.Entity<C_ANA_Operators>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Users");

            entity.ToTable("C_ANA_Operators");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Active).HasColumnName("active");
            entity.Property(e => e.ApiKey).HasMaxLength(100);
            entity.Property(e => e.AssunzioniSpeciale).HasMaxLength(200);
            entity.Property(e => e.Cap).HasMaxLength(50);
            entity.Property(e => e.CellularNumber).HasMaxLength(50);
            entity.Property(e => e.Citta).HasMaxLength(200);
            entity.Property(e => e.CodiceCartaCarb).HasMaxLength(200);
            entity.Property(e => e.CodiceCategoria).HasMaxLength(200);
            entity.Property(e => e.CodiceCausaFineRapporto).HasMaxLength(200);
            entity.Property(e => e.CodiceContratto).HasMaxLength(200);
            entity.Property(e => e.CodiceDipendente).HasMaxLength(200);
            entity.Property(e => e.CodiceEsecutore)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CodiceFiscale).HasMaxLength(200);
            entity.Property(e => e.CodiceLivello).HasMaxLength(200);
            entity.Property(e => e.CodiceNumeroLegge).HasMaxLength(200);
            entity.Property(e => e.CodiceSindacato).HasMaxLength(200);
            entity.Property(e => e.CodiceTipoContratto).HasMaxLength(200);
            entity.Property(e => e.CodiceTipoOrarioLavoro).HasMaxLength(200);
            entity.Property(e => e.CodiceTipoRetribuzione).HasMaxLength(200);
            entity.Property(e => e.CodiceTitoloStudio).HasMaxLength(200);
            entity.Property(e => e.ComuneNascita).HasMaxLength(200);
            entity.Property(e => e.ContrattoFl)
                .HasMaxLength(200)
                .HasColumnName("ContrattoFL");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.DataFineRapporto).HasMaxLength(200);
            entity.Property(e => e.DataUltimoImpiego).HasMaxLength(200);
            entity.Property(e => e.DelmeCodiceDitta)
                .HasMaxLength(200)
                .HasColumnName("DELME_CodiceDitta");
            entity.Property(e => e.DelmeTelefono)
                .HasMaxLength(200)
                .HasColumnName("DELME_Telefono");
            entity.Property(e => e.DescriContratto).HasMaxLength(200);
            entity.Property(e => e.DescriLivello).HasMaxLength(200);
            entity.Property(e => e.DescriQualifica).HasMaxLength(200);
            entity.Property(e => e.DescriTipoContratto).HasMaxLength(200);
            entity.Property(e => e.DescriTipoOrarioLavoro).HasMaxLength(200);
            entity.Property(e => e.DescriTipoRetribuzione).HasMaxLength(200);
            entity.Property(e => e.DescriTitoloStudio).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(200);
            entity.Property(e => e.IButtonCode)
                .HasMaxLength(100)
                .HasColumnName("iButtonCode");
            entity.Property(e => e.Iban)
                .HasMaxLength(200)
                .HasColumnName("IBAN");
            entity.Property(e => e.Idcompany).HasColumnName("IDCompany");
            entity.Property(e => e.IdcreatoDa).HasColumnName("IDCreatoDa");
            entity.Property(e => e.IdeliminatoDa).HasColumnName("IDEliminatoDa");
            entity.Property(e => e.IdmodificatoDa).HasColumnName("IDModificatoDa");
            entity.Property(e => e.Indirizzo).HasMaxLength(200);
            entity.Property(e => e.InternalCode).HasMaxLength(50);
            entity.Property(e => e.IsDemo).HasColumnName("isDemo");
            entity.Property(e => e.IsEmployee).HasColumnName("isEmployee");
            entity.Property(e => e.IsEnabledSupplierBonifico).HasColumnName("isEnabledSupplierBonifico");
            entity.Property(e => e.IsEnabledSupplierOrderConfirm).HasColumnName("isEnabledSupplierOrderConfirm");
            entity.Property(e => e.IsExternal).HasColumnName("isExternal");
            entity.Property(e => e.IsOperatoreLooc).HasColumnName("isOperatoreLOOC");
            entity.Property(e => e.IsParking).HasColumnName("isParking");
            entity.Property(e => e.IsSuperAdmin).HasColumnName("isSuperAdmin");
            entity.Property(e => e.LastName).HasMaxLength(200);
            entity.Property(e => e.LoocPassword)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LoocUsername)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Matricola).HasMaxLength(200);
            entity.Property(e => e.MaxCostSupplierOrderConfirm).HasColumnName("maxCostSupplierOrderConfirm");
            entity.Property(e => e.ModificatoDaId).HasMaxLength(450);
            entity.Property(e => e.Nazione).HasMaxLength(200);
            entity.Property(e => e.NfcCode).HasMaxLength(200);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.Note).HasMaxLength(2000);
            entity.Property(e => e.NumContratto).HasMaxLength(200);
            entity.Property(e => e.NumeroPatente).HasMaxLength(200);
            entity.Property(e => e.OreMedieSettimana).HasMaxLength(200);
            entity.Property(e => e.PartTime).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.PrNascita).HasMaxLength(200);
            entity.Property(e => e.PrimaAssunzione).HasMaxLength(200);
            entity.Property(e => e.PrivateNumber).HasMaxLength(50);
            entity.Property(e => e.Provincia).HasMaxLength(200);
            entity.Property(e => e.QualificaImpiegato).HasMaxLength(200);
            entity.Property(e => e.QualificheSoggetto).HasMaxLength(200);
            entity.Property(e => e.Ral).HasColumnName("RAL");
            entity.Property(e => e.ReceiveEmail).HasColumnName("receiveEmail");
            entity.Property(e => e.ReceiveOtpiride).HasColumnName("receiveOTPIride");
            entity.Property(e => e.ReceiveSms).HasColumnName("receiveSms");
            entity.Property(e => e.Regione).HasMaxLength(200);
            entity.Property(e => e.Rinnovo).HasMaxLength(200);
            entity.Property(e => e.ScadenzaDocumento).HasMaxLength(200);
            entity.Property(e => e.ScadenzaFl)
                .HasMaxLength(200)
                .HasColumnName("ScadenzaFL");
            entity.Property(e => e.ScadenzaRinnovo).HasMaxLength(200);
            entity.Property(e => e.ScadenzaTempoDeterminato).HasMaxLength(200);
            entity.Property(e => e.SpecializzazioniSoggetto).HasMaxLength(200);
            entity.Property(e => e.StatoAccount)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StimaOraria).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.StimaOrariaGalleria).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.StimaOrariaNotturna).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.StimaOrariaStraordinaria).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TipoPatente).HasMaxLength(200);
            entity.Property(e => e.UserName).HasMaxLength(256);
        });

        modelBuilder.Entity<DeviceRegistry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DeviceRe__3214EC07DF53FF07");
            entity.ToTable("DeviceRegistry");

            // Indici
            entity.HasIndex(e => e.CompanyId, "IX_DeviceRegistry_CompanyId");
            entity.HasIndex(e => e.CustomerId, "IX_DeviceRegistry_CustomerId");
            entity.HasIndex(e => e.DeviceId, "IX_DeviceRegistry_DeviceId");
            entity.HasIndex(e => e.DeviceType, "IX_DeviceRegistry_DeviceType");
            entity.HasIndex(e => e.IsDeleted, "IX_DeviceRegistry_IsDeleted");
            entity.HasIndex(e => e.MultitenantId, "IX_DeviceRegistry_MultitenantId");
            entity.HasIndex(e => e.SerialNumber, "IX_DeviceRegistry_SerialNumber");

            // Configurazione proprietà
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd(); // Auto-increment

            entity.Property(e => e.DeviceId)
                .IsRequired(); // Guid obbligatorio

            entity.Property(e => e.CustomerId)
                .IsRequired(false); // Nullable Guid

            entity.Property(e => e.CompanyId)
                .IsRequired(false); // Nullable Guid

            entity.Property(e => e.MultitenantId)
                .IsRequired(false); // Nullable Guid

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
                .IsRequired(false); // Nullable DateOnly

            entity.Property(e => e.ReceiptNumber)
                .HasMaxLength(100)
                .IsRequired(false); // ✅ Nullable string

            entity.Property(e => e.Retailer)
                .HasMaxLength(200)
                .IsRequired(false); // ✅ Nullable string

            entity.Property(e => e.Notes)
                .IsRequired(false); // ✅ Nullable string, NVARCHAR(MAX)

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);
        });
        OnModelCreatingPartial(modelBuilder);

    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
