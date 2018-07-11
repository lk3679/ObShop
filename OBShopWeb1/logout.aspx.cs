using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class logout : System.Web.UI.Page
    {
        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.Header.Title = Page.Header.Title.Split('-')[0] + "-" + ((Utility.Store)int.Parse(Area.WmsAreaXml("ShopType"))).ToString();

                String urlx = "";

                //加自動跳回(2014-0224)
                if (Request["urlx"] != null)
                    urlx = Request["urlx"].ToString();
                //加Pos自動跳回(2014-0409)
                if (Request["Pos"] != null)
                    urlx += "?Pos=" + Request["Pos"].ToString();

                //清空空
                if (Session["Account"] != null)
                {
                    //移除onlineList帳號(2013-1106新增)-----
                    Application.Lock();
                    var onlineList = (List<Utility.OnlinePerson>)Application["onlineList"];
                    if (onlineList.Where(x => x.Name == Session["Name"].ToString() && x.IP == Session["ip"].ToString()).Count() > 0)
                    {
                        onlineList.Remove(onlineList.Where(x => x.Name == Session["Name"].ToString() && x.IP == Session["ip"].ToString()).FirstOrDefault());
                        Application["onlineList"] = onlineList;
                    }
                    Application.UnLock();
                    //--------------------------------------

                    //Session["ID"] = null;
                    //Session["PW"] = null;
                    //Session["Account"] = null;
                    //Session["Name"] = null;
                    //Session["logisticAccount"] = null;
                    //Session["Zone"] = null;
                    //Session["ip"] = null;
                    //Session["ShipOutType"] = null;
                    ////出貨狀態查詢用
                    //Session["editinvoice"] = null;
                    //Session["shipping_id"] = null;
                    ////PDA/InventoryNum盤點用(2013-0402新增)
                    //Session["PDAlack"] = null;
                    //Session["PDAmore"] = null;
                    //Session["PDAproduct"] = null;
                    //Session["ShelfList"] = null;
                    ////DiffList用(2013-0408新增)
                    //Session["product"] = null;
                    //Session["ticketId"] = null;
                    //Session["storage"] = null;
                    //Session["flowType"] = null;
                    //Session["pagekey"] = null;

                    ////權限
                    //Session["kw"] = null;
                    //Session["audit"] = null;
                    //Session["privilege"] = null;
                    //Session["logistics"] = null;
                    //Session["logistics_priv"] = null;
                    //Session["pos"] = null;
                    //Session["promotion_audit"] = null;
                    ////權限List
                    //Session["authority"] = null;

                    Session.Clear();
                }

                if (urlx != null && urlx != "")
                    Response.Redirect("Default.aspx?urlx=" + urlx);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion
    }
}