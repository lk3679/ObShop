using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using POS_Library.Public;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    /// <summary>
    /// 批量合併
    /// 一般入庫
    /// 一般合併
    /// </summary>
    public partial class Merge : System.Web.UI.Page
    {
        #region 宣告

        private CheckFormat CF = new CheckFormat();
        private ShelfProcess sp = new ShelfProcess();

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        private String account;

        //新增內容明細(2013-0308新增)
        private List<ShelfConfig> list = new List<ShelfConfig>();

        private List<ShelfLog> log = new List<ShelfLog>();
        private String countv;

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
                else
                {
                    account = Session["Name"].ToString();
                    lbl_Message.Text = "";
                    txt_Input.Focus();
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
                string str_input = txt_Input.Text.Trim();
                //檢查儲位條碼
                if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
                {
                    #region 儲位條碼

                    int? shelfType = sp.CheckStorage(str_input, _areaId);
                    if (shelfType == null)
                    {
                        lbl_Message.Text = str_input + " 不存在，請設定！";
                    }
                    else
                    {
                        //第一次一定要刷入儲位，檢查來源儲位是否已填入
                        if (string.IsNullOrEmpty(lbl_FromStorage_NO.Text))
                        {
                            var fromType = shelfType.Value;
                            ViewState["fromType"] = fromType;
                            var strShelf = CF.CheckShelfType(fromType, "", (int)POS_Library.ShopPos.EnumData.MergeType.合併儲位); 
                            if (string.IsNullOrEmpty(strShelf))
                            {
                                //檢查來源儲位是否有檢貨中
                                var ckTemp = SeachShelf(str_input);
                                if (ckTemp)
                                {
                                    return;
                                }
                                //新增內容明細(2013-0308新增)----------------------------
                                list = sp.GetSearchProduct(str_input, _areaId);

                                lbl_Info.Text = "";

                                lbl_FromStorage_NO.Text = str_input;
                                lbl_FromStage_NO_Type.Text = CF.TypeToName(fromType);
                                lbl_Message.Text = "";

                                if (list.Count > 0)
                                {
                                    //需顯示內容
                                    if (CB_Info.Checked == true)
                                    {
                                        //轉給label
                                        lbl_Info.Text = ResultToInfoList(list);
                                    }

                                    CountNum(list);
                                    lbl_Volume.Text = countv;
                                }
                                else
                                {
                                    lbl_Info.Text = "空儲位";
                                    lbl_Volume.Text = "(0 件)";
                                }
                                //-------------------------------------------------------
                            }
                            else
                            {
                                lbl_Message.Text = strShelf;
                                return;
                            }
                        }
                        else
                        {
                            if (str_input == lbl_FromStorage_NO.Text)
                            {
                                lbl_Message.Text = "不能和自己合併！";
                            }
                            //輸入目的儲位
                            //確認來源及目標符合規定
                            var fromType = int.Parse(ViewState["fromType"].ToString());
                            var targetType = shelfType.Value;
                            var strShelf = CF.CheckShelfType(fromType, targetType.ToString(), (int)POS_Library.ShopPos.EnumData.MergeType.合併儲位); 
                            if (!string.IsNullOrEmpty(strShelf))
                            {
                                lbl_Message.Text = strShelf;
                                return;
                            }
                            lbl_TargetStorage_NO.Text = str_input;
                            lbl_TargetStorage_NO_Type.Text = CF.TypeToName(shelfType);
                            btn_Submit.Visible = true;
                        }
                    }

                    #endregion 儲位條碼
                }
                else if (string.IsNullOrEmpty(lbl_FromStorage_NO.Text))
                {
                    lbl_Message.Text = "請先刷儲位條碼！";
                }

                txt_Input.Text = "";
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-刷條碼

        #region 主功能-合併

        /// <summary>
        /// 按下確定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                string From, Target;
                From = lbl_FromStorage_NO.Text.Trim();
                Target = lbl_TargetStorage_NO.Text.Trim();
                int? fromShelfType = sp.CheckStorage(From, _areaId);
                if (fromShelfType == null)
                {
                    lbl_Message.Text = From + " 不存在 請設定！";
                }

                if (lbl_Message.Text == "")
                {
                    MsgStatus result = new MsgStatus(); 
                    //檢查是否符合入庫上架
                    var isImport = Utility.GetStorageTempAll().Contains(fromShelfType.Value);  
                    result = sp.MergeStorage(From, Target, null, account, _areaId, isImport); 

                    if (result.Result == "1")
                    {
                        lbl_Message.Text = From + " -> " + Target + " 合併成功！";
                        //清空
                        lbl_FromStorage_NO.Text = "";
                        lbl_TargetStorage_NO.Text = "";
                        lbl_FromStage_NO_Type.Text = "";
                        lbl_TargetStorage_NO_Type.Text = "";
                        lbl_Info.Text = "";
                        lbl_ShelfQuantity.Text = "";
                        lbl_Volume.Text = "";

                        btn_Submit.Visible = false;
                    }
                    else
                    {
                        lbl_Message.Text = result.Reason;
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-合併

        #region 查詢儲位內容

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

        #endregion

        #region 副功能-串儲位內容資訊(2013-0308新增)

        /// <summary>
        /// 串儲位內容資訊
        /// </summary>
        protected String ResultToInfoList(List<ShelfConfig> shelfinfoList)
        {
            String info = "儲位內容：";
            int allcount = 0;
            if (shelfinfoList.Count > 0)
            {
                for (int i = 0; i < shelfinfoList.Count; i++)
                {
                    info += "<br />" + shelfinfoList[i].ProductNumber + " x " + shelfinfoList[i].Quantity + " (" + shelfinfoList[i].Volume + ")";
                    allcount += shelfinfoList[i].Quantity;
                }
            }
            else
            {
                info += "空儲位";
            }

            countv = "(" + allcount + "件) ";

            return info + "<br />";
        }

        /// <summary>
        /// 算件數
        /// </summary>
        protected void CountNum(List<ShelfConfig> shelfinfoList)
        {
            String info = "儲位內容：";
            int allcount = 0;
            if (shelfinfoList.Count > 0)
            {
                allcount = shelfinfoList.Sum(x => x.Quantity);
            }
            else
            {
                info += "空儲位";
            }

            countv = allcount + " 件";
        }

        #endregion 副功能-串儲位內容資訊(2013-0308新增)
    }
}