using System.Threading.Tasks;
using ElectricityCuttingDownManagmentSystem.API.Models;


namespace ElectricityCuttingDownManagmentSystem.API.Interfaces
{
    public interface IIncidentRepository
    {
        // Create
        Task<Cutting_Down_A> AddCabinIncidentAsync(Cutting_Down_A incident);
        Task<Cutting_Down_B> AddCableIncidentAsync(Cutting_Down_B incident);

        // Read
        Task<Cutting_Down_A?> GetCabinIncidentByIdAsync(int id);
        Task<Cutting_Down_B?> GetCableIncidentByIdAsync(int id);

        // Get Open Incidents
        Task<List<Cutting_Down_A>> GetOpenCabinIncidentsAsync();
         Task<List<Cutting_Down_B>> GetOpenCableIncidentsAsync();

        // Update
        Task<bool> UpdateCabinIncidentAsync(Cutting_Down_A incident);
        Task<bool> UpdateCableIncidentAsync(Cutting_Down_B incident);

        // Validation
        Task<bool> CabinExistsAsync(int cabinId);
        Task<bool> CableExistsAsync(int cableId);

        Task<List<int>> GetExistingCabinKeysAsync();
        Task<List<int>> GetExistingCableKeysAsync();

        // Save
        Task<int> SaveChangesAsync();

     
    }
}
