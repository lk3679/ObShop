using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using POS_Library.ShopPos.DataModel;
using POS_Library.ShopPos;
using POS_Library.Public;
using System.Web.Configuration;
using Newtonsoft.Json;
using System.Collections;
using System.Xml;

namespace OBShopWeb.Poslib
{

    public class CheckOut
    {
        public class Clerk
        {
            public string ID;
            public string Name;
        }

        #region 交易結帳

        public static bool AddOrder(string PosNo, List<SaleItemData> ItemList, string Amount, string PayType, string ClerkID, string InvoiceNumberNow, string UniformSerialNumber, string ApprovalNo, string CreditCardData, int PickType, string vip_id, int BonusUsed, List<SaleItemData>  ItemListInvoice)
        {
            DateTime current = DateTime.Now;
            string sql = "";
            Dictionary<string, object> param = new Dictionary<string, object>();
            string OrderDate = current.ToString("yyyy/MM/dd");
            string OrderTime = current.ToString("yyyy/MM/dd HH:mm:ss");
            int CurrentNo = int.Parse(InvoiceNumberNow.Substring(2, 8));
            string InvoiceNo = InvoiceNumberNow;
            //CurrentNo++;

            sql = "insert into PosClient.[dbo].Orders  ";
            sql += "(OrderDate,OrderTime,PosNo,ClerkID,Amount,PayType,[Status],UniformSerialNumber,AuthorizationCode,CreditCardData,PickType) values ";
            sql += " (@OrderDate,@OrderTime,@PosNo,@ClerkID,@Amount,@PayType,1,@UniformSerialNumber,@AcceptNo,@CreditCardData,@PickType); ";
            sql += "Declare @OrderID int=@@IDENTITY; ";
            param.Add("OrderDate", OrderDate);
            param.Add("OrderTime", OrderTime);
            param.Add("PosNo", PosNo);
            param.Add("ClerkID", ClerkID);
            param.Add("Amount", Amount);
            param.Add("PayType", PayType);
            param.Add("PickType", PickType);
            
            param.Add("UniformSerialNumber", UniformSerialNumber);
            param.Add("AcceptNo", ApprovalNo);
            param.Add("CreditCardData", CreditCardData);
            int SaleNo = 1;

            foreach (SaleItemData Item in ItemList)
            {
                string ProductId = Item.itemNo;
                int Price = Item.price;
                int Quantity = Item.quantity;
                int SIAmount = Item.aomout;
                //加入購買名細
                sql += "Insert into PosClient.[dbo].OrderItems (OrderID,ProductId,Price,Quantity,Amount) values ";
                sql += string.Format("(@OrderID, @ProductId{0}, @Price{0},@Quantity{0}, @SIAmount{0} )", SaleNo.ToString());


                param.Add("ProductId" + SaleNo.ToString(), ProductId);
                param.Add("Price" + SaleNo.ToString(), Price);
                param.Add("Quantity" + SaleNo.ToString(), Quantity);
                param.Add("SIAmount" + SaleNo.ToString(), SIAmount);
                SaleNo++;
            }

            //計算發票使用張數，日後要分段作發票再拿掉
            int pageSize = 7;
            int TotalNum = ItemListInvoice.Count;
            int AllPage = Convert.ToInt32(Math.Floor((double)(TotalNum - 1) / pageSize) + 1);

            for (int i = 0; i < AllPage; i++)
            {
                sql += string.Format(";Insert into OrderInvoices (OrderID,InvoiceNo) values(@OrderID,@InvoiceNo{0}) ", i);
                param.Add("InvoiceNo" + i, InvoiceNo.Substring(0, 2) + CurrentNo.ToString());
                CurrentNo++;
            }
            sql += ";update InvoiceMachineSettings set CurrentNo=@CurrentNo where PosNo=@PosNo ";
            param.Add("CurrentNo", CurrentNo);

            //如果是VIP，無論是否有使用紅利，都必須多一筆紀錄，方便以後退款的時候可以退紅利
            if (vip_id.Length > 0)
            {
                sql += ";Insert into [PosClient].[dbo].[OrderVip] (OrderID,VIP,BonusUsed) values(@OrderID,@VIP,@BonusUsed) ";
                param.Add("VIP",vip_id);
                param.Add("BonusUsed", BonusUsed);
            }
         

            return DB.DBNonQuery(sql, param, "PosClient");
        }
        #endregion

