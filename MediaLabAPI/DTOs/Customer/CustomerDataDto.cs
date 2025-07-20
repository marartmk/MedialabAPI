namespace MediaLabAPI.DTOs
{
    public class CustomerDataDto
    {
        public string Tipo { get; set; } = "Privato"; // "Privato" o "Azienda"
        public bool Cliente { get; set; } = true;
        public bool Fornitore { get; set; } = false;
        public string? RagioneSociale { get; set; }
        public string? Cognome { get; set; }
        public string? Nome { get; set; }
        public string? Indirizzo { get; set; }
        public string? Cap { get; set; }
        public string? Regione { get; set; }
        public string? Provincia { get; set; }
        public string? Citta { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? CodiceFiscale { get; set; }
        public string? PartitaIva { get; set; }
        public string? EmailPec { get; set; }
        public string? CodiceSdi { get; set; }
        public string? Iban { get; set; }
    }
}