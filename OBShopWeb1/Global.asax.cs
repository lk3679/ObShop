using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using POS_Library.Public;

namespace OBShopWeb
{
    public class Global : System.Web.HttpApplication
    {
        private int SessionTimeOut = int.Parse(WebConfigurationManager.AppSettings.Get("SessionTimeOut"));
        List<Utility.OnlinePerson> onlineList;

        void Application_Start(object sender, EventArgs e)
        {
            // 應用程式啟動時執行的程式碼
            //將 Application 全域變數初始化為 0
            Application["online"] = 0;

            //將 Application 全域變數初始化為空
            onlineList = new List<Utility.OnlinePerson>();
            Application["onlineList"] = onlineList;
        }

        void Application_End(object sender, EventArgs e)
        {
            //  應用程式關閉時執行的程式碼
            //應用程式結束時將在線人數歸 0
            Application["online"] = 0;
            //應用程式結束時將在線人歸0
            onlineList = new List<Utility.OnlinePerson>();
            Application["onlineList"] = onlineList;
        }

        void Application_Error(object sender, EventArgs e)
        {
            // 發生未處理錯誤時執行的程式碼

        }

        void Session_Start(object sender, EventArgs e)
        {
            // 啟動新工作階段時執行的程式碼

            //Session 存活時間，如需要 Login 則將此段註解
            Session.Timeout = SessionTimeOut;
        }

        void Session_End(object sender, EventArgs e)
        {
            // 工作階段結束時執行的程式碼。 
            // 注意: 只有在 Web.config 檔將 sessionstate 模式設定為 InProc 時，
            // 才會引發 Session_End 事件。如果將工作階段模式設定為 StateServer 
            // 或 SQLServer，就不會引發這個事件。

            if (Session["Name"] != null && Session["ip"] != null)
            {
                Application.Lock();
                //移除onlineList帳號(2013-1106新增)-----
                var onlineList = (List<Utility.OnlinePerson>)Application["onlineList"];
                if (onlineList.Count != 0 && onlineList.Where(x => x.Name == Session["Name"].ToString() && x.IP == Session["ip"].ToString()).Count() > 0)
                {
                    onlineList.Remove(onlineList.Where(x => x.Name == Session["Name"].ToString() && x.IP == Session["ip"].ToString()).FirstOrDefault());
                    Application["onlineList"] = onlineList;
                }
                Application.UnLock();
            }
        }
    }
}