        #region 取得交易商品

        public static CheckOutProduct GetPosCheckOutItem(string barcode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select a.ProductId,a.BarCode,a.Price,a.Color,a.Size,b.Name,b.SerialId  ");
            sb.Append("from PosClient..Product a  ");
            sb.Append("left join ProductSerial b on a.SerialId=b.SerialId  ");
            sb.Append("where a.SerialId=@barcode  OR  a.BarCode=@barcode ");
            string sql = sb.ToString();

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("barcode", barcode);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            CheckOutProduct Co = new CheckOutProduct();

            if (dt.Rows.Count > 0)
            {
                Co.product_id = dt.Rows[0]["ProductId"].ToString();
                Co.SerialId = dt.Rows[0]["SerialId"].ToString();
                Co.series_name = dt.Rows[0]["Name"].ToString();
                Co.size = dt.Rows[0]["Size"].ToString();
                Co.color = dt.Rows[0]["Color"].ToString();
                Co.barcode = dt.Rows[0]["barcode"].ToString();
                Co.price = int.Parse(dt.Rows[0]["Price"].ToString());

                #region 花車折扣

                //20150612如果為花車商品，直接對原價折價，不走折扣活動
                //bool hasStoreFloatDiscount = false;
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(HttpContext.Current.Server.MapPath("StoreFloats.xml"));
                    XmlNode discountNode = doc.SelectSingleNode("Discount");
                    XmlElement element = (XmlElement)discountNode;
                    int DiscountRate = int.Parse(element.GetAttribute("Rate"));
                    XmlNodeList SerialList = doc.DocumentElement.SelectNodes("/Discount/SerialID");

                    foreach (XmlNode node in SerialList)
                    {
                        string SerialID = node.InnerText;
                        if (Co.SerialId.ToUpper() == SerialID.ToUpper())
                        {
                            //打折
                            int DiscountPrice = Convert.ToInt32(Math.Floor((double)Co.price * DiscountRate / 100));
                            Co.price = DiscountPrice;
                            Co.IsStoreFloat = true;
                            break;
                        }
                    }
                }
                catch (Exception e)
                {
                    string a = e.ToString();
                }

                if (Co.IsStoreFloat == true)
                { return Co; }

                #endregion

                #region 折扣判斷 
                DataTable ActivityDT = Activity.GetActivityToday();

                if (ActivityDT.Rows.Count > 0)
                {
                    foreach (DataRow row in ActivityDT.Rows)
                    {
                        string ActivityProductType = row["ActivityProductType"].ToString();
                        string ActivityID = row["ActivityID"].ToString();
                        DataTable SerialIdDT = Activity.GetSerialID(ActivityID);

                        if (SerialIdDT.Rows.Count == 0)
                        {
                            //沒有明細，全部符合折扣
                            //新品折N元一定要有明細才會生效
                            if (bool.Parse(row["VIPOnly"].ToString()))
                            {
                                Co.VIPDiscountType = row["Type"].ToString();
                                Co.VIPDiscountParam = row["Parameters"].ToString();
                            }
                            else
                            {
                                Co.DiscountType = row["Type"].ToString();
                                Co.DiscountParam = row["Parameters"].ToString();
                            }
                        }
                        else
                        {
                            //有明細，需判斷1.此商品是否有在明細中，2.是包含還是排除
                            IEnumerable<DataRow> query = (from a in SerialIdDT.AsEnumerable()
                                                          where a["ProductSerialID"].ToString().ToUpper() == Co.SerialId.ToUpper()
                                                          select a).ToList();

                            if (ActivityProductType == "1" && query.Count() > 0 || ActivityProductType == "0" && query.Count() == 0)
                            {

                                if (row["Type"].ToString() == "新品折X元")
                                {
                                    List<ParamData> ParamList = JsonConvert.DeserializeObject<List<ParamData>>(row["Parameters"].ToString());
                                    Co.price -= ParamList[0].Discount;
                                }
                                else
                                {
                                    if (bool.Parse(row["VIPOnly"].ToString()))
                                    {
                                        Co.VIPDiscountType = row["Type"].ToString();
                                        Co.VIPDiscountParam = row["Parameters"].ToString();
                                    }
                                    else
                                    {
                                        Co.DiscountType = row["Type"].ToString();
                                        Co.DiscountParam = row["Parameters"].ToString();
                                    }
                                }
                            }
                        }
                    }

                }

                #endregion

                if (Co.price > 5000)
                    Co.price=0;
            }

