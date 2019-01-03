using System;
using System.IO;
using System.Globalization;

using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropAPI.SAP
{
    public class DataServicesSQL : IDataServices
    {
        private string _constr = "";
        public string constr
        {
            get { return _constr; }
            set { _constr = constr; }
        }

        public string hrmsDbname = "";
        public string userId = "";


        public DataServicesSQL(string strCon)
        {

            _constr = strCon;

        }


        public string getConStatus()
        {
            string result = "NA";
            SqlConnection con = new SqlConnection();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                result = "Connected";

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


        public object getScallerValue(string strSql)
        {
            object outResult = new object();
            SqlConnection con = new SqlConnection();
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
            long nextId = 0;
            string strSql = " Select isnull(max(convert(int," + idCol + ")),0)  as nextId from " + tblName;
            try
            {
                nextId = Convert.ToInt32(getScallerValue(strSql));
            }
            catch { }
            return nextId;
        }

        public decimal getEmpFieldValue(string idCol, int empId)
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
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }



            return dt;


        }
        public DataTable getDataTable(string strsql, Hashtable pms)
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
                foreach (string key in pms.Keys)
                {
                    if (key.Contains("img"))
                    {
                        cmd.Parameters.Add(key, SqlDbType.Image, 0).Value = GetBytesFromFile(pms[key].ToString());
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue(key, pms[key].ToString());
                    }
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;


            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
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
                result = ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }



            return result;


        }
        public byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)

            FileStream fs = File.OpenRead(fullFilePath);

            try
            {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));

                return bytes;
            }
            finally
            {
                fs.Close();
            }

        }

        public string ExecuteNonQuery(string strsql, Hashtable sqP)
        {
            string result = "OK";
            SqlConnection con = new SqlConnection();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

               
                foreach (string key in sqP.Keys)
                {
                    /*
                     * if (key.Contains("img"))
                   {
                       cmd.Parameters.Add(key, SqlDbType.Image, 0).Value = GetBytesFromFile(sqP[key].ToString());
                   }
                   else
                   {
                       cmd.Parameters.Add(key, sqP[key].ToString());
                   }
                   */
                  strsql=  strsql.Replace(key, "'" + sqP[key].ToString() + "'");

                }

                cmd.CommandText = strsql;


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
