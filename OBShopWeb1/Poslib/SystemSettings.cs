using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OBShopWeb.Poslib
{
    public class SystemSettings
    {
        public static bool GetNeedPrintPickSheet()
        {
            string NeedPrintPickSheet = "";
            bool result=false;
            NeedPrintPickSheet = GetSystemSettings("NeedPrintPickSheet");
            bool.TryParse(NeedPrintPickSheet, out result);
            return result;
        }

        public static bool GetApplyVipDiscount()
        {
            string ApplyVipDiscount = "";
            bool result = false;
            ApplyVipDiscount = GetSystemSettings("ApplyVipDiscount");
            bool.TryParse(ApplyVipDiscount, out result);
            return result;
        }


        public static string GetSystemSettings(string key)
        {
            string cache_id = " | HASH | " + key;
            string result = "";
            string strCache = SiteHelper.GetCache(cache_id) as string;
            if (strCache == null)
            {
                string sql = "SELECT  [ApplyVipDiscount],[NeedPrintPickSheet],[PdfFilePath] FROM [PosClient].[dbo].[SystemSettings]  ";
                Dictionary<string, object> param = new Dictionary<string, object>();
                DataTable dt = DB.DBQuery(sql, param, "PosClient");
                if (dt.Rows.Count > 0)
                {
                    result = dt.Rows[0][key].ToString();
                    SiteHelper.SetCache(cache_id, result, 10);
                }
            }
            else
            {
                result=strCache;
            }

            return result;

        }
    }
}