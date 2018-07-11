using System;
using System.Collections.Generic;
using System.Linq;
using System.Web; 
using System.Text;
using System.Data;
using System.Web.Configuration;
using System.Xml;


namespace OBShopWeb.Poslib
{
    public class Invoice
    {
        public enum PrintResult
        {
            Error = 0,
            Success = 1,
            PrinterError = 2
        }

        public string printStart(int PayType, int invoicePrice, int OriginalAmount, string invoiceNo, string OrderID, List<SaleItemData> ItemList, string PosNo, string received, string change, string CardNo, string AcceptNo, int pageNow, bool LastPage, int TotalQuantity)
        {
            List<string> listHead = InvoiceContent(PayType, invoicePrice, OriginalAmount, invoiceNo, OrderID, ItemList, PosNo, received, change, CardNo, AcceptNo, pageNow, LastPage, TotalQuantity);
            var invAll = new List<string>();
            var inv = string.Empty;
            for (int j = 0; j < listHead.Count; j++)
            {
                inv += listHead[j].ToString();
            }

            return inv;
        }

        public List<string> InvoiceContent(int PayType,int invoicePrice,int OriginalAmount,string invoiceNo, string OrderID, List<SaleItemData> ItemList, string PosNo, string received, string change, string CardNo, string AcceptNo, int pageNow, bool LastPage, int TotalQuantity)
        {

            List<string> listHead = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.Load(HttpContext.Current.Server.MapPath("InvoiceInfo.xml"));
            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/shop/store");

            if (nodes.Count > 0)
            {
                string title1 = nodes[0].SelectSingleNode("title1").InnerText;
                string title2 = nodes[0].SelectSingleNode("title2").InnerText;
                string busNo = nodes[0].SelectSingleNode("busNo").InnerText;
                string tel = nodes[0].SelectSingleNode("tel").InnerText;
                string address1 = nodes[0].SelectSingleNode("address1").InnerText;
                string address2 = nodes[0].SelectSingleNode("address2").InnerText;

                string datetime = (int.Parse(DateTime.Now.Year.ToString()) - 1911).ToString() + DateTime.Now.ToString("/MM/dd HH:mm");
                List<string> data = new List<string>();
                listHead.Add(string.Format("  {0}\n", title1));
                listHead.Add(string.Format("       {0}\n", title2));
                SLine(ref data, busNo + tel, 24);
                SLine(ref data, address1 + address2, 24);
                SLine(ref data, string.Format("{0}機0{1}頁{2}", datetime, PosNo, pageNow.ToString().PadLeft(3, '0')), 24);
                SLine(ref data, string.Format(" 訂單編號：{0}", OrderID), 24);
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^\d{8}$");
                invoiceNo = reg.Match(invoiceNo.Trim()).Value;

                foreach (var item in data)
                {
                    listHead.Add(item);
                }

                if (invoiceNo != "")
                {
                    listHead.Add(string.Format(" 統一編號:{0}\n", invoiceNo));
                }

                listHead.Add(string.Format("                                        \n"));

                foreach (SaleItemData SD in ItemList)
                {
                    //產品編號補空白
                    string productName = SD.itemNo;
                    for (int i = 0; i < 15 - productName.Length; i++)
                        productName += " ";

                    if (SD.itemNo != "紅利")
                    {
                        string productQuantity = SD.quantity.ToString("D2");
                        int productMoney = SD.quantity * SD.price;
                        listHead.Add(string.Format("{0} x{1} {2}\n", productName, productQuantity, productMoney));
                    }
                    else
                    {
                        listHead.Add(string.Format("{0}       {1}\n", productName, SD.aomout));
                    }

                }

                if (LastPage)
                {
                    listHead.Add(string.Format("                                        \n"));
                    listHead.Add(string.Format("合計件數      x{0} {1}TX\n", TotalQuantity.ToString("D2"), OriginalAmount));
                    if (OriginalAmount > invoicePrice)
                    { listHead.Add(string.Format("折扣價            {0}TX\n", invoicePrice)); }

                    if (PayType == 1)
                    {
                        listHead.Add(string.Format("現金  ${0}\n", received));
                        listHead.Add(string.Format("找零  ${0}\n", change));
                    }

                    if (PayType == 2)
                    {
                        listHead.Add(string.Format("卡號:{0}\n", CardNo));
                        listHead.Add(string.Format("授權碼:{0}\n", AcceptNo));
                    }
                }

            }

            return listHead;
        }

