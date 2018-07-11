using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace OBShopWeb.Poslib
{
    public class Log
    {
        public static bool Add(int Type, string ValueStr01, string ValueStr02, string Remark,string ClerkID,string PosNo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Insert into PosClient..Log (Type,ValueStr01,ValueStr02,Remark,ClerkID,PosNo,Time) ");
            sb.Append("Values(@Type,@ValueStr01,@ValueStr02,@Remark,@ClerkID,@PosNo,getdate())  ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Type", Type);
            param.Add("ValueStr01",ValueStr01);
            param.Add("ValueStr02", ValueStr02);
            param.Add("Remark", Remark);
            param.Add("ClerkID", ClerkID);
            param.Add("PosNo", PosNo);
            return DB.DBNonQuery(sb.ToString(), param, "PosClient");
        }

    }
}