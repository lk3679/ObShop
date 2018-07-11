using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;
using Newtonsoft.Json;
using System.Text;

namespace OBShopWeb
{
    public partial class pos_transaction : System.Web.UI.Page
    {
        public DateTime currentTime = DateTime.Now;
        public DataTable OrderList;
        public string act = "";
        public string OrderID;
        public string StartDate;
        public string EndDate;
        public string ShowNullifiedInvoices;
        public string PosNo;
        public string ClerkID;
        public string  PosList;
        public DataTable AllStaff=new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            HttpPostCheck();

            switch (act)
            {
                case "Query":
                    QueryOrder();
                    break;
                case "GetOrderItem":
                    GetOrderItem(OrderID);
                    break;
                default:
                    DataTable dt = PosNumber.GetAllPosNo();
                PosList = JsonConvert.SerializeObject(dt.AsEnumerable().Select(r => new { PosNo = r["PosNo"] }).ToList());
                AllStaff = GetAllStaff();
                break;
            }
            
        }

        public void QueryOrder()
        {
            DataTable OrderDT = GetAllOrderListByDate();
            var OrderList = OrderDT.AsEnumerable().Select(r => new
            {
                OrderID = r["OrderID"],
                PayType = (int)r["PayType"],
                Status = (int)r["Status"],
                Amount = (int)r["Amount"],
                AuthorizationCode = r["AuthorizationCode"],
                ClerkID = r["ClerkID"],
                Name = r["Name"],
                PosNo = r["PosNo"],
                OrderDate = r["OrderDate"],
            }).ToList();


            if (OrderID != "")
            {
                OrderList = (from a in OrderList
                             where a.OrderID.ToString() == OrderID
                             select a).ToList();
            }
            if (PosNo != "")
            {
                OrderList = (from a in OrderList
                             where a.PosNo.ToString() == PosNo
                             select a).ToList();
            }
            if (ClerkID != "")
            {
                OrderList = (from a in OrderList
                             where a.ClerkID.ToString()== ClerkID
                             select a).ToList();
            }
            if (ShowNullifiedInvoices == "true")
            {
                OrderList = (from a in OrderList
                             where a.Status == 2 || a.Status == 6
                             select a).ToList();
            }

            int TotalAmount = (from a in OrderList
                               where a.Status== 1
                               select a).Sum(x=>(int)x.Amount);

            int TotalCash = (from a in OrderList
                             where a.Status == 1 && a.PayType == 1
                             select a).Sum(x => (int)x.Amount);

            int TotalCredit = (from a in OrderList
                               where a.Status==1 && a.PayType==2
                             select a).Sum(x => (int)x.Amount);

            int FailedTotalAmount = (from a in OrderList
                                     where a.Status==2|| a.Status==6
                                     select a).Sum(x => (int)x.Amount);

            int FailedTotalCash = (from a in OrderList
                                   where (a.Status == 2 || a.Status == 6) && a.PayType == 1
                                   select a).Sum(x => (int)x.Amount);

            int FailedTotalCredit = (from a in OrderList
                                     where (a.Status == 2 || a.Status == 6) && a.PayType == 2
                                     select a).Sum(x => (int)x.Amount);


            string html = "";

            foreach (var x in OrderList)
            {
                string OrderStatus = "";
                string InvoiceList="";

                switch (x.Status)
                {
                    case 2:
                        OrderStatus = "作廢";
                        break;
                    case 3:
                        OrderStatus = "待結";
                        break;
                    case 4:
                        OrderStatus = "系統自動取消";
                        break;
                    case 5:
                        OrderStatus = "手動取消";
                        break;
                    case 6:
                        OrderStatus = "折讓";
                        break;
                    default:
                        OrderStatus = "";
                        break;
                }

                InvoiceList=GetInvoiceListByOrderID(x.OrderID.ToString());

                html += "<tr class=\"OrderItem\">";
                html += string.Format("<td style=\"width: 10%;\">{0}</td>", x.OrderID);
                html += string.Format("<td style=\"width: 20%;\"><a href=\"javascript:void(0)\" class=\"OrderID\" data-OrderID=\"{0}\">交易明細</a></td>", x.OrderID);
                html += string.Format("<td style=\"width: 6%;\">{0}</td>", x.AuthorizationCode);
                html += string.Format("<td style=\"width: 10%;\">{0}</td>", x.PayType.ToString() == "1" ? "現金" : "刷卡");
                html += string.Format("<td style=\"width: 10%;\">{0}</td>", x.Amount);
                html += string.Format("<td style=\"width: 10%;\">{0}</td>", InvoiceList);
                html += string.Format("<td style=\"width: 6%;\">{0}</td>", OrderStatus);
                html += string.Format("<td style=\"width: 6%;\">{0}</td>", x.PosNo);
                html += string.Format("<td style=\"width: 20%;\">{0}</td>", x.Name);
                html += "</tr>";
            }

            if (ShowNullifiedInvoices != "true")
            {
                html += AddAccountInfo("現金銷售", TotalCash);
                html += AddAccountInfo("信用卡銷售", TotalCredit);
                html += AddAccountInfo("銷售金額總計", TotalAmount);
            }
            html += AddAccountInfo("現金作廢", FailedTotalCash);
            html += AddAccountInfo("信用卡作廢", FailedTotalCredit);
            html += AddAccountInfo("作廢金額總計", FailedTotalAmount);

            ShowResultOnPage(JsonConvert.SerializeObject(new { OrderList = html }));
        }

