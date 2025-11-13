using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Application.DTO.Event
{
    public  class GetEvent
    {
        public int EventId { get; set; }
        public int BranchId { get; set; }
    }
}
