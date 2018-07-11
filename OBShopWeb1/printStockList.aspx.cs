using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;
using iTextSharp.tool.xml;
using System.Net;
using OBShopWeb.Poslib;
using BarcodeLib;
using System.Collections;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using Newtonsoft.Json;


namespace OBShopWeb
{
    public partial class printStockList : System.Web.UI.Page
    {
        public List<CheckOutProduct> CoList=new List<CheckOutProduct>();
        public string ImgUrl = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            PrintersList();
            GetData();
        }

        public void GetData()
        {
            StringBuilder s = new StringBuilder();
            s.Append("select a.Status, a.OrderID,c.BarCode,b.ProductId,d.Name ProductName,c.Color,c.Size size,b.Quantity,c.Price,b.Amount from Orders a   ");
            s.Append("left join OrderItems b on a.OrderID=b.OrderID  ");
            s.Append("left join Product c on b.ProductId=c.ProductId ");
            s.Append("left join ProductSerial d on c.SerialId=d.SerialId ");
            s.Append("where a.OrderID=@OrderID ");
            string sql = s.ToString();

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("OrderID", 1104);
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            foreach (DataRow row in dt.Rows)
            {
                CheckOutProduct Co = new CheckOutProduct();
                Co.product_id = row["ProductId"].ToString();
                Co.series_name = row["ProductName"].ToString();
                Co.color = row["Color"].ToString();
                Co.size = row["Size"].ToString();
                Co.price = int.Parse(row["Price"].ToString());
                Co.quantity = int.Parse(row["Quantity"].ToString());
                Co.barcode = row["BarCode"].ToString();
                Co.StkQty = 0;
                CoList.Add(Co);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            PrinterSettings ps = new PrinterSettings();
            var Size = ps.PaperSizes;
            Response.Write(JsonConvert.SerializeObject(Size));
            //Print Print = new Print();
            //Print.PrintPickList(CoList);
        }

        private void PrintersList()
        {
            foreach (string sPrintName in PrinterSettings.InstalledPrinters)
            {
                lbPrinter.Items.Add(sPrintName);
            }
            PrintDocument printDoc = new PrintDocument();
            lbPrinter.SelectedItem.Text = printDoc.DefaultPageSettings.PrinterSettings.PrinterName;
        }

    }


}