        public void HttpPostCheck()
        {
            act = (!string.IsNullOrEmpty(Request["act"])) ? Request["act"] : "";
            OrderID = (!string.IsNullOrEmpty(Request["OrderID"])) ? Request["OrderID"] : "";
            StartDate = (!string.IsNullOrEmpty(Request["StartDate"])) ? Request["StartDate"] : "";
            EndDate = (!string.IsNullOrEmpty(Request["EndDate"])) ? Request["EndDate"] : "";
            ShowNullifiedInvoices = (!string.IsNullOrEmpty(Request["ShowNullifiedInvoices"])) ? Request["ShowNullifiedInvoices"] : "";
            PosNo = (!string.IsNullOrEmpty(Request["PosNo"])) ? Request["PosNo"] : "";
            ClerkID = (!string.IsNullOrEmpty(Request["ClerkID"])) ? Request["ClerkID"] : "";

        }

        public void ShowResultOnPage(string JsonResult)
        {
            Response.Clear();
            Response.Write(JsonResult);
            Response.Flush();
            Response.End();
        }

        public string AddAccountInfo(string Name, int Amount)
        {
            string html = "";
            html += "<tr class=\"OrderItem\">";
            html += string.Format("<td style=\"width: 10%;\"></td>");
            html += string.Format("<td style=\"width: 20%;\">");
            html += string.Format("<td style=\"width: 6%;\"></td>");
            html += string.Format("<td style=\"width: 10%;\">{0}</td>", Name);
            html += string.Format("<td style=\"width: 10%;\">{0}</td>", Amount);
            html += string.Format("<td style=\"width: 10%;\"></td>");
            html += string.Format("<td style=\"width: 6%;\"></td>");
            html += string.Format("<td style=\"width: 6%;\"></td>");
            html += string.Format("<td style=\"width: 20%;\"></td>");
            html += "</tr>";

            return html;
        }

        public static DataTable GetAllStaff()
        {
            string sql = "SELECT  [id] ,[barcode],[zone] ,[account] FROM [PosClient].[dbo].[物流人員]";
            Dictionary<string, object> param = new Dictionary<string, object>();
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            return dt;
        }

        public DataTable GetAllOrderListByDate()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select a.OrderID,a.PayType,a.AuthorizationCode,a.ClerkID,a.Amount,a.Status,a.PosNo,a.OrderDate,c.account Name from [PosClient].[dbo].[Orders] a  ");
            sb.Append("left join [PosClient].[dbo].物流人員 c on a.ClerkID=c.id   ");
            sb.Append("where a.OrderDate>=@StartDate and a.OrderDate<=@EndDate ");
            sb.Append("order by orderID desc ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("StartDate", StartDate);
            param.Add("EndDate", EndDate);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        public DataTable GetOrderItemByOrderID(string OrderID)
        {
            string sql = "select a.*,b.BarCode, c.Name from  OrderItems a join Product b on a.ProductId=b.ProductId join ProductSerial c on b.SerialId=c.SerialId where a.OrderID=@OrderID  ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("OrderID", OrderID);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sql, param, "PosClient");
            return dt;
        }

        public string GetInvoiceListByOrderID(string OrderID)
        {
            string InvoiceList="";
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT [InvoiceNo] FROM [PosClient].[dbo].[OrderInvoices] a where a.OrderID=@OrderID ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("OrderID", OrderID);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            foreach (DataRow dr in dt.Rows)
                InvoiceList += dr["InvoiceNo"].ToString() + "<br/>";

            return InvoiceList;
        }

        public void GetOrderItem(string OrderID)
        {
            DataTable dt = GetOrderItemByOrderID(OrderID);
            string HtmlTag = "<div class=\"BIGpro_infoDT\" style=\"width:100%;\" ><h2>交易明細</h2><table style=\"width:100%;\" ><tbody>";
            HtmlTag += "<tr><td style=\"width: 10%;\"></td><td style=\"width: 10%;\">交易序號</td><td style=\"width: 10%;\">產品編號</td><td style=\"width: 10%;\">產品條碼</td><td style=\"width: 20%;\">產品名稱</td><td style=\"width: 10%;\">件數</td><td style=\"width: 10%;\">價格</td><td style=\"width: 10%;\">小計</td></tr>";
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                i++;
                HtmlTag += "<tr>";
                HtmlTag += "<td style=\"width: 10%;\">" + i + "</td>";
                HtmlTag += "<td style=\"width: 10%;\">" + dr["OrderID"].ToString() + "</td>";
                HtmlTag += "<td style=\"width: 10%;\">" + dr["ProductId"].ToString() + "</td>";
                HtmlTag += "<td style=\"width: 10%;\">" + dr["BarCode"].ToString() + "</td>";
                HtmlTag += "<td style=\"width: 20%;\">" + dr["Name"].ToString() + "</td>";
                HtmlTag += "<td style=\"width: 10%;\">" + dr["Quantity"].ToString() + "</td>";
                HtmlTag += "<td style=\"width: 10%;\">" + dr["Price"].ToString() + "</td>";
                HtmlTag += "<td style=\"width: 10%;\">" + dr["Amount"].ToString() + "</td>";
                HtmlTag += "</tr>";
            }
            HtmlTag += "</tbody></table></div>";

            ShowResultOnPage(JsonConvert.SerializeObject(new { result = dt.Rows.Count>0?true:false, data = HtmlTag }));
        }
    }
}