            return Co;
        }

        public static string GetcheckOutItemString(CheckOutProduct Co, string quantity,string PosNo)
        {
            string htmlTag = "";
            bool HasProuduct = false;

            //儲位狀態
            int DivitionStatus =0;

            //沒有庫存：0，B1有庫存：1，B1無庫存，樓上有庫存 or  B1有庫存但不夠，樓上夠:2
            if (!string.IsNullOrEmpty(Co.product_id))
            {
                HasProuduct = true;

                int Show = GetShowQty(Co.product_id);
                int B1 = GetB1Qty(Co.product_id);
                int TempQuantityOthers = GetTempStorageQuantity(Co.product_id, PosNo);
                int TempQuantity = GetTempStorageQuantity(Co.product_id, "");

                if (B1 - TempQuantity - int.Parse(quantity) >= 0)
                {
                    //B1庫存足夠的情況
                    UpdateStorageQuantity(Co.product_id, PosNo, quantity);
                    DivitionStatus = 1;
                    Co.StkQty = B1 - TempQuantityOthers;
                    Co.ShowQty = Show;
                }
                else
                {
                    if ((B1 - TempQuantity - int.Parse(quantity)) + Show >= 0)
                    {
                        //B1不夠，展示夠的情況
                        UpdateStorageQuantity(Co.product_id, PosNo, quantity);
                        DivitionStatus = 2;
                        if (B1 - TempQuantityOthers >= 0)
                        {
                            //B1減掉其他人的暫量後，庫存足夠
                            Co.StkQty = B1 - TempQuantityOthers;
                            Co.ShowQty = Show;
                        }
                        else
                        {
                            //B1減掉其他人的暫量後，庫存不足夠
                            Co.StkQty = 0;
                            Co.ShowQty = (B1 - TempQuantityOthers) + Show;
                        }
                    }
                    else
                    {
                        UpdateStorageQuantity(Co.product_id, PosNo, quantity);
                        DivitionStatus = 0;
                    }
                }

                Co.quantity = int.Parse(quantity);
                Co.Original_price = Co.price;
                Co.Original_amount = Co.Original_price * int.Parse(quantity);

                //一般會員折扣
                if (Co.DiscountType.Length > 0)
                {
                    List<ParamData> ParamList = JsonConvert.DeserializeObject<List<ParamData>>(Co.DiscountParam);
                    ArrayList PriceList = new ArrayList();
                    foreach (ParamData PD in ParamList)
                    {
                        int DiscountPrice=0;

                        if (Co.DiscountType != "新品折X元")
                            DiscountPrice = Convert.ToInt32(Math.Floor((double)Co.price * PD.Discount / 100));
                        else
                            DiscountPrice = Co.price - PD.Discount;

                        PriceList.Add(DiscountPrice);
                    }

                    Co.priceList = JsonConvert.SerializeObject(PriceList);
                }
                else
                {
                    Co.priceList = "[]";
                }

                //VIP會員折扣
                if (Co.VIPDiscountType.Length > 0)
                {

                    List<ParamData> ParamList = JsonConvert.DeserializeObject<List<ParamData>>(Co.VIPDiscountParam);
                    ArrayList PriceList = new ArrayList();

                    foreach (ParamData PD in ParamList)
                    {
                        int DiscountPrice = Convert.ToInt32(Math.Floor((double)Co.price * PD.Discount / 100));
                        PriceList.Add(DiscountPrice);
                    }

                    Co.VipPriceList = JsonConvert.SerializeObject(PriceList);
                }
                else
                {
                    Co.VipPriceList = "[]";
                }

            }

            htmlTag = ItemToRowString(Co, DivitionStatus);
            var result = new { result = HasProuduct, data = htmlTag, itemNo = Co.product_id, quantity = Co.quantity, original_amount = Co.Original_amount, amount = Co.amount, SkuQty = 2, ShowQty = 1, DivitionStatus = 1};
            //var result = new { result = HasProuduct, data = htmlTag, itemNo = Co.product_id, quantity = Co.quantity, original_amount = Co.Original_amount, amount = Co.amount, SkuQty = Co.StkQty, ShowQty = Co.ShowQty, DivitionStatus = DivitionStatus };
            return JsonConvert.SerializeObject(result);

        }


