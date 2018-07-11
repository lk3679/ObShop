using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    /// <summary>
    /// 盤點原則
    ///需盤點數 = 未撿貨確認 + 入庫 + 目前儲位數
    ///未撿貨確認的意思是指已印單卻未去刷確認，所以我們會視他為未從儲位取出
    ///入庫則是海運、換貨...等
    ///已上原則方可盤點，但為了確保儲位準確性如有撿貨未確認或是入庫未過帳之儲位，我們則不提供移除位、合併之動做
    /// </summary>
    public partial class InventoryNum : System.Web.UI.Page
    {
        #region 宣告

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        private ShelfProcess sp = new ShelfProcess();
        private CheckFormat CF = new CheckFormat();
        private String account;

        private setup auth = new setup();

        #endregion 宣告

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= '../logout.aspx' </script> ");
                    Response.End();
                }
                //新權限(2014-0512新增)
                else if (!auth.checkAuthorityPro("13"))
                {
                    Response.Redirect("../Privilege.aspx");
                }
                else
                {
                    account = Session["Name"].ToString();
                    lbl_Message.Text = "";
                    txt_Input.Focus();

                    if (!IsPostBack)
                    {
                        //Session["ShelfList"] = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion Page_Load

        #region 主功能-刷條碼

        /// <summary>
        /// txt改變(刷條碼)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_Input_TextChanged(object sender, EventArgs e)
        {
            try
            {
                SetInventory(txt_Input.Text);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// default
        /// </summary>
        /// <param name="儲位"></param>
        protected void SetInventory(string 輸入)
        {
            try
            {
                string str_input = 輸入.Trim();

                //儲位條碼
                if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
                {
                    Session["ShelfList"] = null;
                    lbl_CurrentNum.Text = "0";
                    int? shelfType = sp.CheckStorage(str_input, _areaId);
                    if (shelfType == null)
                    {
                        lbl_Message.Text = str_input + " 不存在，請設定！";
                        lblInventoryLog.Text = "";
                        return;
                    }

                    var strShelf = CF.CheckShelfType(shelfType.Value, "", (int)POS_Library.ShopPos.EnumData.MergeType.盤點); 
                    if (string.IsNullOrEmpty(strShelf))
                    {
                        //檢查來源儲位是否有檢貨中
                        var ckTemp = SeachShelf(str_input);
                        if (ckTemp)
                        {
                            return;
                        }

                        #region 查盤點紀錄

                        var storagelog = sp.GetInventoryLog(str_input, _areaId);
                        if (storagelog != null)
                            lblInventoryLog.Text = string.Format("上次盤點時間：{0}，盤點者：{1}", storagelog.Date.ToString("yyyy-MM-dd HH:mm:ss"), storagelog.Auditor);

                        #endregion 查盤點紀錄

                        lbl_Storage_NO.Text = str_input;
                        txt_Input.Text = "";
                        txt_Input.Focus();
                        lbl_Product.Text = "";

                        #region 目前儲位內容

                        var 目前儲位內容 = sp.GetNowStorageDetail(str_input, _areaId);
                        if (!目前儲位內容.Any())
                        {
                            lbl_Message.Text = "空儲位";
                            if (CB_EmptyCantDo.Checked)
                            {
                                btn_Submit.Enabled = false;
                                lbl_Message.Text = "空儲位不可使用盤點功能!";
                            }
                            return;
                        }
                        else
                        {
                            Session["ShelfList"] = 目前儲位內容;
                        }

                        #endregion 目前儲位內容

                        lbl_Storage_NO_Type.Text = CF.TypeToName(sp.CheckStorage(str_input, _areaId));

                        //可按盤點完成
                        btn_Submit.Enabled = true;
                    }
                    else
                    {
                        lbl_Message.Text = strShelf;
                        return;
                    }
                }
                else if (string.IsNullOrEmpty(lbl_Storage_NO.Text))
                {
                    lbl_Message.Text = "請先刷儲位條碼！";
                }
                else
                {
                    if (!string.IsNullOrEmpty(str_input))
                    {
                        #region 檢查條碼、產品編號

                        //產編、條碼都可吃
                        string productName = string.Empty;
                        if (CF.CheckID(str_input, CheckFormat.FormatName.Product))
                        {
                            productName = sp.GetProductNum(str_input);
                        }
                        else
                        {
                            string ttt = sp.GetProductBarcode(str_input);
                            if (!string.IsNullOrEmpty(ttt))
                                productName = str_input;
                        }
                        if (string.IsNullOrEmpty(productName))
                        {
                            lbl_Message.Text = "產品條碼錯誤！";
                            return;
                        }

                        #endregion 檢查條碼、產品編號

                        //沒輸入就當做1個
                        int qua = (txt_Num.Text == "" || txt_Num.Text == "1") ? 1 : int.Parse(txt_Num.Text);

                        //儲位內容不為空，一筆一筆過濾
                        if (Session["ShelfList"] != null)
                        {
                            List<ShelfConfig> list = Session["ShelfList"] as List<ShelfConfig>;
                            //輸入數量幾個就跑幾次
                            for (int i = 0; i < qua; i++)
                            {
                                bool result = false;
                                foreach (var data in list)
                                {
                                    if (data.ProductNumber == productName)
                                    {
                                        lbl_Product.Text = productName + "<br />" + lbl_Product.Text;
                                        //算目前件數
                                        AddCurrentNum();
                                        if (--data.Quantity == 0)
                                        {
                                            list.Remove(data);
                                        }
                                        result = true;
                                        break;
                                    }
                                }

                                if (!result)
                                {
                                    lbl_Product.Text = productName + " (多出)<br />" + lbl_Product.Text;
                                    //算目前件數
                                    AddCurrentNum();
                                }
                            }

                            lbl_Message.Text = "";
                        }
                        else
                        {
                            //儲位內容為空，全部都是多出的
                            //輸入數量幾個就跑幾次
                            for (int i = 0; i < qua; i++)
                            {
                                lbl_Product.Text = productName + " (多出)<br />" + lbl_Product.Text;

                                //算目前件數
                                AddCurrentNum();
                            }
                        }
                    }
                }
                txt_Num.Text = "";
                txt_Input.Text = "";
                txt_Input.Focus();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 查詢儲位內容
        /// 門市驗完貨就算過帳，所以移動時只要看是否還在檢貨中
        /// </summary>
        /// <param name="str_input"></param>
        protected bool SeachShelf(string str_input)
        {
            #region 顯示 撿貨未確認

            var 目前儲位內容 = sp.GetStorageConfig(str_input, _areaId);
            //Session["ShelfList"] = 目前儲位內容;
            var 目前儲位內容數 = 目前儲位內容 == null ? 0 : 目前儲位內容.Sum(x => x.Quantity);
            var 撿貨未確認 = sp.GetTempStorageDetail(str_input, "", _areaId).ToList();
            lbl_ShelfQuantity.Text = string.Format("可移：{0}, 撿貨中：{1}", 目前儲位內容數, 撿貨未確認.Count);
            //----------------------------------------------
            if (撿貨未確認.Count > 0)
            {
                lbl_Message.Text = "此儲位含有撿貨未確認的產品!!";
                return true;
            }
            else
            {
                return false;
            }

            #endregion 顯示 撿貨未確認
        }

        /// <summary>
        /// 算目前件數
        /// </summary>
        protected void AddCurrentNum()
        {
            lbl_CurrentNum.Text = (int.Parse(lbl_CurrentNum.Text) + 1).ToString();
        }

        #endregion 主功能-刷條碼

        #region 主功能-按下盤點完成

        /// <summary>
        /// 按下盤點完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(lbl_Storage_NO.Text.Trim()))
                {
                    string[] productList = lbl_Product.Text.Split('<');
                    string more = "";
                    string product = "";
                    string lack = "";
                    List<ShelfConfig> list = Session["ShelfList"] as List<ShelfConfig>;

                    foreach (var data in productList)
                    {
                        var newData = data.Replace("br />", "");

                        if (newData.Contains("(多出)"))
                        {
                            more = string.IsNullOrEmpty(more) ? more : more + ",";
                            more += newData.Replace("(多出)", "");
                        }
                        else
                        {
                            product = string.IsNullOrEmpty(product) ? product : product + ",";
                            product += newData;
                        }
                    }

                    //空儲位不會有缺少的
                    if (list != null)
                    {
                        var lackQuantity = list.Sum(x => x.Quantity);
                        if (lackQuantity > 100)
                        {
                            lbl_Message.Text = "打銷數量請在100件以內！";
                            return;
                        }
                        else
                        {
                            foreach (var data in list)
                            {
                                for (int i = 0; i < data.Quantity; i++)
                                {
                                    lack = string.IsNullOrEmpty(lack) ? lack : lack + ",";
                                    lack += data.ProductNumber;
                                }
                            }
                        }
                    }

                    //Session(2013-0402修改)--------------------------------
                    Session["PDAlack"] = lack;
                    Session["PDAmore"] = more;
                    Session["PDAproduct"] = product;

                    Response.Redirect("DiffListPDA.aspx?storage=" + lbl_Storage_NO.Text);
                    //------------------------------------------------------
                }
                else
                {
                    lbl_Message.Text = "請先刷儲位條碼！";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-按下盤點完成
    }
}