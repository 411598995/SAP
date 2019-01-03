using System;
using System.IO;
using System.Globalization;

using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACHR
{
    public class DataServices 
    {
        string constr = "";
        string hrmsDbname = "";
        string userId = "";

        public DataServices(string strCon)
            
        {
           constr = strCon;
        }
        

        

        
        public object getScallerValue(string strSql)
        {
            object outResult = new object();
            SqlConnection con = new SqlConnection ();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = strSql;
                 outResult = cmd.ExecuteScalar();
              
            }
            catch
            {
            }
            finally
            {
                 if (con.State == ConnectionState.Open) con.Close();
            }

            return outResult;
        }
        public long getMaxId(string tblName, string idCol)
        {
            long nextId = 1;
            string strSql = " Select isnull(max(convert(int," + idCol + ")),0)  as nextId from " + tblName;
           try
           {
               nextId = Convert.ToInt32(getScallerValue(strSql));
            }catch{}
            return nextId;
        }

        public decimal getEmpFieldValue( string idCol,int empId)
        {
            decimal fieldValue = 0.00M;
            string strSql = " Select isnull(" + idCol + ",0)  as fieldValue from " + hrmsDbname + ".dbo.mstEmployee where ID='" + empId.ToString() + "'";
            try
            {
                fieldValue = Convert.ToDecimal(getScallerValue(strSql));
            }
            catch { }
            return fieldValue;
        }
        public DataTable getDataTable(string strsql)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = strsql;



                SqlDataReader dr = cmd.ExecuteReader();

               
                dt.Clear();
                dt.Rows.Clear();
                dt.Load(dr);

                dr.Close();
            }
            catch
            {
                
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }



            return dt;


        }
        public DataTable getDataTable(string strsql,string method2)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection();
            con.ConnectionString = constr;
            try
            {


                if (con.State == ConnectionState.Closed) con.Open(); else con.Close();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = strsql;

                SqlDataAdapter da = new SqlDataAdapter(strsql, con);
                da.Fill(dt);
                return dt;

               
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Closed) con.Open();
            }



          


        }


        public string ExecuteNonQuery(string strsql)
        {
            string result = "OK";
            SqlConnection con = new SqlConnection();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = strsql;



                 cmd.ExecuteNonQuery();


              
               
            }
            catch (Exception ex)
            {
                result =ex.Message;
            }
            finally
            {
                 if (con.State == ConnectionState.Open) con.Close();
            }



            return result;


        }
        public static byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)

            FileStream fs = File.OpenRead(fullFilePath);

            try
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                return bytes;
            }
            finally
            {
                fs.Close();
            }

        }

        public string ExecuteNonQuery(string strsql , Hashtable sqP)
        {
            string result = "OK";
            SqlConnection con = new SqlConnection();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                cmd.CommandText = strsql;
                foreach (string key in sqP.Keys)
                {

                    cmd.Parameters.AddWithValue(key, sqP[key].ToString());

                }
                


                 cmd.ExecuteNonQuery();




            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }



            return result;


        }



       

        
        
     
      
    }
}
