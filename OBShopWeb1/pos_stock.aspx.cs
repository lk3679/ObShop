using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class pos_stock : System.Web.UI.Page
    {
        public string act = "";
        public string barcode = "";
        public string product_id = "";
        public string shelf = "";
        public string quantity = "";
        public string gap = "";
        public string destination = "";
        public string series_id = "";
        public StockData sd;

        protected void Page_Load(object sender, EventArgs e)
        {
            act = (!string.IsNullOrEmpty(Request["act"])) ? Request["act"] : "";
            barcode = (!string.IsNullOrEmpty(Request["barcode"])) ? Request["barcode"] : "";
            product_id = (!string.IsNullOrEmpty(Request["product_id"])) ? Request["product_id"] : "";
            shelf = (!string.IsNullOrEmpty(Request["shelf"])) ? Request["shelf"] : "";
            quantity = (!string.IsNullOrEmpty(Request["quantity"])) ? Request["quantity"] : "";
            gap = (!string.IsNullOrEmpty(Request["gap"])) ? Request["gap"] : "";
            destination = (!string.IsNullOrEmpty(Request["destination"])) ? Request["destination"] : "";

            if (barcode != "")
            {
                sd = OBShopWeb.Poslib.Stock.get_pos_stock(barcode);
                series_id = sd.series_id;

                //if (sd.stockDT.Rows.Count > 0)
                //{
                //    DataView dv = new DataView(sd.stockDT);
                //    dv.RowFilter = "quantity>0";
                //    sd.stockDT = dv.ToTable();
                //}
                
            }

             if (product_id != "")
            {
                sd = OBShopWeb.Poslib.Stock.get_pos_stock_by_product_id(product_id);
                series_id = sd.series_id;

                //if (sd.stockDT.Rows.Count > 0)
                //{
                //    DataView dv = new DataView(sd.stockDT);
                //    dv.RowFilter = "quantity>0";
                //    sd.stockDT = dv.ToTable();
                //}

            }



        }
    }
}