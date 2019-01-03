using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using SAPbobsCOM;
using SAPbouiCOM;

namespace Utilities
{
    public class UtilitiesCls
    {
        
        public static string Comboselect(string comboboxid, SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm)
        {
            string comboselect = "";
            try
            {
                string sRegioncombo = ((SAPbouiCOM.ComboBox)oForm.Items.Item(comboboxid).Specific).Selected.Value.ToString();

                return sRegioncombo;
            }
            catch (Exception ex)
            {
            }
            return comboselect;
        }

        public static string ComboselectDescription(string comboboxid, SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm)
        {
            string comboselect = "";
            try
            {
                string sRegioncombo = ((SAPbouiCOM.ComboBox)oForm.Items.Item(comboboxid).Specific).Selected.Description.ToString();

                return sRegioncombo;
            }
            catch (Exception ex)
            {
            }
            return comboselect;
        }

        public static void ChooseFromListMatrixEvent(SAPbouiCOM.Application SBO_Application, ref SAPbouiCOM.ItemEvent pVal, SAPbouiCOM.Form oForm, string MatrixId, string Column1, string Column2, string field1, string field2)
        {
           SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
          oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));

          string sCFL_ID = null;
            string sCode = null;
            string sName = null;

            SAPbouiCOM.EditText tName = null;
           SAPbouiCOM.EditText tCode = null;

            SAPbouiCOM.Matrix oMatrix;

