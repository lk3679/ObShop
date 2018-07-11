using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;
using System.Data;
using System.Web.Configuration;
namespace OBShopWeb
{
    public partial class day_end_report : System.Web.UI.Page
    {
        public string Date = "";
        public string StoreNo = "";
        public string PosNo = "9";
        public string StartOrderID="";
        public string EndOrderID="";
        public string StartInvoiceNo="";
        public string EndInvoiceNo="";
        public int TotalOrderNum = 0;
        public int TotalOrderAmount = 0;
        public int TotalQuantity = 0;
        public int TotalCashOrderNum = 0;
        public int TotalCardOrderNum = 0;
        public int CashIncome = 0;
        public int CardIncome = 0;
        public int ReturnTotalAmount = 0;
        public int ReturnTotalCashAmount = 0;
        public int ReturnTotalCardAmount = 0;
        public CheckOut.Clerk ck = new CheckOut.Clerk();
        public  List<Order.FailInvoice> FailCashInvoiceList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> FailCardInvoiceList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> FailOhterInvoiceList = new List<Order.FailInvoice>();
        public int FailInvoiceNum = 0;
        public int FailOrderNum = 0;
        public string StartNo = "";
        public string EndNo = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["ClerkName"]))
                ck.Name = Request["ClerkName"];
            else
                Auth();

            if (string.IsNullOrEmpty(Request["Date"]))
                 Date= DateTime.Now.ToString("yyyy/MM/dd");
            else
                Date = Request["Date"];
            
            PosNoBiding();
            if (!string.IsNullOrEmpty(Request["PosNo"]))
            { PosNo = Request["PosNo"]; }

            LoadOrderData();
        }

        public void LoadOrderData()
        {
            if(!string.IsNullOrEmpty(Request["StartNo"])) 
                StartNo=Request["StartNo"];

            if (!string.IsNullOrEmpty(Request["EndNo"]))
                EndNo = Request["EndNo"];

            if (StartNo == "" || EndNo == "" || PosNo=="")
            {return;}

            int _areaId = int.Parse(Area.WmsAreaXml("Area"));
            StoreNo = _areaId.ToString("D3");
            DataTable OrderDT = Order.GetOrderMapingInvoiceRollByDate(Date);
            DataTable OrderItemDT = Order.GetOrderItemByDate(Date, PosNo, StartNo, EndNo);
            DataTable FailInvoiceDT = Order.GetFailInvoiceRollByDate(Date);

            var OrderList = OrderDT.AsEnumerable().Select(r => new
            {
                OrderID = r["OrderID"],
                InvoiceNo = r["InvoiceNo"],
                Amount = (int)r["Amount"],
                PayType = (int)r["PayType"],
                StartingNumber = r["StartingNumber"],
                EndingNumber = r["EndingNumber"],
                PosNo = r["PosNo"],
            }).ToList();

            var FailInvoiceOrderList = FailInvoiceDT.AsEnumerable().Select(r => new
            {
                OrderID = r["OrderID"],
                InvoiceNo = r["InvoiceNo"],
                Amount = (int)r["Amount"],
                PayType = (int)r["PayType"],
                StartingNumber = r["StartingNumber"],
                EndingNumber = r["EndingNumber"],
                PosNo = r["PosNo"],
            }).ToList();


            var OrderListByStartNo = (from a in OrderList
                                      where (PosNo != "All" ? a.PosNo.ToString() == PosNo : true) && a.StartingNumber.ToString() == StartNo
                                      select a).ToList();

            var OrderListCash = (from a in OrderListByStartNo
                                 where a.PayType.ToString() == "1"
                                 select new { OrderID = a.OrderID, Amount = a.Amount }).Distinct().ToList();

            var OrderListCreidt = (from a in OrderListByStartNo
                                   where a.PayType.ToString() == "2"
                                   select new { OrderID = a.OrderID, Amount = a.Amount }).Distinct().ToList();


            var FailInvoiceOrderListByStartNo = (from a in FailInvoiceOrderList
                                                 where (PosNo != "All" ? a.PosNo.ToString() == PosNo : true) && a.StartingNumber.ToString() == StartNo
                                                 select new { OrderID = a.OrderID, Amount = (int)a.Amount, PayType = (int)a.PayType }).Distinct().ToList();

            var FailAllInvoiceListByStartNo = (from a in FailInvoiceOrderList
                                               where (PosNo != "All" ? a.PosNo.ToString() == PosNo : true) && a.StartingNumber.ToString() == StartNo
                                               select a).ToList();

            FailCashInvoiceList = (from a in FailAllInvoiceListByStartNo
                                   where a.PayType.ToString() == "1"
                                   select new Order.FailInvoice { InvoiceNo = a.InvoiceNo.ToString(), InvoiceNoAmount = a.Amount.ToString() }).ToList();

            FailCardInvoiceList = (from a in FailAllInvoiceListByStartNo
                                   where a.PayType.ToString() == "2"
                                   select new Order.FailInvoice { InvoiceNo = a.InvoiceNo.ToString(), InvoiceNoAmount = a.Amount.ToString() }).ToList();

            FailOhterInvoiceList = (from a in FailAllInvoiceListByStartNo
                                    where a.PayType.ToString() == "0"
                                    select new Order.FailInvoice { InvoiceNo = a.InvoiceNo.ToString(), InvoiceNoAmount = a.Amount.ToString() }).ToList();

            //總訂單數量
            TotalOrderNum = (from a in OrderListByStartNo
                             select new { OrderID = a.OrderID, Amount = a.Amount }).Distinct().ToList().Count;

            if (TotalOrderNum > 0)
            {
                StartOrderID = int.Parse(OrderListByStartNo.First().OrderID.ToString()).ToString("D9");
                EndOrderID = int.Parse(OrderListByStartNo.Last().OrderID.ToString()).ToString("D9");
                StartInvoiceNo = OrderListByStartNo.First().InvoiceNo.ToString();
                EndInvoiceNo = OrderListByStartNo.Last().InvoiceNo.ToString();

                foreach (var a in FailInvoiceOrderList)
                {
                    if (int.Parse(a.StartingNumber.ToString()) >= int.Parse(StartNo) && int.Parse(a.EndingNumber.ToString()) <= int.Parse(EndNo))
                    {
                        //把OrderID=0 的單獨作廢的發票，判斷是否比今天這一捆的頭還前面，如果是就取代
                        if (int.Parse(a.InvoiceNo.ToString().Substring(2, 8)) < int.Parse(StartInvoiceNo.Substring(2, 8)) && int.Parse(a.OrderID.ToString()) == 0)
                        {
                            StartInvoiceNo = a.InvoiceNo.ToString();
                        }
                        //把OrderID=0 的單獨作廢的發票，判斷是否比今天這一捆的頭還後面，如果是就取代
                        if (int.Parse(a.InvoiceNo.ToString().Substring(2, 8)) > int.Parse(EndInvoiceNo.Substring(2, 8)) && int.Parse(a.OrderID.ToString()) == 0)
                        {
                            EndInvoiceNo = a.InvoiceNo.ToString();
                        }
                    }
                }

                TotalOrderAmount = (from a in OrderListByStartNo
                                    select new { OrderID = a.OrderID, Amount = a.Amount }).Distinct().ToList().Sum(x => x.Amount);

                CashIncome = OrderListCash.Sum(x => x.Amount);
                TotalCashOrderNum = OrderListCash.Count;
                CardIncome = OrderListCreidt.Sum(x => x.Amount);
                TotalCardOrderNum = OrderListCreidt.Count;
            }

            foreach (DataRow row in OrderItemDT.Rows)
                TotalQuantity += int.Parse(row["Quantity"].ToString());

            //作廢訂單數

            FailOrderNum = FailInvoiceOrderListByStartNo.Count;
            FailInvoiceNum = FailAllInvoiceListByStartNo.Count;
            ReturnTotalAmount = FailInvoiceOrderListByStartNo.Sum(x => x.Amount);
            ReturnTotalCashAmount = (from a in FailInvoiceOrderListByStartNo
                                     where a.PayType == 1
                                     select a
                                        ).ToList().Sum(x => x.Amount);

            ReturnTotalCardAmount = (from a in FailInvoiceOrderListByStartNo
                                     where a.PayType == 2
                                     select a
                                        ).ToList().Sum(x => x.Amount);
        }

        public void PosNoBiding()
        {
            string IPaddress = Request.UserHostAddress;
            PosNo = int.Parse(PosNumber.GetPosNumberMapping(IPaddress)).ToString("D3");
        }

        public void Auth()
        {
            if (Session["Account"] == null)
            {
                Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                Response.End();
            }
            else
            {
                if (Session["ClerkID"] == null)
                {
                    Response.Write("你的身分無法進入日結頁面，請重新登入<br/>");
                    Response.Write("<a href=\"logout.aspx\">登出</a>");
                    Response.End();
                }
                else
                {
                    ck.ID = Session["ClerkID"].ToString();
                    ck.Name = Session["Name"].ToString();
                }
            }
        }

        protected void PrintBtn_Click(object sender, EventArgs e)
        {
            //PrintDayEndReport PDER = new PrintDayEndReport();
            //DayEndReport Der = new DayEndReport();
            //Der.Date = Date;
            //Der.StoreNo = StoreNo;
            //Der.PosNo = PosNo;
            //Der.ClerkName = ck.Name;
            //Der.TotalOrderAmount = TotalOrderAmount;
            //Der.StartOrderID = StartOrderID;
            //Der.EndOrderID = EndOrderID;
            //Der.TotalOrderNum = TotalOrderNum;
            //Der.TotalQuantity = TotalQuantity;
            //Der.TotalCashOrderNum = TotalCashOrderNum;
            //Der.CashIncome = CashIncome;
            //Der.TotalCardOrderNum = TotalCardOrderNum;
            //Der.CardIncome = CardIncome;
            //Der.StartInvoiceNo = StartInvoiceNo;
            //Der.EndInvoiceNo = EndInvoiceNo;
            //Der.ReturnTotalCashAmount = ReturnTotalCashAmount;
            //Der.ReturnTotalCardAmount = ReturnTotalCardAmount;
            //Der.ReturnTotalAmount = ReturnTotalAmount;
            //Der.FailOrderNum = FailOrderNum;
            //Der.FailInvoiceNum = FailInvoiceNum;
            //Der.FailCashInvoiceList = FailCashInvoiceList;
            //Der.FailCardInvoiceList = FailCardInvoiceList;
            //Der.FailOhterInvoiceList = FailOhterInvoiceList;
            //string Result=PDER.PrintReport(Der);

            //if (Result == "Success")
            //    ResultLabel.Text = "列印成功";
            //else
            //    ResultLabel.Text = "列印失敗";
        }
    }
}