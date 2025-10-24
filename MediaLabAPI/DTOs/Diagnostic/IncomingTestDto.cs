namespace MediaLabAPI.DTOs
{
    public class IncomingTestDto
    {
        public int? Id { get; set; }
        public Guid? RepairId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid MultitenantId { get; set; }

        // ===== MAPPING 1:1 CON I CAMPI DEL DATABASE =====
        // Tutti i campi corrispondono ESATTAMENTE alla tabella IncomingTests

        public bool? TelefonoSpento { get; set; }
        public bool? VetroRotto { get; set; }
        public bool? Touchscreen { get; set; }
        public bool? Lcd { get; set; }
        public bool? FrameScollato { get; set; }
        public bool? Batteria { get; set; }
        public bool? DockDiRicarica { get; set; }
        public bool? BackCover { get; set; }
        public bool? Telaio { get; set; }
        public bool? TastiVolumeMuto { get; set; }
        public bool? TastoStandbyPower { get; set; }
        public bool? SensoreDiProssimita { get; set; }
        public bool? MicrofonoChiamate { get; set; }
        public bool? MicrofonoAmbientale { get; set; }
        public bool? AltoparlanteChiamata { get; set; }
        public bool? SpeakerBuzzer { get; set; }
        public bool? VetroFotocameraPosteriore { get; set; }
        public bool? FotocameraPosteriore { get; set; }
        public bool? FotocameraAnteriore { get; set; }
        public bool? TastoHome { get; set; }
        public bool? TouchId { get; set; }
        public bool? FaceId { get; set; }
        public bool? WiFi { get; set; }
        public bool? Rete { get; set; }
        public bool? Chiamata { get; set; }
        public bool? SchedaMadre { get; set; }
        public bool? VetroPosteriore { get; set; }

        // ===== METODI HELPER =====
        public string GetActiveDiagnosticsText()
        {
            var active = new List<string>();

            if (Batteria == true) active.Add("Batteria OK");
            if (WiFi == true) active.Add("Wi-Fi OK");
            if (FotocameraPosteriore == true || FotocameraAnteriore == true) active.Add("Fotocamera OK");
            if (FaceId == true) active.Add("Face ID OK");
            if (Rete == true) active.Add("Rete OK");
            if (SchedaMadre == true) active.Add("Sistema OK");
            if (SensoreDiProssimita == true) active.Add("Sensori OK");

            return active.Any() ? string.Join(", ", active) : "Nessuna diagnostica attiva";
        }

        public int GetActiveTestsCount()
        {
            var properties = GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(bool?))
                .Select(p => p.GetValue(this) as bool?)
                .Where(v => v == true);

            return properties.Count();
        }
    }
}