        //一般結帳
        public static string ItemToRowString(CheckOutProduct Co, int DivitionStatus)
        {
            string htmlTag = "";
            string FontColor = "";
            if (DivitionStatus == 2)
                FontColor = "color:red";

            //贈品
            //if (Co.IsBirthday)
            //    htmlTag += string.Format("<tr class=\"item BirthdayPresent\" id=\"{0}_Item\"> ", Co.product_id);
            //else if (Co.IsPromotion)
            //    htmlTag += string.Format("<tr class=\"item promotion\" id=\"{0}_Item\"> ", Co.product_id);
            //else
            //    htmlTag += string.Format("<tr class=\"item\" id=\"{0}_Item\"> ", Co.product_id);

            htmlTag += string.Format("<tr class=\"item\" id=\"{0}_Item\" data-storefloat =\"{1}\" > ", Co.product_id, Co.IsStoreFloat);
            htmlTag += "<td style=\"width:20px;\" class=\"Delete_img\"></td>";
            htmlTag += string.Format("<td style=\"width:60px;{0}\" class=\"itemNo\">{1}</td>", FontColor, Co.product_id);
            htmlTag += string.Format("<td style=\"width:142px;{0}\">{1}</td>", FontColor, Co.series_name);
            htmlTag += string.Format("<td style=\"width:48px;{0}\"  class=\"color\">{1}</td>", FontColor, Co.color);
            htmlTag += string.Format("<td style=\"width:48px;{0}\">{1}</td>", FontColor, Co.size);
            htmlTag += string.Format("<td style=\"width:48px;{0}\"  id=\"{1}_Qty\" class=\"quantity\">{2}</td>", FontColor, Co.product_id, Co.quantity);
            htmlTag += string.Format("<td style=\"width:48px;{0}\" ><strike style=\"display:none\" class=\"OriginalPrice\">{1}<br/></strike><span class=\"price\">{1}</span><input type=\"hidden\" class=\"discount_price\" value=\"{2}\" /><input type=\"hidden\" class=\"discount_type\" value=\"{3}\" /><input type=\"hidden\" class=\"VIP_discount_price\" value=\"{4}\" /><input type=\"hidden\" class=\"VIP_discount_type\" value=\"{5}\" /></td>", FontColor, Co.Original_price, Co.priceList, Co.DiscountType,Co.VipPriceList,Co.VIPDiscountType);
            htmlTag += string.Format("<td style=\"width:48px;{0}\"   id=\"{1}_Amount\"><strike class=\"OriginalAmount\" style=\"display:none\">{2}<br/></strike><span class=\"amount\">{2}</span></td>", FontColor, Co.product_id, Co.Original_amount);
            htmlTag += string.Format("<td style=\"width:38px;{0}\" id=\"{1}_SkuQty\">{2}</td>", FontColor, Co.product_id, Co.StkQty);
            htmlTag += string.Format("<td style=\"width:38px;{0}\" id=\"{1}_ShowQty\">{2}</td></tr>", FontColor, Co.product_id, Co.ShowQty);

            return htmlTag;
        }

        //退貨用
        public static string ItemToRowString(CheckOutProduct Co, int amount, int DivitionStatus)
        {
            string htmlTag = "";
            string FontColor = "";
            if (DivitionStatus == 2)
                FontColor = "color:red";

            htmlTag += string.Format("<tr class=\"item\" id=\"{0}_Item\"> ", Co.product_id);
            htmlTag += "<td style=\"width:20px;\" class=\"Delete_img\"></td>";
            htmlTag += string.Format("<td style=\"width:60px;{0}\" class=\"itemNo\">{1}</td>", FontColor, Co.product_id);
            htmlTag += string.Format("<td style=\"width:142px;{0}\">{1}</td>", FontColor, Co.series_name);
            htmlTag += string.Format("<td style=\"width:48px;{0}\"  class=\"color\">{1}</td>", FontColor, Co.color);
            htmlTag += string.Format("<td style=\"width:48px;{0}\">{1}</td>", FontColor, Co.size);
            htmlTag += string.Format("<td style=\"width:48px;{0}\"  id=\"{1}_Qty\" class=\"quantity\">{2}</td>", FontColor, Co.product_id, Co.quantity);
            htmlTag += string.Format("<td style=\"width:48px;{0}\" class=\"price\">{1}</td>", FontColor, Co.price);
            htmlTag += string.Format("<td style=\"width:48px;{0}\"   id=\"{1}_Amount\" class=\"amount\">{2}</td>", FontColor, Co.product_id, amount);
            htmlTag += string.Format("<td style=\"width:38px;{0}\" id=\"{1}_SkuQty\">{2}</td>", FontColor, Co.product_id, Co.StkQty);
            htmlTag += string.Format("<td style=\"width:38px;{0}\" id=\"{1}_ShowQty\">{2}</td></tr>", FontColor, Co.product_id, Co.ShowQty);

            return htmlTag;
        }
        #endregion

