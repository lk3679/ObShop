using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OBShopWeb.EntranceService;
using System.Net;
using OBShopWeb.AuthService;
using System.Collections.Specialized;
using System.Web.Configuration;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class Default : System.Web.UI.Page
    {
        #region 宣告

        EntranceClient EC = new EntranceClient();
        setup auth = new setup();
        AuthClient AC = new AuthClient();
        //NameValueCollection collection;
        private int SessionTimeOut = int.Parse(WebConfigurationManager.AppSettings.Get("SessionTimeOut"));
        List<Utility.OnlinePerson> onlineList;
        String urlx = "";
        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.Header.Title = Page.Header.Title.Split('-')[0] + "-" + ((Utility.Store)int.Parse(Area.WmsAreaXml("ShopType"))).ToString();

                Session.Timeout = SessionTimeOut;
                ShowOnlineList();

                if (Request["urlx"] != null)
                    urlx = Request["urlx"].ToString();

                if (!IsPostBack)
                {
                    //cookieIP(2012-1205新增)---------------------
                    if (Request.Cookies["myip"] == null)
                    {
                        var cookie = new HttpCookie("myip", HttpContext.Current.Request.UserHostAddress);
                        cookie.Expires = DateTime.Now.AddYears(1);
                        Response.Cookies.Add(cookie);

                        lbl_CookieIP.Text = HttpContext.Current.Request.UserHostAddress;
                    }
                    else
                    {
                        lbl_CookieIP.Text = Request.Cookies["myip"].Value;
                    }
                    //---------------------------------------------

                    if (Request.Cookies["WMS"] != null && Request.Cookies["WMS"].Values["name"] != null)
                    {
                        var account = Request.Cookies["WMS"].Values["name"];
                        txt_ID.Text = account;
                        ckbRememberMe.Checked = true;
                    }
                }
                else
                {
                    lbl_Message.Text = "";
                }
                //txt_ID.Focus();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region 主功能-登入

        /// <summary>
        /// 按下登入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Login_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session["Account"] == null)
                    Login();
                else
                {
                    if (Session["ClerkID"] != null)
                        Response.Redirect("~/pos_check_out.aspx");
                    else
                        Response.Redirect("~/index.aspx");
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// textbox內容改變
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txt_ID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //if (Session["Account"] == null)
                //    Login();
                //else
                //    Response.Redirect("~/index.aspx");
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }
        /// <summary>
        /// 登入
        /// </summary>
        protected void Login()
        {
            try
            {
                if (txt_ID.Text.Trim() == "guest")
                {
                    String ID = txt_ID.Text = txt_ID.Text.Trim().ToLower();
                    Session["Account"] = "Guest";
                    Session["Name"] = "Guest";
                    //取client ip
                    Session["ip"] = Request.Cookies["myip"].Value;
                    Cookie(ID);

                    //加入onlineList帳號(2013-1106新增)-----
                    AddOnlineList(ID);
                    Response.Redirect("~/index.aspx");
                }
                //帳密登入
                else if (txt_ID.Text.Trim() != "" && txt_PW.Text.Trim() != "")
                {
                    //0b389280842de54c
                    String ID = txt_ID.Text = txt_ID.Text.Trim().ToLower();
                    String PW = txt_PW.Text.Trim();
                    EntranceService.Result Lg = new Result();

                    ADVerifyService.ADVerifyClient ADVC = new ADVerifyService.ADVerifyClient();
                    ADVerifyService.Identity ADI = ADVC.Verify("OBDesign.com.tw", txt_ID.Text, txt_PW.Text);

                    try
                    {
                        Lg = EC.LogIn(ID, PW);

                    }
                    catch (Exception ex)
                    {
                        Response.Write("系統發生錯誤 " + ex.Message);
                    }

                    if (Lg.ResultStatus == ResultType.Success)
                    {
                        String account = Lg.Account;
                        Session["ID"] = ID;
                        Session["PW"] = PW;
                        Session["Account"] = account;
                        //取得中文名(如果AD有)
                        Session["Name"] = string.IsNullOrEmpty(Lg.Fullname) ? Lg.Account : Lg.Fullname;
                        Session["Zone"] = "1";
                        Session["ShipOutType"] = (Utility.Area)_areaId;
                        Session["EID"] = ADI.EmployeeID;

                        //取client ip
                        //正式cookieIP(2012-1205新增)
                        Session["ip"] = Request.Cookies["myip"].Value;
                        //正式
                        //Session["ip"] = HttpContext.Current.Request.UserHostAddress;
                        //測試
                        //Session["ip"] = Dns.Resolve(Dns.GetHostName()).AddressList[0].ToString();

                        //權限設定(2012-0110 舊權限確認方式)
                        //auth.check(account);

                        //(2012-0110 新權限確認方式)
                        auth.setAuthority(Lg.Authoritys);

                        //(2014-0509 新新權限確認方式)
                        auth.setAuthorityPro(ID, 1);
                        var test = auth.checkAuthorityPro("4");

                        Cookie(ID);

                        //加入onlineList帳號(2013-1106新增)-----
                        AddOnlineList(ID);

                        if (urlx != null && urlx != "")
                            Response.Redirect("~/" + urlx);
                        else
                            Response.Redirect("~/index.aspx");
                    }
                    else
                    {
                        lbl_Message.Text = "登入失敗";
                    }
                }
                //密碼為空則使用物流BarCode登入
                else if (txt_ID.Text.Trim() != "" && txt_PW.Text.Trim() == "")
                {
                    //List<EntranceService.StructLogin> Lg = new List<StructLogin>();
                    String ID = txt_ID.Text = txt_ID.Text.Trim();

                    #region ●service登入(舊)

                    //EntranceService.Result Lg = new Result();

                    //try
                    //{
                    //    Lg = EC.EmployeeCode(ID);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Response.Write("系統發生錯誤 " + ex.Message);
                    //}

                    //if (Lg.ResultStatus == ResultType.Success && Lg.Zone == "1")
                    //{
                    //    String account = Lg.Account;
                    //    String zone = Lg.Zone;
                    //    Session["Account"] = account;
                    //    //取得中文名(如果AD有)
                    //    Session["Name"] = string.IsNullOrEmpty(Lg.Fullname) ? Lg.Account : Lg.Fullname;
                    //    Session["logisticAccount"] = true;
                    //    Session["Zone"] = zone;
                    //    Session["ShipOutType"] = "橘熊";

                    //    //取client ip
                    //    //正式cookieIP(2012-1205新增)
                    //    Session["ip"] = Request.Cookies["myip"].Value;
                    //    //正式
                    //    //Session["ip"] = HttpContext.Current.Request.UserHostAddress;
                    //    //測試
                    //    //Session["ip"] = Dns.Resolve(Dns.GetHostName()).AddressList[0].ToString();

                    //    //權限設定(2012-0110 舊權限確認方式)
                    //    //auth.check(account);

                    //    //若有權限則設定(2012-0110 新權限確認方式)
                    //    if (Lg.Authoritys != null)
                    //        auth.setAuthority(Lg.Authoritys);

                    //    //(2014-0509 新新權限確認方式)
                    //    auth.setAuthorityPro(ID, 0);
                    //    var test = auth.checkAuthorityPro("4");

                    //    Cookie(ID);

                    //    //加入onlineList帳號(2013-1106新增)-----
                    //    AddOnlineList(ID);

                    //    if (urlx != null && urlx != "")
                    //        Response.Redirect("~/" + urlx);
                    //    else
                    //        Response.Redirect("~/index.aspx");
                    //}
                    //else
                    //{
                    //    lbl_Message.Text = "登入失敗";
                    //}

                    #endregion

                    #region ●門市client server DB登入
                    
                    POS_Library.ShopPos.LogisticsAccount LG = new POS_Library.ShopPos.LogisticsAccount();

                    var ResultStatus = LG.GetLogisticsDetailByBarcode(ID);

                    if (ResultStatus.Count == 1)
                    {
                        String account = ResultStatus[0].Account;
                        String zone = ResultStatus[0].Zone.ToString();
                        Session["Account"] = account;
                        Session["ClerkID"] = ResultStatus[0].Id;
                        //取得中文名(如果AD有)
                        Session["Name"] = account;
                        Session["logisticAccount"] = true;
                        Session["Zone"] = zone;
                        Session["ShipOutType"] = (Utility.Area)int.Parse(zone);

                        //取client ip
                        //正式cookieIP(2012-1205新增)
                        Session["ip"] = Request.Cookies["myip"].Value;
                        //正式
                        //Session["ip"] = HttpContext.Current.Request.UserHostAddress;
                        //測試
                        //Session["ip"] = Dns.Resolve(Dns.GetHostName()).AddressList[0].ToString();

                        //權限設定(2012-0110 舊權限確認方式)
                        //auth.check(account);

                        //若有權限則設定(2012-0110 新權限確認方式)
                        //if (Lg.Authoritys != null)
                        //    auth.setAuthority(Lg.Authoritys);

                        //(2014-0509 新新權限確認方式)
                        auth.setAuthorityPro(ID, 0);
                        var test = auth.checkAuthorityPro("4");

                        Cookie(ID);

                        //加入onlineList帳號(2013-1106新增)-----
                        AddOnlineList(ID);

                        //if (urlx != null && urlx != "")
                        //    Response.Redirect("~/" + urlx);
                        //else
                        //    Response.Redirect("~/index.aspx");

                        Response.Redirect("~/pos_check_out.aspx");
                    }
                    else
                    {
                        lbl_Message.Text = "登入失敗";
                    }

                    #endregion
                }
                else
                {
                    lbl_Message.Text = "帳號為空白";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        private void Cookie(string name)
        {
            if (ckbRememberMe.Checked)
            {
                var cookie = Request.Cookies["WMS"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("WMS");
                    cookie.Values.Add("name", name);
                    cookie.Expires = DateTime.Now.AddMonths(1);
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    Response.Cookies["WMS"].Values["name"] = name;
                }
            }
            else
            {
                var cookie = Response.Cookies["WMS"];
                if (cookie != null)
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                }
            }
        }

        #endregion

        #region ●顯示線上人數(2013-1106新增)

        protected void ShowOnlineList()
        {
            var temp = (List<Utility.OnlinePerson>)Application["onlineList"];

            lbl_Online.Text = temp.Count.ToString();
        }

        #endregion

        #region ●加入線上使用者

        protected void AddOnlineList(string ID)
        {
            //加入onlineList帳號(2013-1106新增)-----
            Application.Lock();
            onlineList = (List<Utility.OnlinePerson>)Application["onlineList"];
            Utility.OnlinePerson aa = new Utility.OnlinePerson();
            aa.Account = ID;
            aa.Name = Session["Name"].ToString();
            aa.IP = Session["ip"].ToString();
            aa.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            onlineList.Add(aa);
            Application["onlineList"] = onlineList;
            Application.UnLock();
            //--------------------------------------
        }

        #endregion

    }
}