        #region 發票指的每一列規格(限字數)
        public void SLine(ref List<string> data, string str, int length)
        {
            //斷行字元數
            //int length = 24;
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"[\S\s]{1," + length.ToString() + "}");
            var getMats = r.Matches(str);
            string temp = string.Empty;
            int j = 0;

            for (int k = 0; k < getMats.Count; k++)
            {
                var str1 = temp + getMats[k].Value;
                temp = string.Empty;

                int b = System.Text.Encoding.Default.GetBytes(str1).Length - length;
                if (b > 0)
                {
                    for (int i = str1.Length; i > 0; i--)
                    {
                        if (System.Text.Encoding.Default.GetBytes(str1.Substring(0, i)).Length <= length)
                        {
                            j = i;
                            break;
                        }
                    }
                    temp = str1.Substring(j, str1.Length - j);
                    data.Add(str1.Substring(0, j) + "\n");
                }
                else if (b <= 0) data.Add(str1 + "\n");
            }
            if (!string.IsNullOrEmpty(temp)) SLine(ref data, temp, length);
        }
        #endregion

        #region 發票指的每一列規格
        public void SLine(ref List<string> data, string str)
        {
            //斷行字元數
            int length = 24;
            System.Text.RegularExpressions.Regex r =
                new System.Text.RegularExpressions.Regex(@"[\S\s]{1," + length.ToString() + "}");
            var getMats = r.Matches(str);
            string temp = string.Empty;
            int j = 0;

            for (int k = 0; k < getMats.Count; k++)
            {
                var str1 = temp + getMats[k].Value;
                temp = string.Empty;

                int b = System.Text.Encoding.Default.GetBytes(str1).Length - length;
                if (b > 0)
                {
                    for (int i = str1.Length; i > 0; i--)
                    {
                        if (System.Text.Encoding.Default.GetBytes(str1.Substring(0, i)).Length <= length)
                        {
                            j = i;
                            break;
                        }
                    }
                    temp = str1.Substring(j, str1.Length - j);
                    data.Add(str1.Substring(0, j) + "\n");
                }
                else if (b <= 0) data.Add(str1 + "\n");
            }
            if (!string.IsNullOrEmpty(temp)) SLine(ref data, temp);
        }
       #endregion

       

        #region 發票管理

        public static int UpdateInvoiceNumberNow(string PosNo, string InvoiceNumberNow, string ClerkID)
        {
            int stasus = 0;
            if (CheckInvoiceIsUsed(InvoiceNumberNow))
            {
                stasus = -1;
            }
            else
            {
                int CurrentNo = int.Parse(InvoiceNumberNow.Substring(2, 8));
                string sql = "Update InvoiceMachineSettings set CurrentNo=@CurrentNo where PosNo=@PosNo ";
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("PosNo", PosNo);
                param.Add("CurrentNo", CurrentNo);
                Log.Add(6, "修改發票為：" + InvoiceNumberNow, "", "修正發票", ClerkID, PosNo);
                bool result=DB.DBNonQuery(sql, param, "PosClient");
                if (result)
                { stasus = 1; }
                else
                { stasus = -2; }
            }

            return stasus;
        }

