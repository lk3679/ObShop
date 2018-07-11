using Newtonsoft.Json;
using OBShopWeb.Poslib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using POS_Library.ShopPos.DataModel;
using POS_Library.ShopPos;
using POS_Library.Public;
using System.Web.Configuration;
using System.Net;
using System.Collections;

namespace OBShopWeb
{
    public partial class pos_check_out : System.Web.UI.Page
    {
        public string InvoiceNumberNow = "";
        public string InvoiceStartNumber = "";
        public string InvoiceRemainder = "";
        public string PosNo = "";
        public string MachineNo = "";
        public string act = "";
        public string quantity = "0";
        public string barcode = "";
        public string HostName = "";
        public string uniformNo = "";
        public string ClerkName = "";
        public string ClerkID = "";
        public string InvoiceNo = "";
        public string OrderID = "";
        public string ProductID = "";
        public string received = "";
        public string change = "";
        public string ReturnType = "0";
        public string CardNo = "";
        public string ApprovalNo = "";
        public string CreditCardData = "";
        public string PrintFileName = "";
        public string ErrorMsg = "";
        public int OrderStep = 0;
        public CheckOut.Clerk ck = new CheckOut.Clerk();

        //取得倉庫區域 門市旗艦 = 3
        public int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        public string DiscountLimit = "[]";
        public string VIPDiscountLimit = "[]";
        public string DiscountType = "";
        public string VIPDiscountType = "";
        public double VIPDiscount=0;
        public int TouristsDiscountAmount = 0;
        public int TouristsDiscountLimit = 0;
        public bool GetApplyVipDiscount = SystemSettings.GetApplyVipDiscount();
        string vip_id = "";

        protected void Page_Load(object sender, EventArgs e)
        {

            GetHttpRequest();
            PosNoBiding();
            GetInvoiceNumber(PosNo);

            switch (act)
            {
                case "Scan":
                    Scan();
                    break;
                case "SettingInvoiceNumber":
                    SettingInvoiceNumber();
                    break;
                case "VoidedInvoice":
                    VoidedInvoice();
                    break;
                case "ReSettingInvoiceNumber":
                    ReSettingInvoiceNumber();
                    break;
                case "GetOrderItemByOrderID":
                    GetOrderItemByOrderID(OrderID);
                    break;
                case "ClearTempStorage":
                    ClearTempStorage();
                    break;
                case "RePrintPickList":
                    RePrintPickList();
                    break;
                case "AddOrder":
                    AddOrder();
                    break;
                case "ChangeStatusToWaitingForCheckOut":
                    ChangeStatusToWaitingForCheckOut();
                    break;
                case "ReturnPurchase":
                    ReturnePurchase(OrderID, PosNo, ClerkID, ReturnType);
                    break;
                case "ReCheckOut":
                    ReCheckOut();
                    break;
                case "vip":
                    GetVipMember();
                    break;
                case "UpdateVIPBonus":
                    UpdateVIPBonus();
                    break;
                case "CheckPromotion":
                    CheckPromotion();
                    break;
                default:
                    Auth();
                    DiscountLimit = CheckOut.GetDiscountLimit(0);
                    DiscountType = CheckOut.GetDiscountType(0);
                    VIPDiscountLimit = CheckOut.GetDiscountLimit(1);
                    VIPDiscountType = CheckOut.GetDiscountType(1);
                    VIPDiscount = CheckOut.GetVIPDiscount();
                    TouristsDiscountAmount = CheckOut.GetTouristsDiscountAmount();
                    TouristsDiscountLimit = CheckOut.GetTouristsDiscountLimit();
                    break;
            }
        }

        public void Scan()
        {
            string Product = CheckOut.GetcheckOutItemString(CheckOut.GetPosCheckOutItem(barcode), quantity, PosNo);
            ShowResultOnPage(Product);
        }

        #region VIP會員機制
        
        public void GetVipMember()
        {
            string vip_id = string.IsNullOrEmpty(Request["vip_id"]) ? "" : Request["vip_id"];
            string mobile = string.IsNullOrEmpty(Request["mobile"]) ? "" : Request["mobile"];
            MemberData MD = VipMember.GetMemberData(vip_id, mobile);
            bool VipExist = false;

            if (!string.IsNullOrEmpty(MD.create_date))
            { VipExist = true; }

            var result = new { result = VipExist, data = MD };
            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }

        public void UpdateVIPBonus()
        {
            bool HasUseBirthdayDiscount = false;
            int Bonus = 0;
            string vip_id = string.IsNullOrEmpty(Request["vip_id"]) ? "" : Request["vip_id"];
            string LastBonus = string.IsNullOrEmpty(Request["LastBonus"]) ? "0" : Request["LastBonus"];
            bool.TryParse(Request["HasUseBirthdayDiscount"], out HasUseBirthdayDiscount);
            int.TryParse(LastBonus, out Bonus);
            bool BonusResult = VipMember.UpdateVipBonus(vip_id, Bonus, HasUseBirthdayDiscount);
            var result = new { result = BonusResult };
            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }

