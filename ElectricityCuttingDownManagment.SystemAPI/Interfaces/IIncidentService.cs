using System.Threading.Tasks;
using ElectricityCuttingDownManagmentSystem.API.DTOs;

namespace ElectricityCuttingDownManagmentSystem.API.Interfaces
{
    public interface IIncidentService
    {

        //creating new incident
        Task<IncidentResponseDto> CreateIncidentAsync(CreateIncidentDto dto, string sourceType);


        ///closing  exist incident
        Task<IncidentResponseDto> CloseIncidentAsync(CloseIncidentDto dto);

      
        Task<int> GenerateTestDataAsync(int count, string sourceType);
    }
}
