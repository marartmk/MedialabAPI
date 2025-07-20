using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaLabAPI.Models
{
    [Table("IncomingTests")]
    public class IncomingTest
    {
        [Key]
        public int Id { get; set; }

        // 🆕 NUOVO: Collegamento alla riparazione
        public Guid? RepairId { get; set; }

        [Required]
        [Column("company_id")]
        public Guid CompanyId { get; set; }

        [Required]
        [Column("multitenant_id")]
        public Guid MultitenantId { get; set; }

        // Test diagnostici - mappati ai nomi delle colonne del database
        [Column("telefono_spento")]
        public bool? TelefonoSpento { get; set; }

        [Column("vetro_rotto")]
        public bool? VetroRotto { get; set; }

        [Column("touchscreen")]
        public bool? Touchscreen { get; set; }

        [Column("lcd")]
        public bool? Lcd { get; set; }

        [Column("frame_scollato")]
        public bool? FrameScollato { get; set; }

        [Column("batteria")]
        public bool? Batteria { get; set; }

        [Column("dock_di_ricarica")]
        public bool? DockDiRicarica { get; set; }

        [Column("back_cover")]
        public bool? BackCover { get; set; }

        [Column("telaio")]
        public bool? Telaio { get; set; }

        [Column("tasti_volume_muto")]
        public bool? TastiVolumeMuto { get; set; }

        [Column("tasto_standby_power")]
        public bool? TastoStandbyPower { get; set; }

        [Column("sensore_di_prossimita")]
        public bool? SensoreDiProssimita { get; set; }

        [Column("microfono_chiamate")]
        public bool? MicrofonoChiamate { get; set; }

        [Column("microfono_ambientale")]
        public bool? MicrofonoAmbientale { get; set; }

        [Column("altoparlante_chiamata")]
        public bool? AltoparlantteChiamata { get; set; }

        [Column("speaker_buzzer")]
        public bool? SpeakerBuzzer { get; set; }

        [Column("vetro_fotocamera_posteriore")]
        public bool? VetroFotocameraPosteriore { get; set; }

        [Column("fotocamera_posteriore")]
        public bool? FotocameraPosteriore { get; set; }

        [Column("fotocamera_anteriore")]
        public bool? FotocameraAnteriore { get; set; }

        [Column("tasto_home")]
        public bool? TastoHome { get; set; }

        [Column("touch_id")]
        public bool? TouchId { get; set; }

        [Column("face_id")]
        public bool? FaceId { get; set; }

        [Column("wi_fi")]
        public bool? WiFi { get; set; }

        [Column("rete")]
        public bool? Rete { get; set; }

        [Column("chiamata")]
        public bool? Chiamata { get; set; }

        [Column("scheda_madre")]
        public bool? SchedaMadre { get; set; }

        [Column("vetro_posteriore")]
        public bool? VetroPosteriore { get; set; }

        [Column("created_data")]
        public DateTime? CreatedData { get; set; } = DateTime.Now;

        [Column("modified_data")]
        public DateTime? ModifiedData { get; set; }

        [Column("is_deleted")]
        public bool? IsDeleted { get; set; } = false;

        // Navigation Properties
        [ForeignKey("RepairId")]
        public virtual DeviceRepair? DeviceRepair { get; set; }

        [ForeignKey("CompanyId")]
        public virtual C_ANA_Company? Company { get; set; }
    }
}