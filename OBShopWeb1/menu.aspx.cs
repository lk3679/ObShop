using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.EntranceService;
using System.Web.Configuration;

namespace OBShopWeb
{
    public partial class menu : System.Web.UI.Page
    {
        #region 宣告

        EntranceClient EC = new EntranceClient();
        setup auth = new setup();

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Session消失時 導回登出頁面
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                }
                //帳號登入or 物流barcode登入            
                else
                {
                    //使用者帳號
                    lbl_Account.Text = "" + Session["Name"].ToString();

                    //取得權限區域
                    if (Session["Zone"] != null)
                    {
                        switch (Session["Zone"].ToString())
                        {
                            case "administrator": lbl_Zone.Text = "區域：" + "全部"; break;
                            case "1": lbl_Zone.Text = "區域：" + "台灣"; break;
                            case "2": lbl_Zone.Text = "區域：" + "虎門"; break;
                        }
                    }

                    if (Session["logisticAccount"] != null)
                    {
                        bool xbool = bool.Parse(Session["logisticAccount"].ToString());
                        if (xbool)
                        {
                            //HL_menu_ship_manage1.Visible = false;
                            //HL_menu_ship_manage2.Visible = false;
                        }
                    }

                    if (lbl_Account.Text == "Guest")
                    {
                        //HL_BusNO_Invoice_Menu.Visible = false;
                        HL_logistics_account_Menu.Visible = false;
                        //HL_logout.Visible = false;
                        HL_menu_ship_manage1.Visible = false;
                        //HL_menu_ship_manage2.Visible = false;
                        HL_ShipIn_Menu.Visible = false;
                        HL_ShipOut_Menu.Visible = false;
                        HL_Storage1.Visible = false;
                        HL_Storage2.Visible = false;
                        HL_system_Menu.Visible = false;
                        HL_SystemInfo.Visible = false;
                    }

                    ////人員管理
                    //if (auth.checkAuthority("logistics"))
                    //{
                    //    HL_logistics_account_Menu.Visible = true;
                    //}
                    //else
                    //{
                    //    HL_logistics_account_Menu.Visible = false;
                    //}

                    HL_pos_check_out2.Visible = auth.checkAuthorityPro("15") && (Session["ClerkID"] != null);

                    //系統管理
                    if (auth.checkAuthority("administrator") || (auth.checkAuthorityPro("2")))
                    {
                        HL_system_Menu.Visible = true;
                    }
                    else
                    {
                        HL_system_Menu.Visible = false;
                    }
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