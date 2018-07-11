using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class InvoiceAudit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack == true)
            { return; }

            txtStartDate.Value = DateTime.Today.ToString("yyyy/MM/dd");
            txtEndDate.Value = DateTime.Today.ToString("yyyy/MM/dd");

            Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
            string strSQL = "select * from PosClient..InvoiceRolls";
            System.Data.DataTable dt = Poslib.DB.DBQuery(strSQL, QueryParameters, "PosClient");

            ddlInvoiceRoll.Items.Add(new ListItem("不限", ""));
            foreach(System.Data.DataRow r in dt.Rows)
            {
                ddlInvoiceRoll.Items.Add(new ListItem(
                                                        string.Format("{0}{1}~{0}{2}", r["AlphabeticCode"], r["StartingNumber"], r["EndingNumber"]),
                                                        string.Format("{0}|{1}", r["AlphabeticCode"], r["StartingNumber"])
                                                        )
                                        );
            }
            ddlInvoiceRoll.DataBind();
        }

        private IList<OrderEntity> GetData()
        {
            IList<OrderEntity> result;
            Dictionary<string, object> QueryParameters = new Dictionary<string,object>();
            string strSQL;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("select");
            sb.AppendLine("i.OrderID,I.InvoiceNo, o.Amount, o.Status, o.PosNo, o.OrderTime, od.ProductId, r.ReturnTime, ir.*");
            sb.AppendLine("from");
            sb.AppendLine("PosClient..OrderInvoices as i");
            sb.AppendLine("left outer join");
            sb.AppendLine("PosClient..InvoiceRolls as ir");
            sb.AppendLine("on ir.AlphabeticCode = SUBSTRING(i.InvoiceNo, 1, 2) and (SUBSTRING(i.InvoiceNo, 3, 8) between ir.[StartingNumber] and ir.[EndingNumber])");
            sb.AppendLine("left outer join");
            sb.AppendLine("[PosClient].[dbo].[Orders] as o");
            sb.AppendLine("on o.OrderID = i.OrderID");
            sb.AppendLine("left outer join");
            sb.AppendLine("[PosClient].[dbo].[Returns] as r");
            sb.AppendLine("on r.OrderID = o.OrderID");
            sb.AppendLine("left outer join");
            sb.AppendLine("[PosClient].[dbo].[OrderItems] as od");
            sb.AppendLine("on od.OrderID = o.OrderID");
            sb.AppendLine("where o.OrderDate between @OrderDateStart and @OrderDateEnd");

            if (txtOrderIDStart.Text.Trim() != "" && txtOrderIDEnd.Text.Trim() != "")
            {
                sb.AppendLine("and o.OrderID between @OrderIDStart and @OrderIDEnd");
                QueryParameters.Add("OrderIDStart", int.Parse(txtOrderIDStart.Text.Trim()));
                QueryParameters.Add("OrderIDEnd", int.Parse(txtOrderIDEnd.Text.Trim()));
            }

            if (ddlInvoiceRoll.SelectedValue != "")
            {
                sb.AppendLine("and ir.AlphabeticCode = @AlphabeticCode and ir.StartingNumber = @StartingNumber");
                QueryParameters.Add("AlphabeticCode", ddlInvoiceRoll.SelectedValue.Split(new string[] {"|"},  StringSplitOptions.RemoveEmptyEntries)[0]);
                QueryParameters.Add("StartingNumber", int.Parse(ddlInvoiceRoll.SelectedValue.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[1]));
            }

            strSQL = sb.ToString();

            QueryParameters.Add("OrderDateStart", DateTime.Parse(txtStartDate.Value));
            QueryParameters.Add("OrderDateEnd", DateTime.Parse(txtEndDate.Value));

            System.Data.DataTable dt = Poslib.DB.DBQuery(strSQL, QueryParameters, "PosClient");

            result = (from System.Data.DataRow r in dt.Rows
                      group r by new
                      {
                          OrderID = r["OrderID"],
                          Amount = r["Amount"],
                          Status = r["Status"],
                          PosNo = r["PosNo"],
                          OrderTime = r["OrderTime"],
                          ReturnTime = r["ReturnTime"]
                      } into OrderGroup
                      select new OrderEntity
                      {
                          OrderID = OrderGroup.Key.OrderID.ToString(),
                          Amount = (int)(OrderGroup.Key.Amount),
                          Status = OrderGroup.Key.Status.ToString(),
                          PosNo = (int)(OrderGroup.Key.PosNo),
                          OrderTime = (DateTime)(OrderGroup.Key.OrderTime),
                          ReturnTime = OrderGroup.Key.ReturnTime == DBNull.Value ? null : (Nullable<DateTime>)(OrderGroup.Key.ReturnTime),
                          DataRows = OrderGroup.ToList()
                      }
                        ).ToList();

            return result;
        }

        class OrderEntity
        {
            public string OrderID { get; set; }
            public int Amount { get; set; }
            public string Status { get; set; }
            public int PosNo { get; set; }
            public DateTime OrderTime { get; set; }
            public Nullable<DateTime> ReturnTime { get; set; }
            public string InvoiceNo
            {
                get
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    IEnumerable<string> InvoiceNos = (from System.Data.DataRow r in this.DataRows select r["InvoiceNo"].ToString()).ToList().Distinct();
                    foreach (string invNo in InvoiceNos)
                    { sb.AppendLine(invNo); }
                    return sb.ToString();
                }
            }
            public string Products
            {
                get
                {
                    IList<string> InvoiceNos = (from System.Data.DataRow r in this.DataRows select r["InvoiceNo"].ToString()).Distinct().ToList();

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    foreach (System.Data.DataRow r in this.DataRows)
                    {
                        if (r["InvoiceNo"].ToString() == InvoiceNos[0])
                        { sb.AppendLine(r["ProductID"].ToString()); }
                    }
                    return sb.ToString();
                }
            }
            public IList<System.Data.DataRow> DataRows { get; set; }
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            GridView1.DataSource = GetData();
            GridView1.DataBind();
        }

        protected void GridView1_PreRender(object sender, EventArgs e)
        {
            if (GridView1.Rows.Count > 0)
            {
                GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
    }
}