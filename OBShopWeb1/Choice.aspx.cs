using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace OBShopWeb
{
    public partial class Choice : System.Web.UI.Page
    {
        #region 宣告

        String ShipOutType;

        #endregion

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
                    if (!IsPostBack)
                    {
                        if (Session["ShipOutType"] != null)
                        {
                            ShipOutType = Session["ShipOutType"].ToString();

                            if (ShipOutType == "官網")
                            {
                                //btn_KW.BorderColor = Color.Red;
                                //btn_OB.BorderColor = Color.White;
                                //lbl_Choice.Text = "目前選擇：官網";

                                btn_KW_Click(sender, e);
                            }
                            else if (ShipOutType == "橘熊")
                            {
                                //btn_OB.BorderColor = Color.Red;
                                //btn_KW.BorderColor = Color.White;
                                //lbl_Choice.Text = "目前選擇：橘熊";

                                btn_OB_Click(sender, e);
                            }

                            //Page.RegisterClientScriptBlock("checkinput", @"<script>alert('" + lbl_Choice.Text + "');</script>");
                        }
                        else
                        {
                            //lbl_Choice.Text = "請選擇出貨類別";
                            //Session["ShipOutType"] = "橘熊";
                            //btn_OB.BorderColor = Color.Red;
                            //btn_KW.BorderColor = Color.White;
                            //lbl_Choice.Text = "目前選擇：橘熊";

                            btn_OB_Click(sender, e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 主功能-官網/橘熊

        /// <summary>
        /// 點官網
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_KW_Click(object sender, EventArgs e)
        {
            try
            {
                Session["ShipOutType"] = "官網";
                

                //Page.RegisterClientScriptBlock("checkinput", @"<script>alert('" + lbl_Choice.Text + "');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 點橘熊
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_OB_Click(object sender, EventArgs e)
        {
            try
            {
                Session["ShipOutType"] = "橘熊";
                
                //Page.RegisterClientScriptBlock("checkinput", @"<script>alert('" + lbl_Choice.Text + "');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion
        
    }
}