        public void CheckPromotion()
        {
            bool IsBirthday = false;
            bool IsVip = false;
            int amount = 0;
            string ProductList = string.IsNullOrEmpty(Request["ProductList"]) ? "" : Request["ProductList"];
            bool.TryParse(Request["IsBirthday"], out IsBirthday);
            int.TryParse(Request["amount"],out amount);
            bool.TryParse(Request["IsVip"], out IsVip);

            //先清暫量再重新取得滿額禮
            if (ProductList.Length > 0)
            {
                dynamic ProductArray = JsonConvert.DeserializeObject<dynamic>(ProductList);
                foreach (string ProductID in ProductArray)
                {
                    CheckOut.ClearTempStorage(PosNo, ProductID);
                }
            }
               
            string PromotionItem = CheckOut.GetPromotionItemString(amount, IsVip, IsBirthday, PosNo);

            if (PromotionItem.Length > 0)
            {
                ShowResultOnPage(PromotionItem);
            }
            else
            {
                ArrayList ItemList = new ArrayList();
                //沒贈品
                var result = new { result = true, data = ItemList };
                ShowResultOnPage(JsonConvert.SerializeObject(result));
            }
            
        }

        #endregion

        #region 發票管理

        public void SettingInvoiceNumber()
        {
            bool settingResult = Invoice.SettingInvoiceNumber(PosNo, Request["InvoiceStartNumber"], Request["InvoiceEndNumber"], ClerkID);
            var result = new { result = settingResult, data = Request["InvoiceStartNumber"] };
            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }

        public void VoidedInvoice()
        {
            string Message = "";
            bool VoidedResult = false;
            int status = Invoice.VoidedInvoice(PosNo, InvoiceNo, ClerkID);
            switch (status)
            {
                case 1:
                    VoidedResult = true;
                    Message = "發票作廢完成";
                    break;
                case -1:
                    Message = "此發票已作廢過，無法再次作廢";
                    break;
                default:
                    Message = "發票作廢失敗";
                    break;
            }

            GetInvoiceNumber(PosNo);
            var result = new { result = VoidedResult, NumberNow = InvoiceNumberNow, Remainder = InvoiceRemainder, Message = Message };
            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }

        public void ReSettingInvoiceNumber()
        {
            string Message = "";
            bool settingResult = false;
            int status = Invoice.UpdateInvoiceNumberNow(PosNo, InvoiceNo, ClerkID);
            switch (status)
            {
                case 1:
                    settingResult = true;
                    Message = "發票設定完成";
                    break;
                case -1:
                    Message = "此號碼已被使用。";
                    break;
                default:
                    Message = "發票設定失敗";
                    break;
            }
            GetInvoiceNumber(PosNo);
            var result = new { result = settingResult, NumberNow = InvoiceNumberNow, Remainder = InvoiceRemainder, Message = Message };
            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }

        #endregion 

