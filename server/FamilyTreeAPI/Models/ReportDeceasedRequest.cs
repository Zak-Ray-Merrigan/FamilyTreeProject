using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeAPI.Models
{
    public class ReportDeceasedRequest
    {
        public PersonElement Element
        {
            get;
            set;
        }

        public string DeceasedDate
        {
            get;
            set;
        }
    }
}