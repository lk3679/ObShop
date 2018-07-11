using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;

namespace OBShopWeb.Poslib
{
   public class VipMember
    {
       public static MemberData GetMemberData(string vip_id,string mobile){
           DataTable dt=new DataTable();
           MemberData MD = new MemberData();
           DateTime current = DateTime.Now;
           if (vip_id.Length > 0)
               dt = get_pos_vip(vip_id);

           if (mobile.Length > 0)
               dt = get_pos_vip_by_mobile(mobile);

           if (dt.Rows.Count > 0)
           {
               MD.vip_id = dt.Rows[0]["vip_id"].ToString();
               MD.name = dt.Rows[0]["name"].ToString();
               MD.tel = dt.Rows[0]["tel"].ToString();
               MD.mobile = dt.Rows[0]["mobile"].ToString();
               MD.email = dt.Rows[0]["email"].ToString();
               MD.bonus = int.Parse(dt.Rows[0]["bonus"].ToString());
               MD.birthday = current.ToString("yyyy") + "-" + DateTime.Parse(dt.Rows[0]["birthday"].ToString()).ToString("MM-dd");
               MD.last_birthday = (dt.Rows[0]["last_birthday"].ToString() != "") ? DateTime.Parse(dt.Rows[0]["last_birthday"].ToString()).ToString("yyyy-MM-dd") : "1970-01-01";
               MD.valid_date = (dt.Rows[0]["valid_date"].ToString() != "") ? DateTime.Parse(dt.Rows[0]["valid_date"].ToString()).ToString("yyyy-MM-dd") : "";
               MD.create_date = (dt.Rows[0]["create_date"].ToString() != "") ? DateTime.Parse(dt.Rows[0]["create_date"].ToString()).ToString("yyyy-MM-dd") : "";
               MD.status = dt.Rows[0]["status"].ToString();

               MD.discount = Hash.get_hash("pos_vip_discount") != "" ? Double.Parse(Hash.get_hash("pos_vip_discount")) : 1;
               MD.birthday_discount = (DateTime.Parse(MD.last_birthday).Year < current.Year && DateTime.Parse(MD.birthday).Month == current.Month) ? true : false;
               MD.valid = DateTime.Parse(MD.valid_date) >= current;
           }

           return MD;
       }

        public static DataTable get_pos_vip(string vip_id)
        {
            string sql = "select * from pos_vip where vip_id = @vip_id ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("vip_id", vip_id);
            DataTable dt = DB.DBQuery(sql, param, "orangebear");
            return dt;
        }

        public static DataTable get_pos_vip_by_mobile(string mobile)
        {
            string sql = "select * from pos_vip where mobile = @mobile ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("mobile", mobile);
            DataTable dt = DB.DBQuery(sql, param, "orangebear");
            return dt;
        }

       public static bool UpdateVipBonus(string vip_id,int bonus,bool HasUseBirthdayDiscount){
           StringBuilder sb = new StringBuilder();
           sb.AppendLine(" update orangebear..pos_vip set bonus=@bonus ");
           if (HasUseBirthdayDiscount)
           {
               sb.AppendLine(",last_birthday=getdate() ");
           }

           sb.AppendLine("where vip_id=@vip_id  ");
           string sql = sb.ToString();
           Dictionary<string, object> param = new Dictionary<string, object>();
           param.Add("vip_id", vip_id);
           param.Add("bonus", bonus);
           return DB.DBNonQuery(sql, param, "orangebear");
       }

        public static bool add_pos_vip(string vip_id, string name, string tel, string mobile, string email, string birthday)
        {
            string create_date = DateTime.Now.ToString("yyyy-MM-dd");
            string valid_date = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");
            string sql = "insert pos_vip (vip_id, name, tel, mobile, email,birthday, valid_date, create_date) values";
            sql += "(@vip_id, @name, @tel, @mobile, @email,@birthday, @valid_date, @create_date)";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("vip_id", vip_id);
            param.Add("name", name);
            param.Add("tel", tel);
            param.Add("mobile", mobile);
            param.Add("email", email);
            param.Add("birthday", birthday);
            param.Add("valid_date", valid_date);
            param.Add("create_date", create_date);
            return DB.DBNonQuery(sql, param, "orangebear");

        }

        public static bool edit_pos_vip(string vip_id, string name, string tel, string mobile, string email, string birthday)
        {
            string sql = "update pos_vip set name = @name,tel = @tel,mobile = @mobile,email = @email,birthday = @birthday where vip_id = @vip_id ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("vip_id", vip_id);
            param.Add("name", name);
            param.Add("tel", tel);
            param.Add("mobile", mobile);
            param.Add("email", email);
            param.Add("birthday", birthday);
            return DB.DBNonQuery(sql, param, "orangebear");
        }

