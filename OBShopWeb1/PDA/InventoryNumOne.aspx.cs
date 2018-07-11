using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
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
    public partial class InventoryNumOne : System.Web.UI.Page
    {
        //針對單一產品盤點(2013-0409新增)

        #region 宣告

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        private ShelfProcess sp = new ShelfProcess();
        private CheckFormat CF = new CheckFormat();
        private String account;

        setup auth = new setup();

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
                 
                //產品
                if (string.IsNullOrEmpty(lbl_ProductName.Text))
                { 
                    #region 檢查 儲位、條碼、產品編號
                    if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
                    {
                        lbl_Message.Text = "請先輸入欲盤點品項！再輸入儲位！";
                        return;
                    } 
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

                    lbl_ProductName.Text = productName; 
                }
                //儲位條碼
                else if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
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

                    var strShelf = CF.CheckShelfType(shelfType.Value, "", (int)POS_Library.ShopPos.EnumData.MergeType.盤單品); 
                    if (string.IsNullOrEmpty(strShelf))
                    {
                        //檢查來源儲位是否有檢貨中與未過帳傳票
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
                         
                        var 目前儲位內容 = sp.GetNowStorageDetail(str_input, lbl_ProductName.Text, _areaId);
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
                else if (string.IsNullOrEmpty(lbl_Storage_NO.Text) )
                {
                    lbl_Message.Text = "請刷儲位條碼！";
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
                        if (lbl_ProductName.Text != productName)
                        {
                            lbl_Message.Text = "輸入產品與盤點產品不同！";
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
        /// default
        /// </summary>
        /// <param name="儲位"></param>
        protected void SetInventory1(string 輸入)
        {
            try
            {
                String str_input = 輸入.Trim();

                //儲位條碼
                if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
                {
                    #region 儲位條碼

                    if (lbl_ProductName.Text != "")
                    {
                        int? shelfType = sp.CheckStorage(str_input, _areaId);

                        Session["ShelfList"] = null;

                        if (sp.CheckStorage(str_input, _areaId) == null)
                        {
                            lbl_Message.Text = str_input + " 不存在 請設定！";
                            lblInventoryLog.Text = "";
                        }
                        var strShelf = CF.CheckShelfType(shelfType.Value, "", (int)POS_Library.ShopPos.EnumData.MergeType.盤單品); 
                        if (string.IsNullOrEmpty(strShelf))
                        {
                            #region 查盤點紀錄

                            lblInventoryLog.Text = "";
                            lbl_Message.Text = "";

                            var storagelog = sp.GetInventoryLog(str_input, _areaId);
                            if (storagelog != null)
                                lblInventoryLog.Text = string.Format("上次盤點時間：{0}，盤點者：{1}", storagelog.Date.ToString("yyyy-MM-dd HH:mm:ss"), storagelog.Auditor);

                            #endregion 查盤點紀錄

                            #region ●撿貨未確認

                            var 目前儲位內容 = sp.GetNowStorageDetail(str_input, lbl_ProductName.Text, _areaId);
                            var 撿貨未確認 = sp.GetTempStorageDetail(str_input, lbl_ProductName.Text, _areaId);

                            //把儲位數量分開(2014-0219新增)-----------------
                            lbl_ShelfQuantity.Text = string.Format("可盤：{0}, 撿貨中：{1}", 目前儲位內容.Count, 撿貨未確認.Count);
                            //----------------------------------------------
                            if (撿貨未確認.Count > 0)
                            {
                                var msg = "撿貨未確認";
                                var msg2 = "請小心";
                                lbl_Message.Text = str_input + " 此產品含有【" + msg + "】產品，" + msg2 + "盤點！";
                            }

                            #endregion

                            Session["ShelfList"] = 目前儲位內容;

                            lbl_Storage_NO.Text = str_input;
                            lbl_Storage_NO_Type.Text = CF.TypeToName(sp.CheckStorage(str_input, _areaId));

                            if (Session["ShelfList"] == null)
                            {
                                lbl_Message.Text = "儲位無此品項!";
                                return;
                            }
                            lbl_Product.Text = "";

                            //可按盤點完成
                            btn_Submit.Enabled = true;
                        }
                        else
                        {
                            lbl_Message.Text = strShelf;
                            return;
                        }
                    }
                    else
                    {
                        lbl_Message.Text = "請先輸入欲盤點品項!";
                    }

                    #endregion                    
                }
                else if (lbl_ProductName.Text != "" && String.IsNullOrEmpty(lbl_Storage_NO.Text))
                {
                    lbl_Message.Text = "請刷儲位條碼！";
                }
                //記下產品
                else if (lbl_ProductName.Text == "")
                {
                    #region 輸入欲盤點產品編號

                    //產編/條碼都可吃(2013-0329新增)------------
                    String temp = "";

                    if (CF.CheckID(str_input, CheckFormat.FormatName.Product))
                    {
                        temp = sp.GetProductNum(str_input);
                    }
                    else
                    {
                        String ttt = sp.GetProductBarcode(str_input);
                        if (!String.IsNullOrEmpty(ttt))
                            temp = str_input;
                    }

                    String productName = temp;

                    if (!String.IsNullOrEmpty(productName))
                    {
                        lbl_ProductName.Text = productName;
                    }
                    else
                    {
                        lbl_Message.Text = "產品條碼錯誤！";
                    }

                    #endregion
                }
                //產品條碼
                else if (str_input != "")
                {
                    #region 產品編號

                    //產編/條碼都可吃(2013-0329新增)------------
                    String temp = "";

                    if (CF.CheckID(str_input, CheckFormat.FormatName.Product))
                    {
                        temp = sp.GetProductNum(str_input);
                    }
                    else
                    {
                        String ttt = sp.GetProductBarcode(str_input);
                        if (!String.IsNullOrEmpty(ttt))
                            temp = str_input;
                    }

                    String productName = temp;
                    //------------------------------------------

                    if (!String.IsNullOrEmpty(productName))
                    {
                        //核對產品是否符合所盤點產品
                        if (lbl_ProductName.Text == productName)
                        {
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

                                            //算目前件數(2013-0401新增)
                                            lbl_CurrentNum.Text = (int.Parse(lbl_CurrentNum.Text) + 1).ToString();

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

                                        //算目前件數(2013-0401新增)
                                        lbl_CurrentNum.Text = (int.Parse(lbl_CurrentNum.Text) + 1).ToString();
                                    }
                                }

                                lbl_Message.Text = "";
                            }

                            //儲位內容為空，全部都是多出的
                            else
                            {
                                //輸入數量幾個就跑幾次
                                for (int i = 0; i < qua; i++)
                                {
                                    lbl_Product.Text = productName + " (多出)<br />" + lbl_Product.Text;

                                    //算目前件數(2013-0401新增)
                                    lbl_CurrentNum.Text = (int.Parse(lbl_CurrentNum.Text) + 1).ToString();
                                }
                            }
                        }
                        else
                        {
                            lbl_Message.Text = "產品不符！";
                        }
                    }
                    else
                    {
                        lbl_Message.Text = "產品條碼錯誤！";
                    }

                    #endregion
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

            var 目前儲位內容 = sp.GetNowStorageDetail(str_input, lbl_ProductName.Text, _areaId);
            //Session["ShelfList"] = 目前儲位內容;
            var 目前儲位內容數 = 目前儲位內容.Sum(x => x.Quantity);
            var 撿貨未確認 = sp.GetTempStorageDetail(str_input, lbl_ProductName.Text, _areaId).ToList();
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

            #endregion 顯示 撿貨未確認/傳票未過帳
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