        #region 清除占量
        public void ClearTempStorage()
        {
            bool DeleteResult = CheckOut.ClearTempStorage(PosNo, ProductID);
            var result = new { result = DeleteResult };
            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 重印撿貨單
        public void RePrintPickList()
        {
            int pickType = string.IsNullOrEmpty(Request["pickType"]) ? 0 : int.Parse(Request["pickType"]);

            string PickOrderID = "0";
            if (!string.IsNullOrEmpty(Request["PickOrderID"]))
            {
                PickOrderID = Request["PickOrderID"];
            }

            if (int.Parse(PickOrderID) != GetLastOrderID(PosNo))
            {
                return;
            }

            string OrderID = int.Parse(PickOrderID).ToString("D10");
            bool Status = PrintPickList(OrderID, pickType, Request["InvoiceNumberNow"]);
            GetInvoiceNumber(PosNo);
            var result = new { result = Status, NumberNow = InvoiceNumberNow, Remainder = InvoiceRemainder, ErrorMsg = ErrorMsg };
            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }
        #endregion

        #region 網頁登入相關程式

        public void PosNoBiding()
        {
            string IPaddress = Request.UserHostAddress;
            PosNo = PosNumber.GetPosNumberMapping(IPaddress);
            MachineNo = PosNumber.GetInvoiceMachineNo(IPaddress);
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
                    Response.Write("你的身分無法進入結帳頁面，請重新登入<br/>");
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

        public void ShowResultOnPage(string JsonResult)
        {
            Response.Clear();
            Response.Write(JsonResult);
            Response.Flush();
            Response.End();
        }

        public void GetHttpRequest()
        {
            MachineNo = (!string.IsNullOrEmpty(Request["MachineNo"])) ? Request["MachineNo"] : "P9";
            act = (!string.IsNullOrEmpty(Request["act"])) ? Request["act"] : "";
            barcode = (!string.IsNullOrEmpty(Request["barcode"])) ? Request["barcode"] : "";
            quantity = (!string.IsNullOrEmpty(Request["quantity"])) ? Request["quantity"] : "0";

            uniformNo = (!string.IsNullOrEmpty(Request["UniformNo"])) ? Request["UniformNo"] : "";
            ClerkName = (!string.IsNullOrEmpty(Request["ClerkName"])) ? Request["ClerkName"] : "Guest";
            ClerkID = (!string.IsNullOrEmpty(Request["ClerkID"])) ? Request["ClerkID"] : "-1";
            PosNo = (!string.IsNullOrEmpty(Request["PosNo"])) ? Request["PosNo"] : "9";
            InvoiceNo = (!string.IsNullOrEmpty(Request["InvoiceNo"])) ? Request["InvoiceNo"] : "";
            OrderID = (!string.IsNullOrEmpty(Request["OrderID"])) ? Request["OrderID"] : "";
            ProductID = (!string.IsNullOrEmpty(Request["ProductID"])) ? Request["ProductID"] : "";
            received = (!string.IsNullOrEmpty(Request["received"])) ? Request["received"] : "0";
            change = (!string.IsNullOrEmpty(Request["change"])) ? Request["change"] : "0";
            ReturnType = (!string.IsNullOrEmpty(Request["ReturnType"])) ? Request["ReturnType"] : "0";
            CardNo = (!string.IsNullOrEmpty(Request["CardNo"])) ? Request["CardNo"] : "";
            ApprovalNo = (!string.IsNullOrEmpty(Request["ApprovalNo"])) ? Request["ApprovalNo"] : "";
            CreditCardData = (!string.IsNullOrEmpty(Request["CreditCardData"])) ? Request["CreditCardData"] : "";
            OrderStep = (!string.IsNullOrEmpty(Request["OrderStep"])) ? int.Parse(Request["OrderStep"]) : 0;
            vip_id=string.IsNullOrEmpty(Request["vip_id"]) ? "" : Request["vip_id"];
        }

        #endregion

        #region 交易結帳

        public void AddOrder()
        {
            string[] ItemNo = JsonConvert.DeserializeObject<string[]>(Request["ItemNoList"]);
            string[] color = JsonConvert.DeserializeObject<string[]>(Request["ColorList"]);
            string[] quantity = JsonConvert.DeserializeObject<string[]>(Request["QuantityList"]);
            string[] price = JsonConvert.DeserializeObject<string[]>(Request["PriceList"]);
            string Amount = Request["Amount"];
            string OriginalAmount = Request["OriginalAmount"];
            string PayType = Request["PayType"];
            
            ArrayList InvoiceContent = new ArrayList();

            if (CardNo.Length == 16)
            { CardNo = "************" + CardNo.Substring(12, 4); }

            int pickType = string.IsNullOrEmpty(Request["pickType"]) ? 0 : int.Parse(Request["pickType"]);

            List<SaleItemData> ItemList = new List<SaleItemData>();
            List<SaleItemData> ItemListInvoice = new List<SaleItemData>();
            for (int i = 0; i < quantity.Length; i++)
            {
                SaleItemData SD = new SaleItemData();
                SD.itemNo = ItemNo[i];
                SD.price = int.Parse(price[i]);
                SD.quantity = int.Parse(quantity[i]);
                SD.aomout = SD.price * SD.quantity;
                ItemList.Add(SD);
                SaleItemData SDInv = new SaleItemData();
                SDInv.itemNo = ItemNo[i];
                SDInv.price = int.Parse(price[i]);
                SDInv.quantity = int.Parse(quantity[i]);
                SDInv.aomout = SDInv.price * SDInv.quantity;
                ItemListInvoice.Add(SDInv);
            }
            
           
            #region 攤使用紅利

            bool HasUseBonus = false;
            int BonusUseAmount = 0;
            int BonusUsed = 0;
            bool.TryParse(Request["HasUseBonus"], out HasUseBonus);
            
            if (HasUseBonus)
            {
                BonusUseAmount = (!string.IsNullOrEmpty(Request["BonusUseAmount"])) ? int.Parse(Request["BonusUseAmount"]) : 0;
                BonusUsed = BonusUseAmount;
                //要先把扣掉紅利的金額加回來
                int TotalAmount = int.Parse(Amount) + BonusUseAmount;
                int DiscountAmout = TotalAmount - BonusUseAmount;
                int CounterNo = 1;
                SaleItemData Inv = new SaleItemData();
                Inv.itemNo = "紅利";
                Inv.aomout = -BonusUseAmount;
                ItemListInvoice.Add(Inv);

                //因為贈品不需分攤紅利，故要先扣除
                var a = (from x in ItemList
                         where x.price > 0
                         select x).ToList();

                int DiscountTotal= a.Count();

                foreach (SaleItemData SD in ItemList)
                {
                    //攤紅利要排除贈品
                    if (SD.price > 0)
                    {
                        int discount_price = Convert.ToInt32(Math.Floor((double)(SD.price * DiscountAmout / TotalAmount)));
                        int original_amount = SD.price * SD.quantity;
                        int discount_amount = 0;

                        if (CounterNo == DiscountTotal)
                        {
                            discount_amount = original_amount - (BonusUseAmount);
                        }
                        else
                        {
                            discount_amount = discount_price * SD.quantity;
                        }

                        BonusUseAmount -= (original_amount) - discount_amount;
                        SD.price = discount_price;
                        SD.aomout = discount_amount;
                        CounterNo++;
                    }
                    
                }

            }

            #endregion

            if (ItemList.Count == 0)
            { return; }

            bool result = CheckOut.AddOrder(PosNo, ItemList, Amount, PayType, ClerkID, InvoiceNumberNow, uniformNo, ApprovalNo, CreditCardData, pickType, vip_id, BonusUsed, ItemListInvoice);

            if (result)
            {
                InvoiceContent = PrintInvoice(ItemListInvoice, int.Parse(PayType), int.Parse(Amount), int.Parse(OriginalAmount));
                OrderStep = 1;
                OrderID = GetLastOrderID(PosNo).ToString("D10");
                Log.Add(0, OrderID, OrderStep.ToString(), "OrderStep：1，訂單寫入成功", ClerkID, PosNo);
            }

            GetInvoiceNumber(PosNo);
            var AddOrderResult = new { result = result, NumberNow = InvoiceNumberNow, Remainder = InvoiceRemainder, OrderStep = 1, InvoiceContent = InvoiceContent, OrderID = GetLastOrderID(PosNo) };
            ShowResultOnPage(JsonConvert.SerializeObject(AddOrderResult));
        }

        public ArrayList PrintInvoice(List<SaleItemData> ItemList, int PayType, int Amount, int OriginalAmount)
        {
            string OrderID = GetLastOrderID(PosNo).ToString("D10");
            int pageSize = 7;
            int TotalNum = ItemList.Count;
            int TotalQuality = ItemList.Sum(x => x.quantity);
            int AllPage = Convert.ToInt32(Math.Floor((double)(TotalNum - 1) / pageSize) + 1);
            int pageNow = 0;
            bool LastPage;
            ArrayList InvoiceContent = new ArrayList();

            for (int i = 0; i < AllPage; i++)
            {
                pageNow = i;
                List<SaleItemData> NewItemList = new List<SaleItemData>();
                NewItemList = ItemList.Skip(pageNow * pageSize).Take(pageNow * pageSize + pageSize).ToList<SaleItemData>();
                pageNow++;


                if (pageNow == AllPage)
                { LastPage = true; }
                else
                { LastPage = false; }

                Invoice inv = new Invoice();
                InvoiceContent.Add(inv.printStart(PayType, Amount, OriginalAmount, uniformNo, OrderID, NewItemList, PosNo, received, change, CardNo, ApprovalNo, pageNow, LastPage, TotalQuality));

            }
            return InvoiceContent;
        }

        #endregion

        #region 取得退貨商品和資訊

        public void GetOrderItemByOrderID(string OrderID)
        {
            DataTable dt = GetOrderInfo(OrderID);
            string data = "";
            int status = 0;
            int payType = 0;
            int Amount = 0;
            string SerialNo = "";
            string ErrorMsg = "";
            string UniformSerialNumber = "";
            DateTime OrderDate;
            bool IsAllowances = false;
            int AllowanceID = 0;
            int PaperReturnID = 0;

            if (dt.Rows.Count > 0)
            {
                status = int.Parse(dt.Rows[0]["Status"].ToString());
                data = CheckOut.GetOrderItemByOrderID(OrderID, status);
                payType = int.Parse(dt.Rows[0]["PayType"].ToString());
                Amount = int.Parse(dt.Rows[0]["Amount"].ToString());
                ApprovalNo = dt.Rows[0]["AuthorizationCode"].ToString();
                UniformSerialNumber = dt.Rows[0]["UniformSerialNumber"].ToString().Trim();
                OrderDate = DateTime.Parse(dt.Rows[0]["OrderDate"].ToString());
                IsAllowances = CheckIsAllowances(OrderDate);
                if (IsAllowances)
                {AllowanceID = GetNewAllowanceID();}

                string CreditCardData = dt.Rows[0]["CreditCardData"].ToString();
                PaperReturnID = GetPaperReturnID();

                if (!string.IsNullOrEmpty(CreditCardData))
                {
                    try
                    {
                        CreditCardData TResult = JsonConvert.DeserializeObject<CreditCardData>(CreditCardData);
                        SerialNo = TResult.RefNo;
                    }
                    catch (Exception ex)
                    {
                        ErrorMsg = ex.ToString();
                        SerialNo = "";
                    }
                }
            }
            var result = new { 
                result = dt.Rows.Count > 0 ? true : false, 
                data = data, status = status,
                payType = payType, 
                Amount = Amount, 
                ApprovalNo = ApprovalNo, 
                UniformSerialNumber=UniformSerialNumber,
                SerialNo = SerialNo, 
                ErrorMsg = ErrorMsg, 
                IsAllowances = IsAllowances, 
                AllowanceID = AllowanceID ,
                PaperReturnID = PaperReturnID
            };

            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region 退貨

        public void ReturnePurchase(string OrderID, string PosNo, string ClerkID, string ReturnType)
        {
            string[] ItemNo = JsonConvert.DeserializeObject<string[]>(Request["ItemNoList"]);
            int AllowanceID = 0;
            string PaperReturnID = "";

            if(!string.IsNullOrEmpty(Request["AllowanceID"]))
            {
                AllowanceID=int.Parse(Request["AllowanceID"]);
            }

            //紙本刷退
            if (!string.IsNullOrEmpty(Request["PaperReturnID"]))
            {
                PaperReturnID = Request["PaperReturnID"];
            }

            bool status = false;
            DataTable dt = GetOrderInfo(OrderID);
            if (dt.Rows.Count > 0)
            {

                //判斷需不需要退回多少紅利
                int ReturnBonus = 0;
                int Amount = 0;
                if (dt.Rows[0]["VIP"]!=System.DBNull.Value)
                {
                   vip_id= dt.Rows[0]["VIP"].ToString();
                    //剩下要結帳的金額
                    int.TryParse(Request["Amount"].ToString(), out  Amount);
                    int.TryParse(dt.Rows[0]["Amount"].ToString(), out  ReturnBonus);
                    //前訂單的金額減掉這次訂單的金額等於要退還的紅利
                    ReturnBonus = ReturnBonus - Amount;
                }

                var aa = POS_Library.ShopPos.NoProduct.SetSalePackageBack(int.Parse(OrderID), _areaId, ClerkName);
                if (aa == true)
                {
                    Log.Add(0, OrderID, "5", "OrderStep：5，商品退回不可銷庫存完成", ClerkID, PosNo);

                    StringBuilder sb = new StringBuilder();
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    //判斷有沒有折讓編號
                    if (AllowanceID == 0)
                    {
                        sb.Append("update Orders set Status=2,SyncedTime=null where OrderID=@OrderID; ");
                    }
                    else
                    {
                        sb.Append("update Orders set Status=6,SyncedTime=null where OrderID=@OrderID; ");
                        sb.Append("insert into PosClient..Allowances(OrderID,Year,Month,AllowanceID) values(@OrderID,@Year,@Month,@AllowanceID) ");
                        param.Add("Year", DateTime.Now.Year);
                        param.Add("Month", DateTime.Now.Month);
                        param.Add("AllowanceID", AllowanceID);
                    }
                   

                    sb.Append("Insert into Returns(OrderID,Type,PosNo,ClerkID,ReturnTime) ");
                    sb.Append("values(@OrderID,@ReturnType,@PosNo,@ClerkID,GETDATE()); ");

                    //新增發票作廢，有可能會多張
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("Insert into NullifiedInvoices(PosNo,InvoiceNo,ClerkID,NullifiedTime)  ");
                        sb.Append(string.Format("values(@PosNo,@InvoiceNo{0},@ClerkID,getdate()); ", i));
                        param.Add("InvoiceNo" + i, dt.Rows[i]["InvoiceNo"].ToString());
                    }

                    param.Add("OrderID", OrderID);
                    param.Add("PosNo", PosNo);
                    param.Add("ClerkID", ClerkID);
                    param.Add("ReturnType", ReturnType);

                    //寫入紙本刷退紀錄
                    if (string.IsNullOrEmpty(PaperReturnID) == false)
                    {
                        sb.Append("Insert Into [PosClient]..[ReturnCreditCardPaper] (OrderID) values (@OrderID) ");
                        param.Add("PaperReturnID", PaperReturnID);
                    }

                    status = DB.DBNonQuery(sb.ToString(), param, "PosClient");

                    if (status)
                    {
                        #region 退紅利

                        //退紅利
                        if (ReturnBonus > 0)
                        {
                            MemberData MD = VipMember.GetMemberData(vip_id, "");
                            int Bonus = MD.bonus - ReturnBonus;
                            VipMember.UpdateVipBonus(vip_id, Bonus, false);
                        }
                        #endregion

                        if (ItemNo.Length > 0)
                        {
                            AddOrder();
                            return;
                        }
                    }
                }
                else
                {
                    status= aa;
                }
            }
            
            GetInvoiceNumber(PosNo);
            var result = new { result = status, NumberNow = InvoiceNumberNow, Remainder = InvoiceRemainder, OrderStep = 5 };
            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }

        void UpdateReturnsOrderIDRelated()
        {
                string ReturnOrderID = string.IsNullOrEmpty(Request["ReturnOrderID"]) ? "0" : Request["ReturnOrderID"];
                int NewOrderID = GetLastOrderID(PosNo);
                if (NewOrderID > int.Parse(ReturnOrderID))
                {
                    string sql = "Update Returns set OrderIDRelated=@NewOrderID where OrderID=@ReturnOrderID ";
                    Dictionary<string, object> param = new Dictionary<string, object>();
                    param.Add("ReturnOrderID", ReturnOrderID);
                    param.Add("NewOrderID", NewOrderID);
                    DB.DBNonQuery(sql, param, "PosClient");
                }
            
        }
        #endregion

        #region 修改訂單為待結帳
        public void ChangeStatusToWaitingForCheckOut()
        {
            bool result = false;
            string PaperReturnID = "";
            //紙本刷退
            if (!string.IsNullOrEmpty(Request["PaperReturnID"]))
            {
                PaperReturnID = Request["PaperReturnID"];
            }

            string[] ItemNo = JsonConvert.DeserializeObject<string[]>(Request["ItemNoList"]);
            string[] quantity = JsonConvert.DeserializeObject<string[]>(Request["QuantityList"]);
            string[] price = JsonConvert.DeserializeObject<string[]>(Request["PriceList"]);

            List<SaleItemData> ItemList = new List<SaleItemData>();
            for (int i = 0; i < quantity.Length; i++)
            {
                SaleItemData SD = new SaleItemData();
                SD.itemNo = ItemNo[i];
                SD.price = int.Parse(price[i]);
                SD.quantity = int.Parse(quantity[i]);
                ItemList.Add(SD);
            }

            DataTable dt = GetOrderInfo(OrderID);
            if (dt.Rows.Count > 0)
            {
                var aa = POS_Library.ShopPos.NoProduct.SetSalePackageBack(int.Parse(OrderID), _areaId, ClerkName);
                if (aa == true)
                {
                    StringBuilder sb = new StringBuilder();
                    Dictionary<string, object> param = new Dictionary<string, object>();

                    //改訂單狀態為待結
                    sb.Append("update Orders set Status=3,SyncedTime=null where OrderID=@OrderID; ");
                    //新增退貨紀錄
                    sb.Append("Insert into Returns(OrderID,Type,PosNo,ClerkID,ReturnTime) ");
                    sb.Append("values(@OrderID,@ReturnType,@PosNo,@ClerkID,GETDATE()); ");

                    int SaleNo = 1;
                    //紀錄待結商品
                    foreach (SaleItemData Item in ItemList)
                    {
                        string ProductId = Item.itemNo;
                        int Price = Item.price;
                        int Quantity = Item.quantity;
                        int SIAmount = Price * Quantity;

                        sb.Append("Insert into PosClient.[dbo].OrderItemsWaitCheckOut (OrderID,ProductId,Price,Quantity,Amount) values ");
                        sb.Append(string.Format("(@OrderID, @ProductId{0}, @Price{0},@Quantity{0}, @SIAmount{0} )", SaleNo));
                        param.Add("ProductId" + SaleNo.ToString(), ProductId);
                        param.Add("Price" + SaleNo.ToString(), Price);
                        param.Add("Quantity" + SaleNo.ToString(), Quantity);
                        param.Add("SIAmount" + SaleNo.ToString(), SIAmount);
                        SaleNo++;
                    }

                    //新增發票作廢，有可能會多張
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        sb.Append("Insert into NullifiedInvoices(PosNo,InvoiceNo,ClerkID,NullifiedTime)  ");
                        sb.Append(string.Format("values(@PosNo,@InvoiceNo{0},@ClerkID,getdate()); ", i));
                        param.Add("InvoiceNo" + i, dt.Rows[i]["InvoiceNo"].ToString());
                    }
                    param.Add("OrderID", OrderID);
                    param.Add("PosNo", PosNo);
                    param.Add("ClerkID", ClerkID);
                    param.Add("ReturnType", ReturnType);

                    //寫入紙本刷退紀錄
                    if (string.IsNullOrEmpty(PaperReturnID) == false)
                    {
                        sb.Append("Insert Into [PosClient]..[ReturnCreditCardPaper] (OrderID) values (@OrderID) ");
                        param.Add("ReturnPaperID", PaperReturnID);
                    }

                    result = DB.DBNonQuery(sb.ToString(), param, "PosClient");
                }
            }

            var ChangeStatusResult = new { result = result };
            ShowResultOnPage(JsonConvert.SerializeObject(ChangeStatusResult));
        }

        #endregion

        #region 重結

        public void ReCheckOut()
        {
            bool status = false;
            int AllowanceID = 0;
            if (!string.IsNullOrEmpty(Request["AllowanceID"]))
            {
                AllowanceID = int.Parse(Request["AllowanceID"]);
            }

            DataTable dt = GetOrderInfo(OrderID);
            if (dt.Rows.Count > 0)
            {
                vip_id = dt.Rows[0]["VIP"].ToString();
                StringBuilder sb = new StringBuilder();
                Dictionary<string, object> param = new Dictionary<string, object>();

                //改訂單狀態為作廢
                if (AllowanceID == 0)
                {
                    sb.Append("update Orders set Status=2,SyncedTime=null where OrderID=@OrderID; ");
                }
                else
                {
                    sb.Append("update Orders set Status=6,SyncedTime=null where OrderID=@OrderID; ");
                    sb.Append("insert into PosClient..Allowances(OrderID,Year,Month,AllowanceID) values(@OrderID,@Year,@Month,@AllowanceID) ");
                    param.Add("Year", DateTime.Now.Year);
                    param.Add("Month", DateTime.Now.Month);
                    param.Add("AllowanceID", AllowanceID);
                }

                sb.Append("delete from OrderItemsWaitCheckOut where OrderID=@OrderID; ");
                param.Add("OrderID", OrderID);
                status = DB.DBNonQuery(sb.ToString(), param, "PosClient");
                if (status)
                {
                    AddOrder();
                    return;
                }
            }

            GetInvoiceNumber(PosNo);
            var result = new { result = status, NumberNow = InvoiceNumberNow, Remainder = InvoiceRemainder};
            ShowResultOnPage(JsonConvert.SerializeObject(result));
        }

        #endregion

        #region 取得訂單

        public static int GetLastOrderID(string PosNo)
        {
            string sql = "select Max(a.OrderID) as OrderID from Orders a where a.PosNo=@PosNo ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PosNo", PosNo);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sql, param, "PosClient");
            return int.Parse(dt.Rows[0]["OrderID"].ToString());
        }

