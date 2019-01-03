using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PropAPI.Models;

namespace PropAPI.SAP
{
    public class BPRepo
    {
        PMDIApi DI;
        public BPRepo()
        {
            DI = new PMDIApi();
        }

        public BP getBP(string bpCode)
        {

            BP _bp = new BP();

            DataTable dtBP = DI.DataService.getDataTable("SELECT * FROM \"@ACPM_INT_BP\" WHERE \"Code\" = '" + bpCode + "'");
            if (dtBP.Rows.Count > 0)
            {

                DataRow dr = dtBP.Rows[0];
                _bp.BPCode = dr["Code"].ToString();
                _bp.BPName = dr["Name"].ToString();
                _bp.BillingAddress = dr["U_BillingAddress"].ToString();

                _bp.BPGroup = dr["U_BPGroup"].ToString();
                _bp.Email = dr["U_Email"].ToString();
                _bp.Phone = dr["U_Phone"].ToString();
                _bp.PostedSAP = dr["U_PostedSAP"].ToString();
                if (_bp.PostedSAP == "Y")
                {
                    _bp.PostedDt = Convert.ToDateTime(dr["U_PostedSAP"]);
                    _bp.SAPCode = dr["U_SAPCode"].ToString();
                }

            }
            return _bp;


        }
        public List<BP> getBP()
        {
            List<BP> bps = new List<BP>();

            DataTable dtBP = DI.DataService.getDataTable("SELECT * FROM \"@ACPM_INT_BP\" ");
            foreach (DataRow dr in   dtBP.Rows)
            {
                BP _bp = new BP();
                _bp.BPCode = dr["Code"].ToString();
                _bp.BPName = dr["Name"].ToString();
                _bp.BillingAddress = dr["U_BillingAddress"].ToString();

                _bp.BPGroup = dr["U_BPGroup"].ToString();
                _bp.Email = dr["U_Email"].ToString();
                _bp.Phone = dr["U_Phone"].ToString();
                _bp.PostedSAP = dr["U_PostedSAP"].ToString();
                if (_bp.PostedSAP == "Y")
                {
                    _bp.PostedDt = Convert.ToDateTime(dr["U_PostedSAP"]);
                    _bp.SAPCode = dr["U_SAPCode"].ToString();
                }
                bps.Add(_bp);
            }
            return bps;
        }


        public string PostBP(BP bpCode)
        {


            string insertBP = "INSERT INTO \"@ACPM_INT_BP\" ";
            insertBP += " (\"Code\",\"Name\",\"U_BPGroup\",\"U_BillingAddress\",\"U_Email\",\"U_Phone\")";
            insertBP += " VALUES (@Code,@Name,@U_BPGroup,@U_BillingAddress,@U_Email, @U_Phone)";
            Hashtable hp = new Hashtable();
            hp.Add("@Code", bpCode.BPCode);
            hp.Add("@Name", bpCode.BPName);
            hp.Add("@U_BPGroup", bpCode.BPGroup);
            hp.Add("@U_BillingAddress", bpCode.BillingAddress);
            hp.Add("@U_Email", bpCode.Email);
            hp.Add("@U_Phone", bpCode.Phone);
            string result = DI.DataService.ExecuteNonQuery(insertBP, hp);


            return result;


        }

        public string Update(BP _bp)
        {


            string strUpdate = "UPDATE  \"@ACPM_INT_BP\"  SET \"Name\" = @Name,\"U_BPGroup\"=@U_BPGroup,\"U_BillingAddress\"=@U_BillingAddress,\"U_Email\"=@U_Email,\"U_Phone\"=@U_Phone WHERE @Code='" + _bp.BPCode + "'";
            Hashtable hp = new Hashtable();
            hp.Add("@Code", _bp.BPCode);
            hp.Add("@Name", _bp.BPName);
            hp.Add("@U_BPGroup", _bp.BPGroup);
            hp.Add("@U_BillingAddress", _bp.BillingAddress);
            hp.Add("@U_Email", _bp.Email);
            hp.Add("@U_Phone", _bp.Phone);
            string result = DI.DataService.ExecuteNonQuery(strUpdate, hp);


            return result;


        }
        public string DELBp(string bpCode)
        {


            string insertBP = "DELETE  \"@ACPM_INT_BP\"  WHERE \"Code\" = '" + bpCode + "'";
            string result = DI.DataService.ExecuteNonQuery(insertBP);
            return result;


        }

    }
}