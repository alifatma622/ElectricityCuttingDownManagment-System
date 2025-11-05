using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ElectricityCuttingDownManagmentSystem.API.DTOs;
using ElectricityCuttingDownManagmentSystem.API.Interfaces;
using ElectricityCuttingDownManagmentSystem.API.Models;
using ElectricityCuttingDownManagmentSystem.API.Data;

namespace ElectricityCuttingDownManagmentSystem.API.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly Electricity_STAContext _context;

        public IncidentRepository(Electricity_STAContext context)
        {
            _context = context;
        }

        public async Task<Cutting_Down_A> AddCabinIncidentAsync(Cutting_Down_A incident)
        {
            await _context.Cutting_Down_A.AddAsync(incident);
            await _context.SaveChangesAsync();
            return incident;
        }

        public async Task<Cutting_Down_B> AddCableIncidentAsync(Cutting_Down_B incident)
        {
            await _context.Cutting_Down_B.AddAsync(incident);
            await _context.SaveChangesAsync();
            return incident;
        }

        public async Task<Cutting_Down_A?> GetCabinIncidentByIdAsync(int id)
        {
            return await _context.Cutting_Down_A
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CuttingDownAIncidentID == id);
        }

        public async Task<Cutting_Down_B?> GetCableIncidentByIdAsync(int id)
        {
            return await _context.Cutting_Down_B
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CuttingDownBIncidentID == id);
        }

        public async Task<List<Cutting_Down_A>> GetOpenCabinIncidentsAsync()
        {
            return await _context.Cutting_Down_A
                .Where(x => x.EndDate == null && x.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Cutting_Down_B>> GetOpenCableIncidentsAsync()
        {
            return await _context.Cutting_Down_B
                .Where(x => x.EndDate == null && x.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateCabinIncidentAsync(Cutting_Down_A incident)
        {
            _context.Cutting_Down_A.Update(incident);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateCableIncidentAsync(Cutting_Down_B incident)
        {
            _context.Cutting_Down_B.Update(incident);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> CabinExistsAsync(int cabinId)
        {
            return await _context.Cutting_Down_A
                .AnyAsync(x => x.CabinKey == cabinId);
        }

        public async Task<bool> CableExistsAsync(int cableId)
        {
            return await _context.Cutting_Down_B
                .AnyAsync(x => x.CableKey == cableId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetExistingCabinKeysAsync()
        {
            return await _context.Cabin
                .Select(c => c.Cabin_Key)
                .ToListAsync();
        }

        public async Task<List<int>> GetExistingCableKeysAsync()
        {
            return await _context.Cable
                .Select(c => c.Cable_Key)
                .ToListAsync();
        }
    }
}