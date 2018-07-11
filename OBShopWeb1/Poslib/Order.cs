using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace OBShopWeb.Poslib
{
    public class Order
    {
        public class FailInvoice
        {
            public string InvoiceNo;
            public string InvoiceNoAmount;
            public int RowNo;
        }

        public static DataTable GetOrderItemByDate(string Date, string PosNo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select b.Quantity,a.OrderTime,j.StartingNumber,j.EndingNumber from orders a  ");
            sb.AppendLine("join OrderItems b on a.OrderID=b.OrderID ");
            sb.AppendLine("left join PosClient..OrderInvoices c on a.OrderID=c.OrderID ");
            sb.AppendLine("left join PosClient..InvoiceRolls as j on j.AlphabeticCode = SUBSTRING(c.InvoiceNo, 1, 2) and SUBSTRING(c.InvoiceNo, 3, 8)  ");
            sb.AppendLine("between j.StartingNumber and j.EndingNumber  ");
            sb.AppendLine("where a.OrderDate=@Date ");

            if (PosNo != "All")
                sb.AppendLine("and a.PosNo=@PosNo ");

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Date", Date);
            param.Add("PosNo", PosNo);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static DataTable GetOrderItemByMonth(string Year, string Month, string PosNo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select b.Quantity,a.OrderTime,j.StartingNumber,j.EndingNumber from orders a  ");
            sb.AppendLine("join OrderItems b on a.OrderID=b.OrderID ");
            sb.AppendLine("left join PosClient..OrderInvoices c on a.OrderID=c.OrderID ");
            sb.AppendLine("left join PosClient..InvoiceRolls as j on j.AlphabeticCode = SUBSTRING(c.InvoiceNo, 1, 2) and SUBSTRING(c.InvoiceNo, 3, 8)  ");
            sb.AppendLine("between j.StartingNumber and j.EndingNumber  ");
            sb.AppendLine("where month(a.OrderDate)=@Month and YEAR(a.OrderDate)=@Year ");

            if (PosNo != "All")
                sb.AppendLine("and a.PosNo=@PosNo ");

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PosNo", PosNo);
            param.Add("Year", Year);
            param.Add("Month", Month);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }


        public static DataTable GetOrderItemByDate(string Date, string PosNo,string StartNo,string EndNo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select b.Quantity,a.OrderTime,j.StartingNumber,j.EndingNumber from orders a  ");
            sb.AppendLine("join OrderItems b on a.OrderID=b.OrderID ");
            sb.AppendLine("left join PosClient..OrderInvoices c on a.OrderID=c.OrderID ");
            sb.AppendLine("left join PosClient..InvoiceRolls as j on j.AlphabeticCode = SUBSTRING(c.InvoiceNo, 1, 2) and SUBSTRING(c.InvoiceNo, 3, 8)  ");
            sb.AppendLine("between j.StartingNumber and j.EndingNumber  ");
            sb.AppendLine("where a.OrderDate=@Date and j.StartingNumber=@StartNo and j.EndingNumber=@EndNo ");

            if (PosNo != "All")
                sb.AppendLine("and a.PosNo=@PosNo ");

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Date", Date);
            param.Add("PosNo", PosNo);
            param.Add("StartNo", StartNo);
            param.Add("EndNo", EndNo);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static DataTable GetReturnOrderDTByDate(string Date, string PosNo)
        {
            string today = Date;
            string Tomorrow = (DateTime.Parse(Date).AddDays(1)).ToString("yyyy-MM-dd");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select a.OrderID,a.PosNo,a.ReturnTime,a.Type,b.Amount,b.PayType from Returns a  ");
            sb.AppendLine("join Orders b on a.OrderID=b.OrderID ");
            sb.AppendLine("where a.ReturnTime between @today   and @Tomorrow and a.PosNo=@PosNo");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("today ", today);
            param.Add("Tomorrow", Tomorrow);
            param.Add("PosNo", PosNo);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static DataTable GetOrderMapingInvoiceRollByDate(string Date)
        {
            StringBuilder sb = OrderMappingInvoiceRoll();
            sb.AppendLine(" where k.OrderDate=@Date ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Date", Date);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static DataTable GetOrderMapingInvoiceRollByMonth(string Year,string Month)
        {
            StringBuilder sb = OrderMappingInvoiceRoll();
            sb.AppendLine(" where month(k.OrderDate)=@Month and YEAR(k.OrderDate)=@Year ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Year", Year);
            param.Add("Month", Month);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static  StringBuilder OrderMappingInvoiceRoll()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select ROW_NUMBER() OVER(PARTITION BY i.OrderID Order BY i.InvoiceNo) AS RowNo, i.*, j.*,k.Amount,k.Status,k.PayType from PosClient..OrderInvoices as i ");
            sb.AppendLine("left join PosClient..InvoiceRolls as j  ");
            sb.AppendLine("on j.AlphabeticCode = SUBSTRING(i.InvoiceNo, 1, 2) ");
            sb.AppendLine("and SUBSTRING(i.InvoiceNo, 3, 8) between j.StartingNumber and j.EndingNumber ");
            sb.AppendLine("left join PosClient..Orders k on i.OrderID=k.OrderID and k.Status<3 ");
            return sb;
        }


        public static DataTable GetFailInvoiceRollByDate(string Date)
        {
            string today = Date;
            string Tomorrow = (DateTime.Parse(Date).AddDays(1)).ToString("yyyy-MM-dd");
            StringBuilder sb = GetFailInvoiceRoll();
            sb.AppendLine("where a.NullifiedTime>=@today and a.NullifiedTime<@Tomorrow ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("today", today);
            param.Add("Tomorrow", Tomorrow);
            DataTable dt = new DataTable();
            string sql = sb.ToString();
            dt = DB.DBQuery(sql, param, "PosClient");
            return dt;
        }

        public static DataTable GetFailInvoiceRollByMonth(string Year, string Month)
        {
            StringBuilder sb = GetFailInvoiceRoll();
            sb.AppendLine("where month(a.NullifiedTime)=@Month and YEAR(a.NullifiedTime)=@Year ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Year", Year);
            param.Add("Month", Month);
            DataTable dt = new DataTable();
            string sql = sb.ToString();
            dt = DB.DBQuery(sql, param, "PosClient");
            return dt;
        }

        public static StringBuilder GetFailInvoiceRoll()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select ROW_NUMBER() OVER(PARTITION BY c.OrderID Order BY a.InvoiceNo) AS RowNo,a.*,b.StartingNumber,b.EndingNumber,isnull(d.Amount,0) Amount,isnull(d.PayType,0) PayType, isnull(d.OrderID,0) OrderID,isnull(d.Status,0) Status");
            sb.AppendLine("from PosClient..NullifiedInvoices a  ");
            sb.AppendLine("left join PosClient..InvoiceRolls b on b.AlphabeticCode= SUBSTRING(a.InvoiceNo, 1, 2)  ");
            sb.AppendLine("and SUBSTRING(a.InvoiceNo, 3, 8) between b.StartingNumber and b.EndingNumber ");
            sb.AppendLine("left join PosClient..OrderInvoices c on a.InvoiceNo=c.InvoiceNo ");
            sb.AppendLine("left join PosClient..Orders d on c.OrderID=d.OrderID ");
            return sb;
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

        public static DataTable GetSaleItemByDate(DateTime StartDate, DateTime EndDate, int Status)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT b.*FROM [PosClient].[dbo].[Orders] a ");
            sb.Append(" left join [PosClient]..OrderItems b on a.OrderID=b.OrderID ");
            sb.Append("where a.OrderDate>=@StartDate and a.OrderDate<=@EndDate  and a.Status=@Status ");

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("StartDate", StartDate);
            param.Add("EndDate", EndDate);
            param.Add("Status", Status);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static DataTable GetReturnItemByDate(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.ReturnTime OrderTime,a.PosNo,isNull(b.Amount,0) Amount,isNull(b.PayType,0) PayType,isNull(b.Status,0) Status,isNull(a.OrderID,'') OrderID,isNull(c.Quantity,0) Quantity ");
            sb.Append("from PosClient..Returns a  ");
            sb.Append("left join PosClient..Orders b on a.OrderID=b.OrderID ");
            sb.Append("left join PosClient..OrderItems c on a.OrderID=c.OrderID ");
            sb.Append("where a.ReturnTime>=@StartDate and a.ReturnTime<=@EndDate ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("StartDate", StartDate);
            param.Add("EndDate", EndDate);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static DataTable GetAllOrderListByDate(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Select isnull(y.VIP,'') VipNo,a.*,c.account Name,b.InvoiceList  from PosClient..Orders a  ");
            sb.AppendLine("left join (  ");
            sb.AppendLine("select b.OrderID,(select a.InvoiceNo+','  ");
            sb.AppendLine("from PosClient..OrderInvoices a  ");
            sb.AppendLine("where a.OrderID=b.OrderID for xml path('')) InvoiceList  ");
            sb.AppendLine("from PosClient..OrderInvoices b ");
            sb.AppendLine("group by b.OrderID ) b on a.OrderID=b.OrderID ");
            sb.AppendLine("left join 物流人員 c on a.ClerkID=c.id ");
            sb.AppendLine(" left join OrderVip y on a.OrderID=y.OrderID ");
            sb.AppendLine("where a.OrderDate>=@StartDate and a.OrderDate<=@EndDate ");
            sb.AppendLine("order by orderID desc ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("StartDate", StartDate);
            param.Add("EndDate", EndDate);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static DataTable GetReturnOrderByDate(DateTime StartDate, DateTime EndDate)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT isnull(e.VIP,'') VipNo, a.ReturnTime OrderTime,a.PosNo,isNull(b.Amount,0) Amount,isNull(b.PayType,0) PayType,d.account Name,isNull(b.Status,0) Status,isNull(a.OrderID,'') OrderID,c.InvoiceList   ");
            sb.Append("from PosClient..Returns a   ");
            sb.Append("left join PosClient..Orders b on a.OrderID=b.OrderID ");
            sb.Append("left join (  ");
            sb.Append("select b.OrderID,(select a.InvoiceNo+','  ");
            sb.Append("from PosClient..OrderInvoices a  ");
            sb.Append("where a.OrderID=b.OrderID for xml path('')) InvoiceList  ");
            sb.Append("from PosClient..OrderInvoices b ");
            sb.Append("group by b.OrderID ) c on b.OrderID=c.OrderID ");
            sb.Append("left join PosClient..物流人員 d on a.ClerkID=d.id ");
            sb.Append("left join PosClient..OrderVip e on a.OrderID=e.OrderID  ");
            sb.Append("where a.ReturnTime>=@StartDate  and a.ReturnTime<=@EndDate ");
            sb.Append("order by a.ReturnTime desc ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            DataTable dt = new DataTable();
            param.Add("StartDate", StartDate);
            param.Add("EndDate", EndDate);
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public static string GetOrderItemString(DataTable dt)
        {
            string HtmlTag = "<div id=\"content\"><h2>交易明細</h2><table class=\"EU_DataTable\"><tbody>";
            HtmlTag += "<tr><th></th><th>交易序號</th><th>產品編號</th><th>產品條碼</th><th>產品名稱</th><th>件數</th><th>價格</th><th>小計</th></tr>";
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                HtmlTag += "<tr>";
                HtmlTag += "<td>" + i + "</td>";
                HtmlTag += "<td>" + dr["OrderID"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["ProductId"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["BarCode"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["Name"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["Quantity"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["Price"].ToString() + "</td>";
                HtmlTag += "<td>" + dr["Amount"].ToString() + "</td>";
                HtmlTag += "</tr>";
            }
            HtmlTag += "</tbody></table></div>";
            return HtmlTag;
        }

        public static bool CancelOrders(string OrderID, string PosNo, string ClerkID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" Update PosClient..Orders set Status=5,SyncedTime=null where OrderID=@OrderID ");
            sb.Append(" Delete from PosClient..OrderInvoices where OrderID=@OrderID  ");
            sb.Append(" Insert into PosClient..CanceledOrders (OrderID,PosNo,ClerkID,CancelTime) values(@OrderID,@PosNo,@ClerkID,getdate())");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("OrderID", OrderID);
            param.Add("PosNo", PosNo);
            param.Add("ClerkID", ClerkID);
            return DB.DBNonQuery(sb.ToString(), param, "PosClient");
        }
    }
}