        #region 取得該商品的同系列商品

        public static List<CheckOutProduct> GetTheSameSerialItemByProductID(string ProductID, string PosNo)
        {
            StringBuilder s = new StringBuilder();
            s.Append("select a.SerialId,b.ProductId,a.Name,a.RefSerialId,b.SerialId,b.BarCode,b.Color,b.Size,b.Price  ");
            s.Append("from posclient..ProductSerial a   ");
            s.Append("join Product b on a.SerialId=b.SerialId  ");
            s.Append("where (a.SerialId=(select c.SerialId from Product c where c.ProductId=@ProductID) or   ");
            s.Append("RefSerialId=(select c.SerialId from Product c where c.ProductId= @ProductID) )  ");
            s.Append("and b.ProductId!=@ProductID  ");
            s.Append("order by ProductId  ");
            string sql = s.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("ProductID", ProductID);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");

            List<CheckOutProduct> CoList = new List<CheckOutProduct>();

            foreach(DataRow dr in dt.Rows){
                CheckOutProduct Co = new CheckOutProduct();
                Co.product_id = dr["ProductId"].ToString();
                Co.series_name = dr["Name"].ToString();
                Co.size = dr["Size"].ToString();
                Co.color = dr["Color"].ToString();
                Co.barcode = dr["barcode"].ToString();
                Co.price = int.Parse(dr["Price"].ToString());

                int Show = GetShowQty(Co.product_id);
                int B1 = GetB1Qty(Co.product_id);
                int TempQuantityOthers = GetTempStorageQuantity(Co.product_id, PosNo);
                int TempQuantity = GetTempStorageQuantity(Co.product_id, "");

                if (B1 - TempQuantity>= 0)
                {
                    Co.StkQty = B1 - TempQuantityOthers;
                    Co.ShowQty = Show;
                }
                else
                {
                    if ((B1 - TempQuantity )+ Show >= 0)
                    {
                        //B1不夠，展示夠的情況
                        if (B1 - TempQuantityOthers >= 0)
                        {
                            //B1減掉其他人的暫量後，庫存足夠
                            Co.StkQty = B1 - TempQuantityOthers;
                            Co.ShowQty = Show;
                        }
                        else
                        {
                            //B1減掉其他人的暫量後，庫存不足夠
                            Co.StkQty = 0;
                            Co.ShowQty = (B1 - TempQuantityOthers) + Show;
                        }
                    }

                }

                if (Co.price > 5000)
                    Co.price = 0;

                CoList.Add(Co);
            }

            return CoList;
        }
        
        #endregion

        #region 取得退貨商品
        public static string GetOrderItemByOrderID(string OrderID,int status)
        {
            string result = "";

            StringBuilder s = new StringBuilder();
            
            s.Append("select a.Status, a.OrderID,b.ProductId,d.Name ProductName,c.Color,c.Size size,b.Quantity,b.Price,b.Amount from Orders a   ");

            if (status != 3)
                s.Append("left join OrderItems b on a.OrderID=b.OrderID  ");
            else
                s.Append("left join OrderItemsWaitCheckOut b on a.OrderID=b.OrderID  ");

            s.Append("left join Product c on b.ProductId=c.ProductId ");
            s.Append("left join ProductSerial d on c.SerialId=d.SerialId ");
            s.Append("where a.OrderID=@OrderID ");
            string sql = s.ToString();

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("OrderID", OrderID);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            List<CheckOutProduct> OrderItemList = new List<CheckOutProduct>();
            foreach (DataRow row in dt.Rows)
            {
                CheckOutProduct Co = new CheckOutProduct();
                Co.product_id = row["ProductId"].ToString();
                Co.series_name = row["ProductName"].ToString();
                Co.color = row["Color"].ToString();
                Co.size = row["Size"].ToString();
                Co.price = int.Parse(row["Price"].ToString());
                Co.quantity= int.Parse(row["Quantity"].ToString());
                Co.StkQty =0;
  
                int amount = Convert.ToInt16(row["Amount"].ToString());
                result += ItemToRowString(Co, amount,0);
            }
            return result;
        }
        #endregion

