using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM;
using SAPbobsCOM;



namespace ACHR.Screen
{
    class frm_DRSC : HRMSBaseForm
    {

        SAPbouiCOM.DataTable dtHead, dtDetail;
        SAPbouiCOM.Matrix mtDet;
        SAPbouiCOM.EditText txNum;

        #region /////Events

        public override void etAfterClick(ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterClick(ref pVal, ref BubbleEvent);

            if (pVal.ItemUID == "cmdUpdate")
            {

                updateInvoice();
            }
        }
        public override void etAfterValidate(ref ItemEvent pVal, ref bool BubbleEvent)
        {
            base.etAfterValidate(ref pVal, ref BubbleEvent);
            if (pVal.ItemUID == "txNum")
            {
                loadInvoice();
            }
        }
        #endregion


        #region ///Initiallization


        private void InitiallizeForm()
        {
            //  dtHead = oForm.DataSources.DataTables.Item("dtHead");
            // dtHead.Rows.Add(1);

            oForm.Freeze(true);

            dtDetail = oForm.DataSources.DataTables.Item("dtDetail");
            dtHead = oForm.DataSources.DataTables.Item("dtHead");
            dtDetail.Rows.Add(1);
            mtDet = oForm.Items.Item("mtDet").Specific;

            oForm.DataSources.UserDataSources.Add("txNum", SAPbouiCOM.BoDataType.dt_LONG_NUMBER, 12); // Days of Month
            txNum = oForm.Items.Item("txNum").Specific;
            txNum.DataBind.SetBound(true, "", "txNum");



            txNum = oForm.Items.Item("txNum").Specific;

            mtDet.LoadFromDataSource();
            
            oForm.Freeze(false);


        }
        public override void CreateForm(SAPbouiCOM.Application SboApp, string strXml, SAPbobsCOM.Company cmp, string frmId)
        {
            try
            {
                base.CreateForm(SboApp, strXml, cmp, frmId);
                InitiallizeForm();
            }
            catch { }
           
        }
      
        #endregion

