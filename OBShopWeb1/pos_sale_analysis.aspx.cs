using OBShopWeb.Poslib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class pos_sale_analysis : System.Web.UI.Page
    {
        public DataTable ProductDT = new DataTable();
        public bool advancedSearch = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                tbStartDate.Text = DateTime.Now.ToShortDateString();
                tbEndDate.Text = DateTime.Now.ToShortDateString();
            }
        }

        

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable dtSerial=GetProductSerial();
            HiddenShowSort.Value = "1";
            
            if (tbStartDate.Text != "" && tbEndDate.Text != "")
            {
                if (Convert.ToDateTime(tbStartDate.Text) > Convert.ToDateTime(tbEndDate.Text))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "", "alert('起迄時間有誤')", true);
                }
                else 
                {
                    ProductDT.Clear();
                    ProductDT = GetSaleReport("Serial", tbStartDate.Text, tbEndDate.Text, tbSID.Text, tbSName.Text);

                    foreach (DataRow dr in ProductDT.Rows)
                    {
                        DataRow row = dtSerial.Select("SerialId='" + dr["SerialId"].ToString() + "'").FirstOrDefault();
                        dr["Name"] = row["Name"].ToString();
                    }

                    AddColum(ProductDT);
                    ProductDT.Columns.Add("ProductId");
                    ProductDT.Columns.Add("Size");
                    ProductDT.Columns.Add("Color");
                    HiddenCol.Value = "0";
                }
            }
            else 
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "", "alert('請輸入起迄時間')",true);
            }
        }

        protected void btnSearchMore_Click(object sender, EventArgs e)
        {
            HiddenShowSort.Value = "0";
            if (tbStartDate.Text != "" && tbEndDate.Text != "")
            {
                if (Convert.ToDateTime(tbStartDate.Text) > Convert.ToDateTime(tbEndDate.Text))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "", "alert('起迄時間有誤')", true);
                }
                else
                {
                    ProductDT.Clear();
                    ProductDT = GetSaleReport("Product", tbStartDate.Text, tbEndDate.Text, tbSID.Text, tbSName.Text);
                    AddColum(ProductDT);
                    HiddenCol.Value = "1";
                }
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "", "alert('請輸入起迄時間')",true);
            }
        }

        protected void GVSearch_PreRender(object sender, EventArgs e)
        {
            GVSearch.Columns[4].Visible = true;
            GVSearch.Columns[5].Visible = true;
            GVSearch.Columns[6].Visible = true;

            if (HiddenCol.Value == "0")
            {
                GVSearch.Columns[4].Visible = false;
                GVSearch.Columns[5].Visible = false;
                GVSearch.Columns[6].Visible = false;
            }

            GVSearch.DataSource = ProductDT;
            GVSearch.DataBind();
            if (GVSearch.Rows.Count > 0)
            {
                GVSearch.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

        }

        public string lastSID;
        protected void GVSearch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            DataRowView row = (DataRowView)e.Row.DataItem;
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HyperLink ColSid = (HyperLink)e.Row.FindControl("linkSID");

                ((Image)e.Row.FindControl("ImageSID")).ImageUrl = GetProductImg(row["SerialId"].ToString());
                ((Label)e.Row.FindControl("lbSName")).Text = row["Name"].ToString();

                ((HyperLink)e.Row.FindControl("linkSID")).NavigateUrl = "http://erp03.obdesign.com.tw/admin/products/showdetail1.aspx?seriesID=" + row["SerialId"].ToString();
                ((HyperLink)e.Row.FindControl("linkSID")).Text = row["SerialId"].ToString();
                
                ((Label)e.Row.FindControl("lbPID")).Text = row["ProductId"].ToString();
                ((Label)e.Row.FindControl("lbSize")).Text = row["Size"].ToString();
                ((Label)e.Row.FindControl("lbColor")).Text = row["Color"].ToString();

                if (row["SaleCount"].ToString() == "0") //期間銷售0 (紅字)
                {
                    ((Label)e.Row.FindControl("lbSaleCount")).ForeColor = System.Drawing.Color.Red;
                }
                ((Label)e.Row.FindControl("lbSaleCount")).Text = row["SaleCount"].ToString();

                ((Label)e.Row.FindControl("lbStorage")).Text = row["Storage"].ToString();
                ((Label)e.Row.FindControl("lbPurchaseCount")).Text = row["PurchaseCount"].ToString();
                ((Label)e.Row.FindControl("lbTurnover")).Text = row["Turnover"].ToString();

                if (lastSID == ColSid.Text)
                {
                    e.Row.Cells[1].Text = "";
                    e.Row.Cells[2].Text = "";
                    e.Row.Cells[3].Text = "";
                }

                lastSID = ColSid.Text;
            }
        }

        #region Others

        public DataTable GetSaleReport(string ReportType, string Start, string End, string SID, string SName)
        {

            StringBuilder sb = new StringBuilder();
            switch (ReportType)
            {
                case "Serial":
                    sb.AppendLine("SELECT replace(A.SerialId,'-','') SerialId,'' Name,isnull(B.SaleCount,0) SaleCount,isnull(D.Storage,0) Storage,isnull(C.PurchaseCount,0) PurchaseCount,isnull(F.Quantity,0) PreDayQuantity");
					sb.AppendLine("FROM (");
                    sb.AppendLine("     SELECT DISTINCT replace(SerialId,'-','') SerialId");
					sb.AppendLine("     FROM [PosClient].[dbo].[ProductSerial]");

                    if (!string.IsNullOrWhiteSpace(SName))
                    {
                        sb.AppendLine("     WHERE Name LIKE '%'+@SName+'%' ");
                    }

                    sb.AppendLine(") A LEFT JOIN (");
                    sb.AppendLine("     SELECT replace(C.SerialId,'-','')SerialId,SUM(B.Quantity) SaleCount");
                    sb.AppendLine("     FROM [PosClient].[dbo].[Orders] A");
                    sb.AppendLine("     LEFT JOIN [PosClient].[dbo].[OrderItems] B ON (A.OrderID=B.OrderID)");
                    sb.AppendLine("     LEFT JOIN [PosClient].[dbo].[Product] C ON (B.ProductId=C.ProductId)");
                    sb.AppendLine("     WHERE A.OrderDate >=@Start AND A.OrderDate <=@End and A.Status=1");
                    sb.AppendLine("     GROUP BY replace(C.SerialId,'-','')");
                    sb.AppendLine(") B ON A.SerialId=B.SerialId");
                    sb.AppendLine("LEFT JOIN (");
                    sb.AppendLine("     SELECT replace(A.SerialId,'-','')SerialId ,COUNT(*) Storage");
                    sb.AppendLine("     FROM PosClient..Product	A");
                    sb.AppendLine("     LEFT JOIN PosClient..StorageDetail B ON A.ProductId=B.ProductId");
                    sb.AppendLine("     LEFT JOIN  [PosClient].[dbo].[Storage] C ON B.StorageId=C.Id");
                    sb.AppendLine("     WHERE C.StorageTypeId IN (0,1,20)");
                    sb.AppendLine("     GROUP BY replace(A.SerialId,'-','')");
                    sb.AppendLine(") D ON A.SerialId=D.SerialId");
                    sb.AppendLine("LEFT JOIN (");
                    sb.AppendLine("     SELECT  replace(C.SerialId,'-','')SerialId,SUM(B.Quantity) PurchaseCount");
                    sb.AppendLine("     FROM [PosClient].[dbo].[PosTicket] A");
                    sb.AppendLine("     LEFT JOIN [PosClient].[dbo].[ImportTicketLog] B ON A.TicketId=B.TicketId");
                    sb.AppendLine("     LEFT JOIN [PosClient].[dbo].[Product] C ON B.ProductId=C.ProductId");
                    sb.AppendLine("     WHERE A.VerifyDate >=@Start AND A.VerifyDate <=@End AND C.ProductId IS NOT NULL");
                    sb.AppendLine("     GROUP BY replace(C.SerialId,'-','')");
                    sb.AppendLine(") C ON A.SerialId=C.SerialId");
                    sb.AppendLine("LEFT JOIN (");
                    sb.AppendLine("     SELECT replace(A.SerialId,'-','')SerialId,SUM(A.Quantity) Quantity");
		            sb.AppendLine("     FROM [PosClient].[dbo].[DailyStock] A");
                    sb.AppendLine("     WHERE RecordDate=DATEADD(day,-1,@Start)");
                    sb.AppendLine("     GROUP BY replace(A.SerialId,'-','')");
                    sb.AppendLine(") F ON A.SerialId=F.SerialId");
                    sb.AppendLine("WHERE (B.SaleCount IS NOT NULL OR C.PurchaseCount IS NOT NULL OR D.Storage IS NOT NULL) ");
                    sb.AppendLine("AND A.SerialId LIKE '%'+@SID+'%'");
                    break;
                case "Product":
                    sb.AppendLine("SELECT A.ProductId,B.Name,A.SerialId,A.Color,A.Size,isnull(C.SaleCount,0) SaleCount,isnull(E.Storage,0) Storage,isnull(D.PurchaseCount,0)  PurchaseCount,isnull(F.Quantity,0) PreDayQuantity");
                    sb.AppendLine("FROM [PosClient].[dbo].[Product] A");
                    sb.AppendLine("LEFT JOIN [PosClient].[dbo].[ProductSerial] B ON A.SerialId=B.SerialId");
                    sb.AppendLine("LEFT JOIN(");
                    sb.AppendLine("           SELECT B.ProductId,SUM(B.Quantity) SaleCount");
                    sb.AppendLine("           FROM [PosClient].[dbo].[Orders] A");
                    sb.AppendLine("           LEFT JOIN [PosClient].[dbo].[OrderItems] B ON (A.OrderID=B.OrderID)");
                    sb.AppendLine("           WHERE A.OrderDate >=@Start AND A.OrderDate <=@End and A.Status=1");
                    sb.AppendLine("           GROUP BY B.ProductId");
                    sb.AppendLine(")C ON A.ProductId=C.ProductId");
                    sb.AppendLine("LEFT JOIN(");
                    sb.AppendLine("           SELECT B.ProductId,SUM(B.Quantity) PurchaseCount");
                    sb.AppendLine("           FROM [PosClient].[dbo].[PosTicket] A");
                    sb.AppendLine("           LEFT JOIN [PosClient].[dbo].[ImportTicketLog] B ON A.TicketId=B.TicketId");
                    sb.AppendLine("           WHERE A.VerifyDate >=@Start AND A.VerifyDate <=@End");
                    sb.AppendLine("           GROUP BY B.ProductId");
                    sb.AppendLine(")D ON A.ProductId=D.ProductId");
                    sb.AppendLine("LEFT JOIN(");
                    sb.AppendLine("           SELECT A.ProductId,COUNT(*) Storage");
                    sb.AppendLine("           FROM [PosClient].[dbo].[StorageDetail] A");
                    sb.AppendLine("           LEFT JOIN  [PosClient].[dbo].[Storage] B ON A.StorageId=B.Id");
                    sb.AppendLine("           WHERE B.StorageTypeId IN (0,1,20)");
                    sb.AppendLine("           GROUP BY A.ProductId");
                    sb.AppendLine(") E ON A.ProductId=E.ProductId");
                    sb.AppendLine("LEFT JOIN(");
                    sb.AppendLine("           SELECT ProductId,Quantity");
                    sb.AppendLine("           FROM [PosClient].[dbo].[DailyStock]");
                    sb.AppendLine("           WHERE RecordDate=DATEADD(day,-1,@Start)");
                    sb.AppendLine(") F ON A.ProductId=F.ProductId");
                    sb.AppendLine("WHERE (C.SaleCount IS NOT NULL OR D.PurchaseCount IS NOT NULL OR E.Storage IS NOT NULL)");
                    sb.AppendLine("AND B.Name LIKE '%'+@SName+'%' ");
                    sb.AppendLine("AND A.SerialId LIKE '%'+@SID+'%'");
                    sb.AppendLine("ORDER BY A.SerialId,A.ProductId");
                    break;
            }

            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("Start", Start);
            param.Add("End", End);
            param.Add("SName", SName);
            param.Add("SID", SID);

            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            return dt;

        }
        public DataTable GetProductSerial()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT DISTINCT replace([SerialId],'-','') SerialId,Name");
            sb.AppendLine("FROM [PosClient].[dbo].[ProductSerial]");
            string sql = sb.ToString();
            Dictionary<string, object> param = new Dictionary<string, object>();

            DataTable dt = DB.DBQuery(sql, param, "PosClient");
            return dt;

        }
        
        public string GetProductImg(string SerialId)
        {
            string ImgUrl = "";
            bool connectStatus = DB.IsServerConnected("orangebear");
            if (connectStatus)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT b.縮圖,b.圖片1,b.圖片2  ");
                sb.Append("from orangebear..產品系列 b  ");
                sb.Append("where b.系列編號=@SerialId  ");
                string sql = sb.ToString();
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("SerialId", SerialId);
                DataTable dt = DB.DBQuery(sql, param, "orangebear");

                if (dt.Rows.Count > 0)
                {
                    ImgUrl = "http://image.obdesign.com.tw" + dt.Rows[0]["縮圖"].ToString() + dt.Rows[0]["圖片1"].ToString();
                }
            }
            return ImgUrl;

        }
        private void AddColum(DataTable dtTemp)
        {
            dtTemp.Columns.Add("Turnover", typeof(double)); //周轉率
            Double rate = 0;
            foreach (DataRow row in dtTemp.Rows)
            {
                rate = Math.Round((Convert.ToDouble(row["SaleCount"]) / (Convert.ToDouble(row["PreDayQuantity"]) + Convert.ToDouble(row["PurchaseCount"]))), 2, MidpointRounding.AwayFromZero);  
                row["Turnover"] = (Convert.ToDouble(row["PreDayQuantity"]) + Convert.ToInt32(row["PurchaseCount"])) != 0 && Convert.ToDouble(row["SaleCount"]) != 0 ? rate : 0; 
            }
        }
        #endregion

    }
}