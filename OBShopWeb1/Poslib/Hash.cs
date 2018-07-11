using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OBShopWeb.Poslib
{
    public class Hash
    {
        public static string get_hash(string key)
        {
            string cache_id = " | HASH | " + key;
            string strCache = SiteHelper.GetCache(cache_id) as string;
            if (strCache == null)
            {
                string sql = "select value from hash where _key = @key  ";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("key", key);
                DataTable dt = DB.DBQuery(sql, param, "orangebear");
                if (dt.Rows.Count > 0)
                {
                    string result = dt.Rows[0][0].ToString();
                    SiteHelper.SetCache(cache_id, result, 60);
                    return result;
                }
                else
                    return "";
            }
            else
            {
                return strCache;
            }
        }
    }
}