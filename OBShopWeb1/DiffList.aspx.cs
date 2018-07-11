using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using OBShopWeb.PDA;
using POS_Library.Public;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;

namespace OBShopWeb
{
    public partial class DiffList : System.Web.UI.Page
    {
        #region 宣告

        //台灣倉庫 0  虎門倉庫 1 

        //private ShelfProcess sp = new ShelfProcess();
        private CheckFormat CF = new CheckFormat();

        private String account;

        //private int flowType, shipInType, ticketType, pagekey;

        private List<ImportClass.ProductData> tempallproduct = new List<ImportClass.ProductData>();

        string areaId;

        #endregion 宣告

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Account"] == null)
            {
                Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                Response.End();
            }
            else
            {
                account = Session["Account"].ToString(); 
                if (!IsPostBack)
                {
                    if (Request["storage"] == null || Request["ticketType"] == null || Request["flowType"] == null || Request["pagekey"] == null || Request["areaId"] == null || Request["importType"] == null)
                    {
                        lbl_Message.Text = "資料錯誤!!";
                        return;
                    }
                    var shelf = Request["storage"];
                    var importType = Request["importType"];
                    var ticketType = Request["ticketType"];
                    var flowType = int.Parse(Request["flowType"].ToString());
                    var pagekey = Request["pagekey"];
                    var areaId = Request["areaId"];
                    ViewState["flowType"] = flowType;
                    ViewState["pagekey"] = pagekey;
                    ViewState["areaId"] = areaId;
                    ViewState["importType"] = importType;
                    ViewState["ticketType"] = ticketType;
                    lbl_Storage_NO.Text = shelf;
                    if (Session["ProductDatas"] != null)
                    {
                        tempallproduct = Session["ProductDatas"] as List<ImportClass.ProductData>;
                        Session["ProductDatas"] = null;
                    }
                    else
                    {
                        lbl_Message.Text = "時間過長!!請重新動做!!";
                        return;
                    }
                    //改成function(2014-0425修改)
                    var flowT = FlowType(flowType);
                    if (flowT == 0)
                    {
                        lbl_Message.Text = "無此類型入庫!!";
                        return;
                    }

                    var shipDa = new POS_Library.ShopPos.ShipInDA();
                    var ticks = tempallproduct.Select(x => int.Parse(x.Ticket)).Distinct().ToList();
                    int store = POS_Library.Public.Utility.GetStore(int.Parse(areaId));
                    var issue = shipDa.GetIssueReports(ticks, store, flowType).Where(x => string.IsNullOrEmpty(x.HandleAuditot)).ToList();

                    //依驗貨完的傳票尋找是否有回報問題的傳票產品
                    var diffs = issue.Select(x => new ImportClass.ProductData() { Ticket = x.TicketId.Value.ToString(), Name = x.ProductId, Quantity = x.Quantity, IsQuantity = x.IsQuantity }).ToList();
                     
                    if (tempallproduct.Count > 0)
                    {
                        lbl_ProductNormal.Text = "內容：<br />";
                         
                        CompositeListBox(tempallproduct, false); 
                    }
                    else
                    {
                        btn_Check.Visible = false;
                        listboxNormal.Visible = false;
                    }
                    if (issue.Any())
                    {
                        lbl_ProductDiff.Text = "差異：<br />";
                        CompositeListBox(diffs, true);
                    }
                    else
                    {
                        listboxDiff.Visible = false;
                    }

                    //顯示儲位類型
                    lbl_Storage_NO_Type.Text = CF.TypeToName(shipDa.CheckStorage(lbl_Storage_NO.Text, int.Parse(areaId)));
                }
            }
        }

        #endregion Page_Load

        #region 副功能-串lbl

        protected List<ImportClass.ProductData> GetProducts(string[] products, string[] ticketIds)
        {
            var ps = new List<ImportClass.ProductData>();
            for (int i = 0; i < products.Count(); i++)
            {
                var p = new ImportClass.ProductData();
                var reg = Regex.Match(products[i], @"(\w.*)(_)(.*)");
                p.Ticket = ticketIds[i];
                p.Name = reg.Groups[1].Value.Trim();
                p.Quantity = int.Parse(reg.Groups[3].Value);
                p.IsQuantity = true;
                ps.Add(p);
            }
            return ps;
        }

        /// <summary>
        /// 組合ListBox
        /// </summary>
        /// <param name="list"></param>
        /// <param name="isDiff"></param>
        protected void CompositeListBox(List<ImportClass.ProductData> list, bool isDiff)
        {
            foreach (var item in list)
            {
                ListItem listItem = new ListItem();
                listItem.Value = string.Format("{0},{1},{2},{3},{4}", item.Ticket, item.Name, item.Quantity, item.IsQuantity, item.Id);
                if (isDiff)
                {
                    listItem.Text = item.Name + "__X  " + item.Quantity.ToString() + "__" + (item.IsQuantity ? "多" : "少");

                    //產品沒驗到就不顯示少的(2013-0411新增)
                    if (tempallproduct != null && (tempallproduct.Where(x => x.Name == item.Name).Count() == 0) && !item.IsQuantity)
                    { }
                    else
                    {
                        listboxDiff.Items.Add(listItem);
                    }
                }
                else
                {
                    listItem.Text = item.Name + "__X  " + item.Quantity.ToString();
                    listboxNormal.Items.Add(listItem);
                }
            }
        }

        #endregion 副功能-串lbl

        #region 主功能-確認送出