            sCFL_ID = oCFLEvento.ChooseFromListUID;
            SAPbouiCOM.ChooseFromList oCFL = null;
           oCFL = oForm.ChooseFromLists.Item(sCFL_ID);
           if (oCFLEvento.BeforeAction == false)
            {
                SAPbouiCOM.DataTable oDataTable = null;
                oDataTable = oCFLEvento.SelectedObjects;
                if (oDataTable != null)
                {
                    oMatrix = ((SAPbouiCOM.Matrix)(oForm.Items.Item(MatrixId).Specific));
                    try
                    {
                        sCode = System.Convert.ToString(oDataTable.GetValue(field1, 0));
                        tCode = (SAPbouiCOM.EditText)oMatrix.Columns.Item(Column1).Cells.Item(pVal.Row).Specific;
                       tCode.Value = sCode;
                    }
                    catch (Exception)
                    {

                        tCode.Value = sCode;
                        if (Column2 != "" && field2 != "")
                        {
                            tName = (SAPbouiCOM.EditText)oMatrix.Columns.Item(Column2).Cells.Item(pVal.Row).Specific;
                            sName = System.Convert.ToString(oDataTable.GetValue(field2, 0));
                            tName.Value = sName;
                       }
                    }
                }
            }
       }
    
        public static Recordset ExecuteRecordset(string StoredProcName, object[] Parameters, SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany)
        {

            string SqlCommand = GenerateSqlQuery(StoredProcName, Parameters);
            Recordset oRecordSet = (Recordset)oCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
            oRecordSet.DoQuery(SqlCommand);
            return oRecordSet;
        }

        public static void AllowNumericForOdoMeterValue(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent, string EditText, SAPbouiCOM.Form oForm)
        {
            SAPbouiCOM.EditText oEdit = (SAPbouiCOM.EditText)oForm.Items.Item(EditText).Specific;
            string strEdit = oEdit.Value.ToString();
            int intCount = strEdit.Length+1;
            if (intCount != 8)
            {
                try
                {

                    int i = pVal.CharPressed;
                    if ((i >=48 && i <= 57) || (i == 9) || (i == 32) || (i == 14) || (i == 15) || (i == 8) || (i == 127))
                    {
                        BubbleEvent = true;
                    }
                    else
                    {
                        BubbleEvent = false;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public static void AllowNumeric(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            try
            {
                int i = pVal.CharPressed;
                if ((i >= 48 && i <= 57) || (i == 9) || (i == 32) || (i == 14) || (i == 15) || (i == 8) || (i == 127) || (i == 37) || (i == 39) || (i == 46))
                {
                    BubbleEvent = true;
                }
                else
                {
                    BubbleEvent = false;
                }                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void AllowAlphaNumericwithNumber(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            try
            {


                int i = pVal.CharPressed;
                if ((i >= 65 && i <= 90) || (i >= 97 && i <= 122) || (i >= 47 && i <= 57) || (i == 9) || (i == 32) || (i == 14) || (i == 15) || (i == 8) || (i == 127) || (i==45)||(i==92))
                {
                    BubbleEvent = true;

                }
                else
                {
                    BubbleEvent = false;

                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void AllowAlphaNumeric(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            try
            {


                int i = pVal.CharPressed;
                if ((i >= 65 && i <= 90) || (i >= 97 && i <= 122) || (i == 9) || (i == 32) || (i == 14) || (i == 15) || (i == 8) || (i == 127) ||(i==46))
                {
                    BubbleEvent = true;

                }
                else
                {
                    BubbleEvent = false;

                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string GenerateSqlQuery(string spName, object[] objParameters)
        {
            StringBuilder sqlQuery = new StringBuilder(string.Empty);
            if (objParameters != null && objParameters.Length > 0)
            {
                foreach (object objParam in objParameters)
                {
                    if (sqlQuery.Length <= 0)
                    {
                        sqlQuery.Append(spName);
                        sqlQuery.Append(" ");

                        if (objParam.GetType() == typeof(System.Int32))
                            sqlQuery.Append(objParam.ToString());

                        else if (objParam.GetType() == typeof(System.Double))
                            sqlQuery.Append(objParam.ToString());

                        else
                        {
                            sqlQuery.Append("'");
                            sqlQuery.Append(objParam.ToString());
                            sqlQuery.Append("'");
                        }
                    }
                    else
                    {
                        sqlQuery.Append(", ");

                        if (objParam.GetType() == typeof(System.Int32))
                            sqlQuery.Append(objParam.ToString());
                        else if (objParam.GetType() == typeof(System.Double))
                            sqlQuery.Append(objParam.ToString());
                        else
                        {
                            sqlQuery.Append("'");
                            sqlQuery.Append(objParam.ToString() + "'");
                        }
                    }
                }
                return Convert.ToString(sqlQuery);
            }
            return spName;
        }

        public static string ConvertToSAPDate(string sDate)
        {
            sDate = sDate.Insert(4, "/");
            sDate = sDate.Insert(7, "/");
            DateTime oDate;
            oDate = Convert.ToDateTime(sDate);
            return oDate.ToString("yyyyMMdd");
        }

        public static void UDOMatrixAddLine(SAPbouiCOM.Application SBO_Application, string formname, string tablename, string matrixname, string columname)
        {

            try
            {
                SAPbouiCOM.Form oForm;
                oForm = SBO_Application.Forms.Item(formname);
                SAPbouiCOM.Matrix mMatrixRowCount;
                mMatrixRowCount = ((SAPbouiCOM.Matrix)(oForm.Items.Item(matrixname).Specific));
                if (mMatrixRowCount.RowCount == 0)
                {
                    oForm.DataSources.DBDataSources.Item(tablename).Clear();
                    mMatrixRowCount.AddRow(1, -1);

                } 

                else
                {
                    if (((SAPbouiCOM.EditText)mMatrixRowCount.Columns.Item(columname).Cells.Item(mMatrixRowCount.RowCount).Specific).Value != "")
                    {
                        oForm.DataSources.DBDataSources.Item(tablename).Clear();
                        mMatrixRowCount.AddRow(1, -1);
                    }
                }

            }
            catch (Exception e)
            {
                SBO_Application.MessageBox(e.Message, 1, "OK", "", "");
            }

        }

        public static int IUDOMatrixAddLine(SAPbouiCOM.Application SBO_Application, string formname, string tablename, string matrixname, string columname, string columname1)
        {
            int i = 0;
            try
            {

                SAPbouiCOM.Form oForm;
                oForm = SBO_Application.Forms.Item(formname);
                SAPbouiCOM.Matrix mMatrixRowCount;
                mMatrixRowCount = ((SAPbouiCOM.Matrix)(oForm.Items.Item(matrixname).Specific));
                if (mMatrixRowCount.RowCount == 0)
                {
                    oForm.DataSources.DBDataSources.Item(tablename).Clear();
                    mMatrixRowCount.AddRow(1, -1);
                   
                }
                else
                {
                    if (((SAPbouiCOM.EditText)mMatrixRowCount.Columns.Item(columname).Cells.Item(mMatrixRowCount.RowCount).Specific).Value != "" &&Convert.ToDouble  (((SAPbouiCOM.EditText)mMatrixRowCount.Columns.Item(columname1).Cells.Item(mMatrixRowCount.RowCount).Specific).Value)!=0.0)
                    {
                        oForm.DataSources.DBDataSources.Item(tablename).Clear();
                        mMatrixRowCount.AddRow(1, -1);
                        i = 1;
                    }

                }

            }
            catch (Exception e)
            {
                SBO_Application.MessageBox(e.Message, 1, "OK", "", "");
            }
            return i;
        }

        public static void UDOMAtrixDeleteLine(SAPbouiCOM.Application SBO_Application, string formname, string matrixname)
        {

            try
            {
                SAPbouiCOM.Form oForm;
                oForm = SBO_Application.Forms.Item(formname);

                int RowCountInt;
                SAPbouiCOM.Matrix mMatrixRowCount;
                mMatrixRowCount = ((SAPbouiCOM.Matrix)(oForm.Items.Item(matrixname).Specific));
                RowCountInt = mMatrixRowCount.RowCount;
                for (int n = 1; n <= mMatrixRowCount.RowCount; n++)
                {
                    if (mMatrixRowCount.IsRowSelected(n))
                    {
                        mMatrixRowCount.DeleteRow(n);
                    }
                }
                if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                {
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                }
            }
            catch (Exception e)
            {
                SBO_Application.MessageBox(e.Message, 1, "OK", "", "");
            }

        }

        public static void UDOMAtrixDeleteLineforTrip(SAPbouiCOM.Application SBO_Application, string formname, string matrixname)
        {

            try
            {
                SAPbouiCOM.Form oForm;
                oForm = SBO_Application.Forms.Item(formname);

                int RowCountInt;
                SAPbouiCOM.Matrix mMatrixRowCount;
                mMatrixRowCount = ((SAPbouiCOM.Matrix)(oForm.Items.Item(matrixname).Specific));
                RowCountInt = mMatrixRowCount.RowCount;
                for (int n = 1; n <= mMatrixRowCount.RowCount; n++)
                {
                    if (mMatrixRowCount.IsRowSelected(n))
                    {
                        if (mMatrixRowCount.RowCount  ==n)
                            mMatrixRowCount.DeleteRow(n);
                        else
                            SBO_Application.MessageBox("You cannot delete the middle row", 1, "Ok", "", "");
                    }
                }
               
            }
            catch (Exception e)
            {
                SBO_Application.MessageBox(e.Message, 1, "OK", "", "");
            }

        }

        public static void UDOMatrixAddLineinOkMode(SAPbouiCOM.Application SBO_Application, string formname, string tablename, string matrixname, string columname)
        {

            try
            {
                SAPbouiCOM.Form oForm;
                oForm = SBO_Application.Forms.Item(formname);
                SAPbouiCOM.Matrix mMatrixRowCount;
                mMatrixRowCount = ((SAPbouiCOM.Matrix)(oForm.Items.Item(matrixname).Specific));              
                if (mMatrixRowCount.RowCount == 0)
                {
                    oForm.DataSources.DBDataSources.Item(tablename).Clear();
                    mMatrixRowCount.AddRow(1, -1);
                }
                else if (mMatrixRowCount.RowCount != 0)
                {
                    oForm.DataSources.DBDataSources.Item(tablename).Clear();
                    mMatrixRowCount.AddRow(1, -1);
                }
            
                else
                {
                    if (((SAPbouiCOM.EditText)mMatrixRowCount.Columns.Item(columname).Cells.Item(mMatrixRowCount.RowCount).Specific).Value != "")
                    {
                        oForm.DataSources.DBDataSources.Item(tablename).Clear();
                        mMatrixRowCount.AddRow(1, -1);
                    }
                }

            }
            catch (Exception e)
            {
                SBO_Application.MessageBox(e.Message, 1, "OK", "", "");
            }
        }

        public static string convertDate(string sDate)
        {
            sDate = sDate.Insert(4, "/");
            sDate = sDate.Insert(7, "/");
            return sDate;
        }

        public static void CFLCreationMatrix(SAPbouiCOM.Application SBO_Application, string objecttype, string FormID, string cflid, string matrixid, string columnid, string allias, bool Multiselection)
        {
            try
            {
                SAPbouiCOM.Form oForm;
                oForm = SBO_Application.Forms.Item(FormID);
                //oForm = SBO_Application.Forms.GetForm(FormID ,1);
                SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                oCFLs = oForm.ChooseFromLists;
                SAPbouiCOM.ChooseFromList oCFL = null;
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
                oCFLCreationParams.MultiSelection = Multiselection;
                oCFLCreationParams.ObjectType = objecttype;
                oCFLCreationParams.UniqueID = cflid;
                oCFL = oCFLs.Add(oCFLCreationParams);
                SAPbouiCOM.Column cFerro;
                SAPbouiCOM.Matrix oMatrixFerro;
                oMatrixFerro = ((SAPbouiCOM.Matrix)(oForm.Items.Item(matrixid).Specific));
                cFerro = (SAPbouiCOM.Column)oMatrixFerro.Columns.Item(columnid);
                cFerro.ChooseFromListUID = cflid;
                cFerro.ChooseFromListAlias = allias;              

            }
            catch(Exception e)
            { string lstrmsg = e.Message; }

        }

        public static void CFLCreationButtonSystem(SAPbouiCOM.Application SBO_Application, string objecttype, string FormID, string CflID, string itemid, ref SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.Form oForm;
            SAPbouiCOM.Button oButton;
            oForm = SBO_Application.Forms.GetFormByTypeAndCount(pVal.FormType, pVal.FormTypeCount);
            SAPbouiCOM.ChooseFromListCollection oCFls = null;
            oCFls = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromList oCFl = null;
            SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
            oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
            oCFLCreationParams.MultiSelection = true;
            oCFLCreationParams.ObjectType = objecttype;
            oCFLCreationParams.UniqueID = CflID;
            oCFl = oCFls.Add(oCFLCreationParams);
            oItem = oForm.Items.Item(itemid);
            oButton = (SAPbouiCOM.Button)oItem.Specific;
            oButton.ChooseFromListUID = CflID;
        }

        public static void CFLCreationButton(SAPbouiCOM.Application SBO_Application, string objecttype, string FormID, string CflID, string itemid)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.Form oForm;
            SAPbouiCOM.Button oButton;
            oForm = SBO_Application.Forms.Item(FormID);
            SAPbouiCOM.ChooseFromListCollection oCFls = null;
            oCFls = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromList oCFl = null;
            SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
            oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
            oCFLCreationParams.MultiSelection = true;
            oCFLCreationParams.ObjectType = objecttype;
            oCFLCreationParams.UniqueID = CflID;
            oCFl = oCFls.Add(oCFLCreationParams);
            oItem = oForm.Items.Item(itemid);
            oButton = (SAPbouiCOM.Button)oItem.Specific;
            oButton.ChooseFromListUID = CflID;
        }

        public static void ChooseFromListSystemMatrixEvent(SAPbouiCOM.Application SBO_Application, ref SAPbouiCOM.ItemEvent pVal, SAPbouiCOM.Form oForm, string MatrixId, string Column1, string Column2, string field1, string field2)
        {
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
            oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));

            string sCFL_ID = null;
            //string sCode = null;
            //string sName = null;

            //SAPbouiCOM.EditText tName = null;
            //SAPbouiCOM.EditText tCode = null;

            SAPbouiCOM.Matrix oMatrix;

            sCFL_ID = oCFLEvento.ChooseFromListUID;
            SAPbouiCOM.ChooseFromList oCFL = null;
            oCFL = oForm.ChooseFromLists.Item(sCFL_ID);
            if (oCFLEvento.BeforeAction == false)
            {
                SAPbouiCOM.DataTable oDataTable = null;
                oDataTable = oCFLEvento.SelectedObjects;
                if (oDataTable != null)
                {
                    try
                    {
                        string sItem1;
                        string sItem2 = "";
                        SAPbouiCOM.EditText eCode1;
                        SAPbouiCOM.EditText eCode2;

                        sItem1 = System.Convert.ToString(oDataTable.GetValue(field1, 0));
                        if (field2 != "")
                        {
                            sItem2 = System.Convert.ToString(oDataTable.GetValue(field2, 0));
                        }
                        oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item(MatrixId).Specific;
                        int i = pVal.Row;
                        eCode1 = ((SAPbouiCOM.EditText)oMatrix.Columns.Item(Column1).Cells.Item(i).Specific);
                        eCode1.Value = sItem1;
                        if (Column2 != "")
                        {
                            eCode2 = ((SAPbouiCOM.EditText)oMatrix.Columns.Item(Column2).Cells.Item(i).Specific);
                            eCode2.Value = sItem2;
                        }
                    }
                    catch (Exception e)
                    {
                        SBO_Application.MessageBox(e.Message, 1, "OK", "", "");
                    }
                }
            }
        }

        public static void choosefromlistEditText(SAPbouiCOM.Application SBO_Application, string objecttype, string FormID, string CflID, string itemid, string Alias)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.Form oForm;
            SAPbouiCOM.EditText oEditText;
            oForm = SBO_Application.Forms.Item(FormID);
            SAPbouiCOM.ChooseFromListCollection oCFls = null;
            oCFls = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromList oCFl = null;
            SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
            oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
            oCFLCreationParams.MultiSelection = false;
            oCFLCreationParams.ObjectType = objecttype;
            oCFLCreationParams.UniqueID = CflID;
            oCFl = oCFls.Add(oCFLCreationParams);
            oItem = oForm.Items.Item(itemid);
            oEditText = (SAPbouiCOM.EditText)oItem.Specific;
            oEditText.ChooseFromListUID = CflID;
            oEditText.ChooseFromListAlias = Alias;
        }

        public static void ChooseFromlistCondition(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, string ItemUID, string CFLId, string ObjectType, string Alias, string ConditionValue, string sChoosefromlistAlias)
        {
            try
            {
                SAPbouiCOM.ChooseFromListCollection oBPs;
                SAPbouiCOM.ChooseFromList oBP;
                SAPbouiCOM.ChooseFromListCreationParams oBPParams;
                SAPbouiCOM.Conditions oBPConditions;
                SAPbouiCOM.Condition oBPCondition;
                SAPbouiCOM.EditText tCardCode;
                oBPs = oForm.ChooseFromLists;

                oBPParams = (SAPbouiCOM.ChooseFromListCreationParams)SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams);

                oBPParams.MultiSelection = false;
                oBPParams.ObjectType = ObjectType;
                oBPParams.UniqueID = CFLId;
                oBP = oBPs.Add(oBPParams);

                oBPConditions = oBP.GetConditions();
                oBPCondition = oBPConditions.Add();
                oBPCondition.Alias = Alias;
                oBPCondition.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oBPCondition.CondVal = ConditionValue;
                oBP.SetConditions(oBPConditions);

                tCardCode = (EditText)oForm.Items.Item(ItemUID).Specific;
                tCardCode.ChooseFromListUID = CFLId;
                tCardCode.ChooseFromListAlias = sChoosefromlistAlias;
            }
            catch (Exception ex)
            {
            }

        }

        public static void CFLCreationEditTextUserDefinedForm(SAPbouiCOM.Application SBO_Application, string objecttype, string FormID, string CflID, string itemid, string Alias)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.Form oForm;
            SAPbouiCOM.EditText oEditText;
            oForm = SBO_Application.Forms.Item(FormID);
            SAPbouiCOM.ChooseFromListCollection oCFls = null;
            oCFls = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromList oCFl = null;
            SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
            oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
            oCFLCreationParams.MultiSelection = false;
            oCFLCreationParams.ObjectType = objecttype;            
            oCFLCreationParams.UniqueID = CflID;
            oCFl = oCFls.Add(oCFLCreationParams);
            oItem = oForm.Items.Item(itemid);
            oEditText = (SAPbouiCOM.EditText)oItem.Specific;
            oEditText.ChooseFromListUID = CflID;
            oEditText.ChooseFromListAlias = Alias;
        }

        public static void CFLConditionQueryForArrayList(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, ArrayList sQuery, string QueryField, string sCHUD, string sCondAlies, string Matrixname, string columnname)
        {
            try
            {
                SAPbouiCOM.Condition oCond;
                SAPbouiCOM.Conditions Conditions;
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                SAPbouiCOM.ChooseFromList oCFL = null;
                SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                oCFLs = oForm.ChooseFromLists;
                ArrayList sDocEntry = new ArrayList();
                sDocEntry = sQuery;


                oCFLs = oForm.ChooseFromLists;
                oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
                //if (systemMatrix == true)
                //{
                //    SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                //    oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                //    string sCFL_ID = null;
                //    sCFL_ID = oCFLEvento.ChooseFromListUID;
                //    oCFL = oForm.ChooseFromLists.Item(sCFL_ID);
                //}
                //else
                //{
                oCFL = oForm.ChooseFromLists.Item(sCHUD);
                //}
                Conditions = new SAPbouiCOM.Conditions();
                oCFL.SetConditions(Conditions);
                Conditions = oCFL.GetConditions();
                oCond = Conditions.Add();
                oCond.BracketOpenNum = 2;
                for (int i = 0; i <= sDocEntry.Count - 1; i++)
                {
                    if (i > 0)
                    {
                        oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                        oCond = Conditions.Add();
                        oCond.BracketOpenNum = 1;
                    }
                    oCond.Alias = sCondAlies;
                    oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    oCond.CondVal = sDocEntry[i].ToString();
                    if (i + 1 == sDocEntry.Count)
                    {
                        oCond.BracketCloseNum = 2;
                    }
                    else
                    {
                        oCond.BracketCloseNum = 1;
                    }

                }

                oCFL.SetConditions(Conditions);
            }
            catch (Exception ee)
            { }
        }

        public static void CFLCreationEditTextSystemForm(SAPbouiCOM.Application SBO_Application, string objecttype, SAPbouiCOM.Form oForm, string CflID, string itemid, string Alias)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.EditText oEditText;

            SAPbouiCOM.ChooseFromListCollection oCFls = null;
            oCFls = oForm.ChooseFromLists;
            SAPbouiCOM.ChooseFromList oCFl = null;
            SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
            oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
            oCFLCreationParams.MultiSelection = false;
            oCFLCreationParams.ObjectType = objecttype;
            oCFLCreationParams.UniqueID = CflID;
            oCFl = oCFls.Add(oCFLCreationParams);
            oItem = oForm.Items.Item(itemid);
            oEditText = (SAPbouiCOM.EditText)oItem.Specific;
            oEditText.ChooseFromListUID = CflID;
            oEditText.ChooseFromListAlias = Alias;
        }
        //public static void CFLConditionQuery(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string sQuery, string QueryField, string sCHUD, string sCondAlies, bool IsMatrixCondition, bool systemMatrix, string Matrixname, string columnname, bool removelist)

        //{
        //    try
        //    {
        //        SAPbouiCOM.Condition oCond;
        //        SAPbouiCOM.Conditions Conditions;
        //        SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
        //        SAPbouiCOM.ChooseFromList oCFL = null;
        //        SAPbouiCOM.ChooseFromListCollection oCFLs = null;
        //        oCFLs = oForm.ChooseFromLists;
        //        ArrayList sDocEntry = new ArrayList();
        //        ArrayList sDocNum;
        //        ArrayList MatrixItem;

        //        sDocEntry = new ArrayList();
        //        sDocNum = new ArrayList();
        //        MatrixItem = new ArrayList();

        //        SAPbobsCOM.Recordset oRec;
        //        oRec = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
        //        oRec.DoQuery(sQuery);
        //        oRec.MoveFirst();
        //        //int rowcount = oRec.RecordCount;

        //        try
        //        {
        //            if (oRec.EoF)
        //            {
        //                sDocEntry.Add("");
        //            }
        //            else
        //            {
        //                while (!oRec.EoF)
        //                {
        //                    string DocNum = oRec.Fields.Item(QueryField).Value.ToString();
        //                    if (DocNum != "")
        //                        sDocEntry.Add(DocNum);
        //                    oRec.MoveNext();
        //                }
        //            }
        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }

        //        #region Whether Matrix Condition or Edit Text Condition
        //        if (IsMatrixCondition == true)
        //        {
        //            SAPbouiCOM.Matrix oMatrix;
        //            oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item(Matrixname).Specific;

        //            for (int a = 1; a <= oMatrix.RowCount; a++)
        //            {
        //                MatrixItem.Add(((SAPbouiCOM.EditText)oMatrix.Columns.Item(columnname).Cells.Item(a).Specific).Value);
        //            }
        //            if (removelist == true)
        //            {
        //                for (int xx = 0; xx <= MatrixItem.Count - 1; xx++)
        //                {
        //                    string zz = MatrixItem[xx].ToString();
        //                    if (sDocEntry.Contains(zz))
        //                    {
        //                        sDocEntry.Remove(zz);
        //                    }
        //                }
        //            }
        //        }
        //        #endregion


        //        oCFLs = oForm.ChooseFromLists;
        //        oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
        //        if (systemMatrix == true)
        //        {
        //            SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
        //            oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
        //            string sCFL_ID = null;
        //            sCFL_ID = oCFLEvento.ChooseFromListUID;
        //            oCFL = oForm.ChooseFromLists.Item(sCFL_ID);
        //        }
        //        else
        //        {
        //            oCFL = oForm.ChooseFromLists.Item(sCHUD);
        //        }
        //        Conditions = new SAPbouiCOM.Conditions();
        //        oCFL.SetConditions(Conditions);
        //        Conditions = oCFL.GetConditions();
        //        oCond = Conditions.Add();
        //        oCond.BracketOpenNum = 2;
        //        for (int i = 0; i <= sDocEntry.Count - 1; i++)
        //        {
        //            if (i > 0)
        //            {
        //                oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
        //                oCond = Conditions.Add();
        //                oCond.BracketOpenNum = 1;
        //            }
        //            oCond.Alias = sCondAlies;
        //            oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
        //            oCond.CondVal = sDocEntry[i].ToString();
        //            if (i + 1 == sDocEntry.Count)
        //            {
        //                oCond.BracketCloseNum = 2;
        //            }
        //            else
        //            {
        //                oCond.BracketCloseNum = 1;
        //            }

        //        }

        //        oCFL.SetConditions(Conditions);
        //    }
        //    catch (Exception ee)
        //    { }
        //}
        public static void CFLSystemConditionQuery(ref SAPbouiCOM.Application SBO_Application, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string sQuery, string QueryField, string sCHUD, string sCondAlies, bool IsMatrixCondition, bool systemMatrix, string Matrixname, string columnname, bool removelist)
        {
                SAPbouiCOM.Condition oCond;
                SAPbouiCOM.Conditions Conditions = null;
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                SAPbouiCOM.ChooseFromList oCFL = null;
                SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                oCFLs = oForm.ChooseFromLists;
                ArrayList sDocEntry = new ArrayList();
                ArrayList sDocNum;
                ArrayList MatrixItem;

                sDocEntry = new ArrayList();
                sDocNum = new ArrayList();
                MatrixItem = new ArrayList();

                SAPbobsCOM.Recordset oRec;
                oRec = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRec.DoQuery(sQuery);
                oRec.MoveFirst();
                //int rowcount = oRec.RecordCount;

                try
                {
                    if (oRec.EoF)
                    {
                        sDocEntry.Add("");
                    }
                    else
                    {
                        while (!oRec.EoF)
                        {
                            string DocNum = oRec.Fields.Item(QueryField).Value.ToString();
                            if (DocNum != "")
                                sDocEntry.Add(DocNum);
                            oRec.MoveNext();
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                #region Whether Matrix Condition or Edit Text Condition
                if (IsMatrixCondition == true)
                {
                    SAPbouiCOM.Matrix oMatrix;
                    oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item(Matrixname).Specific;

                    for (int a = 1; a <= oMatrix.RowCount; a++)
                    {
                        if (a != pVal.Row)
                        {
                            MatrixItem.Add(((SAPbouiCOM.EditText)oMatrix.Columns.Item(columnname).Cells.Item(a).Specific).Value);
                        }
                    }
                    if (removelist == true)
                    {
                        for (int xx = 0; xx <= MatrixItem.Count - 1; xx++)
                        {
                            string zz = MatrixItem[xx].ToString();
                            if (sDocEntry.Contains(zz))
                            {
                                sDocEntry.Remove(zz);
                            }

                        }
                    }
                }
                #endregion


                oCFLs = oForm.ChooseFromLists;
                oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
                if (systemMatrix == true)
                {
                    SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                    oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                    string sCFL_ID = null;
                    sCFL_ID = oCFLEvento.ChooseFromListUID;
                    oCFL = oForm.ChooseFromLists.Item(sCFL_ID);
                }
                else
                {
                    oCFL = oForm.ChooseFromLists.Item(sCHUD);
                }
                Conditions = new SAPbouiCOM.Conditions();
                oCFL.SetConditions(Conditions);
                Conditions = oCFL.GetConditions();
                oCond = Conditions.Add();
                oCond.BracketOpenNum = 2;
                for (int i = 0; i <= sDocEntry.Count - 1; i++)
                {
                    if (i > 0)
                    {
                        oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                        oCond = Conditions.Add();
                        oCond.BracketOpenNum = 1;
                    }
                    oCond.Alias = sCondAlies;
                    oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    oCond.CondVal = sDocEntry[i].ToString();
                    if (i + 1 == sDocEntry.Count)
                    {
                        oCond.BracketCloseNum = 2;
                    }
                    else
                    {
                        oCond.BracketCloseNum = 1;
                    }

                }

                oCFL.SetConditions(Conditions);
        }
        
        public static void CFLConditionQuery(ref SAPbouiCOM.Application SBO_Application,ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string sQuery, string QueryField, string sCHUD, string sCondAlies, bool IsMatrixCondition, bool systemMatrix, string Matrixname, string columnname, bool removelist)
        {
            try
            {
                SAPbouiCOM.Condition oCond = null;
                SAPbouiCOM.Conditions Conditions = null;
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                SAPbouiCOM.ChooseFromList oCFL = null;
                SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                oCFLs = oForm.ChooseFromLists;
                ArrayList sDocEntry = new ArrayList();
                ArrayList sDocNum;
                ArrayList MatrixItem;

                sDocEntry = new ArrayList();
                sDocNum = new ArrayList();
                MatrixItem = new ArrayList();

                SAPbobsCOM.Recordset oRec;
                oRec = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oRec.DoQuery(sQuery);
                oRec.MoveFirst();
                //int rowcount = oRec.RecordCount;

                try
                {
                    if (oRec.EoF)
                    {
                        sDocEntry.Add("");
                    }
                    else
                    {
                        while (!oRec.EoF)
                        {
                            string DocNum = oRec.Fields.Item(QueryField).Value.ToString();
                            if (DocNum != "")
                                sDocEntry.Add(DocNum);
                            oRec.MoveNext();
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                #region Whether Matrix Condition or Edit Text Condition
                if (IsMatrixCondition == true)
                {
                    SAPbouiCOM.Matrix oMatrix;
                    oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item(Matrixname).Specific;

                    for (int a = 1; a <= oMatrix.RowCount; a++)
                    {
                        if (a != pVal.Row)
                        {
                            MatrixItem.Add(((SAPbouiCOM.EditText)oMatrix.Columns.Item(columnname).Cells.Item(a).Specific).Value);
                        }
                    }
                    if (removelist == true)
                    {
                        for (int xx = 0; xx <= MatrixItem.Count - 1; xx++)
                        {
                            string zz = MatrixItem[xx].ToString();
                            if (sDocEntry.Contains(zz))
                            {
                                sDocEntry.Remove(zz);
                            }

                        }
                    }
                }
                #endregion


                oCFLs = oForm.ChooseFromLists;
                oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
                if (systemMatrix == true)
                {
                    SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                    oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                    string sCFL_ID = null;
                    sCFL_ID = oCFLEvento.ChooseFromListUID;
                    oCFL = oForm.ChooseFromLists.Item(sCFL_ID);
                }
                else
                {
                    oCFL = oForm.ChooseFromLists.Item(sCHUD);
                }
                Conditions = new SAPbouiCOM.Conditions();
                oCFL.SetConditions(Conditions);
                Conditions = oCFL.GetConditions();
                oCond = Conditions.Add();
                oCond.BracketOpenNum = 2;
                for (int i = 0; i <= sDocEntry.Count - 1; i++)
                {
                    if (i > 0)
                    {
                        oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                        oCond = Conditions.Add();
                        oCond.BracketOpenNum = 1;
                    }
                    oCond.Alias = sCondAlies;
                    oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    oCond.CondVal = sDocEntry[i].ToString();
                    if (i + 1 == sDocEntry.Count)
                    {
                        oCond.BracketCloseNum = 2;
                    }
                    else
                    {
                        oCond.BracketCloseNum = 1;
                    }

                }

                oCFL.SetConditions(Conditions);
            }
            catch (Exception ee)
            { }
        }

        public static void ClearCFLConditionQuery(ref SAPbouiCOM.Application SBO_Application, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string sQuery, string QueryField, string sCHUD, string sCondAlies, bool IsMatrixCondition, bool systemMatrix, string Matrixname, string columnname, bool removelist)
        {
            try
            {
                SAPbouiCOM.Condition oCond = null;
                SAPbouiCOM.Conditions Conditions = null;
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                SAPbouiCOM.ChooseFromList oCFL = null;
                SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                oCFLs = oForm.ChooseFromLists;
                ArrayList sDocEntry = new ArrayList();
                ArrayList sDocNum;
                ArrayList MatrixItem;

                sDocEntry = new ArrayList();
                sDocNum = new ArrayList();
                MatrixItem = new ArrayList();

                //SAPbobsCOM.Recordset oRec;
                //oRec = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                //oRec.DoQuery(sQuery);
                //oRec.MoveFirst();
                ////int rowcount = oRec.RecordCount;

                //try
                //{
                //    if (oRec.EoF)
                //    {
                //        sDocEntry.Add("");
                //    }
                //    else
                //    {
                //        while (!oRec.EoF)
                //        {
                //            string DocNum = oRec.Fields.Item(QueryField).Value.ToString();
                //            if (DocNum != "")
                //                sDocEntry.Add(DocNum);
                //            oRec.MoveNext();
                //        }
                //    }
                //}
                //catch (Exception)
                //{

                //    throw;
                //}

                #region Whether Matrix Condition or Edit Text Condition
                if (IsMatrixCondition == true)
                {
                    SAPbouiCOM.Matrix oMatrix;
                    oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item(Matrixname).Specific;

                    for (int a = 1; a <= oMatrix.RowCount; a++)
                    {
                        if (a != pVal.Row)
                        {
                            MatrixItem.Add(((SAPbouiCOM.EditText)oMatrix.Columns.Item(columnname).Cells.Item(a).Specific).Value);
                        }
                    }
                    if (removelist == true)
                    {
                        for (int xx = 0; xx <= MatrixItem.Count - 1; xx++)
                        {
                            string zz = MatrixItem[xx].ToString();
                            if (sDocEntry.Contains(zz))
                            {
                                sDocEntry.Remove(zz);
                            }

                        }
                    }
                }
                #endregion


                oCFLs = oForm.ChooseFromLists;
                oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
                if (systemMatrix == true)
                {
                    SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
                    oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
                    string sCFL_ID = null;
                    sCFL_ID = oCFLEvento.ChooseFromListUID;
                    oCFL = oForm.ChooseFromLists.Item(sCFL_ID);
                }
                else
                {
                    oCFL = oForm.ChooseFromLists.Item(sCHUD);
                }
                Conditions = new SAPbouiCOM.Conditions();
                oCFL.SetConditions(Conditions);
                Conditions = oCFL.GetConditions();
                oCond = Conditions.Add();
                oCond.BracketOpenNum = 2;
                for (int i = 0; i <= sDocEntry.Count - 1; i++)
                {
                    if (i > 0)
                    {
                        oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                        oCond = Conditions.Add();
                        oCond.BracketOpenNum = 1;
                    }
                    oCond.Alias = sCondAlies;
                    oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    oCond.CondVal = sDocEntry[i].ToString();
                    if (i + 1 == sDocEntry.Count)
                    {
                        oCond.BracketCloseNum = 2;
                    }
                    else
                    {
                        oCond.BracketCloseNum = 1;
                    }

                }

                oCFL.SetConditions(null);
            }
            catch (Exception ee)
            { }
        }

        public static void CFLBeforeActionFalseEditTextPairFill(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string dataTableCode, string dataTableName, string EditTextCode, string EditTextName)
        {
            DataTable oDataTable;
            oDataTable = UtilitiesCls.DataTable(ref SBO_Application,ref oCompany, oForm, ref pVal);
            if (oDataTable != null)
            {
                string sCardCode = "", sCardName = "";

                sCardCode = System.Convert.ToString(oDataTable.GetValue(dataTableCode, 0));
                sCardName = System.Convert.ToString(oDataTable.GetValue(dataTableName, 0));

                try
                {
                ((SAPbouiCOM.EditText)oForm.Items.Item(EditTextName).Specific).Value = sCardName;
                ((SAPbouiCOM.EditText)oForm.Items.Item(EditTextCode).Specific).Value = sCardCode;
                }
                catch (Exception z)
                {   
                    ((SAPbouiCOM.EditText)oForm.Items.Item(EditTextCode).Specific).Value = sCardCode;
                }
            }
        }

        public static void EmployeeCFLBeforeActionFalseEditTextPairFill(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string dataTableCode, string dataTableName, string dataTableName2, string EditTextCode, string EditTextName)
        {
            DataTable oDataTable;
            oDataTable = UtilitiesCls.DataTable(ref SBO_Application,ref oCompany, oForm, ref pVal);
            if (oDataTable != null)
            {
                string sCode = "", sFirstName = "";//, sLastName = "";

                sCode = System.Convert.ToString(oDataTable.GetValue(dataTableCode, 0));
                sFirstName = System.Convert.ToString(oDataTable.GetValue(dataTableName, 0));
                //sLastName = System.Convert.ToString(oDataTable.GetValue(dataTableName2, 0));

                try
                {
                    ((SAPbouiCOM.EditText)oForm.Items.Item(EditTextCode).Specific).Value = sCode;
                    ((SAPbouiCOM.EditText)oForm.Items.Item(EditTextName).Specific).Value = sFirstName;// +" " + sLastName;

                }
                catch (Exception z)
                {
                    ((SAPbouiCOM.EditText)oForm.Items.Item(EditTextName).Specific).Value = sFirstName;// +" " + sLastName;

                }
            }
        }

        public static void CFLBeforeActionFalseEditTextSingleBoxFill(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string dataTableCode, string EditTextCode)
        {

            DataTable oDataTable = UtilitiesCls.DataTable(ref SBO_Application,ref oCompany, oForm, ref pVal);
            if (oDataTable != null)
            {
                string sCardCode = "";//, sPlace = "";
                sCardCode = System.Convert.ToString(oDataTable.GetValue(dataTableCode, 0));
                //sPlace = System.Convert.ToString(oDataTable.GetValue(dataTableCode, 0));
                try
                {
                    ((SAPbouiCOM.EditText)oForm.Items.Item(EditTextCode).Specific).Value = sCardCode;
                }
                catch (Exception z)
                {
                    ((SAPbouiCOM.EditText)oForm.Items.Item(EditTextCode).Specific).Value = sCardCode;
                }
            }
        }

        public static void MatrixCFLBeforeActionFalseEditTextSingleBoxFill(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string dataTableCode, string colId, int RowId, string MatId)
        {

            DataTable oDataTable = UtilitiesCls.DataTable(ref SBO_Application,ref oCompany, oForm, ref pVal);
            if (oDataTable != null)
            {
                string sCode = "";
                sCode = System.Convert.ToString(oDataTable.GetValue(dataTableCode, 0));
                SAPbouiCOM.Matrix oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item(MatId).Specific;
                try
                {
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId).Cells.Item(RowId).Specific).Value = sCode;
                }
                catch (Exception z)
                {
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId).Cells.Item(RowId).Specific).Value = sCode;
                }
            }
        }

        public static void CreateStaticItem(SAPbouiCOM.Form oForm, string sstaticID, int iLeft, int iTop, int iWidth, int iHeight, string sCaption, int PaneLevel, bool Enable)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.StaticText oStatic;
            oItem = oForm.Items.Add(sstaticID, SAPbouiCOM.BoFormItemTypes.it_STATIC);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Width = iWidth;
            oItem.Height = iHeight;
            oStatic = (SAPbouiCOM.StaticText)oItem.Specific;
            oStatic.Caption = sCaption;
            oItem.FromPane = PaneLevel;
            oItem.ToPane = PaneLevel;
            oItem.Enabled = Enable;
        }

        public static void CreateEditItem(SAPbouiCOM.Form oForm, string sEditID, int iLeft, int iTop, int iWidth, int iHeight, string sTable, string sFieldName, bool Enable, int PaneLevel, string LinkTo)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.EditText oEdit;
            oItem = oForm.Items.Add(sEditID, SAPbouiCOM.BoFormItemTypes.it_EDIT);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Width = iWidth;
            oItem.Height = iHeight;
            oItem.Enabled = Enable;
            oEdit = (SAPbouiCOM.EditText)oItem.Specific;
            oEdit.DataBind.SetBound(true, sTable, sFieldName);
           oItem.FromPane = PaneLevel;
           oItem.ToPane = PaneLevel;
            oItem.LinkTo = LinkTo;
        }

        public static void CreateExtEditItem(SAPbouiCOM.Form oForm, string sEditID, int iLeft, int iTop, int iWidth, int iHeight, string sTable, string sFieldName, bool Enable, int PaneLevel, string LinkTo)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.EditText oEdit;
            oItem = oForm.Items.Add(sEditID, SAPbouiCOM.BoFormItemTypes.it_EXTEDIT);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Width = iWidth;
            oItem.Height = iHeight;
            oItem.Enabled = Enable;
            oEdit = (SAPbouiCOM.EditText)oItem.Specific;
            oEdit.DataBind.SetBound(true, sTable, sFieldName);           
            oItem.LinkTo = LinkTo;
        }
            
        public static void CreateButtonItem(SAPbouiCOM.Form oForm, string sButtonID, int iLeft, int iTop, int iWidth, int iHeight, string sCaption, bool VisibleF, int PaneLevel, string LinkTo)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.Button oButton;
            oItem = oForm.Items.Add(sButtonID, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Height = iHeight;
            oItem.Width = iWidth;
            oItem.Visible = VisibleF;
            oButton = (SAPbouiCOM.Button)oItem.Specific;
            oButton.Caption = sCaption;
           // oItem.FromPane = PaneLevel;
           // oItem.ToPane = PaneLevel;
            oItem.LinkTo = LinkTo;
        }

        public static void CreateCheckBox(SAPbouiCOM.Form oForm, string sCheckID, int iLeft, int iTop, int iWidth, int iHeight, string sCaption, bool VisibleF, int PaneLevel, string LinkTo, string sTable, string sFieldName)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.CheckBox  oCheckbox;
            oItem = oForm.Items.Add(sCheckID, SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Height = iHeight;
            oItem.Width = iWidth;
            oItem.Visible = VisibleF;
            oCheckbox = (SAPbouiCOM.CheckBox )oItem.Specific;
            oCheckbox.DataBind.SetBound(true, sTable, sFieldName);
            oCheckbox.Caption = sCaption;
           // oItem.FromPane = PaneLevel;
            //oItem.ToPane = PaneLevel;
            oItem.LinkTo = LinkTo;
        }

        public static void CreateRadioButton(SAPbouiCOM.Form oForm, string sCheckID, int iLeft, int iTop, int iWidth, int iHeight, string sCaption, bool VisibleF, int PaneLevel, string LinkTo, string sTable, string sFieldName)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.OptionBtn oCheckbox;
            oItem = oForm.Items.Add(sCheckID, SAPbouiCOM.BoFormItemTypes.it_OPTION_BUTTON);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Height = iHeight;
            oItem.Width = iWidth;
            oItem.Visible = VisibleF;
            oCheckbox = (SAPbouiCOM.OptionBtn)oItem.Specific;
            oCheckbox.DataBind.SetBound(true, sTable, sFieldName);
            oCheckbox.Caption = sCaption;
            //oItem.FromPane = PaneLevel;
           // oItem.ToPane = PaneLevel;
            oItem.LinkTo = LinkTo;
        }

        public static void createComboBoxItemPaneLevel(SAPbouiCOM.Form oForm, string sComboID, int iLeft, int iTop, int iWidth, int iHeight, string sTable, string sFieldName, int PaneLevel, string LinkTo)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.ComboBox oCombo;
            oItem = oForm.Items.Add(sComboID, SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX);

            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Width = iWidth;
            oItem.Height = iHeight;
            oCombo = (SAPbouiCOM.ComboBox)oItem.Specific;
            oCombo.DataBind.SetBound(true, sTable, sFieldName);
            oItem.DisplayDesc = true;
           // oItem.FromPane = PaneLevel;
           // oItem.ToPane = PaneLevel;
            oItem.LinkTo = LinkTo;
        }

        public static void createButtonComboBoxItemPaneLevel(SAPbouiCOM.Form oForm, string sComboID, int iLeft, int iTop, int iWidth, int iHeight, string sTable, string sFieldName, int PaneLevel, string LinkTo)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.ButtonCombo oCombo;
            oItem = oForm.Items.Add(sComboID, SAPbouiCOM.BoFormItemTypes.it_BUTTON_COMBO);

            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Width = iWidth;
            oItem.Height = iHeight;
            oCombo = (SAPbouiCOM.ButtonCombo)oItem.Specific;
            oCombo.DataBind.SetBound(true, sTable, sFieldName);
            oItem.DisplayDesc = true;            
            oItem.LinkTo = LinkTo;
        }

        public static void ComboBoxValidValuesAdd(SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, string sComboID, string Query, string oRecCode, string oRecName)
        {
            SAPbobsCOM.Recordset oRecordSet;
            SAPbouiCOM.ComboBox oCombo;
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            oCombo = (SAPbouiCOM.ComboBox)(oForm.Items.Item(sComboID).Specific);

            oRecordSet.DoQuery(Query);
            //string Code1 = "Define New";
            //string Name1 = "Define New";
            
            if (oRecordSet.RecordCount > 0)
            {

                while (!oRecordSet.EoF)
                {
                    string Code = oRecordSet.Fields.Item(oRecCode).Value.ToString();
                    string Name = oRecordSet.Fields.Item(oRecName).Value.ToString();
                    try
                    {
                        oCombo.ValidValues.Add(Code, Name);
                    }
                    catch (Exception z)
                    {

                    }
                    oRecordSet.MoveNext();
                }
              

            }
            //oCombo.ValidValues.Add(Code1, Name1);
        }

        public static void ComboBoxValidValuesAdd1(SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, string sComboID, string Query, string oRecCode, string oRecName, int variable)
        {
            if (variable == 1)
            {
                SAPbobsCOM.Recordset oRecordSet;
                SAPbouiCOM.ComboBox oCombo;
                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                oCombo = (SAPbouiCOM.ComboBox)(oForm.Items.Item(sComboID).Specific);

                oRecordSet.DoQuery(Query);
                string Code1 = "All";
                string Name1 = "All";

                oCombo.ValidValues.Add(Code1, Name1);

                if (oRecordSet.RecordCount > 0)
                {
                    while (!oRecordSet.EoF)
                    {
                        string Code = oRecordSet.Fields.Item(oRecCode).Value.ToString();
                        string Name = oRecordSet.Fields.Item(oRecName).Value.ToString();
                        try
                        {
                            oCombo.ValidValues.Add(Code, Name);
                        }
                        catch (Exception z)
                        {
                        }
                        oRecordSet.MoveNext();
                    }


                }
            }
            else if (variable == 2)
            {
                SAPbobsCOM.Recordset oRecordSet;
                SAPbouiCOM.ComboBox oCombo;
                oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                oCombo = (SAPbouiCOM.ComboBox)(oForm.Items.Item(sComboID).Specific);

                oRecordSet.DoQuery(Query);

                if (oRecordSet.RecordCount > 0)
                {

                    while (!oRecordSet.EoF)
                    {
                        string Code = oRecordSet.Fields.Item(oRecCode).Value.ToString();
                        string Name = oRecordSet.Fields.Item(oRecName).Value.ToString();
                        try
                        {
                            oCombo.ValidValues.Add(Code, Name);
                        }
                        catch (Exception z)
                        {
                        }
                        oRecordSet.MoveNext();
                    }


                }
            }

        }

        public static void ComboBoxValidValuesAddForPurchase(SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, string sComboID, string Query, string oRecCode, string oRecName)
        {
            SAPbobsCOM.Recordset oRecordSet;
            SAPbouiCOM.ComboBox oCombo;
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            oCombo = (SAPbouiCOM.ComboBox)(oForm.Items.Item(sComboID).Specific);

            oRecordSet.DoQuery(Query);
            if (oRecordSet.RecordCount > 0)
            {

                while (!oRecordSet.EoF)
                {
                    string Code = oRecordSet.Fields.Item(oRecCode).Value.ToString();
                    string Name = oRecordSet.Fields.Item(oRecName).Value.ToString();
                    try
                    {
                        oCombo.ValidValues.Add(Code, Name);
                    }
                    catch (Exception z)
                    {
                    }
                    oRecordSet.MoveNext();
                }


            }

        }

        public static SAPbouiCOM.DataTable DataTable(ref SAPbouiCOM.Application SBO_Application,ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal)
        {
            SAPbouiCOM.IChooseFromListEvent oCFLEvento = null;
            oCFLEvento = ((SAPbouiCOM.IChooseFromListEvent)(pVal));
            string sCFL_ID = null;
            sCFL_ID = oCFLEvento.ChooseFromListUID;
            //oForm = SBO_Application.Forms.Item(FormUID);
            SAPbouiCOM.ChooseFromList oCFL = null;
            oCFL = oForm.ChooseFromLists.Item(sCFL_ID);
            SAPbouiCOM.DataTable oDataTable = null;
            oDataTable = oCFLEvento.SelectedObjects;
            return oDataTable;
        }

        public static void CreateFolderItem(SAPbouiCOM.Form oForm, string sEditID, int iLeft, int iTop, int iWidth, int iHeight, string sTable, string sFieldName, bool Enable, int PaneLevel, string LinkTo)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.Folder oFolderItem;
            oItem = oForm.Items.Add(sEditID, SAPbouiCOM.BoFormItemTypes.it_FOLDER);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Width = iWidth;
            oItem.Height = iHeight;
            oItem.Enabled = Enable;
            oFolderItem = (SAPbouiCOM.Folder)oItem.Specific;
            oFolderItem.Caption = "Service Items";
            
            oFolderItem.GroupWith("2013");
            oForm.DataSources.UserDataSources.Add("FolderDs", SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 1);
            oFolderItem.DataBind.SetBound(true, "", "FolderDs");
            //oItem.FromPane = 10;
            //oItem.ToPane = 10;
            
        }

        public static void CreateEditItemPaneLevel(SAPbouiCOM.Form oForm, string sEditID, int iLeft, int iTop, int iWidth, int iHeight, string sTable, string sFieldName, bool Enable, int PaneLevel, string LinkTo)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.EditText oEdit;
            oItem = oForm.Items.Add(sEditID, SAPbouiCOM.BoFormItemTypes.it_EDIT);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Width = iWidth;
            oItem.Height = iHeight;
            oItem.Enabled = Enable;
            oEdit = (SAPbouiCOM.EditText)oItem.Specific;
            oEdit.DataBind.SetBound(true, sTable, sFieldName);
            oItem.FromPane = PaneLevel;
            oItem.ToPane = PaneLevel;
            oItem.LinkTo = LinkTo;


        }
                
        public static void CreateStaticItemPaneLevel(SAPbouiCOM.Form oForm, string sstaticID, int iLeft, int iTop, int iWidth, int iHeight, string sCaption, int PaneLevel, string LinkTo)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.StaticText oStatic;
            oItem = oForm.Items.Add(sstaticID, SAPbouiCOM.BoFormItemTypes.it_STATIC);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Width = iWidth;
            oItem.Height = iHeight;
            oStatic = (SAPbouiCOM.StaticText)oItem.Specific;
            oStatic.Caption = sCaption;
            oItem.FromPane = PaneLevel;
            oItem.ToPane = PaneLevel;
            oItem.LinkTo = LinkTo;

        }

        public static string MethodForDate(string str)
        {
            string datepart = str.Substring(3, 2);
            string monthpart = str.Substring(0, 2);
            string yearpart = str.Substring(7, 4);


            string fulldate = datepart + "/" + monthpart + "/" + yearpart;
            return fulldate;
        }

        public static string MethodForDateConversion(string str)
        {
            if (str != "")
            {
                string str1 = str.Insert(4, "/");
                string str2 = str1.Insert(7, "/");

                string datepart = str2.Substring(8, 2);
                string monthpart = str2.Substring(5, 2);
                string yearpart = str2.Substring(0, 4);

                string fulldate = monthpart + "/" + datepart + "/" + yearpart;
                return fulldate;
            }
            else
            {
                string strnew = "";
                return strnew;
            }
        }

        public static DateTime SBODATE(string str)
        {
            DateTime fulldate = new DateTime();
            if (str != "")
            {
                string str1 = str.Insert(4, "/");
                string str2 = str1.Insert(7, "/");

                string datepart = str2.Substring(8, 2);
                string monthpart = str2.Substring(5, 2);
                string yearpart = str2.Substring(0, 4);

                fulldate = Convert.ToDateTime(yearpart + "/" + monthpart + "/" + datepart);               
            }
            return fulldate;
        }

        public static void ComboBoxValidValuesClear(SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, string sComboID)
        {
            try
            {
                SAPbouiCOM.ComboBox ocombo1 = (SAPbouiCOM.ComboBox)oForm.Items.Item(sComboID).Specific;
                int intComboCount = ocombo1.ValidValues.Count;


                if (intComboCount > 0)
                {
                    for (int i = 0; i <intComboCount; i++)
                    {
                        ocombo1.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
                        intComboCount = ocombo1.ValidValues.Count;
                        i = -1;
                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public static void CreateLinkItem(SAPbouiCOM.Form oForm, string sEditID, int iLeft, int iTop, int iWidth, int iHeight, string sTable, string sFieldName, bool Enable, int PaneLevel, string LinkTo, string strLinkedObject, string Type)
        {
            SAPbouiCOM.Item oItem;
            SAPbouiCOM.LinkedButton oEdit;
            oItem = oForm.Items.Add(sEditID, SAPbouiCOM.BoFormItemTypes.it_LINKED_BUTTON);
            oItem.Left = iLeft;
            oItem.Top = iTop;
            oItem.Width = iWidth;
            oItem.Height = iHeight;
            oItem.Enabled = Enable;
            oEdit = (SAPbouiCOM.LinkedButton)oItem.Specific;            
            //oEdit.LinkedObject = SAPbouiCOM.BoLinkedObject.lf_UserDefinedObject;
            oEdit.LinkedObjectType = Type;
            oItem.LinkTo = LinkTo;            
        }

        public static void ComboBoxValidValuesForMatrixColumn(SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, string Query, string oRecCode, string oRecName, string ItemUID, string ColUID, int RowId)
        {
            SAPbouiCOM.Matrix oMatrix1;
            oMatrix1 = (SAPbouiCOM.Matrix)oForm.Items.Item(ItemUID).Specific;

            SAPbobsCOM.Recordset oRecordSet;
            oRecordSet = (SAPbobsCOM.Recordset)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbouiCOM.ComboBox oCombo;
            oCombo = (SAPbouiCOM.ComboBox)oMatrix1.Columns.Item(ColUID).Cells.Item(RowId).Specific;

            oRecordSet.DoQuery(Query);
            //string Code1 = "Define New";
            //string Name1 = "Define New";

            if (oRecordSet.RecordCount > 0)
            {
                oRecordSet.MoveFirst();
                while (!oRecordSet.EoF)
                {
                    string Code = oRecordSet.Fields.Item(oRecCode).Value.ToString();
                    string Name = oRecordSet.Fields.Item(oRecName).Value.ToString();
                    try
                    {
                        oCombo.ValidValues.Add(Code, Name);

                    }
                    catch (Exception z)
                    {
                    }
                    oRecordSet.MoveNext();
                }
                //oCombo.ValidValues.Add(Code1, Name1);


            }

        }

        public static void ComboBoxValidValuesClearForMatrixColumn(SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, string ItemUID, string ColId, int RowId)
        {

            SAPbouiCOM.Matrix oMatrix1;
            oMatrix1 = (SAPbouiCOM.Matrix)oForm.Items.Item(ItemUID).Specific;

            try
            {
                SAPbouiCOM.ComboBox ocombo1 = (SAPbouiCOM.ComboBox)oMatrix1.Columns.Item(ColId).Cells.Item(RowId).Specific;
                int intComboCount = ocombo1.ValidValues.Count;


                if (intComboCount > 0)
                {
                    for (int i = 0; i <= intComboCount - 1; i++)
                    {
                        ocombo1.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
                      
                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }


        }

        public static void MatrixCFLBeforeActionFalseEditTextDoubleBoxFill(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string dataTableCode,string dataTableCode2, string colId1, string colId2, int RowId, string MatId)
        {

            DataTable oDataTable = UtilitiesCls.DataTable(ref SBO_Application, ref oCompany, oForm, ref pVal);
            if (oDataTable != null)
            {
                string sCode = "";
                string sDescription = "";
                sCode = System.Convert.ToString(oDataTable.GetValue(dataTableCode, 0));
                sDescription = System.Convert.ToString(oDataTable.GetValue(dataTableCode2, 0));

                SAPbouiCOM.Matrix oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item(MatId).Specific;

                try
                {
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId1).Cells.Item(RowId).Specific).Value = sCode;
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId2).Cells.Item(RowId).Specific).Value = sDescription;
                }
                catch (Exception z)
                {
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId1).Cells.Item(RowId).Specific).Value = sCode;
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId2).Cells.Item(RowId).Specific).Value = sDescription;
                }
            }
        }

        public static string GetBPDetails(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, string Code)
        {
            string BpCode = "";
            try
            {
                SAPbobsCOM.Recordset oRec = ExecuteRecordset("select 'Cardcode' from OCRD where 'CardName' ='" + Code + "'", null, SBO_Application, oCompany);

                BpCode = oRec.Fields.Item("Cardcode").Value.ToString();
            }
            catch (Exception ex)
            {

            }
            return BpCode;
        }

        public static void MatrixCFLBeforeActionFalseEditTextThreeBoxFill(SAPbouiCOM.Application SBO_Application, SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm, ref SAPbouiCOM.ItemEvent pVal, string dataTableCode, string dataTableCode2, string dataTableCode3, string colId1, string colId2, string colId3, int RowId, string MatId)
        {

            DataTable oDataTable = UtilitiesCls.DataTable(ref SBO_Application,ref oCompany, oForm, ref pVal);
            if (oDataTable != null)
            {
                string sCode = "";
                string sDescription = "";
                string sName = "";
                sCode = System.Convert.ToString(oDataTable.GetValue(dataTableCode, 0));
                sDescription = System.Convert.ToString(oDataTable.GetValue(dataTableCode2, 0));
                sName = System.Convert.ToString(oDataTable.GetValue(dataTableCode3, 0));
                SAPbouiCOM.Matrix oMatrix = (SAPbouiCOM.Matrix)oForm.Items.Item(MatId).Specific;

                try
                {
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId1).Cells.Item(RowId).Specific).Value = sCode;
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId2).Cells.Item(RowId).Specific).Value = sDescription;
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId3).Cells.Item(RowId).Specific).Value = sName;
                }
                catch (Exception z)
                {
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId1).Cells.Item(RowId).Specific).Value = sCode;
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId2).Cells.Item(RowId).Specific).Value = sDescription;
                    ((SAPbouiCOM.EditText)oMatrix.Columns.Item(colId3).Cells.Item(RowId).Specific).Value = sName;
                }
            }
        }

        public static void CFLConditionQueryOnStockItem(ref SAPbouiCOM.Application SBO_Application, ref SAPbobsCOM.Company oCompany, SAPbouiCOM.Form oForm,
            ref SAPbouiCOM.ItemEvent pVal, Hashtable oConditionList, string sCHUD)
        {
            try
            {
                SAPbouiCOM.Condition oCond;
                SAPbouiCOM.Conditions Conditions = null;
                SAPbouiCOM.ChooseFromListCreationParams oCFLCreationParams = null;
                SAPbouiCOM.ChooseFromList oCFL = null;
                SAPbouiCOM.ChooseFromListCollection oCFLs = null;
                oCFLs = oForm.ChooseFromLists;

                oCFLs = oForm.ChooseFromLists;
                oCFLCreationParams = ((SAPbouiCOM.ChooseFromListCreationParams)(SBO_Application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
                oCFL = oForm.ChooseFromLists.Item(sCHUD);

                Conditions = new SAPbouiCOM.Conditions();
                oCFL.SetConditions(Conditions);
                Conditions = oCFL.GetConditions();
                oCond = Conditions.Add();
                oCond.BracketOpenNum = 2;

                int intConditionCnt = oConditionList.Count;
                int intCount = 0;

                foreach (DictionaryEntry item in oConditionList)
                {
                    if (intCount > 0)
                    {
                        oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                        oCond = Conditions.Add();
                        oCond.BracketOpenNum = 1;
                    }

                    oCond.Alias = item.Key.ToString();
                    oCond.Operation = SAPbouiCOM.BoConditionOperation.co_GRATER_THAN;
                    oCond.CondVal = item.Value.ToString();
                    intCount++;
                    oCond.BracketCloseNum = 1;

                    oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                    oCond = Conditions.Add();
                    oCond.BracketOpenNum = 1;
                    oCond.Alias = "ItemType";
                    oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    oCond.CondVal = "L";
                }

                oCond.BracketCloseNum = 2;
                oCFL.SetConditions(Conditions);
            }
            catch (Exception ee)
            { }
        }
    }
}
