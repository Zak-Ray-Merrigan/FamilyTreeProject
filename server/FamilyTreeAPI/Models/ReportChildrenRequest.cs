using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FamilyTreeAPI.Models
{
    public class ReportChildrenRequest
    {
        public FamilyElement Parent
        {
            get;
            set;
        }

        public FamilyElement Child
        {
            get;
            set;
        }
    }
}