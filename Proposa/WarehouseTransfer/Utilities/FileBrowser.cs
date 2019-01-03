using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;



namespace Utilities
{
    public class WindowWrapper : System.Windows.Forms.IWin32Window
    {
        private IntPtr _hwnd;

        public WindowWrapper(IntPtr handle)
        {
            _hwnd = handle;
        }

        public System.IntPtr Handle
        {
            get { return _hwnd; }
        }
    }

    class FileBrowser
    {
        Thread ShowFolderBrowserThread = null;
        public  SAPbouiCOM.Application oApplication;

        #region ShowFolderBrowser

     

        public void ShowFolderBrowserAbsent()
        {

            SAPbouiCOM.Form oForm = oApplication.Forms.ActiveForm;
            OpenFileDialog MyTest = new OpenFileDialog();
            Process[] MyProcs;
            MyProcs = Process.GetProcessesByName("SAP Business One");
            if (MyProcs.Length != 0)
            {
                for (int i = 0; i <= 0; i++)
                {
                    WindowWrapper MyWindow = new WindowWrapper(MyProcs[i].MainWindowHandle);
                    MyTest.Filter = "Excel files 97-2003 (*.xls)|*.xls|Excel files(*.xlsx)|*.xlsx";
                    if (MyTest.ShowDialog(MyWindow) == DialogResult.OK)
                    {
                        //if (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE)
                        //{
                        //    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                        //}
                        //oForm.DataSources.UserDataSources.Add("usdPath", SAPbouiCOM.BoDataType.dt_LONG_TEXT, 250);
                        //oForm.DataSources.UserDataSources.Item("usdPath").ValueEx = MyTest.FileName;
                        ((SAPbouiCOM.EditText)(oForm.Items.Item("EdImport").Specific)).Value = MyTest.FileName;

                        System.Windows.Forms.Application.ExitThread();
                    }
                    else
                    {
                        System.Windows.Forms.Application.ExitThread();
                    }
                }
            }
            else
            {
                Console.WriteLine("No SBO instances found.");
            }
        }

        #endregion

        #region BrowseFileDialog
        

   

        public void BrowseFileDialogAbsent()
        {
            try
            {
                ShowFolderBrowserThread = new System.Threading.Thread(ShowFolderBrowserAbsent);
                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                }
                //ShowFolderBrowserThread.Join() 
                else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();
                }
            }
            catch (Exception ex)
            {
                //objMain.objApplication.StatusBar.SetText(ex.Message);
            }
        }

        #endregion

        //public void DisplayFile()
        //{
        //    string strFullPath = string.Empty;
        //    //Dim oEditColPath As SAPbouiCOM.EditTextColumn
        //    int CurrentPane = oForm.PaneLevel;
        //    if (CurrentPane == 11)
        //    {
        //        oEditText = (SAPbouiCOM.EditText)oForm.Items.Item("et_path").Specific;
        //    }
        //    if (CurrentPane == 12)
        //    {
        //        oEditText = (SAPbouiCOM.EditText)oForm.Items.Item("et_Path1").Specific;
        //    }
        //    string strImgPath = oEditText.Value.ToString();
        //    if (strImgPath != "")
        //    {
        //        openFile(strImgPath);
        //    }
        //}

        public void openFile(string strpath)
        {
            ProcessStartInfo X = new ProcessStartInfo();
            X.UseShellExecute = true;
            X.FileName = strpath;
            Process.Start(X);
            X = null;
        }

    }


}
