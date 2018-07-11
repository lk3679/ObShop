using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using System.Threading;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class CreateDoc : System.Web.UI.Page
    {
        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        private int _PrintPageSize = int.Parse(Area.WmsAreaXml("PrintPageSize"));

        protected void Page_Load(object sender, EventArgs e)
        {
            var pickType = 0;
            var ticketId = 0;
            var store = 0;
            var area = 0;
            try
            {
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    if (Request["pickType"] == null || Request["tick"] == null || Request["area"] == null || Request["store"] == null)
                    {
                        throw new Exception("資料錯誤，無法印單！");
                    }
                    ticketId = int.Parse(Request["tick"].ToString());
                    area = int.Parse(Request["area"].ToString());
                    store = int.Parse(Request["store"].ToString());
                    pickType = int.Parse(Request["pickType"].ToString());

                    var shipDa = new ShipOutDA(); 
                    var pickList = shipDa.LeaveWith(ticketId, pickType, store, area);
                    if (pickList.Any())
                    {
                        var p = new OBShopWeb.Poslib.Print();
                        //p.PrintPickList(pickList, "", "調出明細");

                        List<TicketShelfTemp> One = new List<TicketShelfTemp>();
                        var listCount = pickList.Count;
                        var xxi = 0;
                        //web.config設定每頁幾筆
                        var pernum = _PrintPageSize;
                        while (xxi * pernum < listCount)
                        {
                            One = pickList.Skip(xxi * pernum).Take(pernum).ToList();
                            xxi++;
                            var result = p.PrintPickList(One, "", ticketId + " 調出明細" + xxi.ToString("D2"));
                            Thread.Sleep(1000);
                        }
                        Page.RegisterClientScriptBlock("checkinput", @"<script>alert('成功!');window.close();</script>"); 
                    }
                    else
                    {
                        Page.RegisterClientScriptBlock("checkinput", @"<script>alert('無此調撥資料！！請重試！');window.close();</script>"); 
                    }
                } 
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
         
        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                //POS_Library.ShopPos.ShipOut.testShipOut();

                //訂單號碼
                var ShipId = 123456;
                //出貨 傳票固定代入0
                var Ticket = 0;
                //呼叫取得store 門市旗艦 = 6
                var store = POS_Library.Public.Utility.GetStore(_areaId);
                //一般扣數不含展售 = 0
                var pickType = (int)POS_Library.Public.Utility.ShipPDF.出貨;
                //如果是特殊扣數含展售 = 5
                pickType = (int)POS_Library.Public.Utility.ShipPDF.出貨含展售;
                //銷售日期(看訂單)
                var Date = "2014-12-24";

                ShipOutDA SO = new ShipOutDA();
                //建立撿貨明細List
                List<TicketShelfTemp> ticketTemp = new List<TicketShelfTemp>();
                //For迴圈建立明細-----------------------------------
                TicketShelfTemp detail = new TicketShelfTemp();
                detail.Account = "test";
                detail.GuestId = 1;
                detail.ShipId = ShipId.ToString();
                detail.ShipoutDate = Date;
                //代入產品編號
                detail.ProductId = "YA032-40";
                //代入產品個數
                detail.Quantity = 1;
                detail.Ticket = Ticket;
                detail.Store = POS_Library.Public.Utility.GetStore(_areaId);
                ticketTemp.Add(detail);
                //-------------------------------------------------

                //呼叫印單扣數結果
                var temp = SO.PrintPickListsShelf(ticketTemp, ShipId, pickType, store, _areaId);
            }
            catch (Exception ex)
            {
                lbl_Message.Text = ex.Message;
            }
            
        }
    }
}