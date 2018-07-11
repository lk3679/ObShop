using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.IO;

namespace OBShopWeb.Poslib
{
    class DB
    {
        //public static string ConnetionString(string DBname)
        //{
        //    string DBhost = "Data Source=IT-DB;User ID=sa;Password=*H7y_EYdw*LiGaee;Initial Catalog=";
        //    string OBOB = "Data Source=OBDB;User ID=orangebear;Password=V82M[8uN*1kzbAC;Initial Catalog=";
        //    string KWhost = "Data Source=KW-SQL-CLUSTER;User ID=orangebear;Password=V82M[8uN*1kzbAC;Initial Catalog=";
        //    string PosDBhost = "Data Source=S08001;User ID=shoppos;Password=X3h+jfeYEq;Initial Catalog=";
        //    int treasury=3;
        //    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["treasury"],out treasury);

        //    if (treasury == 4)
        //    {
        //        PosDBhost = "Data Source=S03001;User ID=shopposap;Password=zdvawY&cU#;Initial Catalog=";
        //    }

        //    bool mode = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["testmode"]);
        //    if (mode)
        //    {
        //        switch (DBname)
        //        {
        //            case "A0POS":
        //                return DBhost + DBname;
        //            case "kw":
        //                return DBhost + DBname;
        //            case "OBPOS":
        //                return DBhost + DBname;
        //            case "orangebear":
        //                return DBhost + DBname;
        //            case "SERVERPOS":
        //                return DBhost + DBname;
        //            case "XPPOS":
        //                return DBhost + DBname;
        //            case "PosClient":
        //                return DBhost + DBname;
        //            default:
        //                return DBhost + "kw";
        //        }

        //    }
        //    else
        //    {
        //        switch (DBname)
        //        {
        //            case "A0POS":
        //                return OBOB + DBname;
        //            case "kw":
        //                return KWhost + DBname;
        //            case "OBPOS":
        //                return OBOB + DBname;
        //            case "orangebear":
        //                //return OBOB + DBname;
        //                return DBhost + DBname;
        //            case "SERVERPOS":
        //                return OBOB + DBname;
        //            case "XPPOS":
        //                return OBOB + DBname;
        //            case "PosClient":
        //                return PosDBhost + DBname;
        //            default:
        //                return KWhost + "kw";
        //        }
        //    }
        //}

        public static string ConnetionString(string DBname)
        {
            string DBString = "";
            string XMLFile = HttpContext.Current.Server.MapPath("ConnectionString.xml");
            if (File.Exists(XMLFile)==false)
            { return DBString; }

            try {
                XmlDocument doc = new XmlDocument();
                doc.Load(XMLFile);
                XmlNode ConnetionString = doc.SelectSingleNode("/ConnetionString/" + DBname);
                XmlElement element = (XmlElement)ConnetionString;

                if (element.HasChildNodes)
                {
                    string DataSource = element.SelectSingleNode("DataSource").InnerText;
                    string ID = element.SelectSingleNode("ID").InnerText;
                    string Password = Cryptography.decode(element.SelectSingleNode("Password").InnerText);
                    string InitialCatalog = element.SelectSingleNode("InitialCatalog").InnerText;

                    DBString = string.Format("Data Source={0};User ID={1};Password={2};Initial Catalog={3}", DataSource, ID, Password, InitialCatalog);
                }
            }
            catch (Exception e){
                string errorMsg = e.ToString();
            }
            
            return DBString;
            
        }

        public static bool IsServerConnected(string DBname)
        {
            using (var l_oConnection = new SqlConnection(DB.ConnetionString(DBname)))
            {
                try
                {
                    l_oConnection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        public static DataTable DBQuery(string SQL, Dictionary<string, object> param,string DBname)
        {
            string DBstring=DB.ConnetionString(DBname);
            DataTable dt = new DataTable();

            if (DBstring == "")
            {
                HttpContext.Current.Response.Redirect("Default.aspx");
                return dt;
            }

            if (IsServerConnected(DBname) == false)
            {
                HttpContext.Current.Response.Redirect("Default.aspx");
                return dt;
            }
            
            SqlDataAdapter da = new SqlDataAdapter(SQL, DBstring);
            foreach (KeyValuePair<string, object> item in param)
                da.SelectCommand.Parameters.AddWithValue("@" + item.Key, item.Value);

            da.Fill(dt);
            return dt;
        }

        public static bool DBNonQuery(string SQL, Dictionary<string, object> param, string DBname)
        {
            int result = 0;
            SqlConnection connection = new SqlConnection(DB.ConnetionString(DBname));
            using (SqlCommand sqlCmd = new SqlCommand(SQL, connection))
            {
                connection.Open();
                SqlTransaction transaction;
                transaction = connection.BeginTransaction();
                sqlCmd.Transaction = transaction;
                if (sqlCmd.Connection.State != ConnectionState.Open) sqlCmd.Connection.Open();

                foreach (KeyValuePair<string, object> item in param){
                    sqlCmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                    //System.Diagnostics.Debug.Print("{0}, {1}", item.Key, item.Value.ToString().Length);
                }
                try
                {
                    result = sqlCmd.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch { transaction.Rollback(); }

                if (sqlCmd.Connection.State != ConnectionState.Closed) sqlCmd.Connection.Close();
            }
            return result > 0;
        }


    }
}