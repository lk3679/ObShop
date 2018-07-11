using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.PDA;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;

namespace OBShopWeb
{
    public partial class ShipInVerify : System.Web.UI.Page
    {
        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //判斷帳號登入
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {

                    lbl_Message.Text = string.Empty;
                    if (!IsPostBack)
                    {
                        var account = Session["Account"].ToString(); 
                        var box = Request["box"].Trim();
                        var areaId = int.Parse(Request["areaId"].Trim());
                        var importType = int.Parse(Request["importType"].Trim());
                        var ticketType = int.Parse(Request["ticketType"].Trim());
                        var flowType = importType == 0 ? (int)EnumData.FlowType.門市進貨 : importType;
                        ViewState["areaId"] = areaId;
                        ViewState["flowType"] = flowType;
                        ViewState["ticketType"] = ticketType;
                        ViewState["importType"] = importType;
                        lblbox.Text = box;
                        var shipDa = new POS_Library.ShopPos.ShipInDA();
                        var tickets = shipDa.PosBoxDetail(box, areaId).Where(x => x.Verify == false).Select(x => x.TicketId).Distinct().ToArray();
                        lblTicketId.Text = string.Join(",", tickets);
                        GetListBox(box, account, areaId, flowType);
                    }

                    txt_VerifyCheck_NO.Focus();
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region ListBox

        /// <summary>
        /// 驗傳票
        /// </summary>
        /// <param name="ticketId"></param>
        /// <param name="account"></param>
        /// <param name="areaId"></param>
        /// <param name="flowType"></param>
        public void GetListTicket(int ticketId, string account, int areaId, int flowType)
        {
            lblTicketId.Text = ticketId.ToString();
            var list = new List<POS_Library.ShopPos.DataModel.ImportClass.TicketDetailModel>();
            var shipDa = new POS_Library.ShopPos.ShipInDA();
            try
            {
                list = shipDa.GetTicketDetail(ticketId, account, areaId);
                if (!list.Any())
                {
                    lbl_Message.Text = "此傳票已無可驗的資料!!請離開!!";
                    return;
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ListItem LItem = new ListItem();
                        LItem.Value = list[i].Barcode + "," + list[i].TicketId + "," + flowType + "," + list[i].Quantity + "," + list[i].Id;
                        LItem.Text = list[i].ProductId + "__X " + list[i].Quantity.ToString();
                        LItem.Attributes.Add("TicketId", list[i].TicketId);
                        LItem.Attributes.Add("ProductId", list[i].ProductId);
                        LItem.Attributes.Add("Quantity", list[i].Quantity.ToString());
                        LB_Product_Id1.Items.Add(LItem);
                    }
                    List<string> errorList = new List<string>();
                    if (errorList.Count > 0)
                    {
                        LB_Product_Id1.Items.Clear();
                        txt_VerifyCheck_NO.Enabled = false;
                        foreach (var item in errorList)
                        {
                            Response.Write(item);
                        }
                    }
                    CheckNumToLabel();
                }
            }
            catch (Exception ex)
            {
                lbl_Message.Text = ex.Message;
            }
        }

