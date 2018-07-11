using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using POS_Library.Public;
using POS_Library.ShopPos.DataModel;
using POS_Library.ShopPos;
using OBShopWeb.PDA;
using System.Web.Configuration;
using System.Threading;
using OBShopWeb.Poslib;
using System.Configuration;

namespace OBShopWeb
{
    static public class GvLightBar
    {
        static private string _changeColor = Area.WmsAreaXml("changeColor");
        static private string _changeColorOld = Area.WmsAreaXml("changeColor");
        static private string _backgroundColor = Area.WmsAreaXml("backgroundColor");

        /// <summary>
        /// 光棒效果(原光棒js跟updatepanel衝到所以要用後端)
        /// </summary>
        /// <param name="e"></param>
        static public void lightbar(GridViewRowEventArgs e, int gvcolortype)
        {
            switch (gvcolortype)
            {
                case 1: _changeColor = _changeColorOld; _backgroundColor = "#FFFBD6"; break;
                case 2: _changeColor = _changeColorOld; _backgroundColor = "#F7F6F3"; break;
                case 3: _changeColor = "#FF9933"; _backgroundColor = "#E3EAEB"; break;
            }

            //判定row的型態是資料行
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //滑鼠移到變色
                e.Row.Attributes.Add("onmouseover", string.Format("this.style.backgroundColor='{0}';",
                    _changeColor));
                //判定row的型態是替代行
                if (e.Row.RowState == DataControlRowState.Alternate)
                    //滑鼠移開底色恢復為白色
                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='White';");
                //滑鼠移開底色恢復為設定好的底色
                else
                    e.Row.Attributes.Add("onmouseout", string.Format("this.style.backgroundColor='{0}';",
                        _backgroundColor));

            }
        }
    }
}