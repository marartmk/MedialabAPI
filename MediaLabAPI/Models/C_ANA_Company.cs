using System;
using System.Collections.Generic;

namespace MediaLabAPI.Models;

public partial class C_ANA_Company
{
    public Guid Id { get; set; }

    public Guid? IdWhr { get; set; }

    public int IdSeq { get; set; }

    public string? RagioneSociale { get; set; }

    public string? Cognome { get; set; }

    public string? Nome { get; set; }

    public string? PIva { get; set; }

    public string? FiscalCode { get; set; }

    public string? EmailPec { get; set; }

    public string? CodiceSdi { get; set; }

    public string? Nazione { get; set; }

    public string? Regione { get; set; }

    public string? Provincia { get; set; }

    public string? Citta { get; set; }

    public string? Indirizzo { get; set; }

    public string? EmailAziendale { get; set; }

    public string? SitoWeb { get; set; }

    public string? Telefono { get; set; }

    public string? Fax { get; set; }

    public bool? isSupplier { get; set; }

    public bool? isCustomer { get; set; }

    public bool? isOfficina { get; set; }

    public Guid? ParentID { get; set; }

    public byte[]? Img { get; set; }

    public bool? active { get; set; }

    public string? CodiceLooc { get; set; }

    public Guid? MultiTenantId { get; set; }

    public string? InternalCode { get; set; }

    public bool? isExternal { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public string? DeletedBy { get; set; }

    public int? Pagamento { get; set; }

    public string? Banca { get; set; }

    public string? IBAN { get; set; }

    public bool? isCopiedOnLooc { get; set; }

    public string? DatevCode { get; set; }

    public int? TipoPagamentoID { get; set; }

    public int? ModalitaPagamentoID { get; set; }

    public string? PagheCodFiliale { get; set; }

    public string? PagheCodAzienda { get; set; }

    public string? Cap { get; set; }

    public string? ApiKey { get; set; }

    public string? usernameGPSServer { get; set; }

    public string? passwordGPSServer { get; set; }

    public string? smtpProvider { get; set; }

    public string? smtpPassword { get; set; }

    public int? numProgressivoFatture { get; set; }

    public Guid? IdContoContropartita { get; set; }

    public string? codiceAziendaTS { get; set; }

    public string? Tipologia { get; set; }

    public string? apiKeyScontrino { get; set; }

    public string? apiPwdScontrino { get; set; }

    public string? usernameAde { get; set; }

    public string? pinAde { get; set; }

    public string? pswAde { get; set; }

    public int? numProgressivoProForma { get; set; }

    public int? IdAliquotaDefault { get; set; }

    public double? CostoManodopera { get; set; }

    public int? id_iva_acquisto { get; set; }

    public int? id_iva_vendita { get; set; }

    public Guid? Id_conto_vendita { get; set; }

    public Guid? Id_conto_acquisto { get; set; }

    public int? Id_pagamento_acquisto { get; set; }

    public int? Id_pagamento_vendita { get; set; }

    public Guid? IdContoScontrinoIncasso { get; set; }

    public Guid? IdContoScontrinoRicavo { get; set; }

    public Guid? IdSedePrincipale { get; set; }

    public bool? IsGenericCostoManodopera { get; set; }

    public bool? isMedialab { get; set; }

    public bool? isModello231 { get; set; }

    public string? IdRegimeFiscale { get; set; }

    public string? Civico { get; set; }

    public int? DecimaliEuro { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public bool EnabledFE { get; set; }

    public bool IsVendolo { get; set; }

    public bool IsVendoloFE { get; set; }

    public string? istatCitta { get; set; }

    public Guid? IdContoFattureVendita { get; set; }

    public Guid? IdContoFattureAcquisto { get; set; }

    public Guid? IdContoContantiCassa { get; set; }

    public bool? AccettazioneTermini { get; set; }

    public Guid? IdPosCC { get; set; }
}
