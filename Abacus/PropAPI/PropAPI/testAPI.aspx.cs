using System;
using System.Data;

using System.Data.Odbc;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PropAPI.SAP;

namespace PropAPI
{
    public partial class testAPI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Write("Hello World New to test constr");
            PMDIApi di = new PMDIApi();

            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = di.constr;
            try
            {


                con.Open();

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
            finally
            {
                ////if (con.State == ConnectionState.Open) con.Close();
            }


           DataTable dt = di.DataService.getDataTable("SELECT * FROM \"@ACPM_INT_USR\" where \"Code\" = 'ExternApp' AND  \"U_APIKey\"  ='Xui==sx908sx!='");

            if (dt != null && dt.Rows.Count > 0)
            {
                Response.Write("User FOund");
            }
            else
            {
                Response.Write("User not found");
            }

            Response.Write(di.constr);

        }
    }
}