        /// <summary>
        /// 確認送出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Check_Click(object sender, EventArgs e)
        {
            try
            {
                //寫入
                MsgStatus result = Report();

                if (result.Result == "0")
                {
                    lbl_Message.Text = result.Reason;
                }
                else
                {
                    lbl_Message.Text = "成功!";
                    lbl_Product.Text = "";

                    btn_Check.Enabled = false;
                    var box = ViewState["pagekey"];
                    var flowType = int.Parse(ViewState["flowType"].ToString());
                    var importType = ViewState["importType"].ToString();
                    var areaId = ViewState["areaId"].ToString();
                    var ticketType =ViewState["ticketType"].ToString()  ;
                      
                    //自動重整 海運/換貨 (2013-0315新增)
                    var url = "ShipInVerify.aspx?box=" + box + "&ticketType=" + ticketType + "&areaId=" + areaId + "&importType=" + importType;
                    Page.RegisterClientScriptBlock("checkinput", @"<script>window.open('" + url + "','_self');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 回報
        /// </summary>
        /// <returns></returns>
        protected MsgStatus Report()
        {
            try
            {
                var shipDa = new POS_Library.ShopPos.ShipInDA();
                var flowType = int.Parse(ViewState["flowType"].ToString());
                var box = ViewState["pagekey"].ToString();
                var importType = int.Parse(ViewState["importType"].ToString());
                var ticketType = int.Parse(ViewState["ticketType"].ToString());

                var areaId = int.Parse(ViewState["areaId"].ToString()); 
                var result = new MsgStatus();

                //正確驗貨產品
                var products = new List<ImportClass.ProductData>();

                for (int i = 0; i < listboxNormal.Items.Count; i++)
                {
                    var product = new ImportClass.ProductData();
                    string[] array = listboxNormal.Items[i].Value.Split(',');
                    product.Ticket = array[0];
                    product.Name = array[1].Trim().ToUpper();
                    product.Quantity = int.Parse(array[2]);
                    product.IsQuantity = true;
                    product.Id = array[4];
                    products.Add(product);
                }
                //差異產品
                var diffs = new List<ImportClass.ProductData>();
                for (int i = 0; i < listboxDiff.Items.Count; i++)
                {
                    var diff = new ImportClass.ProductData();
                    string[] array = listboxDiff.Items[i].Value.Split(',');
                    diff.Shelf = box;
                    diff.Ticket = array[0];
                    diff.Name = array[1].Trim().ToUpper();
                    diff.Quantity = int.Parse(array[2]);
                    diff.IsQuantity = bool.Parse(array[3]);
                    diff.Id = array[4];
                    diffs.Add(diff);
                }

                #region ※比對差異回報有無異常防呆(2014-0425新增)

                var ticks = products.Select(x => int.Parse(x.Ticket)).Distinct().ToList();
                int store = POS_Library.Public.Utility.GetStoreForShop(areaId);
                var p = products.Select(x => x.Name).ToArray();
                var issue = shipDa.GetIssueReports(ticks, store, flowType).Where(x => string.IsNullOrEmpty(x.HandleAuditot) && p.Contains(x.ProductId) ).ToList();
                var diffs2 = issue.Where(x=>!x.IsQuantity).Select(x => new ImportClass.ProductData() { Ticket = x.TicketId.Value.ToString(), Name = x.ProductId, Quantity = x.Quantity, IsQuantity = x.IsQuantity }).ToList();
                var diffC = diffs2.Count;
                for (int i = 0; i < diffC; i++)
                {
                    if ((products.Count(x => x.Name == diffs2[i].Name) == 0))
                    {
                        diffs2.Remove(diffs2[i]);
                    }
                }
                //foreach (var item in diffs2)
                //{
                //    //產品沒驗到就不顯示少的(2013-0411新增)
                //    if ((products.Count(x => x.Name == item.Name) == 0) && !item.IsQuantity)
                //    {
                //        diffs2.Remove(item);
                //        if (!diffs2.Any())
                //            break;
                //    }
                //}

                if (diffs.Count(x => !x.IsQuantity) != diffs2.Count)
                {
                    return POS_Library.Public.Utility.GetMessage("0", "差異回報筆數異常!請重新驗貨進入此頁面!");
                }

                #endregion ※比對差異回報有無異常防呆(2014-0425新增)

                //依驗貨完的傳票尋找是否有回報問題的傳票產品
                var ckNormals = shipDa.CkNormals(products, diffs);
                if (ckNormals.Result == "0")
                {
                    return ckNormals;
                }

                if (products.Count > 0)
                {
                    result = shipDa.SetTempShelfProduct(lbl_Storage_NO.Text, products, account, true, flowType, areaId);
                    if (result.Result == "0")
                    {
                        return result;
                    }
                }
                if (diffs.Count > 0)
                {
                    result = shipDa.SetTempDiffEdit(lbl_Storage_NO.Text, diffs, importType, ticketType, flowType, account, store, areaId);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion 主功能-確認送出

        #region FlowType

        /// <summary>
        /// FlowType
        /// </summary>
        /// <param name="flowType"></param>
        /// <returns></returns>
        protected int FlowType(int flowType)
        {
            var flowT = 0;
            switch (flowType)
            {
                case (int)EnumData.FlowType.門市進貨://28 
                    flowT = (int)EnumData.FlowType.門市進貨;
                    break; 
                default:
                    flowT = 0;
                    break;
            }

            return flowT;
        }

        #endregion FlowType
    }
}