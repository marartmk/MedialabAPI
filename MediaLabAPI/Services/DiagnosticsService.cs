using Microsoft.EntityFrameworkCore;
using MediaLabAPI.Data;
using MediaLabAPI.DTOs;
using MediaLabAPI.Models;

namespace MediaLabAPI.Services
{
    public class DiagnosticsService : IDiagnosticsService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<DiagnosticsService> _log;

        public DiagnosticsService(AppDbContext db, ILogger<DiagnosticsService> log)
        {
            _db = db;
            _log = log;
        }

        // ===================== INCOMING =====================
        public async Task<IncomingTestDto?> GetIncomingAsync(Guid repairId)
        {
            var e = await _db.IncomingTests.AsNoTracking()
                     .FirstOrDefaultAsync(x => x.RepairId == repairId && x.IsDeleted == false);
            return e is null ? null : MapIncoming(e);
        }

        public async Task UpsertIncomingAsync(Guid repairId, IncomingTestDto dto)
        {
            var rep = await _db.DeviceRepairs.AsNoTracking()
                        .FirstOrDefaultAsync(r => r.RepairId == repairId && !r.IsDeleted);
            if (rep == null) throw new ArgumentException("Repair not found");

            var e = await _db.IncomingTests
                        .FirstOrDefaultAsync(x => x.RepairId == repairId && x.IsDeleted == false);

            if (e == null)
            {
                e = new IncomingTest
                {
                    RepairId = repairId,
                    CompanyId = rep.CompanyId,
                    MultitenantId = rep.MultitenantId,
                    CreatedData = DateTime.Now,
                    IsDeleted = false
                };
                _db.IncomingTests.Add(e);
            }

            CopyIncoming(dto, e);
            e.ModifiedData = DateTime.Now;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteIncomingAsync(Guid repairId)
        {
            var e = await _db.IncomingTests
                        .FirstOrDefaultAsync(x => x.RepairId == repairId && x.IsDeleted == false);
            if (e == null) return;
            e.IsDeleted = true;
            e.ModifiedData = DateTime.Now;
            await _db.SaveChangesAsync();
        }

        // ===================== EXIT =====================
        public async Task<ExitTestDto?> GetExitAsync(Guid repairId)
        {
            var e = await _db.ExitTests.AsNoTracking()
                     .FirstOrDefaultAsync(x => x.RepairId == repairId && (x.IsDeleted == false || x.IsDeleted == null));
            return e is null ? null : MapExit(e);
        }

        public async Task UpsertExitAsync(Guid repairId, ExitTestDto dto)
        {
            var rep = await _db.DeviceRepairs.AsNoTracking()
                        .FirstOrDefaultAsync(r => r.RepairId == repairId && !r.IsDeleted);
            if (rep == null) throw new ArgumentException("Repair not found");

            var e = await _db.ExitTests
                        .FirstOrDefaultAsync(x => x.RepairId == repairId && (x.IsDeleted == false || x.IsDeleted == null));

            if (e == null)
            {
                e = new ExitTest
                {
                    RepairId = repairId,
                    CompanyId = rep.CompanyId,
                    MultitenantId = rep.MultitenantId,
                    CreatedData = DateTime.Now,
                    IsDeleted = false
                };
                _db.ExitTests.Add(e);
            }

            CopyExit(dto, e);
            e.ModifiedData = DateTime.Now;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteExitAsync(Guid repairId)
        {
            var e = await _db.ExitTests
                        .FirstOrDefaultAsync(x => x.RepairId == repairId && (x.IsDeleted == false || x.IsDeleted == null));
            if (e == null) return;
            e.IsDeleted = true;
            e.ModifiedData = DateTime.Now;
            await _db.SaveChangesAsync();
        }

        // ===================== MAPPING HELPERS - 1:1 PURO =====================

        private static IncomingTestDto MapIncoming(IncomingTest e) => new()
        {
            Id = e.Id,
            RepairId = e.RepairId,
            CompanyId = e.CompanyId,
            MultitenantId = e.MultitenantId,

            // ===== MAPPING 1:1 COMPLETO - NESSUN ALIAS =====
            TelefonoSpento = e.TelefonoSpento,
            VetroRotto = e.VetroRotto,
            Touchscreen = e.Touchscreen,
            Lcd = e.Lcd,
            FrameScollato = e.FrameScollato,
            Batteria = e.Batteria,
            DockDiRicarica = e.DockDiRicarica,
            BackCover = e.BackCover,
            Telaio = e.Telaio,
            TastiVolumeMuto = e.TastiVolumeMuto,
            TastoStandbyPower = e.TastoStandbyPower,
            SensoreDiProssimita = e.SensoreDiProssimita,
            MicrofonoChiamate = e.MicrofonoChiamate,
            MicrofonoAmbientale = e.MicrofonoAmbientale,
            AltoparlanteChiamata = e.AltoparlanteChiamata,
            SpeakerBuzzer = e.SpeakerBuzzer,
            VetroFotocameraPosteriore = e.VetroFotocameraPosteriore,
            FotocameraPosteriore = e.FotocameraPosteriore,
            FotocameraAnteriore = e.FotocameraAnteriore,
            TastoHome = e.TastoHome,
            TouchId = e.TouchId,
            FaceId = e.FaceId,
            WiFi = e.WiFi,
            Rete = e.Rete,
            Chiamata = e.Chiamata,
            SchedaMadre = e.SchedaMadre,
            VetroPosteriore = e.VetroPosteriore
        };

        private static ExitTestDto MapExit(ExitTest e) => new()
        {
            Id = e.Id,
            RepairId = e.RepairId,
            CompanyId = e.CompanyId,
            MultitenantId = e.MultitenantId,
            TelefonoSpento = e.TelefonoSpento,

            // ===== MAPPING 1:1 COMPLETO - NESSUN ALIAS =====
            VetroRotto = e.VetroRotto,
            Touchscreen = e.Touchscreen,
            Lcd = e.Lcd,
            FrameScollato = e.FrameScollato,
            Batteria = e.Batteria,
            DockDiRicarica = e.DockDiRicarica,
            BackCover = e.BackCover,
            Telaio = e.Telaio,
            TastiVolumeMuto = e.TastiVolumeMuto,
            TastoStandbyPower = e.TastoStandbyPower,
            SensoreDiProssimita = e.SensoreDiProssimita,
            MicrofonoChiamate = e.MicrofonoChiamate,
            MicrofonoAmbientale = e.MicrofonoAmbientale,
            AltoparlanteChiamata = e.AltoparlanteChiamata,
            SpeakerBuzzer = e.SpeakerBuzzer,
            VetroFotocameraPosteriore = e.VetroFotocameraPosteriore,
            FotocameraPosteriore = e.FotocameraPosteriore,
            FotocameraAnteriore = e.FotocameraAnteriore,
            TastoHome = e.TastoHome,
            TouchId = e.TouchId,
            FaceId = e.FaceId,
            WiFi = e.WiFi,
            Rete = e.Rete,
            Chiamata = e.Chiamata,
            SchedaMadre = e.SchedaMadre,
            VetroPosteriore = e.VetroPosteriore
        };

        private static void CopyIncoming(IncomingTestDto s, IncomingTest d)
        {
            // ===== MAPPING 1:1 COMPLETO - NESSUN ALIAS =====
            d.TelefonoSpento = s.TelefonoSpento;
            d.VetroRotto = s.VetroRotto;
            d.Touchscreen = s.Touchscreen;
            d.Lcd = s.Lcd;
            d.FrameScollato = s.FrameScollato;
            d.Batteria = s.Batteria;
            d.DockDiRicarica = s.DockDiRicarica;
            d.BackCover = s.BackCover;
            d.Telaio = s.Telaio;
            d.TastiVolumeMuto = s.TastiVolumeMuto;
            d.TastoStandbyPower = s.TastoStandbyPower;
            d.SensoreDiProssimita = s.SensoreDiProssimita;
            d.MicrofonoChiamate = s.MicrofonoChiamate;
            d.MicrofonoAmbientale = s.MicrofonoAmbientale;
            d.AltoparlanteChiamata = s.AltoparlanteChiamata;
            d.SpeakerBuzzer = s.SpeakerBuzzer;
            d.VetroFotocameraPosteriore = s.VetroFotocameraPosteriore;
            d.FotocameraPosteriore = s.FotocameraPosteriore;
            d.FotocameraAnteriore = s.FotocameraAnteriore;
            d.TastoHome = s.TastoHome;
            d.TouchId = s.TouchId;
            d.FaceId = s.FaceId;
            d.WiFi = s.WiFi;
            d.Rete = s.Rete;
            d.Chiamata = s.Chiamata;
            d.SchedaMadre = s.SchedaMadre;
            d.VetroPosteriore = s.VetroPosteriore;
        }

        private static void CopyExit(ExitTestDto s, ExitTest d)
        {
            // ===== MAPPING 1:1 COMPLETO - NESSUN ALIAS =====
            d.TelefonoSpento = s.TelefonoSpento;
            d.VetroRotto = s.VetroRotto;
            d.Touchscreen = s.Touchscreen;
            d.Lcd = s.Lcd;
            d.FrameScollato = s.FrameScollato;
            d.Batteria = s.Batteria;
            d.DockDiRicarica = s.DockDiRicarica;
            d.BackCover = s.BackCover;
            d.Telaio = s.Telaio;
            d.TastiVolumeMuto = s.TastiVolumeMuto;
            d.TastoStandbyPower = s.TastoStandbyPower;
            d.SensoreDiProssimita = s.SensoreDiProssimita;
            d.MicrofonoChiamate = s.MicrofonoChiamate;
            d.MicrofonoAmbientale = s.MicrofonoAmbientale;
            d.AltoparlanteChiamata = s.AltoparlanteChiamata;
            d.SpeakerBuzzer = s.SpeakerBuzzer;
            d.VetroFotocameraPosteriore = s.VetroFotocameraPosteriore;
            d.FotocameraPosteriore = s.FotocameraPosteriore;
            d.FotocameraAnteriore = s.FotocameraAnteriore;
            d.TastoHome = s.TastoHome;
            d.TouchId = s.TouchId;
            d.FaceId = s.FaceId;
            d.WiFi = s.WiFi;
            d.Rete = s.Rete;
            d.Chiamata = s.Chiamata;
            d.SchedaMadre = s.SchedaMadre;
            d.VetroPosteriore = s.VetroPosteriore;
        }
    }
}