        public static DataTable GetOrderInfo(string OrderID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT a.[OrderID] ,[OrderDate],[OrderTime],[PosNo],[ClerkID],[Amount],[PayType],[AuthorizationCode],CreditCardData,[Status] ,b.InvoiceNo,a.UniformSerialNumber,c.VIP  ");
            sb.Append("FROM [Orders] a ");
            sb.Append("left join OrderInvoices b on a.OrderID=b.OrderID ");
            sb.Append("left join OrderVip c on a.OrderID=c.OrderID  ");
            sb.Append("where a.OrderID=@OrderID ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("OrderID", OrderID);
            DataTable dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            return dt;
        }

        #endregion

        #region 取得發票號碼

        public void GetInvoiceNumber(string PosNo)
        {
            string sql = "SELECT  [PosNo] ,[AlphabeticLetters],[StartNo],[EndNo],[CurrentNo] FROM  InvoiceMachineSettings a where a.PosNo=@PosNo ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PosNo", PosNo);
            DataTable dt = new DataTable();
            dt = DB.DBQuery(sql, param, "PosClient");
            if (dt.Rows.Count > 0)
            {
                InvoiceStartNumber = dt.Rows[0]["AlphabeticLetters"].ToString() + Convert.ToInt32(dt.Rows[0]["StartNo"]).ToString("D8");
                InvoiceNumberNow = dt.Rows[0]["AlphabeticLetters"].ToString() + Convert.ToInt32(dt.Rows[0]["CurrentNo"]).ToString("D8");
                int NowNum = Convert.ToInt32(dt.Rows[0]["CurrentNo"]);
                int EndNum = Convert.ToInt32(dt.Rows[0]["EndNo"]);
                InvoiceRemainder = (EndNum - NowNum + 1).ToString();
            }
        }