        /// <summary>
        /// 驗箱號
        /// </summary>
        /// <param name="box"></param>
        /// <param name="account"></param>
        /// <param name="areaId"></param>
        /// <param name="flowType"></param>
        public void GetListBox(string box, string account, int areaId, int flowType)
        {
            var list = new List<POS_Library.ShopPos.DataModel.ImportClass.TicketDetailModel>();
            var shipDa = new POS_Library.ShopPos.ShipInDA();
            try
            {
                list = shipDa.GetBoxDetail(box, account, areaId);
                if (list == null)
                {
                    lbl_Message.Text = "此傳票已無可驗的資料!!請離開!!";
                    return;
                }
                else
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        ListItem LItem = new ListItem();
                        LItem.Value = list[i].Barcode + "," + list[i].TicketId + "," + flowType + "," + list[i].Quantity + "," + list[i].Id;
                        LItem.Text = list[i].ProductId + "__X " + list[i].Quantity;
                        LItem.Attributes.Add("TicketId", list[i].TicketId);
                        LItem.Attributes.Add("ProductId", list[i].ProductId);
                        LItem.Attributes.Add("Quantity", list[i].Quantity.ToString());
                        LB_Product_Id1.Items.Add(LItem);
                    }
                    List<string> errorList = new List<string>();
                    if (errorList.Count > 0)
                    {
                        LB_Product_Id1.Items.Clear();
                        txt_VerifyCheck_NO.Enabled = false;
                        foreach (var item in errorList)
                        {
                            Response.Write(item);
                        }
                    }
                    CheckNumToLabel();
                }
            }
            catch (Exception ex)
            {
                lbl_Message.Text = ex.Message;
            }
        }

        /// <summary>
        /// 更新數量
        /// </summary>
        /// <param name="i"></param>
        protected void CheckNumToLabel()
        {
            //更新數量
            lbl_Product_Id1num.Text = LB_Product_Id1.Items.Count.ToString(); 
            lbl_Product_Id2num.Text = LB_Product_Id2.Items.Count.ToString();
        }

        #endregion ListBox

        #region 主功能-條碼判斷

        /// <summary>
        /// 原js改寫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_VerifyCheck_NO_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lbl_Message.Text = "";
                CheckFormat CF = new CheckFormat();
                LB_Product_Id1.ClearSelection();
                LB_Product_Id2.ClearSelection();
                //全過
                if (txt_VerifyCheck_NO.Text == "2")
                {
                    int total = LB_Product_Id1.Items.Count;
                    for (int i = 0; i < total; i++)
                    {
                        LB_Product_Id2.Items.Insert(0, LB_Product_Id1.Items[0]);
                        LB_Product_Id1.Items.Remove(LB_Product_Id1.Items[0]);
                    }
                }
                //還原一筆
                else if (txt_VerifyCheck_NO.Text == "0")
                {
                    if (LB_Product_Id2.Items.Count > 0)
                    {
                        LB_Product_Id1.Items.Insert(0, LB_Product_Id2.Items[0]);
                        LB_Product_Id2.Items.Remove(LB_Product_Id2.Items[0]);
                    }
                }
                else if (Regex.IsMatch(txt_VerifyCheck_NO.Text, "^\\d{8}$"))
                {
                    bool find = false;
                    for (int i = 0; i < LB_Product_Id1.Items.Count; i++)
                    {
                        if (LB_Product_Id1.Items[i].Value.Split(',')[0] == txt_VerifyCheck_NO.Text)
                        {
                            LB_Product_Id2.Items.Insert(0, LB_Product_Id1.Items[i]);
                            LB_Product_Id1.Items.Remove(LB_Product_Id1.Items[i]);

                            find = true;
                            break;
                        }
                    }

                    //如果不在清單中 顯示(多出)
                    if (!find)
                    {
                        lbl_Message.Text = "這個商品不是這張傳票的！";
                    }
                }
                //如果輸入的是儲位barcode 跳至差異清單
                else if (CF.CheckID(txt_VerifyCheck_NO.Text.Trim(), CheckFormat.FormatName.Storage))
                { 
                    string storage = txt_VerifyCheck_NO.Text.Trim();
                    var listLeft = new List<string>();
                    var listRight = new List<string>();
                    int areaId = int.Parse(ViewState["areaId"].ToString());
                    var flowType = ViewState["flowType"].ToString();
                    var importType = ViewState["importType"].ToString();
                    var ticketType = ViewState["ticketType"].ToString();
                    ShelfProcess sp = new ShelfProcess();
                    var shelf = sp.GetSearchStorage(storage, areaId);
                    var storageType = EnumData.StorageType.標準暫存儲位;
                    //if (_flowType == (int)EnumData.FlowType.門市調回)
                    //{
                    //    storageType = EnumData.StorageType.調回暫存儲位;
                    //}
                    //else if (_flowType == (int)EnumData.FlowType.換貨)
                    //{
                    //    storageType = EnumData.StorageType.換貨暫存儲位;
                    //}
                    //else if (_flowType == (int)EnumData.FlowType.台組進貨)
                    //{
                    //    storageType = EnumData.StorageType.海運暫存儲位;
                    //}

                    if (shelf.Type != (int)storageType)
                    {
                        lbl_Message.Text = "這個儲位不屬於" + storageType.ToString() + "！";
                        return;
                    }

                    //串多少& 缺少的字串
                    for (int i = 0; i < LB_Product_Id1.Items.Count; i++)
                    {
                        listLeft.Add(LB_Product_Id1.Items[i].Text.Split('_')[0]);
                    }
                    var productDatas = new List<ImportClass.ProductData>();
                    for (int i = 0; i < LB_Product_Id2.Items.Count; i++)
                    {
                        var productData = new ImportClass.ProductData();
                        productData.Id = LB_Product_Id2.Items[i].Value.Split(',')[4];
                        productData.Name = LB_Product_Id2.Items[i].Text.Split('_')[0];
                        productData.Quantity = int.Parse(LB_Product_Id2.Items[i].Value.Split(',')[3]);
                        productData.Ticket = LB_Product_Id2.Items[i].Value.Split(',')[1];
                        productDatas.Add(productData);

                        listRight.Add(LB_Product_Id2.Items[i].Text.Split('_')[0]);
                    }

                    if (listLeft.Any())
                    {
                        lbl_Message.Text = "以下產品尚未驗完：" + string.Join(",", listLeft.Count);
                        txt_VerifyCheck_NO.Text = "";
                        return;
                    }

                    if (!listRight.Any())
                    {
                        lbl_Message.Text = "還沒驗到貨！請再次確認！";
                        return;
                    }

                    Session["ProductDatas"] = productDatas;
                    Session["storage"] = storage;
                    Session["flowType"] = flowType;
                    Session["pagekey"] = lblbox.Text;
                    //------------------------------------------------------
                    //箱號驗貨
                    var url = "DiffList.aspx?storage=" + storage + "&ticketType=" + ticketType + "&flowType=" + flowType + "&pagekey=" + lblbox.Text + "&areaId=" + areaId + "&importType=" + importType;

                    //Response.Redirect(url);
                    Page.RegisterClientScriptBlock("checkinput", @"<script>window.open('" + url + "','_self');</script>");

                    txt_VerifyCheck_NO.Enabled = false;
                }
                else
                {
                    lbl_Message.Text = "這個商品不是這張傳票的！";
                }

                txt_VerifyCheck_NO.Text = "";

                //更新數量
                CheckNumToLabel();

                txt_VerifyCheck_NO.Focus();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-條碼判斷
    }
}