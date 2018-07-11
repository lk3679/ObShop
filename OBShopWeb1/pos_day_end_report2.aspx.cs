
using OBShopWeb.Poslib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class pos_day_end_report2 : System.Web.UI.Page
    {
        public string Date = "";
        public string Month = "";
        public string Year = "";
        public string StoreNo = "";
        public string PosNo = "All";
        public string StartOrderID = "";
        public string EndOrderID = "";
        public string StartInvoiceNo = "";
        public string EndInvoiceNo = "";
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
        public List<Order.FailInvoice> FailCashInvoiceList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> FailCardInvoiceList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> FailOhterInvoiceList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> AllowancesCashList = new List<Order.FailInvoice>();
        public List<Order.FailInvoice> AllowancesCardList = new List<Order.FailInvoice>();
        public int FailInvoiceNum = 0;
        public int FailOrderNum = 0;
        public int AllowancesCashAmount = 0;
        public int AllowancesCardAmount = 0;
        public DataTable PosDT = new DataTable();
        public List<InvoiceRoll> InvoiceList = new List<InvoiceRoll>();
 
        protected void Page_Load(object sender, EventArgs e)
        {
            PosDT = PosNumber.GetAllPosNo();

            if (string.IsNullOrEmpty(Request["Date"]))
            { Date = DateTime.Now.ToString("yyyy/MM/dd"); }
            else
            { Date = Request["Date"]; }

            if (string.IsNullOrEmpty(Request["Month"]))
            { Month = "";  }
            else
            {
                string[] datelist = Request["Month"].ToString().Split('/');
                Year = datelist[0];
                Month = datelist[1];
            }

            if (!string.IsNullOrEmpty(Request["PosNo"]))
            { PosNo = Request["PosNo"]; }

            LoadOrderData();
        }

        public void LoadOrderData()
        {
            DataTable OrderDT=new DataTable();
            DataTable OrderItemDT = new DataTable();
            DataTable FailInvoiceDT = new DataTable();

            //儲位所在地
            int _areaId = int.Parse(Area.WmsAreaXml("Area"));
            StoreNo = _areaId.ToString("D3");
            if (!string.IsNullOrEmpty(Date))
            {
                OrderDT = Order.GetOrderMapingInvoiceRollByDate(Date);
                OrderItemDT = Order.GetOrderItemByDate(Date, PosNo);
                FailInvoiceDT = Order.GetFailInvoiceRollByDate(Date);
            }

            if (!string.IsNullOrEmpty(Month) && !string.IsNullOrEmpty(Year))
            {
                OrderDT = Order.GetOrderMapingInvoiceRollByMonth(Year,Month);
                OrderItemDT = Order.GetOrderItemByMonth(Year,Month,PosNo);
                FailInvoiceDT = Order.GetFailInvoiceRollByMonth(Year,Month);
            }
           

            var OrderList = OrderDT.AsEnumerable().Select(r => new
            {
                OrderID = r["OrderID"],
                InvoiceNo = r["InvoiceNo"],
                Amount = (int)r["Amount"],
                PayType = (int)r["PayType"],
                StartingNumber = r["StartingNumber"],
                EndingNumber = r["EndingNumber"],
                PosNo = r["PosNo"],
                RowNo = Convert.ToInt32(r["RowNo"])
            }).ToList();

            var FailInvoiceOrderList = FailInvoiceDT.AsEnumerable().Select(r => new
            {
                RowNo=r["RowNo"],
                OrderID = r["OrderID"],
                InvoiceNo = r["InvoiceNo"],
                Amount = (int)r["Amount"],
                PayType = (int)r["PayType"],
                StartingNumber = r["StartingNumber"],
                EndingNumber = r["EndingNumber"],
                PosNo = r["PosNo"],
                Status=(int)r["Status"]
            }).ToList();


            var OrderListByStartNo = (from a in OrderList
                                      where (PosNo != "All" ? a.PosNo.ToString() == PosNo : true) 
                                      select a).ToList();

            var OrderListCash = (from a in OrderListByStartNo
                                 where a.PayType.ToString() == "1"
                                 select new { OrderID = a.OrderID, Amount = a.Amount }).Distinct().ToList();

            var OrderListCreidt = (from a in OrderListByStartNo
                                   where a.PayType.ToString() == "2"
                                   select new { OrderID = a.OrderID, Amount = a.Amount }).Distinct().ToList();

            var FailInvoiceOrderListByPosNo = (from a in FailInvoiceOrderList
                                                 where (PosNo != "All" ? a.PosNo.ToString() == PosNo : true)
                                               select new { RowNo = Convert.ToInt32(a.RowNo), OrderID = a.OrderID, Amount = (int)a.Amount, PayType = (int)a.PayType, InvoiceNo = a.InvoiceNo, StartNo = a.StartingNumber,Status=a.Status}).Distinct().ToList();

            //總訂單數量
            TotalOrderNum = (from a in OrderListByStartNo
                             select new { OrderID = a.OrderID, Amount = a.Amount }).Distinct().ToList().Count;


            #region 計算訂單金額
            if (TotalOrderNum > 0)
            {
                var InvoiceRollList = (from o in OrderList
                                       where (PosNo != "All" ? o.PosNo.ToString() == PosNo : true)
                                       group o by new { startNo = o.StartingNumber, endNo = o.EndingNumber, PosNo = o.PosNo}
                                           into RollList
                                           select new { 
                                               startNo = RollList.Key.startNo, 
                                               endNo = RollList.Key.endNo, 
                                               PosNo = RollList.Key.PosNo, 
                                               InvList = RollList.ToList()});

                StartOrderID = int.Parse(OrderListByStartNo.First().OrderID.ToString()).ToString("D9");
                EndOrderID = int.Parse(OrderListByStartNo.Last().OrderID.ToString()).ToString("D9");

                #region 計算起始發票號碼 
                foreach (var r in InvoiceRollList)
                {
                    InvoiceRoll roll = new InvoiceRoll();
                    roll.StartInvoice = r.InvList.First().InvoiceNo.ToString();
                    roll.EndInvoice = r.InvList.Last().InvoiceNo.ToString();
                    roll.Cash=(from x in r.InvList
                                   where x.PayType==1&&x.RowNo==1
                               select new { OrderID = x.OrderID, Amount = x.Amount, RowNo=x.RowNo }).Sum(y => y.Amount);
                    
                    roll.Credit=(from x in r.InvList
                                 where x.PayType == 2 && x.RowNo == 1
                                 select new { OrderID = x.OrderID, Amount = x.Amount, RowNo = x.RowNo }).Sum(y => y.Amount);

                    foreach (var a in FailInvoiceOrderListByPosNo)
                    {
                        //如果作廢發票在同一捆
                        if (a.StartNo.Equals(r.startNo))
                        {
                            //把OrderID=0 的單獨作廢的發票，判斷是否比今天這一捆的頭還前面，如果是就取代
                            if (int.Parse(a.InvoiceNo.ToString().Substring(2, 8)) < int.Parse(roll.StartInvoice.Substring(2, 8)) && int.Parse(a.OrderID.ToString()) == 0)
                            {
                                roll.StartInvoice = a.InvoiceNo.ToString();
                            }
                            //把OrderID=0 的單獨作廢的發票，判斷是否比今天這一捆的頭還後面，如果是就取代
                            if (int.Parse(a.InvoiceNo.ToString().Substring(2, 8)) > int.Parse(roll.EndInvoice.Substring(2, 8)) && int.Parse(a.OrderID.ToString()) == 0)
                            {
                                roll.EndInvoice = a.InvoiceNo.ToString();
                            }
                        }
                        
                    }

                    InvoiceList.Add(roll);
                }

                #endregion

                TotalOrderAmount = (from a in OrderListByStartNo
                                    select new { OrderID = a.OrderID, Amount = a.Amount }).Distinct().ToList().Sum(x => x.Amount);

                CashIncome = OrderListCash.Sum(x => x.Amount);
                TotalCashOrderNum = OrderListCash.Count;
                CardIncome = OrderListCreidt.Sum(x => x.Amount);
                TotalCardOrderNum = OrderListCreidt.Count;

            }

            foreach (DataRow row in OrderItemDT.Rows)
                TotalQuantity += int.Parse(row["Quantity"].ToString());

            #endregion

            #region 作廢訂單計算
            FailCashInvoiceList = (from a in FailInvoiceOrderListByPosNo
                                   where a.PayType.ToString() == "1" && a.Status == 2
                                   select new Order.FailInvoice { RowNo = Convert.ToInt32(a.RowNo), InvoiceNo = a.InvoiceNo.ToString(), InvoiceNoAmount = (Convert.ToInt32(a.RowNo) == 1) ? a.Amount.ToString() : "0" }).OrderBy(x => x.InvoiceNo).ToList();

            FailCardInvoiceList = (from a in FailInvoiceOrderListByPosNo
                                   where a.PayType.ToString() == "2" && a.Status == 2
                                   select new Order.FailInvoice { RowNo = Convert.ToInt32(a.RowNo), InvoiceNo = a.InvoiceNo.ToString(), InvoiceNoAmount = (Convert.ToInt32(a.RowNo) == 1) ? a.Amount.ToString() : "0" }).OrderBy(x => x.InvoiceNo).ToList();

            FailOhterInvoiceList = (from a in FailInvoiceOrderListByPosNo
                                    where a.Status == 0
                                    select new Order.FailInvoice { RowNo = Convert.ToInt32(a.RowNo), InvoiceNo = a.InvoiceNo.ToString(), InvoiceNoAmount = a.Amount.ToString() }).OrderBy(x => x.InvoiceNo).ToList();

            FailOrderNum = (from a in FailInvoiceOrderListByPosNo
                            where a.RowNo == 1&&(a.Status==2||a.Status==6)
                            select a).ToList().Count;

            FailInvoiceNum = FailInvoiceOrderListByPosNo.Count;

            ReturnTotalAmount = (from a in FailInvoiceOrderListByPosNo
                                 where a.RowNo == 1 && a.Status == 2
                                 select a).ToList().Sum(x => x.Amount);

            ReturnTotalCashAmount = (from a in FailInvoiceOrderListByPosNo
                                     where a.PayType == 1 && a.RowNo == 1 && a.Status == 2
                                     select a
                                        ).ToList().Sum(x => x.Amount);

            ReturnTotalCardAmount = (from a in FailInvoiceOrderListByPosNo
                                     where a.PayType == 2 && a.RowNo == 1 && a.Status == 2
                                     select a
                                        ).ToList().Sum(x => x.Amount);

            #endregion

            #region 折讓訂單計算
            AllowancesCashList = (from a in FailInvoiceOrderListByPosNo
                                  where a.PayType.ToString() == "1" && a.Status == 6
                                  select new Order.FailInvoice { RowNo = Convert.ToInt32(a.RowNo), InvoiceNo = a.InvoiceNo.ToString(), InvoiceNoAmount = (Convert.ToInt32(a.RowNo) == 1) ? a.Amount.ToString() : "0" }).OrderBy(x => x.InvoiceNo).ToList();

            AllowancesCardList = (from a in FailInvoiceOrderListByPosNo
                                  where a.PayType.ToString() == "2" && a.Status == 6
                                  select new Order.FailInvoice { RowNo = Convert.ToInt32(a.RowNo), InvoiceNo = a.InvoiceNo.ToString(), InvoiceNoAmount = (Convert.ToInt32(a.RowNo) == 1) ? a.Amount.ToString() : "0" }).OrderBy(x => x.InvoiceNo).ToList();

            AllowancesCashAmount = (from a in FailInvoiceOrderListByPosNo
                                    where a.PayType == 1 && a.RowNo == 1 && a.Status == 6
                                    select a
                                        ).ToList().Sum(x => x.Amount);

            AllowancesCardAmount = (from a in FailInvoiceOrderListByPosNo
                                    where a.PayType == 2 && a.RowNo == 1 && a.Status == 6
                                    select a
                                       ).ToList().Sum(x => x.Amount);
            #endregion
        }
    }
}