using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ACHR.Common
{
   
        public enum eFileDialog { en_OpenFile = 0, en_SaveFile = 1 };
    

   
        #region  The class implements FileDialog for open in front of B1 window

        #region an example of using GetFileNameClass()
        /*
     using(GetFileNameClass oGetFileName = new GetFileNameClass())
     {
          oGetFileName.Filter = "txt files (.txt)|.txt|All files (.)|." ;
          oGetFileName.InitialDirectory = "c:
";
 
          Thread threadGetFile = new Thread(new ThreadStart(oGetFileName.GetFileName));
          threadGetFile.ApartmentState = ApartmentState.STA;
          try
          {
               threadGetFile.Start();
               while (!threadGetFile.IsAlive); // Wait for thread to get started
               Thread.Sleep(1);                    // Wait a sec more
               threadGetFile.Join();               // Wait for thread to end
 
               // Use file name as you will here
               if (oGetFileName.FileName != string.Empty)
               {
                    //
                    //  ADD YOU CODE HERE!!!!
                    //
               }
          }
          catch(Exception ex)
          {
               SBOApp.MessageBox(ex.Message,1,"OK","","");
          }
     }
     */
        #endregion

        public class GetFileNameClass : IDisposable
        {
            [DllImport("user32.dll")]
            private static extern IntPtr GetForegroundWindow();

            System.Windows.Forms.FileDialog _oFileDialog;

            // Properties
            public string FileName
            {
                get { return _oFileDialog.FileName; }
                set { _oFileDialog.FileName = value; }
            }

            public string[] FileNames
            {
                get { return _oFileDialog.FileNames; }
            }

            public string Filter
            {
                get { return _oFileDialog.Filter; }
                set { _oFileDialog.Filter = value; }
            }

            public string InitialDirectory
            {
                get { return _oFileDialog.InitialDirectory; }
                set { _oFileDialog.InitialDirectory = value; }
            }

            // Constructor
            public GetFileNameClass(eFileDialog dlg)
            {
                switch ((int)dlg)
                {
                    case 0: _oFileDialog = new System.Windows.Forms.OpenFileDialog(); break;
                    case 1: _oFileDialog = new System.Windows.Forms.SaveFileDialog(); break;
                    default: throw new ApplicationException("GetFileNameClass incorrect parameter");
                }
            }

            public GetFileNameClass()
                : this(eFileDialog.en_OpenFile)
            {

            }

            // Dispose
            public void Dispose()
            {
                _oFileDialog.Dispose();
            }

            // Methods

            public void GetFileName()
            {
                IntPtr ptr = GetForegroundWindow();
                WindowWrapper oWindow = new WindowWrapper(ptr);
                if (_oFileDialog.ShowDialog(oWindow) != System.Windows.Forms.DialogResult.OK)
                {
                    _oFileDialog.FileName = string.Empty;
                }
                oWindow = null;
            } // End of GetFileName
        }
        #endregion

        #region WindowWrapper : System.Windows.Forms.IWin32Window

        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {
            private IntPtr _hwnd;

            // Property
            public virtual IntPtr Handle
            {
                get { return _hwnd; }
            }

            // Constructor
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }
        }
        #endregion

    }

