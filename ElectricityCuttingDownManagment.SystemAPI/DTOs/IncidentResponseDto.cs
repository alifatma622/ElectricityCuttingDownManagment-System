namespace ElectricityCuttingDownManagmentSystem.API.DTOs
{
    public class IncidentResponseDto
    {
     
        public int IncidentID { get; set; }


        public string SourceType { get; set; }

        public int ResourceKey { get; set; }

        public int ProblemTypeKey { get; set; }
        public DateTime CreateDate { get; set; }


        public DateTime? EndDate { get; set; }


        public bool IsPlanned { get; set; }

        public bool IsGlobal { get; set; }


        public DateTime? PlannedStartDTS { get; set; }

        public DateTime? PlannedEndDTS { get; set; }

        public bool IsActive { get; set; }

   
        public string CreatedUser { get; set; }

    
        public string? UpdatedUser { get; set; }


        public bool IsProcessed { get; set; }

        public string Status => EndDate.HasValue ? "Closed" : "Opened";


        public string Message { get; set; }

        public DateTime ResponseTime { get; set; } = DateTime.Now;
    }
}
