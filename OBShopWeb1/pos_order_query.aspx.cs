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
    public partial class pos_order_query : System.Web.UI.Page
    {
        public DataTable OrderDT = new DataTable();
        public DataTable OrderItemDT = new DataTable();
        public DataTable FailedOrderItemDT = new DataTable();
        public int FirstYear = 2014;
        public string startYear = "";
        public string startMonth = "";
        public string startDay = "";
        public string endYear = "";
        public string endMonth = "";
        public string endDay = "";
        public DateTime currentTime = DateTime.Now;
        public int TotalAmount = 0;
        public int TotalCash = 0;
        public int TotalCredit = 0;
        public int FailedTotalAmount = 0;
        public int FailedTotalCash = 0;
        public int FailedTotalCredit = 0;
        public int SaleNum;
        public int FailedNum;
        public CheckOut.Clerk ck = new CheckOut.Clerk();

        protected void Page_Load(object sender, EventArgs e)
        {
            string act = (string.IsNullOrEmpty(Request["act"])) ? "" : Request["act"];
            string thisYear = currentTime.ToString("yyyy");
            string thisMonth = currentTime.ToString("MM");
            string thisDay = currentTime.ToString("dd");
            startYear = (!string.IsNullOrEmpty(Request["startYear"])) ? Request["startYear"] : thisYear;
            startMonth = (!string.IsNullOrEmpty(Request["startMonth"])) ? Request["startMonth"] : thisMonth;
            startDay = (!string.IsNullOrEmpty(Request["startDay"])) ? Request["startDay"] : thisDay;
            endYear = (!string.IsNullOrEmpty(Request["endYear"])) ? Request["endYear"] : thisYear;
            endMonth = (!string.IsNullOrEmpty(Request["endMonth"])) ? Request["endMonth"] : thisMonth;
            endDay = (!string.IsNullOrEmpty(Request["endDay"])) ? Request["endDay"] : thisDay;
            string QueryType = (!string.IsNullOrEmpty(Request["QueryType"])) ? Request["QueryType"] : "";

            if (QueryType == "訂單資料查詢" || QueryType == "")
            {
                LoadOrderData();
            }
            else
            {
                DateFormating();
                DateTime start_date = new DateTime(int.Parse(startYear), int.Parse(startMonth), int.Parse(startDay));
                DateTime end_date = new DateTime(int.Parse(endYear), int.Parse(endMonth), int.Parse(endDay), 23, 59, 59);
                OrderDT = Order.GetReturnOrderByDate(start_date, end_date);
                FailedOrderItemDT = Order.GetReturnItemByDate(start_date, end_date);

                var Orderlist = OrderDT.AsEnumerable().Select(r => new { OrderID = r["OrderID"], Amount = (int)r["Amount"], PayType = (int)r["PayType"], Status = (int)r["Status"] }).Distinct();
                var FailedOrderItemList = FailedOrderItemDT.AsEnumerable().Select(r => new { OrderID = r["OrderID"], Quantity = (int)r["Quantity"] });
                FailedTotalAmount = (from a in Orderlist
                                     where a.OrderID.ToString() != "0"
                                     select a).Sum(x => x.Amount);

                FailedTotalCash = (from a in Orderlist
                                   where a.OrderID.ToString() != "0" && a.PayType == 1
                                   select a).Sum(x => x.Amount);

                FailedTotalCredit = (from a in Orderlist
                                     where a.OrderID.ToString() != "0" && a.PayType == 2
                                     select a).Sum(x => x.Amount);

                FailedNum = (from a in FailedOrderItemList
                             where a.OrderID.ToString() != "0"
                             select a).Sum(x => x.Quantity);

            }


            if (act == "GetOrderItem")
            {
                DataTable dt = Order.GetOrderItemByOrderID(Request["OrderID"]);
                string OrderItem = Order.GetOrderItemString(dt);
                var result = new { result = dt.Rows.Count > 0, data = OrderItem };
                ShowResultOnPage(JsonConvert.SerializeObject(result));
            }

            if (act == "CancelOrders")
            {
                string ErrorMsg = "";
                bool status = false;
                if (Auth())
                {
                    string IPaddress = Request.UserHostAddress;
                    string PosNo = PosNumber.GetPosNumberMapping(IPaddress);
                    status = Order.CancelOrders(Request["OrderID"], PosNo, ck.ID);
                    if (status)
                    { ErrorMsg = "新增成功"; }
                    else
                    { ErrorMsg = "新增失敗"; }
                }
                else
                {
                    ErrorMsg = "尚未登入";
                }
                var result = new { result = status, ErrorMsg = ErrorMsg };
                ShowResultOnPage(JsonConvert.SerializeObject(result));
            }
        }

        public void ShowResultOnPage(string JsonResult)
        {
            Response.Clear();
            Response.Write(JsonResult);
            Response.Flush();
            Response.End();
        }

        public void LoadOrderData()
        {

            if (startYear != "" && startMonth != "" && startDay != "" && endYear != "" && endMonth != "" && endDay != "")
            {
                DateFormating();

                DateTime start_date = new DateTime(int.Parse(startYear), int.Parse(startMonth), int.Parse(startDay));
                DateTime end_date = new DateTime(int.Parse(endYear), int.Parse(endMonth), int.Parse(endDay), 23, 59, 59);
                OrderDT = Order.GetAllOrderListByDate(start_date, end_date);
                OrderItemDT = Order.GetSaleItemByDate(start_date, end_date, 1);
                FailedOrderItemDT = Order.GetSaleItemByDate(start_date, end_date, 2);
            }

            var Orderlist = OrderDT.AsEnumerable().Select(r => new
            {
                OrderID = r["OrderID"],
                Amount = (int)r["Amount"],
                PayType = (int)r["PayType"],
                Status = (int)r["Status"],
                PosNo = r["PosNo"],
                Name = r["Name"],
                OrderTime = r["OrderTime"]
            });

            var OrderItemList = OrderItemDT.AsEnumerable().Select(r => new { Quantity = (int)r["Quantity"] }).ToList();
            var FailedOrderItemList = FailedOrderItemDT.AsEnumerable().Select(r => new { Quantity = (int)r["Quantity"] }).ToList();

            SaleNum = OrderItemList.Sum(x => x.Quantity);
            FailedNum = FailedOrderItemList.Sum(x => x.Quantity);

            foreach (var x in Orderlist)
            {
                if (x.Status == 1)
                {
                    TotalAmount += x.Amount;
                    if (x.PayType == 1)
                        TotalCash += x.Amount;
                    else
                        TotalCredit += x.Amount;
                }

                if (x.Status == 2 || x.Status == 6)
                {
                    FailedTotalAmount += x.Amount;
                    if (x.PayType == 1)
                        FailedTotalCash += x.Amount;
                    else
                        FailedTotalCredit += x.Amount;
                }

            }
        }

        public bool Auth()
        {
            if (Session["Account"] == null)
            {
                return false;
            }
            else
            {
                if (Session["ClerkID"] == null)
                {
                    return false;
                }
                else
                {
                    ck.ID = Session["ClerkID"].ToString();
                    ck.Name = Session["Name"].ToString();
                    return true;
                }
            }
        }

        public string GetInvoiceList(string InvoiceList)
        {
            string InvoiceListString = "";
            string[] List = InvoiceList.Trim(',').Split(',');

            foreach (string Invoice in List)
                InvoiceListString += Invoice + "<br/>";

            return InvoiceListString;
        }

        public void DateFormating()
        {
            if (startDay == "31" || (int.Parse(startMonth) == 2 && int.Parse(startDay) > 28))
                startDay = DateTime.DaysInMonth(int.Parse(startYear), int.Parse(startMonth)).ToString();

            if (endDay == "31" || (int.Parse(startMonth) == 2 && int.Parse(startDay) > 28))
                endDay = DateTime.DaysInMonth(int.Parse(endYear), int.Parse(endMonth)).ToString();
        }

    }
}