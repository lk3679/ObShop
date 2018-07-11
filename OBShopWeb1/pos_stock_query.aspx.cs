using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;
using System.Text;
using System.Data;

namespace OBShopWeb
{
    public partial class pos_stock_query : System.Web.UI.Page
    {
        public string act = "";
        public string key = "";
        public int QueryType;
        public string product_id = "";
        public string shelf = "";
        public string quantity = "";
        public string gap = "";
        public string destination = "";
        public string series_id = "";
        public StockData sd;
        public string QueryResultMsg;

        protected void Page_Load(object sender, EventArgs e)
        {
            act = (!string.IsNullOrEmpty(Request["act"])) ? Request["act"] : "";
            key = (!string.IsNullOrEmpty(Request["key"])) ? Request["key"] : "";
            QueryType = (!string.IsNullOrEmpty(Request["QueryType"])) ? int.Parse(Request["QueryType"]) : 0;
            product_id = (!string.IsNullOrEmpty(Request["product_id"])) ? Request["product_id"] : "";
            shelf = (!string.IsNullOrEmpty(Request["shelf"])) ? Request["shelf"] : "";
            quantity = (!string.IsNullOrEmpty(Request["quantity"])) ? Request["quantity"] : "";
            gap = (!string.IsNullOrEmpty(Request["gap"])) ? Request["gap"] : "";
            destination = (!string.IsNullOrEmpty(Request["destination"])) ? Request["destination"] : "";
            QueryResultMsg = "";

           if (key!= "")
            {
                sd = Stock.GrtPosStock(key, QueryType);
                series_id = sd.series_id;
                if (sd.stockDT==null)
                {
                    QueryResultMsg = "查無產品資料";
                }
            }
        }


    }
}