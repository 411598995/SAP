using System;
using System.IO;
using System.Globalization;

using System.Data;
using System.Data.Odbc;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace PropAPI.SAP
{
    public class DataServicesHANA : IDataServices
    {
        string _constr = "";

        public string constr
        {
            get { return _constr; }
            set { _constr = constr; }
        }
        public DataServicesHANA(string strCon)
        {
            _constr = strCon;

        }



        public string getConStatus()
        {
            string result = "NA";
            OdbcConnection con = new OdbcConnection();
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
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                OdbcCommand cmd = new OdbcCommand();

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


        public DataTable getDataTable(string strsql)
        {
            DataTable dt = new DataTable();
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                OdbcCommand cmd = new OdbcCommand();
                cmd.Connection = con;

                cmd.CommandText = strsql;



                OdbcDataReader dr = cmd.ExecuteReader();


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
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = constr;
            try
            {


                if (con.State == ConnectionState.Closed) con.Open(); else con.Close();
                OdbcCommand cmd = new OdbcCommand();
                cmd.Connection = con;

                cmd.CommandText = strsql;
                foreach (string key in pms.Keys)
                {

                    cmd.Parameters.AddWithValue(key, pms[key].ToString());

                }

                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
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
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                OdbcCommand cmd = new OdbcCommand();
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
        public static byte[] GetBytesFromFile(string fullFilePath)
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
            OdbcConnection con = new OdbcConnection();
            con.ConnectionString = constr;
            try
            {

                if (con.State == ConnectionState.Closed) con.Open();
                OdbcCommand cmd = new OdbcCommand();
                cmd.Connection = con;

                foreach (string key in sqP.Keys)
                {
                    /*
                    if (key.Contains("img"))
                    {
                        cmd.Parameters.Add(key, OdbcType.Image, 0).Value = GetBytesFromFile(sqP[key].ToString());
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
                result = strsql + "\n\r" +  ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }



            return result;


        }










    }
}