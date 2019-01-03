using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR.Screen
{
    class frm_TSS : HRMSBaseForm
    {

        public bool isForLoading = false;
        SAPbouiCOM.Folder tbORDR1, tbORDR2, tbORDR3, tbORDR4, tbOpr1, tbOpr2, tbOpr3, tbOpr4 ;
        SAPbouiCOM.Matrix mtSOP, mtTOR, mtSTO, mtORI, mtORAT, mtStock;
        
    


        private void InitiallizeForm()
        {


            oForm.Freeze(true);

         
            oForm.Freeze(false);



           




        }

    }
}
