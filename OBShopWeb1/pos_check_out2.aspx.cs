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
    public partial class pos_check_out2 : System.Web.UI.Page
    {
        public string InvoiceNumberNow = "";
        public string InvoiceStartNumber = "";
        public string InvoiceRemainder = "";
        public string PosNo = "9";
        public string MachineNo = "P9";
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
        public Double DiscountRate = 1.0;
        public string DiscountLimit = "";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

    }

}