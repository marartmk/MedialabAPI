using Microsoft.EntityFrameworkCore;  // 👈 AGGIUNTO: Import necessario per AnyAsync
using MediaLabAPI.Data;
using MediaLabAPI.Models;

namespace MediaLabAPI.Services
{
    public interface IRepairCodeGenerator
    {
        Task<string> GenerateUniqueRepairCodeAsync();
        bool IsValidRepairCode(string code);
    }

    public class RepairCodeGenerator : IRepairCodeGenerator
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RepairCodeGenerator> _logger;

        public RepairCodeGenerator(AppDbContext context, ILogger<RepairCodeGenerator> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GenerateUniqueRepairCodeAsync()
        {
            const int maxAttempts = 10;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                var code = GenerateRepairCode();

                // 🔧 FIX: Verifica unicità nel database con using EF
                var exists = await _context.DeviceRepairs
                    .AnyAsync(r => r.RepairCode == code);

                if (!exists)
                {
                    _logger.LogInformation("Generated unique repair code: {Code}", code);
                    return code;
                }

                attempts++;
                _logger.LogWarning("Repair code collision attempt {Attempt}: {Code}", attempts, code);

                // 👈 AGGIUNTO: Piccolo delay per evitare collisioni rapide
                await Task.Delay(10);
            }

            // Fallback con timestamp più preciso e random aggiuntivo
            var fallbackCode = $"REP{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";
            _logger.LogWarning("Using fallback repair code: {Code}", fallbackCode);
            return fallbackCode;
        }

        private string GenerateRepairCode()
        {
            var now = DateTime.Now;
            var year = now.ToString("yyyy");
            var month = now.ToString("MM");
            var day = now.ToString("dd");

            // 🔧 MIGLIORATO: Algoritmo più robusto per evitare collisioni
            var timeComponent = now.ToString("HHmmss");
            var randomComponent = new Random().Next(100, 999);

            // Formato: REP + YYYY + MM + DD + HHMMSS + RND
            // Esempio: REP20250719143525742
            return $"REP{year}{month}{day}{timeComponent}{randomComponent}";
        }

        public bool IsValidRepairCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            // Formato atteso: REP + YYYYMMDDHHMMSS + RRR (minimo 20 caratteri)
            return code.StartsWith("REP", StringComparison.OrdinalIgnoreCase) &&
                   code.Length >= 20 &&
                   code.Length <= 25 &&
                   code.Substring(3).All(char.IsDigit);
        }

        // 🆕 AGGIUNTO: Metodo helper per estrarre info dal codice
        public static (DateTime? createdDate, bool isValid) ParseRepairCode(string repairCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(repairCode) || !repairCode.StartsWith("REP"))
                    return (null, false);

                // Estrai la parte data: YYYYMMDDHHMMSS
                var datePart = repairCode.Substring(3, Math.Min(14, repairCode.Length - 3));

                if (datePart.Length >= 8) // Almeno YYYYMMDD
                {
                    var year = int.Parse(datePart.Substring(0, 4));
                    var month = int.Parse(datePart.Substring(4, 2));
                    var day = int.Parse(datePart.Substring(6, 2));

                    var hour = datePart.Length >= 10 ? int.Parse(datePart.Substring(8, 2)) : 0;
                    var minute = datePart.Length >= 12 ? int.Parse(datePart.Substring(10, 2)) : 0;
                    var second = datePart.Length >= 14 ? int.Parse(datePart.Substring(12, 2)) : 0;

                    var date = new DateTime(year, month, day, hour, minute, second);
                    return (date, true);
                }
            }
            catch
            {
                // Ignore parsing errors
            }

            return (null, false);
        }
    }
}