        #region //Common Methods
        private bool issuperUser(string username)
        {
            bool result = false;

            string sel = " Select SUPERUSER from ousr where U_NAME='" + username + "'  ";
            System.Data.DataTable dtRows = Program.objHrmsUI.getDataTable(sel, "Getting invoice detail");
           int i = 0;
           if (dtRows != null && dtRows.Rows.Count > 0)
           {
               if (dtRows.Rows[0]["SUPERUSER"].ToString() == "Y")
               {
                   result = true;
               }
           }


            return result;
        }
        private void loadInvoice()
        {
            if (txNum.Value.ToString() == "") return;
            dtDetail.Rows.Clear();
            mtDet.LoadFromDataSource();
            oForm.Items.Item("txCode").Specific.Value = "";
            oForm.Items.Item("txCName").Specific.Value = "";
            string userCode = oCompany.UserName;
            
            string strQuery = "SELECT oinv.cardcode,oinv.cardname, inv1.docentry, dbo.INV1.LineNum, dbo.INV1.ItemCode, dbo.INV1.Dscription, dbo.INV1.ShipDate, dbo.INV1.U_DelShift, dbo.INV1.U_DelTypCd, dbo.INV1.U_PickDate, dbo.INV1.U_PickShift, dbo.INV1.U_DeliveryWH,  ";
            strQuery += " dbo.INV1.U_PickingWh , isnull(dbo.OUSR.user_code,'') as Owner , isnull(dbo.OUSR.SUPERUSER,'N') as SUPERUSER    FROM   dbo.INV1 INNER JOIN  dbo.OINV ON dbo.INV1.DocEntry = dbo.OINV.DocEntry  ";
            strQuery += "  INNER JOIN ohem on ohem.empid = oinv.ownercode inner join  dbo.OUSR ON ohem.userid = dbo.OUSR.USERID inner join owhs on inv1.whscode = owhs.whscode ";
            strQuery += " where oinv.docnum='" + txNum.Value.ToString() + "'  and '" + userCode + "' in ('manager', isnull(dbo.OUSR.User_Code,'') , owhs.u_WhsSprv ) ";


            strQuery = @"SELECT        dbo.OINV.CardCode, dbo.OINV.CardName, dbo.INV1.DocEntry, dbo.INV1.LineNum, dbo.INV1.ItemCode, dbo.INV1.Dscription, dbo.INV1.ShipDate, dbo.INV1.U_DelShift, dbo.INV1.U_DelTypCd, dbo.INV1.U_PickDate, 
                         dbo.INV1.U_PickShift, dbo.INV1.U_DeliveryWH, dbo.INV1.U_PickingWh , 'manager' as Owner
FROM            dbo.INV1 INNER JOIN
                         dbo.OINV ON dbo.INV1.DocEntry = dbo.OINV.DocEntry
WHERE        (dbo.OINV.DocNum = '" + txNum.Value.ToString() + @"') ";

  //    oApplication.SetStatusBarMessage(strQuery);
            System.Data.DataTable dtRows = Program.objHrmsUI.getDataTable(strQuery,"Getting invoice detail");
            




            int i = 0;
            if (dtRows != null && dtRows.Rows.Count > 0)
            {
                string userName = dtRows.Rows[0]["Owner"].ToString();
                bool isnotSuper = ! issuperUser(oCompany.UserName);
                //if ((isnotSuper) && (oCompany.UserName != userName))
                //{
                //    oApplication.MessageBox("You are not authorize to update this document");
                //   // txNum.Value = "";
                //    return;
                //}
                oForm.Items.Item("txCode").Specific.Value = dtRows.Rows[0]["cardcode"].ToString();
                oForm.Items.Item("txCName").Specific.Value = dtRows.Rows[0]["cardname"].ToString();
               
                foreach (DataRow dr in dtRows.Rows)
                {
                    dtDetail.Rows.Add(1);
                     dtDetail.SetValue("DocEntry", i, dr["DocEntry"].ToString());
                    
                    dtDetail.SetValue("LineNum", i, dr["LineNum"].ToString());
                    dtDetail.SetValue("ItemCode", i, dr["ItemCode"].ToString());
                    dtDetail.SetValue("ItemName", i, dr["Dscription"].ToString());
                    dtDetail.SetValue("DelDate", i, Convert.ToDateTime(dr["shipdate"]));
                    dtDetail.SetValue("DelShift", i, dr["U_DelShift"].ToString());
                    dtDetail.SetValue("nDelDate", i, Convert.ToDateTime(dr["shipdate"]));
                    dtDetail.SetValue("nDelShift", i, dr["U_DelShift"].ToString());
                   
                    if (dr["U_PickDate"] != null && Convert.ToDateTime( dr["U_PickDate"] ) >  Convert.ToDateTime("1/1/2000")  )
                    {
                        dtDetail.SetValue("PickDate", i,  Convert.ToDateTime(dr["u_PickDate"]));
                        dtDetail.SetValue("nPickDate", i, Convert.ToDateTime(dr["u_PickDate"]));
                    }
                    dtDetail.SetValue("PickShift", i, dr["U_PickShift"].ToString());
                    dtDetail.SetValue("nPickShift", i, dr["U_PickShift"].ToString());

                    dtDetail.SetValue("TypeCode", i, dr["U_DelTypCd"].ToString());
                    dtDetail.SetValue("DelWhs", i, dr["U_DeliveryWH"].ToString());
                    dtDetail.SetValue("PickWhs", i, dr["U_PickingWh"].ToString());
                  
                 
                    i++;
                }
                mtDet.LoadFromDataSource();
            }
            else
            {
                oApplication.MessageBox("Invoice not found");
            }
           
        }
       

