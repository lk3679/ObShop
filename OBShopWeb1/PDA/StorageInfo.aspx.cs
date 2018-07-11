using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public partial class StorageInfo : System.Web.UI.Page
    {
        #region 宣告

        CheckFormat CF = new CheckFormat();
        ShelfProcess sp = new ShelfProcess();

        List<ShelfConfig> list = new List<ShelfConfig>();
        List<ShelfLog> log = new List<ShelfLog>();
        String account;
        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        String countv;

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Account"] == null)
            {
                Response.Write(" <script> parent.document.location= '../logout.aspx' </script> ");
                Response.End();
            }
            else
            {
                account = Session["Name"].ToString();
                txt_Input.Focus();

                //有值代入參數轉Session
                if (Request["SearchID"] != null)
                {
                    Session["SearchID"] = Request["SearchID"].ToString();
                    Response.Redirect("StorageInfo.aspx");
                }
                else if (Session["SearchID"] != null)
                {
                    txt_Input.Text = Session["SearchID"].ToString();
                    Session["SearchID"] = null;
                    CB_Log.Checked = true;
                    txt_Input_TextChanged(sender, e);
                }
            }
        }

        #endregion

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
                lbl_Message.Text = "";

                String str_input = txt_Input.Text.Trim();

                if (CF.CheckID(str_input, CheckFormat.FormatName.Storage))
                {
                    lbl_Info.Text = "";
                    lbl_Log.Text = "";
                    lbl_Storage_NO.Text = "";
                    lbl_Storage_NO_Type.Text = "";
                    lbl_Volume.Text = "";

                    if (sp.CheckStorage(str_input, _areaId) == null)
                    {
                        lbl_Message.Text = str_input + " 不存在 請設定！";
                    }
                    else
                    {

                        list = sp.GetSearchProduct(str_input, _areaId).OrderBy(x=>x.ProductNumber).ToList();
                        lbl_Storage_NO.Text = str_input;
                        lbl_Storage_NO_Type.Text = CF.TypeToName(sp.CheckStorage(str_input, _areaId));

                        if (list.Count > 0)
                        {
                            //轉給label
                            lbl_Info.Text = ResultToInfoList(list);
                            lbl_Volume.Text = countv;
                        }
                        else
                        {
                            lbl_Info.Text = "空儲位";
                            //lbl_Volume.Text = "0 / " + sp.GetMaxVolume(str_input, int_treasurytype);
                        }

                        //顯示log
                        if (CB_Log.Checked == true)
                        {
                            var loglist = sp.GetStorageLog(str_input, _areaId);
                            lbl_Log.Text = ResultToLogList((loglist == null) ? loglist :
                                loglist.Where(x => (CB_BOTLog.Checked) ? true : (x.LogAccount != "_BOT_" && x.StorageDetailTypeId != 4)).ToList(), str_input);
                        }
                        else
                        {
                            lbl_Log.Text = "";
                        }
                    }
                }
                else
                {
                    lbl_Message.Text = "請輸入儲位條碼！";
                }

                txt_Input.Text = "";
                txt_Input.Focus();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 副功能-串儲位內容資訊/串儲位Log資訊(2013-0711修改) 加來源、目的儲位 PDA沒加

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
                    info += "<br />" + shelfinfoList[i].ProductNumber + " x " + shelfinfoList[i].Quantity;// +" (" + shelfinfoList[i].Volume + ")";
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
        /// 串儲位Log資訊(2013-0711修改) 加來源、目的儲位 PDA沒加
        /// </summary>
        protected String ResultToLogList(List<ShelfLog> shelfLogList, String shelf)
        {
            String info = "儲位Log：(增加● 撿取○)";
            if (shelfLogList == null )
            {
                info += "<br />無Log";
            }
            else if (shelfLogList.Count > 0)
            {
                for (int i = 0; i < shelfLogList.Count; i++)
                {
                    bool inout = (shelfLogList[i].TargetStorage == shelf) ? true : false;
                    info += "<br />" + shelfLogList[i].LogDateTime.ToString("MM/dd") + "  " + shelfLogList[i].LogAccount + (inout ? " ● " : " ○ ") +
                        shelfLogList[i].ProductNumber + " x " + shelfLogList[i].Quantity + ", " + (inout ? "←" + shelfLogList[i].FromStorage : "→" + shelfLogList[i].TargetStorage);
                }
            }
            else
            {
                info += "<br />無Log";
            }
            return info;
        }

        #endregion
    }
}