        #endregion

        #region 扣數並列印揀貨單

        public bool PrintPickList(string OrderID, int pickType, string InvoiceNumberNow)
        {
            Log.Add(0, OrderID, OrderStep.ToString(), "OrderStep：2，發票列印完成", ClerkID, PosNo);

            bool result = false;

            string[] ItemNo = JsonConvert.DeserializeObject<string[]>(Request["ItemNoList"]);
            string[] color = JsonConvert.DeserializeObject<string[]>(Request["ColorList"]);
            string[] quantity = JsonConvert.DeserializeObject<string[]>(Request["QuantityList"]);
            string[] price = JsonConvert.DeserializeObject<string[]>(Request["PriceList"]);
            var ShipId = int.Parse(OrderID);
            var store = POS_Library.Public.Utility.GetStoreForShop(_areaId);
            List<TicketShelfTemp> ticketTemp = new List<TicketShelfTemp>();

            try
            {

                #region 建立撿貨明細

                var Ticket = 0;
                var Date = DateTime.Today.ToString("yyyy-MM-dd");
                ShipOutDA SO = new ShipOutDA();
                for (int i = 0; i < quantity.Length; i++)
                {
                    TicketShelfTemp detail = new TicketShelfTemp();
                    detail.Account = ClerkName;
                    detail.GuestId = 1;
                    detail.ShipId = ShipId.ToString();
                    detail.ShipoutDate = Date;
                    detail.ProductId = ItemNo[i];
                    detail.Quantity = int.Parse(quantity[i]);
                    detail.ProductColor = color[i];
                    detail.Ticket = Ticket;
                    detail.Store = store;

                    ticketTemp.Add(detail);
                }

                List<TicketShelfTemp> PrintPickListsShelf = new List<TicketShelfTemp>();
                string MachineName = HttpContext.Current.Server.MachineName;

                #endregion

                #region 扣數

                if (pickType != 1)
                {
                    if (pickType == 0)
                        PrintPickListsShelf = SO.PrintPickListsShelf(ticketTemp, ShipId, (int)Utility.ShipPDF.出貨, store, _areaId);

                    if (pickType == 5)
                        PrintPickListsShelf = SO.PrintPickListsShelf(ticketTemp, ShipId, (int)Utility.ShipPDF.出貨含展售, store, _areaId);

                    Log.Add(0, OrderID, "3", "OrderStep：3，撿貨完成", ClerkID, PosNo);

                    //判斷設定檔再決定要不要印撿貨單
                    if (SystemSettings.GetNeedPrintPickSheet()==true)
                    {
                        Print p = new Print();
                        string PrintResult = p.PrintPickList(PrintPickListsShelf, InvoiceNumberNow, "交易明細");

                        if (PrintResult == "Success")
                        { Log.Add(0, OrderID, "4", "OrderStep：4，列印撿貨單完成", ClerkID, PosNo); }
                        else
                        {
                            string Event1 = string.Format("發票號碼:{0}，產生揀貨單失敗!", InvoiceNumberNow);
                            string Event2 = string.Format("產生失敗原因：{0}", PrintResult);
                            Log.Add(5, OrderID, InvoiceNumberNow, Event1 + Event2, ClerkID, PosNo);
                        }
                    }
                    else
                    {
                        var SO2 = new POS_Library.ShopPos.ShipOutDA();
                        var MSListNewFinal = SO2.GetPerformanceSale(ClerkName, (int)POS_Library.Public.Utility.LogisticsType.撿貨, ShipId, _areaId);
                        Log.Add(0, OrderID, "4", "OrderStep：4，交易完成，無需印撿貨單", ClerkID, PosNo);
                    }
                   

                }

                if (pickType == 1)
                {
                    PrintPickListsShelf = SO.PrintPickListsShelf(ticketTemp, ShipId, (int)Utility.ShipPDF.出貨重出, store, _areaId);
                    var SO2 = new POS_Library.ShopPos.ShipOutDA();
                    var MSListNewFinal = SO2.GetPerformanceSale(ClerkName, (int)POS_Library.Public.Utility.LogisticsType.撿貨, ShipId, _areaId);
                    Log.Add(0, OrderID, "3", "OrderStep：3 商品從不可銷庫存扣數完成，不需印檢貨單", ClerkID, PosNo);
                    UpdateReturnsOrderIDRelated();
                }
                result = true;

                #endregion
            }
            catch (Exception ex)
            {
                result = false;
                ErrorMsg = ex.Message;
                string Event1 = string.Format("訂單編號:{0},發票號碼:{1}，揀貨時發生失敗", OrderID, InvoiceNumberNow);
                string Event2 = string.Format("揀貨失敗原因：{0}", ErrorMsg);
                Log.Add(5, OrderID, InvoiceNumberNow, Event1 + Event2, ClerkID, PosNo);

                #region 記錄傳給WMS扣數之前的所有變數資料
                var PickListData = new { ticketTemp = ticketTemp, ShipId = ShipId, pickType = pickType, store = store, int_treasurytype = _areaId };
                string PickListDataString = JsonConvert.SerializeObject(PickListData);
                string FilePath = HttpContext.Current.Server.MapPath(".") + string.Format("\\{0:yyyyMMdd}_{1}.txt", DateTime.Today, ShipId);
                using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(FilePath))
                {
                    streamWriter.WriteLine("{0:yyyy/MM/dd HH:mm:ss ffff}, {1}", DateTime.Now, PickListDataString);
                }
                #endregion

            }
            return result;
        }

        #endregion

        #region  折讓計算

        private bool CheckIsAllowances(DateTime OrderDate)
        {
            DateTime Today = DateTime.Now;
            int OrderPeriod = Convert.ToInt32(Math.Floor((Double)(OrderDate.Month - 1) / 2)) + 1;
            int NowPeriod = Convert.ToInt32(Math.Floor((Double)(Today.Month - 1) / 2)) + 1;
            bool IsSamePeriod = CheckIsSamePeriod(OrderDate, OrderPeriod, NowPeriod);

            if (IsSamePeriod)
            {return false;}

            if(Today.Month % 2 == 0)
            { return true; }

            if (Today.Day >= 5)
            { return true; }

            if (Today.Year - OrderDate.Year > 1)
            { return true; }

            //上一期
            if (Today.Year == OrderDate.Year)
            {
                if (NowPeriod - OrderPeriod == 1)
                { return false; }
            }
            else
            { 
                if(NowPeriod - OrderPeriod == -5)
                { return false; }
            }

            return true;
        }

        private bool CheckIsSamePeriod(DateTime OrderDate, int OrderPeriod, int NowPeriod)
        {
            DateTime Today = DateTime.Today;

            if (Today.Year != OrderDate.Year)
            { return false; }

            if (Today.Month == OrderDate.Month)
            { return true; }
            
            if (OrderPeriod == NowPeriod)
            { return true; }

            return false;
        }

        private int GetNewAllowanceID()
        {
            DateTime Today = DateTime.Now;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT  isNull(Max([AllowanceID]),0) MaxSeqNo ");
            sb.AppendLine("FROM [PosClient].[dbo].[Allowances] a  ");
            sb.AppendLine("where a.[Month]=@Month and a.Year=@Year ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Year", Today.Year);
            param.Add("Month", Today.Month);
            DataTable dt = DB.DBQuery(sb.ToString(), param, "PosClient");
            int AllowanceID = int.Parse(dt.Rows[0][0].ToString());
            if (AllowanceID == 0)
            {
                string no = Today.Year.ToString() + Today.Month.ToString("D2") + "001";
                return int.Parse(no);
            }
            else
            { return ++AllowanceID;}
        }

        #endregion

        private int GetPaperReturnID()
        {
            int PaperReturnID = 0;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT  isNull(Max([PaperID]),0) MaxSeqNo ");
            sb.AppendLine("from PosClient..ReturnCreditCardPaper ");
            Dictionary<string, object> param = new Dictionary<string, object>();
            string sql = sb.ToString();
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            if (dt.Rows.Count > 0)
            {
                PaperReturnID = int.Parse(dt.Rows[0][0].ToString()) + 1;
            }
            return PaperReturnID;
        }

    }

}