        private void updateInvoice()
        {

            int k = dtDetail.Rows.Count;

            Hashtable oldDelWhs = new Hashtable(), oldTypeCode = new Hashtable(), oldPickWhs = new Hashtable();
          
            for (int i = 0; i < k; i++)
            {
                oldDelWhs.Add(i, Convert.ToString(dtDetail.GetValue("DelWhs", i)));
                oldTypeCode.Add(i, Convert.ToString(dtDetail.GetValue("TypeCode", i)));
                oldPickWhs.Add(i, Convert.ToString(dtDetail.GetValue("PickWhs", i)));

            }
            mtDet.FlushToDataSource();

            //if (!validateinput())
            //{
            //    return;
            //}


            string DIERror = "";


            string updatShipDate = "";

            for (int i = 0; i < k; i++)
            {
                DateTime ndelDate  = Convert.ToDateTime (dtDetail.GetValue("nDelDate", i));
                DateTime deldate = Convert.ToDateTime(dtDetail.GetValue("DelDate", i));
                if (ndelDate != deldate)
                {
                    if (ndelDate < DateTime.Now.Date)
                    {
                        oApplication.SetStatusBarMessage("Old Delivery Date for Line : " + (i+1).ToString());
                        return;
                    }
                }
            }


            updatShipDate = "";
            for (int i = 0; i < k; i++)
            {
               // updatShipDate += "Update inv1  set shipdate='" + Convert.ToString(dtDetail.GetValue("nDelDate", i)) + "', FreeTxt = 'what reading'   where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ;";

                string PickDate = Convert.ToString(dtDetail.GetValue("nPickDate", i));
                string PickShift = dtDetail.GetValue("nPickShift", i);
                if (PickDate != "" && PickShift != "")
                {
                    updatShipDate += "Update inv1 set U_DelTypCd = '" + Convert.ToString(dtDetail.GetValue("TypeCode", i)) + "' ,  shipdate='" + Convert.ToString(dtDetail.GetValue("nDelDate", i)) + "' , U_DelShift='" + Convert.ToString(dtDetail.GetValue("nDelShift", i)) + "',";
                    updatShipDate += "  U_DeliveryWH ='" + Convert.ToString(dtDetail.GetValue("DelWhs", i)) + "',U_PickingWh='" + Convert.ToString(dtDetail.GetValue("PickWhs", i)) + "' , U_PickShift ='" + Convert.ToString(dtDetail.GetValue("nPickShift", i)) + "' , u_PickDate = '" + Convert.ToString(dtDetail.GetValue("nPickDate", i)) + "'  where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ;";

                }
                else
                {
                    updatShipDate += "Update inv1 set  U_DelTypCd = '" + Convert.ToString(dtDetail.GetValue("TypeCode", i)) + "' , shipdate='" + Convert.ToString(dtDetail.GetValue("nDelDate", i)) + "' , U_DeliveryWH ='" + Convert.ToString(dtDetail.GetValue("DelWhs", i)) + "',U_PickingWh='" + Convert.ToString(dtDetail.GetValue("PickWhs", i)) + "' ,  U_DelShift='" + Convert.ToString(dtDetail.GetValue("nDelShift", i)) + "' where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ;";
                }
            
            }


            Program.objHrmsUI.ExecQuery(updatShipDate, "Temp Update Ship Date");


            try
            {
               
                SAPbobsCOM.Documents ARInv = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
                ARInv.GetByKey(Convert.ToInt32(Convert.ToString(dtDetail.GetValue("DocEntry", 0))));

                for (int i = 0; i < k; i++)
                {
                    int lineNum = Convert.ToInt32(Convert.ToString(dtDetail.GetValue("LineNum", i)));
                    ARInv.Lines.SetCurrentLine(i);
                    try
                    {
                        // ARInv.Lines.ShipDate = Convert.ToDateTime(dtDetail.GetValue("nDelDate", i));
                        ARInv.Lines.UserFields.Fields.Item("U_DelTypCd").Value = Convert.ToString(dtDetail.GetValue("TypeCode", i));
                        ARInv.Lines.UserFields.Fields.Item("U_DelShift").Value = Convert.ToString(dtDetail.GetValue("nDelShift", i));
                        ARInv.Lines.UserFields.Fields.Item("U_DeliveryWH").Value = Convert.ToString(dtDetail.GetValue("DelWhs", i));
                        ARInv.Lines.UserFields.Fields.Item("U_PickingWh").Value = Convert.ToString(dtDetail.GetValue("PickWhs", i));
                        ARInv.Lines.UserFields.Fields.Item("U_PickShift").Value = Convert.ToString(dtDetail.GetValue("nPickShift", i));
                        if (Convert.ToString(dtDetail.GetValue("nPickDate", i)) != "")
                        {

                            ARInv.Lines.UserFields.Fields.Item("U_PickDate").Value = Convert.ToDateTime(dtDetail.GetValue("nPickDate", i));
                        }
                        else
                        {
                            ARInv.Lines.UserFields.Fields.Item("U_PickDate").Value = "";
                        }
                     //   ARInv.Lines.FreeText = "What Reaching";
                   //     ARInv.Lines.SetCurrentLine(lineNum);

                    }
                    catch (Exception ex)
                    {
                        oApplication.SetStatusBarMessage("Assigning Values : " + ex.Message);
                        DIERror += "Assignment Error : " + ex.Message;
                    }
                }
                int result = 0;
                result = ARInv.Update();
                if (result != 0)
                {
                    int errorCode = 0;
                    string errmsg = "";
                    oCompany.GetLastError(out errorCode, out errmsg);
                    oApplication.SetStatusBarMessage(errmsg);
                    DIERror += errmsg;
                    updatShipDate = "";
                    for (int i = 0; i < k; i++)
                    {
                       // updatShipDate += "Update inv1  set shipdate='" + Convert.ToString(dtDetail.GetValue("DelDate", i)) + "'  where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ;";


                        string PickDate = Convert.ToString(dtDetail.GetValue("nPickDate", i));
                        string PickShift = dtDetail.GetValue("nPickShift", i);
                        if (PickDate != "" && PickShift != "")
                        {
                            updatShipDate += "Update inv1 set U_DelTypCd = '" + oldTypeCode[i].ToString()  + "' ,  shipdate='" + Convert.ToString(dtDetail.GetValue("DelDate", i)) + "' , U_DelShift='" + Convert.ToString(dtDetail.GetValue("DelShift", i)) + "',";
                            updatShipDate += "  U_DeliveryWH ='" + oldDelWhs[i].ToString() + "',U_PickingWh='" + oldPickWhs[i].ToString() +"' , U_PickShift ='" + Convert.ToString(dtDetail.GetValue("PickShift", i)) + "' , u_PickDate = '" + Convert.ToString(dtDetail.GetValue("PickDate", i)) + "'  where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ;";

                        }
                        else
                        {
                            updatShipDate += "Update inv1 set  U_DelTypCd = '" + oldTypeCode[i].ToString() + "' , shipdate='" + Convert.ToString(dtDetail.GetValue("DelDate", i)) + "' , U_DeliveryWH ='" + oldDelWhs[i].ToString() + "',U_PickingWh='" + oldPickWhs[i].ToString() + "' ,  U_DelShift='" + Convert.ToString(dtDetail.GetValue("DelShift", i)) + "' where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ;";
                        }
                    }
                    Program.objHrmsUI.ExecQuery(updatShipDate, "Temp Update Ship Date");

                }
                else
                {
                    for (int i = 0; i < k; i++)
                    {
                        string oinvHeader = "";
                       
                        oinvHeader += " update oinv set header = isnull( convert(varchar,header),'')  + ' Schedule Updated for  " + Convert.ToString(dtDetail.GetValue("LineNum", i)) + " -" + Convert.ToString(dtDetail.GetValue("ItemCode", i)) + ". Old Values (" + Convert.ToString(dtDetail.GetValue("DelDate", i)) + "," + Convert.ToString(dtDetail.GetValue("DelShift", i)) + "," + Convert.ToString(dtDetail.GetValue("PickDate", i)) + "," + Convert.ToString(dtDetail.GetValue("PickShift", i)) + ") ";
                        oinvHeader += "  New Values (" + Convert.ToString(dtDetail.GetValue("nDelDate", i)) + "," + Convert.ToString(dtDetail.GetValue("nDelShift", i)) + "," + Convert.ToString(dtDetail.GetValue("nPickDate", i)) + "," + Convert.ToString(dtDetail.GetValue("nPickShift", i)) + ") ' where docentry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' ";

                        Program.objHrmsUI.ExecQuery(oinvHeader, "Updating Remarks");


                    }

                }

            }
            catch (Exception ex)
            {

                DIERror = ex.Message;

                oApplication.SetStatusBarMessage("General Error !" + ex.Message + " Known Error " + DIERror, BoMessageTime.bmt_Short, false);
                updatShipDate = "";
                for (int i = 0; i < k; i++)
                {

                   // updatShipDate += "Update inv1  set shipdate='" + Convert.ToString(dtDetail.GetValue("DelDate", i)) + "'  where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ; ";
                    string PickDate = Convert.ToString(dtDetail.GetValue("nPickDate", i));
                    string PickShift = dtDetail.GetValue("nPickShift", i);
                    if (PickDate != "" && PickShift != "")
                    {
                        updatShipDate += "Update inv1 set U_DelTypCd = '" + oldTypeCode[i].ToString() + "' ,  shipdate='" + Convert.ToString(dtDetail.GetValue("DelDate", i)) + "' , U_DelShift='" + Convert.ToString(dtDetail.GetValue("DelShift", i)) + "',";
                        updatShipDate += "  U_DeliveryWH ='" + oldDelWhs[i].ToString() + "',U_PickingWh='" + oldPickWhs[i].ToString() + "' , U_PickShift ='" + Convert.ToString(dtDetail.GetValue("PickShift", i)) + "' , u_PickDate = '" + Convert.ToString(dtDetail.GetValue("PickDate", i)) + "'  where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ;";

                    }
                    else
                    {
                        updatShipDate += "Update inv1 set  U_DelTypCd = '" + oldTypeCode[i].ToString() + "' , shipdate='" + Convert.ToString(dtDetail.GetValue("DelDate", i)) + "' , U_DeliveryWH ='" + oldDelWhs[i].ToString() + "',U_PickingWh='" + oldPickWhs[i].ToString() + "' ,  U_DelShift='" + Convert.ToString(dtDetail.GetValue("DelShift", i)) + "' where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ;";
                    }
                }
                Program.objHrmsUI.ExecQuery(updatShipDate, "Temp Update Ship Date");
            }
         
           
          




            //// Old Code /// 

            /*
            if (!validateinput())
            {
                return;
            }
            int k = dtDetail.Rows.Count;
            for (int i = 0; i < k; i++)
            {
                string updateLine="";
                string oinvHeader = "";
                string PickDate = Convert.ToString( dtDetail.GetValue("nPickShift",i));
                string PickShift = dtDetail.GetValue("nPickShift",i);
                if(PickDate !="" && PickShift!="")
                {
                    updateLine = "Update inv1 set U_DelTypCd = '" + Convert.ToString(dtDetail.GetValue("TypeCode", i)) + "' ,  shipdate='" + Convert.ToString(dtDetail.GetValue("nDelDate", i)) + "' , U_DelShift='" + Convert.ToString(dtDetail.GetValue("nDelShift", i)) + "',";
                    updateLine += "  U_DeliveryWH ='" + Convert.ToString(dtDetail.GetValue("DelWhs", i)) + "',U_PickingWh='" + Convert.ToString(dtDetail.GetValue("PickWhs", i)) + "' , U_PickShift ='" + Convert.ToString(dtDetail.GetValue("nPickShift", i)) + "' , u_PickDate = '" + Convert.ToString(dtDetail.GetValue("nPickDate", i)) + "'  where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ";
                    
                }
                else
                {
                    updateLine = "Update inv1 set  U_DelTypCd = '" + Convert.ToString(dtDetail.GetValue("TypeCode", i)) + "' , shipdate='" + Convert.ToString(dtDetail.GetValue("nDelDate", i)) + "' , U_DeliveryWH ='" + Convert.ToString(dtDetail.GetValue("DelWhs", i)) + "',U_PickingWh='" + Convert.ToString(dtDetail.GetValue("PickWhs", i)) + "' ,  U_DelShift='" + Convert.ToString(dtDetail.GetValue("nDelShift", i)) + "' where DocEntry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' and linenum='" + Convert.ToString(dtDetail.GetValue("LineNum", i)) + "' ";
                }
                oinvHeader += " update oinv set header = isnull( convert(varchar,header),'')  + ' Schedule Updated for  " + Convert.ToString(dtDetail.GetValue("LineNum", i)) + " -" + Convert.ToString(dtDetail.GetValue("ItemCode", i)) + ". Old Values (" + Convert.ToString(dtDetail.GetValue("DelDate", i)) + "," + Convert.ToString(dtDetail.GetValue("DelShift", i)) + "," + Convert.ToString(dtDetail.GetValue("PickDate", i)) + "," + Convert.ToString(dtDetail.GetValue("PickShift", i)) + ") ";
                oinvHeader += "  New Values (" + Convert.ToString(dtDetail.GetValue("nDelDate", i)) + "," + Convert.ToString(dtDetail.GetValue("nDelShift", i)) + "," + Convert.ToString(dtDetail.GetValue("nPickDate", i)) + "," + Convert.ToString(dtDetail.GetValue("nPickShift", i)) + ") ' where docentry = '" + Convert.ToString(dtDetail.GetValue("DocEntry", i)) + "' ";
         
                Program.objHrmsUI.ExecQuery(updateLine, "Updating Schedule");
                Program.objHrmsUI.ExecQuery(oinvHeader, "Updating Remarks");
                
            

            }

          
             
             *//// old Code Ended


        //    oApplication.SetStatusBarMessage("Schedule Updated Successfully!", BoMessageTime.bmt_Short, false);
            if (DIERror == "")
            {
                oApplication.SetStatusBarMessage("Schedule Updated Successfully!", BoMessageTime.bmt_Short, false);
                loadInvoice();
            }
           
             

                  
        }
        private bool validateinput()
        {
            bool result = true;
            mtDet.FlushToDataSource();
             int k = dtDetail.Rows.Count;
             for (int i = 0; i < k; i++)
             {
                 string nDeliveryDate = Convert.ToString(dtDetail.GetValue("nDelDate", i));
                 string nDelShift = Convert.ToString(dtDetail.GetValue("nDelShift", i));

                string DeliveryDate = Convert.ToString(dtDetail.GetValue("DelDate", i));
                string PickDate = Convert.ToString(dtDetail.GetValue("PickDate", i));


                 string invEntry = Convert.ToString(dtDetail.GetValue("DocEntry", i));

                 string strAvailCnt = " select [dbo].[getAvailableSlotCnt]('" + dtDetail.GetValue("ItemCode", i) + "','" + dtDetail.GetValue("DelWhs", i) + "','" + dtDetail.GetValue("nDelDate", i) + "','" + dtDetail.GetValue("nDelShift", i) + "','" + dtDetail.GetValue("TypeCode", i) + "'," + invEntry  + ",0) as AvailDelCnt ";
                 strAvailCnt += " ,  [dbo].[getAvailableSlotCnt]('" + dtDetail.GetValue("ItemCode", i) + "','" + dtDetail.GetValue("PickWhs", i) + "','" + dtDetail.GetValue("nPickDate", i) + "','" + dtDetail.GetValue("nPickShift", i) + "','" + dtDetail.GetValue("TypeCode", i) + "'," + invEntry + ",0) as AvailPickCnt ";


                 int availPickCnt = 0;
                 int availDelCnt = 0;

                 string strPickAvailCnt = "select sShift,AvailableSlot from [dbo].[ShiftnSlotsRS]('" + dtDetail.GetValue("ItemCode", i) + "','" + dtDetail.GetValue("PickWhs", i) + "','" + dtDetail.GetValue("nPickDate", i) + "','" + dtDetail.GetValue("TypeCode", i) + "','" + dtDetail.GetValue("DocEntry", i) + "') where sShift='" + dtDetail.GetValue("nPickShift", i) + "'";
                 System.Data.DataTable dtAvailCnt = Program.objHrmsUI.getDataTable(strPickAvailCnt, "Available Cnt");

                 if (dtAvailCnt != null && dtAvailCnt.Rows.Count > 0)
                 {
                     availPickCnt = Convert.ToInt32(dtAvailCnt.Rows[0]["AvailableSlot"]);

                 }
                 string strDelAvailCnt = "select sShift,AvailableSlot from [dbo].[ShiftnSlotsRS]('" + dtDetail.GetValue("ItemCode", i) + "','" + dtDetail.GetValue("DelWhs", i) + "','" + dtDetail.GetValue("nDelDate", i) + "','" + dtDetail.GetValue("TypeCode", i) + "','" + dtDetail.GetValue("DocEntry", i) + "') where sShift='" + dtDetail.GetValue("nDelShift", i) + "'";
                 dtAvailCnt = Program.objHrmsUI.getDataTable(strDelAvailCnt, "Available Cnt");
                 if (dtAvailCnt != null && dtAvailCnt.Rows.Count > 0)
                 {
                     availDelCnt = Convert.ToInt32(dtAvailCnt.Rows[0]["AvailableSlot"]);
                 }

                 if (nDeliveryDate == "" || nDelShift == "")
                 {
                     oApplication.SetStatusBarMessage("Delivery Date and Delivery Shift is required");
                     return false;
                 }

                 string nPickDate = Convert.ToString(dtDetail.GetValue("nPickDate", i));
                 DateTime dtDelDate = Convert.ToDateTime(nDeliveryDate);
                 DateTime dtOldDelDate = Convert.ToDateTime(DeliveryDate);
                 if (dtDelDate.Date != dtOldDelDate.Date && dtDelDate.Date < DateTime.Now.Date)
                 {
                     oApplication.SetStatusBarMessage("Past date is not allowed in delivery date ");
                     return false;
                 }
                 if (nPickDate == null) nPickDate = "";
                 if (nPickDate != "" )
                 {
                     DateTime dtPickDate = Convert.ToDateTime(nPickDate);
                     DateTime dtOldPickDate = Convert.ToDateTime(PickDate);
                     if ( dtPickDate.Date!=dtOldPickDate.Date &&  dtPickDate.Date < DateTime.Now.Date)
                     {
                         oApplication.SetStatusBarMessage("Past date is not allowed in pick date ");
                         return false;
                     }
                 }
                 string nPickShift = Convert.ToString(dtDetail.GetValue("nPickShift", i));
                 if (nPickShift == null) nPickShift = "";
                 if (nPickDate != "" && nPickShift == "")
                 {
                     oApplication.SetStatusBarMessage("Pick Shift is required if pick date provided");
                     return false;
                 }





                 if (dtDelDate.Date != dtOldDelDate.Date && availDelCnt <= 0)
                 {
                     oApplication.SetStatusBarMessage("Delivery Slot Not Available");
                     return false;
                 }
                 if ( nPickDate != "" && availPickCnt <= 0)
                 {
                     DateTime dtPickDate = Convert.ToDateTime(nPickDate);
                     DateTime dtOldPickDate = Convert.ToDateTime(PickDate);
                     if (dtPickDate.Date != dtOldPickDate.Date)
                     {
                         oApplication.SetStatusBarMessage("Pick Slot Not Available");
                         return false;
                     }
                 }
               
             }

            return result;
        }
        #endregion

       

       
    }

}

