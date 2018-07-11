using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace OBShopWeb.Poslib
{
    public class Stock
    {
      
        #region 舊門市取得庫存

        public static StockData get_pos_stock(string barcode)
        {
            string sql="select 產品系列.系列編號 series_id, 產品系列.關連系列編號 _series_id,產品資料.產品編號 product_id,系列名稱 series_name, ";
            sql+="縮圖 + 圖片1 img1,縮圖 + 圖片2 img2,縮圖 + 圖片3 img3, ";
            sql += "縮圖 + 圖片4 img4,縮圖 + 圖片5 img5,縮圖 + 圖片6 img6, ";
            sql += "縮圖 + 圖片7 img7,縮圖 + 圖片8 img8 ";
            sql+="from 產品系列 ";
            sql+="inner join 產品資料 on 產品系列.系列編號 = 產品資料.系列編號 ";
            sql+="inner join XPPOS.[dbo].Item on 產品資料.產品編號 = ItemMark  ";
            sql += "where ItemNo = @barcode  ";

            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("barcode", barcode);
            DataTable dt = DB.DBQuery(sql, param, "orangebear");
            StockData sd = new StockData();
            if (dt.Rows.Count > 0)
            {
                sd.series_id = dt.Rows[0]["series_id"].ToString();
                sd._series_id = dt.Rows[0]["_series_id"].ToString();
                sd.product_id = dt.Rows[0]["product_id"].ToString();
                sd.series_name = dt.Rows[0]["series_name"].ToString();
                sd.img1 = dt.Rows[0]["img1"].ToString();
                sd.img2 = dt.Rows[0]["img2"].ToString();
                sd.img3 = dt.Rows[0]["img3"].ToString();
                sd.img4 = dt.Rows[0]["img4"].ToString();
                sd.img5 = dt.Rows[0]["img5"].ToString();
                sd.img6 = dt.Rows[0]["img6"].ToString();
                sd.img7 = dt.Rows[0]["img7"].ToString();
                sd.img8 = dt.Rows[0]["img8"].ToString();


                sql = "select 系列編號 series_id, 產品編號 product_id, ItemNo barcode,尺寸 size, 顏色 color, StkQty quantity, 下標價 price,StockAllocation.Quantity stock ";
                sql += "from 產品資料  ";
                sql += " inner join orangebear.dbo.StockAllocation on 產品資料.產品編號 = StockAllocation.ProductId ";
                sql += " inner join XPPOS.[dbo].Item on 產品編號 = ItemMark ";
                sql += " where (系列編號 = @series_id or 系列編號 =@_series_id) and ";
                sql += "(產品編號 <> @series_id or 產品編號 <> @_series_id) ";
                sql += "and StockAllocation.StockType =3 order by 產品編號";
                param.Add("series_id", sd.series_id);
                param.Add("_series_id", sd._series_id);
                sd.stockDT = DB.DBQuery(sql, param, "orangebear");
            }
            return sd;
        }

        public static StockData get_pos_stock_by_product_id(string product_id)
        {
            string sql="select top 1 產品系列.系列編號 series_id,產品系列.關連系列編號 _series_id,產品資料.產品編號 product_id,系列名稱 series_name,  ";
            sql+="縮圖 + 圖片1 img1,縮圖 + 圖片2 img2,縮圖 + 圖片3 img3 ";
            sql+="from 產品系列 ";
            sql+="inner join 產品資料 on 產品系列.系列編號 = 產品資料.系列編號 ";
            sql += "where 產品資料.產品編號 like @product_id+'%'   ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("product_id", product_id);
            DataTable dt = DB.DBQuery(sql, param, "orangebear");
            StockData sd = new StockData();
            if(dt.Rows.Count>0){
                sd.series_id = dt.Rows[0]["series_id"].ToString();
                sd._series_id = dt.Rows[0]["_series_id"].ToString();
                sd.product_id = dt.Rows[0]["product_id"].ToString();
                sd.series_name = dt.Rows[0]["series_name"].ToString();
                sd.img1 = dt.Rows[0]["img1"].ToString();
                sd.img2 = dt.Rows[0]["img2"].ToString();
                sd.img3 = dt.Rows[0]["img3"].ToString();

                sql = "select 產品編號 product_id, ItemNo barcode, ";
                sql += "尺寸 size, 顏色 color, StkQty quantity, 下標價 price,  ";
                sql += "Yahoo上架數量 + 庫存數量 stock from 產品資料 ";
                sql += "inner join XPPOS.[dbo].Item on 產品編號 = ItemMark ";
                sql += " where (系列編號 = @series_id or 系列編號 =@_series_id) and ";
                sql += "(產品編號 <> @series_id or 產品編號 <> @_series_id) ";
                sql += "order by 產品編號";
                param.Add("series_id", sd.series_id);
                param.Add("_series_id", sd._series_id);
                sd.stockDT = DB.DBQuery(sql, param, "orangebear");
            }
            return sd;
            
        }

        #endregion

        #region 新門市取得庫存
        public static StockData GrtPosStock(string key, int QueryType)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select a.SerialId,b.ProductId,a.Name,a.RefSerialId from ProductSerial a ");
            sb.Append("join Product b on a.SerialId=b.SerialId ");

            if (QueryType == 0)
            { sb.Append("where b.BarCode=@BarCode "); }
            else
            { sb.Append("where a.SerialId=@SerialId or a.RefSerialId=@SerialId or a.SerialId=@RefSerialId or a.RefSerialId=@RefSerialId"); }
            
            string sql = sb.ToString();
            sb.Clear();
            Dictionary<string, object> param = new Dictionary<string, object>();

            if (QueryType == 0)
            { param.Add("BarCode", key); }
            else
            {
                param.Add("SerialId", key);
                param.Add("RefSerialId", RefSerialId(key));
            }
            
            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            StockData sd = new StockData();
            if (dt.Rows.Count > 0)
            {
                sd.series_id = dt.Rows[0]["SerialId"].ToString();
                sd._series_id = dt.Rows[0]["RefSerialId"].ToString();
                sd.product_id = dt.Rows[0]["ProductId"].ToString();
                sd.series_name = dt.Rows[0]["Name"].ToString();
                sb.Append("select a.SerialId,b.ProductId,a.Name,a.RefSerialId,b.SerialId,b.BarCode,b.Color,b.Size,b.Price ");
                sb.Append("from posclient..ProductSerial a  ");
                sb.Append("join Product b on a.SerialId=b.SerialId  ");
                sb.Append("where (a.SerialId=@SerialId or a.RefSerialId=@SerialId or a.SerialId=@RefSerialId or a.RefSerialId=@RefSerialId)");
                sb.Append("order by ProductId ");
                if (QueryType == 0)
                { 
                    param.Add("SerialId", sd.series_id);
                    param.Add("RefSerialId", RefSerialId(sd.series_id));
                }

                

                sql = sb.ToString();
                sd.stockDT = DB.DBQuery(sql, param, "PosClient");

            }
            return sd;
        }
        #endregion

        public static string GetKWStockAllocation(string ProductID)
        {
            bool connectStatus = DB.IsServerConnected("orangebear");

            if (connectStatus)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select a.Quantity from StockAllocation a  ");
                sb.Append("where a.ProductId=@ProductID and a.StockType=3 ");
                string sql = sb.ToString();
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("ProductID", ProductID);
                DataTable dt = DB.DBQuery(sql, param, "orangebear");
                if (dt.Rows.Count > 0)
                    return dt.Rows[0]["Quantity"].ToString();
                else
                    return "查無商品庫存";
            }
            else
            {
                return "無法取得";
            }

        }

        private static string RefSerialId(string SerialId)
        {
            if (string.IsNullOrEmpty(SerialId))
            { return ""; }

            if (SerialId.EndsWith("-"))
            { return SerialId.Substring(0, SerialId.Length - 1); }
            else
            { return SerialId + "-"; }
        }


        public static int GetB1Stock(string ProductID)
        {
            int B1 = CheckOut.GetB1Qty(ProductID);
            return B1;
        }

        public static int GetShowStock(string ProductID)
        {
            int Show = CheckOut.GetShowQty(ProductID);
            return Show;
        }

        public static int GetLocalStockByProductID(string ProductID)
        {
            int B1 = CheckOut.GetB1Qty(ProductID);
            int Show = CheckOut.GetShowQty(ProductID);
            return (B1 + Show);
        }

    }

     public class StockData{
        public string series_id="";
        public string _series_id="";
        public string product_id="";
        public string series_name="";
        public string B1_stock = "";
        public string img1="";
        public string img2="";
        public string img3="";
        public string img4 = "";
        public string img5 = "";
        public string img6 = "";
        public string img7 = "";
        public string img8 = "";
        public DataTable stockDT=new DataTable();

    }
}