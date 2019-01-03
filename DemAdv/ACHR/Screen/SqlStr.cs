using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class SqlStr
    {

        public string frm_150_getChild1(string fatherCode)
        {
            return @"Select * from [@B1_ITB] where U_Father='" + fatherCode + "' order by convert(int,code) ";
        }

    }

}
