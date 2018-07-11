using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using OBShopWeb.PDA;
using POS_Library.Public;
using POS_Library.ShopPos;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class ShippingMoreLack : System.Web.UI.Page
    {
        #region 宣告

        //private ShipIn shipIn = new ShipIn();

        private CheckFormat CF = new CheckFormat();
        private ShelfProcess sp = new ShelfProcess();

        //private List<IssueReport> IssueReportList = new List<IssueReport>();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        private String account, num;
        private int store;

        private MsgStatus result;

        #endregion 宣告

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Session["Account"] = "michael";
                if (Session["Account"] == null || Request["ticket_id"] == null || Request["flowType"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    account = Session["Account"].ToString();
                    string flowType = Request["flowType"].ToString();
                    lbl_Message.Text = "";
                    txt_More_ProdoctID.Focus();

                    var ticketId = Request["ticket_id"];
                    if (!IsPostBack)
                    {
                        if (ticketId != null)
                        {
                            lbl_ticket_id.Text = txt_ticket_id.Text = ticketId;
                        }
                        if (Request["ProductId"] != null)
                        {
                            txt_More_ProdoctID.Text = Request["ProductId"].ToString();
                            lbl_P.Text = Request["ProductId"];
                            lbl_Q.Text = Request["Q"];
                        }
                        if (Request["flowType"] != null)
                            lbl_FlowStatus.Text = TypeToName(flowType);
                    }
                    ////加原因(2013-0902新增)
                    //if (lbl_FlowStatus.Text == "海運")
                    //{
                    lbl_Reason.Visible = true;
                    DDL_Reason.Visible = true;
                    //}
                    //else
                    //{
                    //    lbl_Reason.Visible = false;
                    //    DDL_Reason.Visible = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion Page_Load

        #region 主功能-驗貨回報加入缺件多件

        /// <summary>
        /// 加入缺件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Lack_Click(object sender, EventArgs e)
        {
            try
            {
                #region 撿查

                //缺少數量條件為 : 輸入缺少數量不可大於原傳票產品數量
                var listBoxs = ListBoxProducts();
                //輸入產品、輸入數量
                var inputProductId = txt_More_ProdoctID.Text;
                var inputQ = txt_More_ProdoctNum.Text;
                //原產品、原數量
                var oldProductId = lbl_P.Text;
                var oldQ = lbl_Q.Text;
                var ticketId = txt_ticket_id.Text.Trim();
                //目前缺少總數量
                var nowlackQ = 0;
                if (string.IsNullOrEmpty(inputQ))
                {
                    lbl_Message.Text = " 請輸入數量!";
                    return;
                }
                if (listBoxs.FirstOrDefault(x => x.IsLack == false && x.Product == inputProductId) != null)
                {
                    lbl_Message.Text = " 輸入缺少數量不可同時有多出的產品!";
                    return;
                }

                var listBox = listBoxs.FirstOrDefault(x => x.Product == inputProductId && x.IsLack);
                if (listBox != null)
                {
                    nowlackQ = int.Parse(inputQ) + listBox.Quantity;
                }
                else
                {
                    nowlackQ = int.Parse(inputQ);
                }
                if (inputProductId == oldProductId && nowlackQ > int.Parse(oldQ))
                {
                    lbl_Message.Text = " 輸入缺少數量不可大於原傳票產品數量!";
                    return;
                }
                //檢查原因
                if (DDL_Reason.SelectedValue == "未選擇")
                {
                    lbl_Message.Text = "海運差異→請選擇原因";
                    return;
                }

                var flowType = NameToType(lbl_FlowStatus.Text);

                //檢查差異回報是否大於傳票數
                var shipDa = new POS_Library.ShopPos.ShipInDA();
                var ckTicketQ = shipDa.CkTicketAndDiff(int.Parse(ticketId), inputProductId, nowlackQ, _areaId);
                if (ckTicketQ.Result == "0")
                {
                    lbl_Message.Text = ckTicketQ.Reason;
                    return;
                }

                if (oldProductId != inputProductId)
                {
                    lbl_Message.Text = " 不可輸入原產品以外的產品!";
                    return;
                }

                if (flowType == 0)
                {
                    lbl_Message.Text = "字形無法解析!請重新回報! " + lbl_FlowStatus.Text;
                    return;
                }

                #endregion 撿查

                DiffBox(true, flowType);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 加入多件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_More_Click(object sender, EventArgs e)
        {
            try
            {
                #region 撿查

                //原產品
                var oldProductId = lbl_P.Text;
                var listBoxs = ListBoxProducts();
                var inputProductId = txt_More_ProdoctID.Text.Trim();

                if (listBoxs.FirstOrDefault(x => x.IsLack && x.Product == inputProductId) != null)
                {
                    lbl_Message.Text = " 輸入多出數量不可同時有缺少的產品!";
                    return;
                }
                //檢查原因
                if (DDL_Reason.SelectedValue == "未選擇")
                {
                    lbl_Message.Text = "海運差異→請選擇原因";
                    return;
                }
                //多不限制(2015-0324修改)
                //if (oldProductId != inputProductId)
                //{
                //    lbl_Message.Text = " 不可輸入原產品以外的產品!";
                //    return;
                //}
                var flowType = NameToType(lbl_FlowStatus.Text);
                if (flowType == 0)
                {
                    lbl_Message.Text = "字形無法解析!請重新回報!";
                    return;
                }

                #endregion 撿查

                DiffBox(false, flowType);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 差異
        /// </summary>
        /// <param name="isLack"></param>
        /// <param name="flowType"></param>
        protected void DiffBox(bool isLack, int flowType)
        {
            try
            {
                var productId = txt_More_ProdoctID.Text.Trim();
                num = txt_More_ProdoctNum.Text.Trim();

                if (productId != "" && num != "")
                {
                    //條碼轉產編(2013-0319新增)---------------------------------
                    if (CF.CheckID(productId, CheckFormat.FormatName.Product))
                        productId = sp.GetProductNum(productId);
                    //----------------------------------------------------------

                    //productId = WMS_Library.ProductStorage.NoProduct.GetProduct(productId);
                    //if (String.IsNullOrEmpty(productId))
                    //{
                    //    lbl_Message.Text = "請輸入正確產品!";
                    //    return;
                    //}
                    if (int.Parse(num) > 0)
                    {
                        if (!string.IsNullOrEmpty(txt_ticket_id.Text))
                        {
                            var shipDa = new POS_Library.ShopPos.ShipInDA();
                            int store = POS_Library.Public.Utility.GetStore(_areaId);
                            var isTrue = shipDa.IsIssueReport(int.Parse(txt_ticket_id.Text.Trim()), productId, store, flowType);
                            if (isTrue.Result == "1")
                            {
                                lbl_Message.Text = "此產品已回報過!";
                                return;
                            }
                        }
                        ListItem LItem = new ListItem();
                        string quantity = string.Empty;
                        if (isLack)
                        {
                            quantity = "-" + txt_More_ProdoctNum.Text.Trim();
                        }
                        else
                        {
                            quantity = txt_More_ProdoctNum.Text.Trim();
                        }
                        LItem.Value = productId + "," + txt_More_ProdoctNum.Text.Trim() + "," + isLack;
                        LItem.Text = productId + " x " + quantity;
                        LB_Product_Id1.Items.Add(LItem);
                        txt_More_ProdoctNum.Text = "";
                        txt_More_ProdoctID.Text = "";
                    }
                    else
                    {
                        lbl_Message.Text = "請輸入正確數量!";
                    }
                }
                else
                {
                    lbl_Message.Text = "請輸入產品名稱及數量!";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-驗貨回報加入缺件多件

        #region 主功能-回報

        /// <summary>
        /// 傳票數量異常回報(第一次驗貨回報)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Send_Click(object sender, EventArgs e)
        {
            try
            {
                if (DDL_Reason.SelectedValue == "未選擇")
                {
                    lbl_Message.Text = "請選擇原因";
                    return;
                }

                var listBoxs = ListBoxProducts();

                var ticketId = int.Parse(txt_ticket_id.Text.Trim());
                if (listBoxs.Any())
                {
                    var flowType = NameToType(lbl_FlowStatus.Text);
                    if (flowType == 0)
                    {
                        lbl_Message.Text = "字形無法解析!請重新回報!";
                        return;
                    }

                    var issueReportList = new List<POS_Library.DB.IssueReport>();
                    var shipDa = new POS_Library.ShopPos.ShipInDA();
                    int store = POS_Library.Public.Utility.GetStore(_areaId);
                    var box = shipDa.GetPosTicketBox(int.Parse(txt_ticket_id.Text.Trim()), _areaId);
                    var comment = "POS:" + DDL_Reason.SelectedValue;
                    //把ListBox中的串成List
                    foreach (var item in listBoxs)
                    {
                        //經由傳票回報所以一定要有傳票資訊
                        POS_Library.DB.IssueReport iss = new POS_Library.DB.IssueReport();

                        //填入傳票,建立者,建立日期,產品編號,數量,缺或多件,倉庫類別
                        iss.TicketId = ticketId;
                        iss.BoxNum = box;
                        iss.CreateAuditor = account;
                        iss.CreateDate = DateTime.Now;
                        iss.ProductId = item.Product;
                        iss.Quantity = item.Quantity;
                        iss.IsQuantity = item.IsLack != true;
                        iss.ShopType = store;
                        iss.FlowStatus = flowType;
                        iss.Comment = comment;
                        issueReportList.Add(iss);
                    }

                    //寫入issueReport
                    result = shipDa.SetDiff(issueReportList);

                    if (result != null)
                    {
                        lbl_Message.Text = result.Reason;

                        //成功清空
                        if (result.Result == "1")
                        {
                            LB_Product_Id1.Items.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-回報

        #region 副功能-類別

        /// <summary>
        /// TypeToName
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        protected String TypeToName(String Type)
        {
            switch (Type)
            {
                case "28": return POS_Library.ShopPos.EnumData.FlowType.門市進貨.ToString();
                default: return "其他";
            }
        }

        /// <summary>
        /// NameToType
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        protected int NameToType(string Name)
        {
            var flowName = HttpUtility.UrlDecode(Name);
            switch (flowName)
            {
                case "門市進貨": return (int)POS_Library.ShopPos.EnumData.FlowType.門市進貨;
                default: return 0;
            }
        }

        public class ListBoxProducta
        {
            public string Product;
            public int Quantity;
            public bool IsLack;
        }

        public List<ListBoxProducta> ListBoxProducts()
        {
            var listBoxProducts = new List<ListBoxProducta>();
            for (int i = 0; i < LB_Product_Id1.Items.Count; i++)
            {
                var productData = new ListBoxProducta();
                productData.Product = LB_Product_Id1.Items[i].Value.Split(',')[0];
                productData.Quantity = int.Parse(LB_Product_Id1.Items[i].Value.Split(',')[1]);
                productData.IsLack = bool.Parse(LB_Product_Id1.Items[i].Value.Split(',')[2]);
                listBoxProducts.Add(productData);
            }
            var list = listBoxProducts.GroupBy(x => new { x.Product, x.IsLack }).Select(x => new ListBoxProducta { Product = x.Key.Product, Quantity = x.Sum(xx => xx.Quantity), IsLack = x.Key.IsLack }).ToList();
            return list;
        }

        #endregion 副功能-類別
    }
}