        public static bool SettingInvoiceNumber(string PosNo, string InvoiceStartNumber, string InvoiceEndNumber, string ClerkID)
        {
            string AlphabeticLetters = InvoiceStartNumber.Substring(0, 2);
            int StartNo = int.Parse(InvoiceStartNumber.Substring(2, 8));
            int EndNo = int.Parse(InvoiceEndNumber.Substring(2, 8));
            string sql = "Delete InvoiceMachineSettings where PosNo =@PosNo;  ";
            sql += "Insert into InvoiceMachineSettings(PosNo,AlphabeticLetters,StartNo,EndNo,CurrentNo) ";
            sql += "values(@PosNo,@AlphabeticLetters,@StartNo,@EndNo,@StartNo) ";

            sql += "insert into InvoiceRolls(AlphabeticCode,StartingNumber,EndingNumber,ClerkID,PosNo,OperateTime)  ";
            sql += " values(@AlphabeticLetters,@StartNo,@EndNo,@ClerkID,@PosNo,getDate())  ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("PosNo", PosNo);
            param.Add("AlphabeticLetters ", AlphabeticLetters);
            param.Add("StartNo", StartNo);
            param.Add("EndNo", EndNo);
            param.Add("ClerkID", ClerkID);
            Log.Add(7, "發票開頭：" + InvoiceStartNumber, "發票結尾：" + InvoiceEndNumber, "更換整捲發票", ClerkID, PosNo);
            return DB.DBNonQuery(sql, param, "PosClient");

        }

        //作廢發票
        public static int VoidedInvoice(string PosNo, string InvoiceNo,string ClerkID)
        {
            int stasus = 0;
            if (CheckIncoiceIsNullified(InvoiceNo))
            {
                stasus = -1;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Insert into NullifiedInvoices(PosNo,InvoiceNo,ClerkID,NullifiedTime)  ");
                sb.Append("values(@PosNo,@InvoiceNo,@ClerkID,getdate()); ");
                string sql = sb.ToString();
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("PosNo", PosNo);
                param.Add("InvoiceNo", InvoiceNo);
                param.Add("ClerkID", ClerkID);
                Log.Add(8, "作廢發票為：" + InvoiceNo, "", "作廢發票", ClerkID, PosNo);
                bool result = DB.DBNonQuery(sql, param, "PosClient");
                if (result)
                { stasus = 1; }
                else
                { stasus = -2; }
            }
            return stasus;
        }

        #endregion


        //建立訂單使用之發票
        public bool AddOrderInvoice(string OrderID,int pageSize, int TotalNum, int AllPage, string InvoiceNo,string PosNo)
        {
            string sql="";
            int CurrentNo = int.Parse(InvoiceNo.Substring(2, 8));
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("OrderID", OrderID);

            sql = "update PosClient..Orders set Status=1 where OrderID=@OrderID ";

            for (int i = 0; i < AllPage; i++)
            {
                sql += string.Format("Insert into OrderInvoices (OrderID,InvoiceNo) values(@OrderID,@InvoiceNo{0}) ", i);
                
                param.Add("InvoiceNo" + i, InvoiceNo.Substring(0, 2) + CurrentNo.ToString());
                CurrentNo++;
            }
            sql += ";update InvoiceMachineSettings set CurrentNo=@CurrentNo where PosNo=@PosNo ";
            param.Add("CurrentNo", CurrentNo);
            param.Add("PosNo", PosNo);

            return DB.DBNonQuery(sql, param, "PosClient");
        
        }


        //檢查訂發票是否已被使用
        public static bool CheckInvoiceIsUsed(string InvoiceNo)
        {
            string sql = "Select * from PosClient..OrderInvoices a where a.InvoiceNo=@InvoiceNo   ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("InvoiceNo", InvoiceNo);
            DataTable dt=DB.DBQuery(sql, param, "PosClient");
            return (dt.Rows.Count > 0);
        }

        //檢查訂發票是已作廢過
        public static bool CheckIncoiceIsNullified(string InvoiceNo)
        {
            string sql = "Select * from PosClient..NullifiedInvoices a where a.InvoiceNo=@InvoiceNo   ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("InvoiceNo", InvoiceNo);
            DataTable dt=DB.DBQuery(sql, param, "PosClient");
            return (dt.Rows.Count > 0);
        }
    }


    
}