#region Using directives
using System;
using System.Collections.Generic;
using System.Windows.Forms;
#endregion

namespace WarehouseTransfer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            clsStartup StartUp = null;
            StartUp = new clsStartup();
            System.Windows.Forms.Application.Run();
        }        
    }
}