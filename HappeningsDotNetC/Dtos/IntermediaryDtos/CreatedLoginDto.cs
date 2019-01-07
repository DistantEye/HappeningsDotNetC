using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappeningsDotNetC.Dtos.IntermediaryDtos
{
    // Avoids confusion as far as Hashed/Unhashed PW, not strictly necessary as a separate Dto
    public class CreatedLoginDto
    {

        public string UserName { get; set; }
        public string Hash { get; set; }

    }
}
