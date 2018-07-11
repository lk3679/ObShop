using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data; 
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Configuration;
using System.Web.Configuration;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class logistics_account : System.Web.UI.Page
    {
        #region 宣告

        DataTable dt = new DataTable("ALLData");
        List<POS_Library.ShopPos.DataModel.LogisticsModel> LDList = new List<POS_Library.ShopPos.DataModel.LogisticsModel>();
        String zone, act, id, editid, account;

        LinkButton LBtn_Temp;
        Label lbl_space; 
        bool Result;
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Session["Account"] = "jack";
                //Session["Account"] = "AW";
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    account = Session["Account"].ToString();
                    lbl_Count.Text = "";

                    //使用ViewState 記錄datatable狀態
                    if (ViewState["dt"] == null)
                        ViewState["dt"] = dt;
                    //暫時記錄修改的值
                    if (txt_EditAccount.Text != "")
                        Session["EditAccount"] = txt_EditAccount.Text;

                    if (Request["act"] != null)
                        act = Request["act"].ToString();

                    zone = _areaId.ToString();
                    
                    if (Request["id"] != null)
                        id = Request["id"].ToString();
                    if (Request["EditAccount"] != null)
                        editid = Request["EditAccount"].ToString();


                    SetTable();
                    if (act != "delete_account")
                    {
                        LoadLogisticsList();
                    }

                    switch (act)
                    {
                        case "add": setAddView(); break;
                        case "edit": setEditView(); break;
                        case "barcode": BarcodePDF(); break;
                        case "delete_account": delete_account(); break;
                        default: break;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 設定

        /// <summary>
        /// 建立dt
        /// </summary>
        protected void SetTable()
        {
            try
            {
                //datatable
                dt.Columns.Add("ID", typeof(String));
                dt.Columns.Add("條碼", typeof(String));
                dt.Columns.Add("帳號", typeof(String));
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 主功能-讀取名單

        /// <summary>
        /// 讀取名單
        /// </summary>
        protected void LoadLogisticsList()
        {
            try
            {
                dt.Clear();

                var logistics = new POS_Library.ShopPos.LogisticsAccount();
                try
                {
                    if (zone != null)
                        LDList = logistics.GetLoadLogistics(zone);
                }
                catch (Exception ex)
                {
                    Response.Write("系統發生錯誤 " + ex.Message);
                }

                #region ●有特定帳號的話過濾(2014-0115新增)

                //有特定帳號的話過濾------------------------
                var SearchName = txt_Name.Text.Trim();
                if (!string.IsNullOrEmpty(SearchName))
                    LDList = LDList.Where(x => x.Account.Contains(SearchName)).ToList();

                #endregion

                //List資料丟進datatable內
                if (LDList.Count > 0)
                {
                    for (int i = 0; i < LDList.Count; i++)
                    {
                        DataRow dr = dt.NewRow();
                        dr["ID"] = LDList[i].Id;
                        dr["條碼"] = LDList[i].Barcode;
                        dr["帳號"] = LDList[i].Account;

                        dt.Rows.Add(dr);
                    }
                }

                ViewState["dt"] = dt;

                gv_Account.DataSource = dt;
                gv_Account.DataBind();

                var 總筆數 = LDList.Count;
                lbl_Count.Text = "總筆數：" + 總筆數;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 介面

        /// <summary>
        /// 轉至新增版面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/logistics_account.aspx?act=add&zone=" + zone);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 顯示新增區塊
        /// </summary>
        protected void setAddView()
        {
            try
            {
                P_list.Visible = false;
                P_add.Visible = true;
                P_edit.Visible = false;
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 轉至修改版面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Edit_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect("~/logistics_account.aspx?act=edit&id=1");
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 顯示修改區塊
        /// </summary>
        protected void setEditView()
        {
            try
            {
                P_list.Visible = false;
                P_add.Visible = false;
                P_edit.Visible = true;


                var logistics = new POS_Library.ShopPos.LogisticsAccount();
                LDList = logistics.GetLogisticsDetail(id);
                
                //只有一筆
                if (LDList.Count == 1)
                {
                    lbl_ID.Text = LDList.First().Id.ToString();
                    lbl_barcode.Text = LDList.First().Barcode;
                    txt_EditAccount.Text = LDList.First().Account;
                }
                else
                {
                    Response.Redirect("~/logistics_account.aspx");
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 主功能-新增

        /// <summary>
        /// 新增確定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_AddSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                P_list.Visible = true;
                P_add.Visible = false;
                P_edit.Visible = false;

                AddSubmit();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 新增帳號
        /// </summary>
        protected void AddSubmit()
        {
            try
            {
                //lbl_AddMessage.Text = "";

                if (txt_AddAccount.Text != "")
                {

                    var logistics = new POS_Library.ShopPos.LogisticsAccount();
                    var MSList = logistics.SetLogistics(txt_AddAccount.Text, zone); 

                    if (MSList.Result == "1")
                    {
                        //Response.Write("<script>alert('新增成功!');location.href='logistics_account.aspx';</script>");
                        Page.RegisterClientScriptBlock("checkinput", @"<script>alert('新增成功!');location.href='logistics_account.aspx';</script>");
                    }
                    else
                    {
                        //Response.Write("<script>alert('此帳號已存在，新增失敗!');</script>");
                        Page.RegisterClientScriptBlock("checkinput", @"<script>alert('失敗! " + MSList.Reason + "');location.href='logistics_account.aspx';</script>");
                    }
                }
                else
                {
                    //Response.Write("<script>alert('帳號不可為空');location.href='logistics_account.aspx?act=add&zone=1';</script>");
                    Page.RegisterClientScriptBlock("checkinput", @"<script>alert('帳號不可為空');location.href='logistics_account.aspx?act=add&zone=" + zone + "';</script>");
                    //lbl_AddMessage.Text = "帳號不可為空";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 主功能-修改

        /// <summary>
        /// 修改確定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_EditSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                P_list.Visible = true;
                P_add.Visible = false;
                P_edit.Visible = false;

                EditSubmit();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 修改帳號
        /// </summary>
        protected void EditSubmit()
        {
            try
            {
                //lbl_EditMessage.Text = "";
                if (Session["EditAccount"] != null)
                    txt_EditAccount.Text = Session["EditAccount"].ToString();

                if (txt_EditAccount.Text != "")
                {
                    var logistics = new POS_Library.ShopPos.LogisticsAccount();
                    var MSList = logistics.UpdateLogistics(lbl_ID.Text, txt_EditAccount.Text);

                    if (MSList.Result == "1")
                    {
                        //Response.Write("<script>alert('修改成功!');location.href='logistics_account.aspx';</script>");
                        Page.RegisterClientScriptBlock("checkinput", @"<script>alert('修改成功!');location.href='logistics_account.aspx';</script>");
                        Session["EditAccount"] = null;
                    }
                    else
                    {
                        //Response.Write("<script>alert('此帳號已存在，修改失敗!');</script>");
                        Page.RegisterClientScriptBlock("checkinput", @"<script>alert('失敗! " + MSList.Reason + "');</script>");
                    }
                }
                else
                {
                    //Response.Write("<script>alert('帳號不可為空');location.href='logistics_account.aspx?act=edit&id=" + lbl_ID.Text + "';</script>");
                    Page.RegisterClientScriptBlock("checkinput", @"<script>alert('帳號不可為空');location.href='logistics_account.aspx?act=edit&id=" + lbl_ID.Text + "';</script>");
                    //lbl_AddMessage.Text = "帳號不可為空";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 主功能-取消/刪除

        /// <summary>
        /// 取消新增or編輯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Cancel_Click(object sender, EventArgs e)
        {
            try
            {
                P_list.Visible = true;
                P_add.Visible = false;
                P_edit.Visible = false;

                Response.Redirect("~/logistics_account.aspx");
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 刪除帳號(2012-0528新增)
        /// </summary>
        protected void delete_account()
        {
            try
            {

                var logistics = new POS_Library.ShopPos.LogisticsAccount();
                Result = logistics.SetDelLogistics(id);

                switch (Result)
                {
                    case true:   //成功
                        Page.RegisterClientScriptBlock("checkinput", @"<script>alert('刪除 成功!');window.close();</script>");
                        break;
                    case false:   //失敗
                        Page.RegisterClientScriptBlock("checkinput", @"<script>alert('刪除 失敗!');window.close();</script>");
                        break;

                    default: break;
                }

            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region GV介面

        /// <summary>
        /// gv換頁設定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_Account_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                gv_Account.PageIndex = e.NewPageIndex;
                gv_Account.DataSource = (DataTable)ViewState["dt"];
                gv_Account.DataBind();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 查詢(無用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            try
            {
                LoadLogisticsList();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 主功能-產生barcode PDF

        /// <summary>
        /// 產生barcode PDF
        /// </summary>
        protected void BarcodePDF()
        {
            try
            {

                var logistics = new POS_Library.ShopPos.LogisticsAccount();
                LDList = logistics.GetLogisticsDetail(id);
                var LBList = logistics.GetBarcode(id);
                    
                //if有找到資料
                if (LDList.Count > 0)
                {
                    //宣告文件doc1
                    Document doc1 = new Document(PageSize.A4, 20, 20, 40, 20);
                    MemoryStream Memory = new MemoryStream();
                    PdfWriter PdfWriter = PdfWriter.GetInstance(doc1, Memory);

                    //建立中文字型
                    BaseFont bfChinese = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\mingliu.ttc,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);

                    //開始編輯doc1
                    doc1.Open();

                    //取得內容物件DirectContent
                    PdfContentByte PCB = PdfWriter.DirectContent;

                    //啟用PCB
                    PCB.SaveState();
                    PCB.BeginText();
                    PCB.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL);
                    PCB.SetFontAndSize(bfChinese, 14);

                    float offset_xi = 50, offset_yi = 800;

                    for (int i = 0; i < 10; i++)
                    {
                        switch (i)
                        {
                            case 0: offset_xi = 60; offset_yi = 770; break;
                            case 1: offset_xi = 285; offset_yi = 770; break;
                            case 2: offset_xi = 60; offset_yi = 630; break;
                            case 3: offset_xi = 285; offset_yi = 630; break;
                            case 4: offset_xi = 60; offset_yi = 490; break;
                            case 5: offset_xi = 285; offset_yi = 490; break;
                            case 6: offset_xi = 60; offset_yi = 350; break;
                            case 7: offset_xi = 285; offset_yi = 350; break;
                            case 8: offset_xi = 60; offset_yi = 210; break;
                            case 9: offset_xi = 285; offset_yi = 210; break;
                        }

                        //設定帳號及圖片位置
                        PCB.ShowTextAligned(0, LDList[0].Account, offset_xi, offset_yi, 0);
                        iTextSharp.text.Image img;
                        img = iTextSharp.text.Image.GetInstance(LBList[0].ImageSource);
                        img.ScalePercent(80, 40);
                        img.SetAbsolutePosition(offset_xi - 35, offset_yi - 70);
                        PCB.AddImage(img);

                    }
                    PCB.EndText();
                    //畫格線
                    PCB.SetRGBColorFill(0x00, 0x00, 0x00);
                    //上下兩條橫線
                    PCB.Rectangle(30, 810, 450, 1);
                    PCB.Fill();
                    PCB.Rectangle(30, 109, 450, 1);
                    PCB.Fill();
                    //三條垂直線
                    PCB.Rectangle(30, 110, 1, 700);
                    PCB.Fill();
                    PCB.Rectangle(480, 109, 1, 702);
                    PCB.Fill();
                    PCB.Rectangle(255, 110, 1, 701);
                    PCB.Fill();
                    //中間四條橫線
                    PCB.Rectangle(30, 249, 450, 1);
                    PCB.Fill();
                    PCB.Rectangle(30, 389, 450, 1);
                    PCB.Fill();
                    PCB.Rectangle(30, 529, 450, 1);
                    PCB.Fill();
                    PCB.Rectangle(30, 669, 450, 1);
                    PCB.Fill();

                    //關閉PCB
                    //PCB.EndText();
                    PCB.RestoreState();

                    doc1.Close();

                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment;filename=logistics_account_barcode_" + id + ".pdf");
                    Response.ContentType = "application/octet-stream";
                    Response.OutputStream.Write(Memory.GetBuffer(), 0, Memory.GetBuffer().Length);
                    Response.OutputStream.Flush();
                    Response.OutputStream.Close();
                    Response.Flush();
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region gv_Account_RowDataBound

        /// <summary>
        /// 在RowDataBound時建立HyperLink控制項
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_Account_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //產生TableCell
                TableCell tc = new TableCell();
                //若為DataRow則放入HyperLink
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //修改
                    HyperLink HL_Edit = new HyperLink();
                    HL_Edit.Text = "修改";
                    HL_Edit.NavigateUrl = "~/logistics_account.aspx?act=edit&id=" + e.Row.Cells[0].Text;
                    tc.Controls.Add(HL_Edit);
                    //空白
                    lbl_space = new Label();
                    lbl_space.Text = " ";
                    tc.Controls.Add(lbl_space);
                    //列印條碼
                    HyperLink HL_PrintBarcode = new HyperLink();
                    HL_PrintBarcode.Text = "列印條碼";
                    HL_PrintBarcode.NavigateUrl = "~/logistics_account.aspx?act=barcode&id=" + e.Row.Cells[0].Text;
                    tc.Controls.Add(HL_PrintBarcode);
                    //空白
                    lbl_space = new Label();
                    lbl_space.Text = " ";
                    tc.Controls.Add(lbl_space);
                    //產生Link(淘寶交易號)
                    LBtn_Temp = new LinkButton();
                    LBtn_Temp.Text = "刪除";
                    LBtn_Temp.OnClientClick = "return delete_account(this, '" + e.Row.Cells[0].Text + "');";
                    tc.Controls.Add(LBtn_Temp);
                }
                //若為Header則改標頭
                if (e.Row.RowType == DataControlRowType.Header)
                    tc.Text = "動作";
                //將TableCell加入Gridview中
                e.Row.Cells.Add(tc);

                //光棒效果
                //判定row的型態是資料行
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    GvLightBar.lightbar(e, 1);
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

    }
}