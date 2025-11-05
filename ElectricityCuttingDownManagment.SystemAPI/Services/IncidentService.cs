using System.Threading.Tasks;
using ElectricityCuttingDownManagmentSystem.API.DTOs;
using ElectricityCuttingDownManagmentSystem.API.Interfaces;
using ElectricityCuttingDownManagmentSystem.API.Models;
using ElectricityCuttingDownManagmentSystem.API.Data;


namespace ElectricityCuttingDownManagmentSystem.API.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly IIncidentRepository _repository;

        public IncidentService(IIncidentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IncidentResponseDto> CreateIncidentAsync(CreateIncidentDto dto, string sourceType)
        {

            if (dto.ResourceKey <= 0)
                throw new ArgumentException("مفتاح المورد غير صحيح");

            if (sourceType == "A")
            {

                if (!await _repository.CabinExistsAsync(dto.ResourceKey))
                    throw new KeyNotFoundException($"الكابينة بالرقم {dto.ResourceKey} غير موجودة");

                var incident = new Cutting_Down_A
                {
                    CabinKey = dto.ResourceKey,
                    ProblemTypeKey = dto.ProblemTypeKey,
                    CreateDate = dto.CreateDate,
                    EndDate = dto.EndDate,
                    IsPlanned = dto.IsPlanned,
                    IsGlobal = dto.IsGlobal,
                    PlannedStartDTS = dto.PlannedStartDTS,
                    PlannedEndDTS = dto.PlannedEndDTS,
                    IsActive = dto.IsActive,
                    CreatedUser = dto.CreatedUser,
                    IsProcessed = dto.IsProcessed
                };

                var result = await _repository.AddCabinIncidentAsync(incident);

                return new IncidentResponseDto
                {
                    IncidentID = result.CuttingDownAIncidentID,
                    SourceType = "A",
                    ResourceKey = result.CabinKey,
                    ProblemTypeKey = result.ProblemTypeKey,
                    CreateDate = result.CreateDate,
                    EndDate = result.EndDate,
                    IsPlanned = result.IsPlanned,
                    IsGlobal = result.IsGlobal,
                    PlannedStartDTS = result.PlannedStartDTS,
                    PlannedEndDTS = result.PlannedEndDTS,
                    IsActive = result.IsActive,
                    CreatedUser = result.CreatedUser,
                    IsProcessed = result.IsProcessed,
                    Message = "تم إنشاء حادثة الكابينة بنجاح",
                    ResponseTime = DateTime.Now
                };
            }
            else if (sourceType == "B")
            {
                // التحقق من وجود الكابل
                if (!await _repository.CableExistsAsync(dto.ResourceKey))
                    throw new KeyNotFoundException($"الكابل بالرقم {dto.ResourceKey} غير موجود");

                var incident = new Cutting_Down_B
                {
                    CableKey = dto.ResourceKey,
                    ProblemTypeKey = dto.ProblemTypeKey,
                    CreateDate = dto.CreateDate,
                    EndDate = dto.EndDate,
                    IsPlanned = dto.IsPlanned,
                    IsGlobal = dto.IsGlobal,
                    PlannedStartDTS = dto.PlannedStartDTS,
                    PlannedEndDTS = dto.PlannedEndDTS,
                    IsActive = dto.IsActive,
                    CreatedUser = dto.CreatedUser,
                    IsProcessed = dto.IsProcessed
                };

                var result = await _repository.AddCableIncidentAsync(incident);

                return new IncidentResponseDto
                {
                    IncidentID = result.CuttingDownBIncidentID,
                    SourceType = "B",
                    ResourceKey = result.CableKey,
                    ProblemTypeKey = result.ProblemTypeKey,
                    CreateDate = result.CreateDate,
                    EndDate = result.EndDate,
                    IsPlanned = result.IsPlanned,
                    IsGlobal = result.IsGlobal,
                    PlannedStartDTS = result.PlannedStartDTS,
                    PlannedEndDTS = result.PlannedEndDTS,
                    IsActive = result.IsActive,
                    CreatedUser = result.CreatedUser,
                    IsProcessed = result.IsProcessed,
                    Message = "تم إنشاء حادثة الكابل بنجاح",
                    ResponseTime = DateTime.Now
                };
            }
            else
            {
                throw new ArgumentException("نوع المصدر غير صحيح. يجب أن يكون A أو B");
            }
        }

        public async Task<IncidentResponseDto> CloseIncidentAsync(CloseIncidentDto dto)
        {
            if (dto.SourceType == "A")
            {
                var incident = await _repository.GetCabinIncidentByIdAsync(dto.IncidentID);
                if (incident == null)
                    throw new KeyNotFoundException($"حادثة الكابينة بالرقم {dto.IncidentID} غير موجودة");

                incident.EndDate = dto.EndDate ?? DateTime.Now;
                incident.UpdatedUser = dto.UpdatedUser;
                incident.IsProcessed = dto.IsProcessed;

                await _repository.UpdateCabinIncidentAsync(incident);

                return new IncidentResponseDto
                {
                    IncidentID = incident.CuttingDownAIncidentID,
                    SourceType = "A",
                    ResourceKey = incident.CabinKey,
                    ProblemTypeKey = incident.ProblemTypeKey,
                    CreateDate = incident.CreateDate,
                    EndDate = incident.EndDate,
                    IsPlanned = incident.IsPlanned,
                    IsGlobal = incident.IsGlobal,
                    PlannedStartDTS = incident.PlannedStartDTS,
                    PlannedEndDTS = incident.PlannedEndDTS,
                    IsActive = incident.IsActive,
                    CreatedUser = incident.CreatedUser,
                    UpdatedUser = incident.UpdatedUser,
                    IsProcessed = incident.IsProcessed,
                    Message = "تم إغلاق حادثة الكابينة بنجاح",
                    ResponseTime = DateTime.Now
                };
            }
            else if (dto.SourceType == "B")
            {
                var incident = await _repository.GetCableIncidentByIdAsync(dto.IncidentID);
                if (incident == null)
                    throw new KeyNotFoundException($"حادثة الكابل بالرقم {dto.IncidentID} غير موجودة");

                incident.EndDate = dto.EndDate ?? DateTime.Now;
                incident.UpdatedUser = dto.UpdatedUser;
                incident.IsProcessed = dto.IsProcessed;

                await _repository.UpdateCableIncidentAsync(incident);

                return new IncidentResponseDto
                {
                    IncidentID = incident.CuttingDownBIncidentID,
                    SourceType = "B",
                    ResourceKey = incident.CableKey,
                    ProblemTypeKey = incident.ProblemTypeKey,
                    CreateDate = incident.CreateDate,
                    EndDate = incident.EndDate,
                    IsPlanned = incident.IsPlanned,
                    IsGlobal = incident.IsGlobal,
                    PlannedStartDTS = incident.PlannedStartDTS,
                    PlannedEndDTS = incident.PlannedEndDTS,
                    IsActive = incident.IsActive,
                    CreatedUser = incident.CreatedUser,
                    UpdatedUser = incident.UpdatedUser,
                    IsProcessed = incident.IsProcessed,
                    Message = "تم إغلاق حادثة الكابل بنجاح",
                    ResponseTime = DateTime.Now
                };
            }
            else
            {
                throw new ArgumentException("نوع المصدر غير صحيح. يجب أن يكون A أو B");
            }
        }

        public async Task<int> GenerateTestDataAsync(int count, string sourceType)
        {
            var generatedCount = 0;
            var random = new Random();

            // نجيب المفاتيح الموجودة فعلاً في الداتابيز
            var existingCabinKeys = await _repository.GetExistingCabinKeysAsync();
            var existingCableKeys = await _repository.GetExistingCableKeysAsync();

            if (!existingCabinKeys.Any() && sourceType == "A")
            {
                throw new Exception("No cabins found in database");
            }

            if (!existingCableKeys.Any() && sourceType == "B")
            {
                throw new Exception("No cables found in database");
            }

            for (int i = 0; i < count; i++)
            {
                try
                {
                    if (sourceType == "A")
                    {
                        var randomCabinKey = existingCabinKeys[random.Next(existingCabinKeys.Count)];

                        var incident = new Cutting_Down_A
                        {
                            CabinKey = randomCabinKey,
                            ProblemTypeKey = random.Next(1, 13),
                            CreateDate = DateTime.Now.AddDays(-random.Next(1, 30)),
                            EndDate = random.Next(2) == 0 ? DateTime.Now.AddDays(-random.Next(1, 10)) : null,
                            IsPlanned = random.Next(2) == 0,
                            IsGlobal = random.Next(2) == 0,
                            PlannedStartDTS = random.Next(2) == 0 ? DateTime.Now.AddDays(random.Next(1, 15)) : null,
                            PlannedEndDTS = random.Next(2) == 0 ? DateTime.Now.AddDays(random.Next(16, 30)) : null,
                            IsActive = true,
                            CreatedUser = "TestUser",
                            IsProcessed = false
                        };

                        await _repository.AddCabinIncidentAsync(incident);
                        generatedCount++;
                    }
                    else if (sourceType == "B")
                    {
                        var randomCableKey = existingCableKeys[random.Next(existingCableKeys.Count)];

                        var incident = new Cutting_Down_B
                        {
                            CableKey = randomCableKey,
                            ProblemTypeKey = random.Next(1, 13), 
                            CreateDate = DateTime.Now.AddDays(-random.Next(1, 30)),  
                            EndDate = random.Next(2) == 0 ? DateTime.Now.AddDays(-random.Next(1, 10)) : null,  // ⬅️ نضيف
                            IsPlanned = random.Next(2) == 0,  
                            IsGlobal = random.Next(2) == 0,  
                            PlannedStartDTS = random.Next(2) == 0 ? DateTime.Now.AddDays(random.Next(1, 15)) : null,  // ⬅️ نضيف
                            PlannedEndDTS = random.Next(2) == 0 ? DateTime.Now.AddDays(random.Next(16, 30)) : null,  // ⬅️ نضيف
                            IsActive = true,  
                            CreatedUser = "TestUser",  
                            IsProcessed = false 
                        };

                        await _repository.AddCableIncidentAsync(incident);
                        generatedCount++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generating test data: {ex.Message}");
                }
            }

            return generatedCount;
        }
    }
    }