        public static bool exchage_pos_vip(string VipID, string NewVipID)
        {
            bool MemberDataChanged = false;

            //舊訂單和會員資料異動
            string sql = "update pos_vip set vip_id=@NewVipID where vip_id=@VipID; ";
            sql += "update XPPOS.[dbo].Transation set VipNo=@NewVipID  where VipNo=@VipID ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("VipID", VipID);
            param.Add("NewVipID", NewVipID);
            MemberDataChanged=DB.DBNonQuery(sql, param, "orangebear");

            //新訂單異動
            sql = "update PosClient..OrderVip set VIP=@NewVipID where VIP=@VipID ";
            DB.DBNonQuery(sql, param, "PosClient");

            return MemberDataChanged;
        }

        public static bool CancelVIP(string vip_id)
        {
            string sql = "update orangebear..pos_vip set mobile ='' ";
            sql += "where vip_id=@vip_id ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("vip_id", vip_id);
            return DB.DBNonQuery(sql, param, "orangebear");
        }

        public static bool CheckHasTheSameVIP(string mobile)
        {
            DataTable dt = get_pos_vip_by_mobile(mobile);
            if (dt.Rows.Count > 0)
            {
                DataView dv = new DataView(dt);
                dv.RowFilter = string.Format("valid_date>='{0}' ", DateTime.Now.ToString("yyyy/MM/dd"));
                dt = dv.ToTable();
            }
            return dt.Rows.Count > 0 ? true : false;
        }

        public static DataTable GetOrderByVIP(string vip_id)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select a.OrderID,a.Amount,iif(a.PayType=1,'現金','信用卡') PayType,a.PosNo,a.OrderTime,b.BonusUsed,d.InvoiceNo,e.account from PosClient..Orders a   ");
            sb.AppendLine("join PosClient..OrderVip b on a.OrderID=b.OrderID ");
            sb.AppendLine("left join(Select ROW_NUMBER() OVER(PARTITION BY c.OrderID Order BY c.InvoiceNo) AS RowNo,c.InvoiceNo,c.OrderID from PosClient..OrderInvoices c) d on a.OrderID=d.OrderID and d.RowNo=1 ");
            sb.AppendLine("left join  PosClient..物流人員 e on a.ClerkID=e.id ");
            sb.AppendLine("where b.VIP=@vip_id and a.Status=1 ");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("vip_id", vip_id);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static DataTable GetOrderItemByOrderID(string OrderID)
        {
            string sql = "select a.*,b.BarCode, c.Name from  OrderItems a join Product b on a.ProductId=b.ProductId join ProductSerial c on b.SerialId=c.SerialId ";
            sql += "where a.OrderID=@OrderID  ";

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("OrderID", OrderID);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sql, param, "PosClient");
            return dt;
        }

        public static int FindLastYearTotalAmountByVip(string vip_id, string valid_date)
        {
            int OldOrder = 0;
            int NewOrder = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT ISNULL(SUM(a.TrAmount),0) AS TOTAL   ");
            sb.AppendLine("FROM  XPPOS..Transation a  ");
            sb.AppendLine("where a.VipNo=@vip_id and a.TrDate >=DATEADD(d, -366, @valid_date)  ");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("vip_id", vip_id);
            param.Add("valid_date", valid_date);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sql, param, "orangebear");

            if (dt.Rows.Count > 0)
            {
                OldOrder = int.Parse(dt.Rows[0]["TOTAL"].ToString());
            }

            sb.Clear();
            //只看自己門市
            //sb.AppendLine("SELECT  ISNULL(SUM(a.Amount),0) AS TOTAL     ");
            //sb.AppendLine("from PosClient..Orders a  ");
            //sb.AppendLine("left join PosClient..OrderVip b on a.OrderID=b.OrderID ");
            //sb.AppendLine("where a.OrderTime>DATEADD(d, -365, @valid_date)  and a.Status=1 and b.VIP=@vip_id ");
            //sql = sb.ToString();
            //dt = DB.DBQuery(sql, param, "PosClient");

            //所有門市，但是會少今天的訂單，需等同步
            sb.AppendLine("SELECT  ISNULL(SUM(a.Amount),0) AS TOTAL ");
            sb.AppendLine("from PosManage..OrderM a  ");
            sb.AppendLine("left join PosManage..OrderVip b on a.OrderNo=b.OrderNo ");
            sb.AppendLine("where a.OrderTime>DATEADD(d, -365, @valid_date)  and a.Status=1 and b.VIP=@vip_id ");
            sql = sb.ToString();
            dt = DB.DBQuery(sql, param, "orangebear");

            if (dt.Rows.Count > 0)
            {
                NewOrder = int.Parse(dt.Rows[0]["TOTAL"].ToString());
            }

            return OldOrder + NewOrder;
        }

        public static bool VipExtend(string vip_id)
        {
            string sql = "UPDATE  pos_vip SET  valid_date=DateAdd(\"D\",365,valid_date)  ";
            sql += "where vip_id=@vip_id   ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("vip_id", vip_id);
            return DB.DBNonQuery(sql, param, "orangebear");
        }


    }

   public class MemberData
   {
       public string vip_id;
       public string name;
       public string tel;
       public string mobile;
       public string email;
       public int bonus;
       public string birthday;
       public string last_birthday;
       public string valid_date;
       public string create_date;
       public string status;
       public double discount;
       public bool birthday_discount;
       public bool valid;
   }
}