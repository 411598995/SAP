using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Modal
{
    class Project
    {
        public string Active { get; set; }
        public string Code { get; set; } // max length 30;
        public string Name { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        // Need to figure out user fiels

        public Project()
        {

        }


    }
}
