using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ContactViewModel
    {
        public string Company { get; set; }
        public string CompanyTitle { get; set; }
        public string Website { get; set; }
        public string BriefDescription { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Phone { get; set; }
        public string Cell { get; set; }
        public string Email { get; set; }
        public string BusinessHours1 { get; set; }
        public string BusinessHours2 { get; set; }
    }
}
