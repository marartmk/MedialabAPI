namespace MediaLabAPI.DTOs
{
    public class IncomingTestDto
    {
        public int? Id { get; set; }
        public Guid? RepairId { get; set; }                   // 🆕 Collegamento alla riparazione
        public Guid CompanyId { get; set; }
        public Guid MultitenantId { get; set; }

        // ===== MAPPING FRONTEND DIAGNOSTICA → DATABASE =====
        // Frontend IDs → Database Fields
        public bool? DeviceInfo { get; set; }                 // → Info generali
        public bool? ApplePay { get; set; }                   // → Nuovo campo o mapping personalizzato
        public bool? Battery { get; set; }                    // → batteria
        public bool? Bluetooth { get; set; }                  // → Nuovo campo bluetooth
        public bool? Camera { get; set; }                     // → fotocamera_posteriore + fotocamera_anteriore
        public bool? Cellular { get; set; }                   // → rete
        public bool? Clock { get; set; }                      // → Sistema generale
        public bool? Sim { get; set; }                        // → Nuovo campo sim
        public bool? FaceId { get; set; }                     // → face_id
        public bool? Scanner { get; set; }                    // → touch_id (scanner UDID)
        public bool? MagSafe { get; set; }                    // → Nuovo campo magsafe
        public bool? Sensors { get; set; }                    // → sensore_di_prossimita
        public bool? Services { get; set; }                   // → Sistema generale
        public bool? Software { get; set; }                   // → Sistema generale
        public bool? System { get; set; }                     // → scheda_madre
        public bool? WiFi { get; set; }                       // → wi_fi
        public bool? RfCellular { get; set; }                 // → rete (RF cellulare)
        public bool? WirelessProblem { get; set; }            // → Problema wireless generale

        // ===== CAMPI DATABASE DIRETTI =====
        // Per compatibilità e controlli manuali
        public bool? TelefonoSpento { get; set; }             // → telefono_spento
        public bool? VetroRotto { get; set; }                 // → vetro_rotto
        public bool? Touchscreen { get; set; }                // → touchscreen
        public bool? Lcd { get; set; }                        // → lcd
        public bool? FrameScollato { get; set; }              // → frame_scollato
        public bool? DockDiRicarica { get; set; }             // → dock_di_ricarica
        public bool? BackCover { get; set; }                  // → back_cover
        public bool? Telaio { get; set; }                     // → telaio
        public bool? TastiVolumeMuto { get; set; }            // → tasti_volume_muto
        public bool? TastoStandbyPower { get; set; }          // → tasto_standby_power
        public bool? MicrofonoChiamate { get; set; }          // → microfono_chiamate
        public bool? MicrofonoAmbientale { get; set; }        // → microfono_ambientale
        public bool? AltoparlantteChiamata { get; set; }      // → altoparlante_chiamata
        public bool? SpeakerBuzzer { get; set; }              // → speaker_buzzer
        public bool? VetroFotocameraPosteriore { get; set; }  // → vetro_fotocamera_posteriore
        public bool? TastoHome { get; set; }                  // → tasto_home
        public bool? TouchId { get; set; }                    // → touch_id
        public bool? Chiamata { get; set; }                   // → chiamata
        public bool? VetroPosteriore { get; set; }            // → vetro_posteriore

        // ===== METODI HELPER =====
        public string GetActiveDiagnosticsText()
        {
            var active = new List<string>();

            if (Battery == true) active.Add("Batteria OK");
            if (WiFi == true) active.Add("Wi-Fi OK");
            if (Camera == true) active.Add("Fotocamera OK");
            if (FaceId == true) active.Add("Face ID OK");
            if (Cellular == true) active.Add("Rete OK");
            if (System == true) active.Add("Sistema OK");
            if (Sensors == true) active.Add("Sensori OK");

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