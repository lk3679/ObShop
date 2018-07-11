using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OBShopWeb.Poslib
{
    public class Activity
    {
        public static bool AddNewActivity(string Name, string Type, string StartDate, string EndDate, string ClerkID, string Parameters, string VIPOnly, bool CanApplyWithOthers)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Insert into posclient..Activities(Name,Type,StartDate,EndDate,ClerkID,[Parameters],CreateDate,VIPOnly,CanApplyWithOthers) ");
            sb.AppendLine("values(@Name,@Type,@StartDate,@EndDate,@ClerkID,@Parameters,GETDATE(),@VIPOnly,@CanApplyWithOthers) ");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Name", Name);
            param.Add("Type", Type);
            param.Add("StartDate", StartDate);
            param.Add("EndDate", EndDate);
            param.Add("ClerkID", ClerkID);
            param.Add("Parameters", Parameters);
            param.Add("VIPOnly", VIPOnly);
            param.Add("CanApplyWithOthers", CanApplyWithOthers);
            return DB.DBNonQuery(sql, param, "PosClient");
        }

        public static bool CheckHasActivitySamePeroid(DateTime StartDate, DateTime EndDate, string ActivityID,bool VIPOnly,string ActType)
        {
            bool CanApplyWithOthers = false;
         
            if (ActType == "新品折X元")
            { CanApplyWithOthers = true; }

            DataTable ActivitiesDT = GetAllActitivities();
            IEnumerable<DataRow> query;
            if (CanApplyWithOthers)
            {
                query = (from a in ActivitiesDT.AsEnumerable()
                         where (StartDate >= DateTime.Parse(a["StartDate"].ToString()) && StartDate <= DateTime.Parse(a["EndDate"].ToString())
                         || (EndDate >= DateTime.Parse(a["StartDate"].ToString()) && EndDate <= DateTime.Parse(a["EndDate"].ToString())))
                         && (a["ActivityID"].ToString() != ActivityID) && bool.Parse(a["CanApplyWithOthers"].ToString()) == CanApplyWithOthers
                         select a).ToList();

            }
            else
            {
                query = (from a in ActivitiesDT.AsEnumerable()
                         where (StartDate >= DateTime.Parse(a["StartDate"].ToString()) && StartDate <= DateTime.Parse(a["EndDate"].ToString())
                         || (EndDate >= DateTime.Parse(a["StartDate"].ToString()) && EndDate <= DateTime.Parse(a["EndDate"].ToString())))
                         && (a["ActivityID"].ToString() != ActivityID) && bool.Parse(a["VIPOnly"].ToString()) == VIPOnly && bool.Parse(a["CanApplyWithOthers"].ToString()) == CanApplyWithOthers
                         select a).ToList();

            }

            if (query.Count() == 0)
                return false;
            else
                return true;
           
                
        }

        public static bool UpdateActivity(string ActivityID, string Name, string Type, string StartDate, string EndDate, string ClerkID, string Parameters, string ActivityProductType, string VIPOnly, bool CanApplyWithOthers)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("update PosClient..Activities ");
            sb.AppendLine("set Name=@Name,Type=@Type,StartDate=@StartDate,EndDate=@EndDate,");
            sb.AppendLine("ClerkID=@ClerkID,Parameters=@Parameters,ActivityProductType=@ActivityProductType,CreateDate=GETDATE(),VIPOnly=@VIPOnly ");
            sb.AppendLine("where ActivityID=@ActivityID ");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ActivityID", ActivityID);
            param.Add("Name", Name);
            param.Add("Type", Type);
            param.Add("StartDate", StartDate);
            param.Add("EndDate", EndDate);
            param.Add("ClerkID", ClerkID);
            param.Add("Parameters", Parameters);
            param.Add("ActivityProductType", ActivityProductType);
            param.Add("VIPOnly", VIPOnly);
            param.Add("CanApplyWithOthers", CanApplyWithOthers);
            return DB.DBNonQuery(sql, param, "PosClient");
        }

        public static void DeleteActivity(string ActivityID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Delete from PosClient..Activities where ActivityID=@ActivityID; ");
            sb.AppendLine("delete from [PosClient].[dbo].[ActivityProducts]  where ActivityID=@ActivityID ");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ActivityID", ActivityID);
            bool DeleteResult = DB.DBNonQuery(sql, param, "PosClient");
            var Result = new { result = DeleteResult };
            string JsonResult = JsonConvert.SerializeObject(Result);
            ShowResultOnPage(JsonResult);
        }

        public static void DeleteActivitySerialID(string ActivityID, string SerialID, string ClerkID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("delete from [PosClient].[dbo].[ActivityProducts] ");
            sb.AppendLine("where ActivityID=@ActivityID and ProductSerialID=@SerialID  ");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ActivityID", ActivityID);
            param.Add("SerialID", SerialID);
            bool DeleteResult = DB.DBNonQuery(sql, param, "PosClient");

            //紀錄LOG
            if (DeleteResult)
            {
                AddLog(ActivityID, ClerkID);
            }

            var Result = new { result = DeleteResult };
            string JsonResult = JsonConvert.SerializeObject(Result);
            ShowResultOnPage(JsonResult);
        }

        public static void ShowResultOnPage(string JsonResult)
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write(JsonResult);
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();
        }

        public static DataTable GetAllActitivities()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT [ActivityID],[Name],[Type] ,[Parameters] ,[StartDate] ,[EndDate] ,[ClerkID] ,[CreateDate],ActivityProductType,VIPOnly,[CanApplyWithOthers] ");
            sb.AppendLine("FROM [PosClient].[dbo].[Activities] ");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            return DB.DBQuery(sql, param, "PosClient");
        }

        public static DataTable GetActivityToday()
        {
            DataTable dt = GetAllActitivities();
            IEnumerable<DataRow> query = (from a in dt.AsEnumerable()
                                          where DateTime.Now >= Convert.ToDateTime(a["StartDate"]) && DateTime.Now <= Convert.ToDateTime(a["EndDate"]).AddDays(1)
                                          select a).ToList();

            DataTable ActivityDT = new DataTable();

            if (query.Count() > 0)
                ActivityDT = query.CopyToDataTable();

            return ActivityDT;
        }

        public static DataTable GetSerialID(string ActivityID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT [ActivityID],[ProductSerialID] ");
            sb.AppendLine("FROM [PosClient].[dbo].[ActivityProducts]  a  ");
            sb.AppendLine("where ActivityID=@ActivityID ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ActivityID", ActivityID);
            DataTable dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static bool AddSerialID(string ActivityID, string ProductSerialID, int Type)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT [ActivityID],[ProductSerialID],[Type] ");
            sb.AppendLine("FROM [PosClient].[dbo].[ActivityProducts]  a  ");
            sb.AppendLine("where a.ProductSerialID=@ProductSerialID and ActivityID=@ActivityID ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ProductSerialID", ProductSerialID);
            param.Add("ActivityID", ActivityID);
            param.Add("Type", Type);
            DataTable dt = DB.DBQuery(sb.ToString(), param, "PosClient");

            if (dt.Rows.Count > 0)
                return true;

            sb.Clear();
            sb.AppendLine("Insert into PosClient..ActivityProducts(ActivityID,ProductSerialID) ");
            sb.AppendLine("values (@ActivityID,@ProductSerialID)  ");
            sb.AppendLine("update PosClient..Activities set ActivityProductType=@Type ");
            sb.AppendLine("where ActivityID=@ActivityID ");
            string sql = sb.ToString();
            return DB.DBNonQuery(sql, param, "PosClient");
        }

        public static bool DeleteAllSerialID(string ActivityID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("delete from [PosClient].[dbo].[ActivityProducts]  where ActivityID=@ActivityID ");
            sb.AppendLine("update PosClient..Activities set ActivityProductType=null where ActivityID=@ActivityID ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ActivityID", ActivityID);
            string sql = sb.ToString();
            return DB.DBNonQuery(sql, param, "PosClient");
        }

        public static bool ValidateDate(string date)
        {
            try
            {
                string[] dateParts = date.Split('/');
                DateTime testDate = new
                    DateTime(Convert.ToInt32(dateParts[0]),
                    Convert.ToInt32(dateParts[1]),
                    Convert.ToInt32(dateParts[2]));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void AddLog(string ActivityID, string ClerkID)
        {
            DataTable SerialDT = GetSerialID(ActivityID);
            string ActivityProductType = Activity.FindActivityType(ActivityID);
            //如果是新品折X元，要多寫LOG
            if (ActivityProductType == "新品折X元")
            {
                //刪除今天的LOG
                DeleteActivityProductHistoryToday(ActivityID);
                foreach(DataRow row in SerialDT.Rows){
                    AddActivityProductHistory(ActivityID, row["ProductSerialID"].ToString(), ClerkID);
                }
                
            }
        }

        public static bool AddActivityProductHistory(string ActivityID, string ProductSerialID, string ClerkID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Insert into PosClient..ActivityProductHistory(ActivityID,ProductSerialID,ClerkID,UpdateTime)  ");
            sb.AppendLine("values(@ActivityID,@ProductSerialID,@ClerkID,GETDATE()) ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ActivityID", ActivityID);
            param.Add("ProductSerialID", ProductSerialID);
            param.Add("ClerkID", ClerkID);
            string sql = sb.ToString();
            return DB.DBNonQuery(sql, param, "PosClient");
        }

        public static string FindActivityType(string ActivityID)
        {
            string ActivityProductType = "";
            DataTable ActivitiesDT = new DataTable();
            ActivitiesDT = Activity.GetAllActitivities();
            var  query = (from a in ActivitiesDT.AsEnumerable()
                                          where a["ActivityID"].ToString() == ActivityID
                                          select new {ActivityID = a["ActivityID"].ToString(),ActType=a["Type"].ToString()
                }).ToList();

            ActivityProductType = query[0].ActType;
            return ActivityProductType;
        }

        public static bool DeleteActivityProductHistoryToday(string ActivityID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("delete from PosClient..ActivityProductHistory  ");
            sb.AppendLine("where convert(varchar(10), UpdateTime, 102) = convert(varchar(10), getdate(), 102) and ActivityID=@ActivityID ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ActivityID", ActivityID);
            string sql = sb.ToString();
            return DB.DBNonQuery(sql, param, "PosClient");
        }

        public static DataTable GetActivityProductHistoryToday(string ActivityID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT  [ActivityID],[ProductSerialID],[ClerkID],UpdateTime ");
            sb.AppendLine("FROM PosClient..ActivityProductHistory  ");
            sb.AppendLine("where convert(varchar(10), UpdateTime, 102) = convert(varchar(10), getdate(), 102) and ActivityID=@ActivityID ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ActivityID", ActivityID);
            DataTable dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static bool IsDiscountNPriceChanged(string ActivityID)
        {
            bool result = false;
            DataTable tempDT = new DataTable();
            string ActivityProductType = Activity.FindActivityType(ActivityID);
            if (ActivityProductType == "新品折X元")
            {
                tempDT = GetActivityProductHistoryToday(ActivityID);
                if (tempDT.Rows.Count > 0)
                { result = true; }
            }

            return result;
        }

    }
}