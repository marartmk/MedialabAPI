using System;
using System.Collections.Generic;

namespace MediaLabAPI.Models;

public partial class C_ANA_Operators
{
    public Guid Id { get; set; }

    public Guid? IdWhr { get; set; }

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool? EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public bool? PhoneNumberConfirmed { get; set; }

    public bool? TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool? LockoutEnabled { get; set; }

    public int? AccessFailedCount { get; set; }

    public string? InternalCode { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public byte[]? ProfilePicture { get; set; }

    public Guid? Idcompany { get; set; }

    public int? Active { get; set; }

    public DateTime? DataCreazione { get; set; }

    public string? IdcreatoDa { get; set; }

    public DateTime? DataModifica { get; set; }

    public string? IdmodificatoDa { get; set; }

    public string? ModificatoDaId { get; set; }

    public DateTime? DataEliminazione { get; set; }

    public string? IdeliminatoDa { get; set; }

    public int? Gender { get; set; }

    public int? IsEmployee { get; set; }

    public string? StatoAccount { get; set; }

    public DateTime? DataStatoAccount { get; set; }

    public TimeOnly? OraStatoAccount { get; set; }

    public string? CodiceFiscale { get; set; }

    public string? NfcCode { get; set; }

    public string? Nazione { get; set; }

    public string? Regione { get; set; }

    public string? Provincia { get; set; }

    public string? Citta { get; set; }

    public string? Indirizzo { get; set; }

    public string? TipoPatente { get; set; }

    public string? NumeroPatente { get; set; }

    public DateTime? DataScadenzaPatente { get; set; }

    public string? CodiceCartaCarb { get; set; }

    public string? CodiceDipendente { get; set; }

    public string? DelmeCodiceDitta { get; set; }

    public string? DelmeTelefono { get; set; }

    public string? ScadenzaDocumento { get; set; }

    public DateTime? DataNascita { get; set; }

    public string? ComuneNascita { get; set; }

    public string? PrNascita { get; set; }

    public string? NumContratto { get; set; }

    public string? CodiceContratto { get; set; }

    public string? DescriContratto { get; set; }

    public string? QualificaImpiegato { get; set; }

    public string? DescriQualifica { get; set; }

    public string? CodiceLivello { get; set; }

    public string? DescriLivello { get; set; }

    public string? CodiceTipoRetribuzione { get; set; }

    public string? DescriTipoRetribuzione { get; set; }

    public string? CodiceTipoOrarioLavoro { get; set; }

    public string? DescriTipoOrarioLavoro { get; set; }

    public string? CodiceTitoloStudio { get; set; }

    public string? DescriTitoloStudio { get; set; }

    public string? CodiceTipoContratto { get; set; }

    public string? DescriTipoContratto { get; set; }

    public string? ScadenzaTempoDeterminato { get; set; }

    public string? Rinnovo { get; set; }

    public string? ScadenzaRinnovo { get; set; }

    public string? ContrattoFl { get; set; }

    public string? ScadenzaFl { get; set; }

    public string? PartTime { get; set; }

    public string? OreMedieSettimana { get; set; }

    public string? PrimaAssunzione { get; set; }

    public string? AssunzioniSpeciale { get; set; }

    public string? CodiceNumeroLegge { get; set; }

    public string? CodiceCategoria { get; set; }

    public string? DataUltimoImpiego { get; set; }

    public string? DataFineRapporto { get; set; }

    public string? CodiceCausaFineRapporto { get; set; }

    public string? Iban { get; set; }

    public string? Matricola { get; set; }

    public string? CodiceSindacato { get; set; }

    public string? Note { get; set; }

    public string? QualificheSoggetto { get; set; }

    public string? SpecializzazioniSoggetto { get; set; }

    public int? IsExternal { get; set; }

    public string? Cap { get; set; }

    public Guid? MultiTenantId { get; set; }

    public decimal? StimaOraria { get; set; }

    public decimal? StimaOrariaStraordinaria { get; set; }

    public decimal? StimaOrariaGalleria { get; set; }

    public decimal? StimaOrariaNotturna { get; set; }

    public Guid? InternalCompanyReferenceId { get; set; }

    public string? CodiceEsecutore { get; set; }

    public int? IsParking { get; set; }

    public string? CellularNumber { get; set; }

    public string? PrivateNumber { get; set; }

    public string? LoocUsername { get; set; }

    public string? LoocPassword { get; set; }

    public bool? IsEnabledSupplierOrderConfirm { get; set; }

    public int? MaxCostSupplierOrderConfirm { get; set; }

    public bool? IsEnabledSupplierBonifico { get; set; }

    public bool? IsSuperAdmin { get; set; }

    public bool? IsDemo { get; set; }

    public string? ApiKey { get; set; }

    public string? IButtonCode { get; set; }

    public bool? ReceiveSms { get; set; }

    public bool? ReceiveEmail { get; set; }

    public double? Ral { get; set; }

    public int? IsOperatoreLooc { get; set; }

    public bool? ReceiveOtpiride { get; set; }

    public Guid? IdSede { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }
}
