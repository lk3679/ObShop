using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace OBShopWeb.Poslib
{
    public class PosNumber
    {
        static string DefaultPrintMachineNo = "MM";
        static string DefaultInvoiceMachineNo = "P9";
        static string DefaultPosNo = "9";

        public static string GetPosNumberMapping(string IP)
        {
            string sql = "Select a.PosNo from PosNumberMapping a where a.ClientIP=@IP ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("IP", IP);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["PosNo"].ToString();
            else
                return DefaultPosNo;
        }

        public static string GetInvoiceMachineNo(string IP)
        {
            string sql = "Select a.InvoiceMachineNo from PosNumberMapping a where a.ClientIP=@IP ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("IP", IP);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["InvoiceMachineNo"].ToString();
            else
                return DefaultInvoiceMachineNo;
        }

        public static string GetPrintMachineNo(string IP)
        {
            
            string sql = "Select a.PrintMachineNo from PosNumberMapping a where a.ClientIP=@IP ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("IP", IP);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["PrintMachineNo"].ToString();
            else
                return DefaultPrintMachineNo;
        }

        public static DataTable GetAllPosNo()
        {
            string sql = "Select PosNo from PosNumberMapping";
            Dictionary<string, object> param = new Dictionary<string, object>();
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            return dt;
        }

    }
}