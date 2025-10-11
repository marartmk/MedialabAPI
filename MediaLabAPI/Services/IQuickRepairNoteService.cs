using MediaLabAPI.DTOs.Repair;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaLabAPI.Services
{
    public interface IQuickRepairNoteService
    {
        Task<QuickRepairNoteResponseDto> CreateQuickNoteAsync(CreateQuickRepairNoteDto dto);
        Task<QuickRepairNoteDetailDto?> GetQuickNoteByIdAsync(int id);
        Task<QuickRepairNoteDetailDto?> GetQuickNoteByNoteIdAsync(Guid noteId);
        Task<IEnumerable<QuickRepairNoteDetailDto>> SearchQuickNotesAsync(QuickRepairNoteSearchDto searchDto);
        Task UpdateQuickNoteAsync(Guid noteId, UpdateQuickRepairNoteDto dto);
        Task DeleteQuickNoteAsync(Guid noteId);
    }
}