        #region 占量庫存
        
        public static  int GetTempStorageQuantity(string PruductID,string PosNo)
        {
            string sql = "select isNull(sum(a.Amount),0) TempQuantity from TempStorage a where a.ProductID=@PruductID ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PruductID", PruductID);
            
            if(!string.IsNullOrEmpty(PosNo)){
                sql+="and a.PosNo!=@PosNo";
                param.Add("PosNo", PosNo);
            }
            
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            return Convert.ToInt32(dt.Rows[0][0]);
        }

        public static bool UpdateStorageQuantity(string PruductID, string PosNo, string Quantity)
        {
            string sql = "select * from TempStorage a where a.PosNo=@PosNo and a.ProductID=@PruductID ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PruductID", PruductID);
            param.Add("PosNo", PosNo);
            param.Add("Quantity", Quantity);

            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            if (dt.Rows.Count > 0)
                sql = "update TempStorage set Amount+=@Quantity,Time=GetDate() where PosNo=@PosNo and ProductID=@PruductID ";
            else
                sql = "insert into TempStorage(PosNo,ProductID,Amount,Time) values(@PosNo,@PruductID,@Quantity,GetDate()) ";

            return DB.DBNonQuery(sql, param, "PosClient");
        }

        public static bool ClearTempStorage(string PosNo, string PruductID)
        {
            string sql = "delete from TempStorage where PosNo=@PosNo ";

            if (!string.IsNullOrEmpty(PruductID))
            {
                sql += " and ProductID=@PruductID";
            }

            sql += ";delete from PosClient..TempStorage where  DATEDIFF(MINUTE,[Time],GETDATE())>30 ";

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PosNo", PosNo);
            param.Add("PruductID", PruductID);
            return DB.DBNonQuery(sql, param, "PosClient");
        }

        #endregion

        #region 取得庫存
       
        public static int GetB1Qty(string ProductID)
        {
            //儲位所在地
            int _areaId = int.Parse(Area.WmsAreaXml("Area"));
            ShelfProcess sp = new ShelfProcess();
            int[] B1儲位 = { (int)EnumData.StorageType.標準儲位, (int)EnumData.StorageType.超散貨儲位 };
            var B1 = sp.ShelfGroupList(ProductID, -1, _areaId, false).Where(x => B1儲位.Contains(x.StorageTypeId)).ToList().Sum(x => x.Quantity);
            return B1;
        }

        public static int GetShowQty(string ProductID)
        {
            //儲位所在地
            int _areaId = int.Parse(Area.WmsAreaXml("Area"));
            ShelfProcess sp = new ShelfProcess();
            var Show = sp.ShelfGroupList(ProductID, (int)EnumData.StorageType.展售儲位, _areaId, false).ToList().Sum(x => x.Quantity);
            return Show;
        }
        #endregion

        #region 折扣 
        public static int GetTouristsDiscountAmount()
        {
            int DiscountAmount =0 ;         
            string StartTimeString = WebConfigurationManager.AppSettings.Get("DiscountStartTime");
            string EndTimeString = WebConfigurationManager.AppSettings.Get("DiscountEndTime");
            DateTime StartTime;
            DateTime EndTime;

            if (DateTime.TryParse(StartTimeString, out StartTime) && DateTime.TryParse(EndTimeString, out EndTime))
            {
                if (DateTime.Now > StartTime && DateTime.Now < EndTime)
                {
                    int.TryParse(WebConfigurationManager.AppSettings.Get("DiscountAmount"), out DiscountAmount);
                }
            }

            return DiscountAmount;
        }

        public static int GetTouristsDiscountLimit()
        {
            int DiscountLimit = 0;
            int.TryParse(WebConfigurationManager.AppSettings.Get("DiscountLimit"), out DiscountLimit);
            return DiscountLimit;
        }

        public static string GetDiscountType(int VIPOnly)
        {
            string sql = "SELECT [Parameters],a.Type FROM [PosClient].[dbo].[Activities] a  ";
            sql += "where  GETDATE()>= a.StartDate and GETDATE()<a.EndDate+1 and VIPOnly=@VIPOnly and  Type!='新品折X元' ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("VIPOnly", VIPOnly);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["Type"].ToString();
            else
                return "";
        }

        public static string GetDiscountLimit(int VIPOnly)
        {
            string sql = "SELECT [Parameters],a.Type FROM [PosClient].[dbo].[Activities]  a ";
            sql+="where Type!='新品折X元'  and   GETDATE()>= a.StartDate and GETDATE()<a.EndDate+1 and VIPOnly=@VIPOnly";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("VIPOnly", VIPOnly);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            ArrayList LimitList = new ArrayList();

            if (dt.Rows.Count > 0){
                List<ParamData> ParamList = JsonConvert.DeserializeObject<List<ParamData>>(dt.Rows[0]["Parameters"].ToString());
                foreach (ParamData PD in ParamList)
                {
                    int Limit = PD.Limit;
                    LimitList.Add(Limit);
                }
            }

            return JsonConvert.SerializeObject(LimitList);
           
        }

        public static double GetVIPDiscount()
        {
            double Discount = 1;
            string sql = "select a.[Parameters] from PosClient..Activities a ";
            sql += " where GETDATE()>= a.StartDate and GETDATE()<a.EndDate+1 and  VIPOnly=1  and  Type!='新品折X元' ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            DataTable dt = DB.DBQuery(sql, param, "PosClient");

            if (dt.Rows.Count > 0)
            {
                dynamic d = JsonConvert.DeserializeObject<dynamic>(dt.Rows[0]["Parameters"].ToString());
                Discount = (double)d[0].Discount/ 100;
            }

            return Discount;
        }

        #endregion

        #region 取得促銷商品

        public static string GetPromotionItemString(int amount, bool IsVip, bool IsBirthday, string PosNo)
        {
            string htmlTag = "";
            DataTable promotionDT = get_pos_promotion(amount, IsVip, IsBirthday);
            ArrayList ItemList = new ArrayList();

            foreach (DataRow row in promotionDT.Rows)
            {
                string SerialId = row["product_id"].ToString().Trim();
                CheckOutProduct Co = GetPosCheckOutItem(SerialId);
                int.TryParse(row["bonus_multiplication"].ToString(), out Co.bonus_multiplication);
                Co.price = 0;

                if (IsBirthday)
                    Co.IsBirthday = true;
                else
                    Co.IsPromotion = true;
               
                string ItemString = GetcheckOutItemString(Co, "1", PosNo);
                dynamic d = JsonConvert.DeserializeObject<dynamic>(ItemString);
                ItemList.Add(d);
            }

            htmlTag = JsonConvert.SerializeObject(ItemList);
            return htmlTag;
        }

        public static DataTable get_pos_promotion(int amount, bool IsVip, bool IsBirthday)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "select pos_promotion.*, 原始貨號 product_name from pos_promotion ";
            sql += "inner join 產品系列 on product_id = 系列編號 ";
            sql += "where start_date <=@now and end_date >=@now ";

            if (IsBirthday)
            {
                //只取得壽星禮
                sql += "and vip=1 and birthday=1 ";
            }
            else
            {
                //取得滿額禮
                if (IsVip)
                    sql += "and @amount>amount  and (vip=1 OR vip=0) ";
                else
                    sql += "and @amount>amount  and vip=0";
            }

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("now", now);
            param.Add("amount", amount);
            DataTable dt = DB.DBQuery(sql, param, "orangebear");
            return dt;
        }

        #endregion
    }

    public class CheckOutProduct
    {
        public string product_id = "";
        public string SerialId = "";
        public string series_name = "";
        public string size = "";
        public string color = "";
        public string barcode = "";
        public int StkQty = 0;
        public int ShowQty = 0;
        public int price = 0;
        public string priceList = "";
        public string VipPriceList = "";
        public int Original_price = 0;
        public int amount = 0;
        public int Original_amount = 0;
        public int quantity = 0;
        public string saleitem = "";
        public string DiscountType = "";
        public string VIPDiscountType = "";
        public string DiscountParam = "";
        public string VIPDiscountParam = "";
        public bool IsPromotion = false;
        public bool IsBirthday = false;
        public int bonus_multiplication = 1;
        public bool IsStoreFloat = false;
    }

    public class SaleItemData
    {
        public string itemNo;
        public int quantity;
        public int price;
        public int aomout;
    }

    public class ParamData
    {
        public int Limit;
        public int Discount;
    }
}