using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using OBShopWeb.PDA;
using POS_Library.Public;
using POS_Library.ShopPos;

namespace OBShopWeb
{
    public partial class ShipOutVerify : System.Web.UI.Page
    {
        #region 宣告

        private string account;

        #endregion 宣告

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Session["Account"] = "AW";
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
                        if (Request["tick"] == null || Request["area"] == null || Request["store"] == null)
                        {
                            throw new Exception("資料錯誤！");
                        }

                        var ticketId = int.Parse(Request["tick"].ToString());
                        var areaId = int.Parse(Request["area"].ToString());
                        var store = int.Parse(Request["store"].ToString());
                        ViewState["areaId"] = areaId;
                        ViewState["store"] = store;
                        lbl_ticket_id.Text = ticketId.ToString();
                        txt_VerifyCheck_NO.Focus();

                        setListBox(ticketId, store, areaId);
                    }
                    txt_VerifyCheck_NO.Focus();
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion Page_Load

        #region 主功能-取得產品List

        /// <summary>
        /// 將取得的產品List放入ListBox
        /// </summary>
        protected void setListBox(int ticketId, int store, int areaId)
        {
            try
            {
                //lbl_Message.Text += " 提醒：請檢查是否有撿貨單未刷確認!";
                var shipDa = new POS_Library.ShopPos.ShipOutDA();

                var list = shipDa.GetTicketDetail(int.Parse(lbl_ticket_id.Text.Trim()), areaId, false, false);

                if (list.Count > 0)
                {
                    var pickNum = string.Format("{0}{1}{2}{3}", ticketId, store.ToString().PadLeft(2, '0'), areaId.ToString().PadLeft(2, '0'), ((int)Utility.ShipPDF.寄倉調出).ToString().PadLeft(2, '0'));
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Quantity > 0)
                        {
                            ListItem LItem = new ListItem();
                            LItem.Text = list[i].ProductId + "__X " + list[i].Quantity;
                            LItem.Value = list[i].Barcode + "," + list[i].ProductId + "," + list[i].Quantity + "," + pickNum;
                            LItem.Attributes.Add("value2", list[i].TicketId.ToString());
                            LB_Product_Id1.Items.Add(LItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-取得產品List

        #region 主功能-再確認

        /// <summary>
        /// 輸入txtbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_VerifyCheck_NO_TextChanged(object sender, EventArgs e)
        {
            lbl_Message.Text = "";

            var store = int.Parse(ViewState["store"].ToString());
            var areaId = int.Parse(ViewState["areaId"].ToString());
            ShelfProcess sp = new ShelfProcess();
            CheckFormat CF = new CheckFormat(); 
            LB_Product_Id1.ClearSelection();
            LB_Product_Id2.ClearSelection();
            var 傳票ID = lbl_ticket_id.Text.Trim();
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

                //確認
            else if (txt_VerifyCheck_NO.Text == "1")
            {
                if (LB_Product_Id1.Items.Count != 0)
                {
                    lbl_Message.Text = "部分商品未驗！";
                    return;
                }
                else if (string.IsNullOrEmpty(傳票ID))
                {
                    lbl_Message.Text = "傳票不可為空！";
                    return;
                }
                else
                {
                    var shipDa = new POS_Library.ShopPos.ShipOutDA();
                    var msg = shipDa.LeaveWithVerify(int.Parse(傳票ID), account, store, DateTime.Now, areaId);
                    if (msg.Result == "1")
                    {
                        lbl_Message.Text = "驗貨完成！";
                        txt_VerifyCheck_NO.Enabled = false;
                    }
                    else
                    {
                        lbl_Message.Text = msg.Reason;
                    }
                }
            }

                //產品編號
            else if (Regex.IsMatch(txt_VerifyCheck_NO.Text, "^\\d{8}$"))
            {
                bool find = false;

                for (int i = 0; i < LB_Product_Id1.Items.Count; i++)
                {
                    //if (LB_Product_Id1.Items[i].Value == txt_VerifyCheck_NO.Text)
                    //{
                    //lbl_Message.Text = LB_Product_Id1.Items[i].Value.Split(',')[4];
                    if (LB_Product_Id1.Items[i].Value.Split(',')[0] == txt_VerifyCheck_NO.Text)
                    {
                        LB_Product_Id2.Items.Insert(0, LB_Product_Id1.Items[i]);
                        LB_Product_Id1.Items.Remove(LB_Product_Id1.Items[i]);
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    lbl_Message.Text = "這個商品不是這張傳票的！";

                    //LB_Product_Id2.Items.Insert(0, new ListItem(sp.GetProductNum(txt_VerifyCheck_NO.Text) + "(多出)", txt_VerifyCheck_NO.Text));
                }
            }
            else if (CF.CheckID(txt_VerifyCheck_NO.Text, CheckFormat.FormatName.Storage))
            {
                if (LB_Product_Id1.Items.Count == 0)
                {
                    int? type = sp.CheckStorage(txt_VerifyCheck_NO.Text, areaId);

                    if (type == 6 || type == 7)
                    {
                        string product = "";

                        for (int i = 0; i < LB_Product_Id2.Items.Count; i++)
                        {
                            product = string.IsNullOrEmpty(product) ? product : product + ",";
                            product += LB_Product_Id2.Items[i].Text;
                        }

                        //Response.Redirect("~/DiffList.aspx?product=" + product + "&ticketId=" + lbl_ticket_id.Text + "&storage=" + txt_VerifyCheck_NO.Text.Trim());
                    }
                    else
                    {
                        lbl_Message.Text = "僅可為暫存儲位或不良暫存儲位！";
                    }
                }
                else
                {
                    lbl_Message.Text = "尚有產品未驗！";
                }
            }
            else
            {
                lbl_Message.Text = "這個商品不是這張傳票的！";
            }

            txt_VerifyCheck_NO.Text = "";
            txt_VerifyCheck_NO.Focus();
        }

        #endregion 主功能-再確認

        protected void LB_GoShippingMoreLack_Click(object sender, EventArgs e)
        {
            var pick = LB_Product_Id1.SelectedValue;
            if (!string.IsNullOrEmpty(pick))
            {
                var productid = pick.Split(',')[1];
                var pickNum = pick.Split(',')[3];
                var url = "NoProduct.aspx?pickNum=" + pickNum + "&ProductId=" + productid + "&PageKey=ShipOutVerify";

                Page.RegisterClientScriptBlock("checkinput", @"<script>window.open('" + url + "','_self');</script>");
            }
            else
            {
                lbl_Message.Text = "左邊清單尚未選擇異常回報產品！";
            }
        }
    }
}