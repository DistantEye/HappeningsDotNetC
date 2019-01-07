using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Dtos.IntermediaryDtos
{
    // Avoids confusion as far as Hashed/Unhashed PW, not strictly necessary as a separate Dto
    public class CalendarSearchFilterDto
    {
        [Required]
        public Guid? UserId { get; set; }
        public bool TextualDisplay { get; set; }
        public DateTime? StartDate { get; set; } // both inclusive
        public DateTime? EndDate { get; set; } 

    }
 
}
