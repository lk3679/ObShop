using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using OBShopWeb.publics;
using POS_Library.ShopPos;
using OBShopWeb.Poslib;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public partial class PrintStorageNew : System.Web.UI.Page
    {
        #region 宣告

        //儲位所在地
        static private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        int store = POS_Library.Public.Utility.GetStore(_areaId);

        private String account;

        private ShelfProcess sp = new ShelfProcess();
        private CheckFormat CF = new CheckFormat();

        #endregion 宣告

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

                if (!IsPostBack)
                {
                    radioStore_SelectedIndexChanged(sender, e);
                    SetDDL(ddl_Shelf1);
                    SetDDL(ddl_Shelf2);
                    SetDDL(ddl_Shelf3);
                    ddl_Area.DataSource = sp.GetAreaAll(_areaId);
                    ddl_Area.DataBind();
                    ddl_Area_SelectedIndexChanged(ddl_Area, null);
                }
            }
        }

        /// <summary>
        /// 串DDL
        /// </summary>
        /// <param name="DDL"></param>
        protected void SetDDL(DropDownList DDL)
        {
            for (int i = 1; i < 100; i++)
            {
                ListItem a = new ListItem();
                a.Text = a.Value = i.ToString("00");
                DDL.Items.Add(a);
            }
            DDL.SelectedIndex = 1;
        }

        #endregion Page_Load

        #region 主功能-查詢儲位列表

        /// <summary>
        /// 更變樓層
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_Floor_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ddl_Area.DataSource = sp.GetAreaAll(_areaId);
                ddl_Area.DataBind();
                ddl_Area_SelectedIndexChanged(ddl_Area, null);
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 更變區域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddl_Area_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 過濾查詢
        /// </summary>
        protected void Search()
        {
            var list = POS_Library.ShopPos.Storage.GetStoragesArea(ddl_Area.Text, _areaId).ToList();
            var shelf1 = ddl_Shelf1.SelectedValue;
            var shelf2 = ddl_Shelf2.SelectedValue;
            var shelf3 = ddl_Shelf3.SelectedValue;

            gv_List.DataSource = (from x in list
                                  where ((shelf1 == "-1") ? true : x.ShelfId.Substring(1, 2) == int.Parse(shelf1).ToString("00")) &&
                                        ((shelf2 == "-1") ? true : x.ShelfId.Substring(3, 2) == int.Parse(shelf2).ToString("00")) &&
                                        ((shelf3 == "-1") ? true : x.ShelfId.Substring(5, 2) == int.Parse(shelf3).ToString("00"))
                                  orderby x.ShelfId
                                  select new
                                  {
                                      儲位名稱 = CF.TransShelfIdToLabel(x.ShelfId),
                                      儲位類別 = GetStorageType(x.StorageTypeId),
                                      儲位容量 = x.Volume,
                                      撿貨群組 = x.Group,
                                      儲位類別ID = x.StorageTypeId,
                                  }).OrderBy(x => x.儲位名稱).ToList();

            gv_List.DataBind();
        }

        #endregion 主功能-查詢儲位列表

        #region 主功能-執行列印

        /// <summary>
        /// 列印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Print_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> shelfBarcode = new List<string>();
                List<POS_Library.DB.Storage> BCList = new List<POS_Library.DB.Storage>();

                foreach (GridViewRow row in gv_List.Rows)
                {
                    CheckBox cbTest = row.Cells[0].FindControl("CB_Select") as CheckBox;

                    if (cbTest.Checked)
                    {
                        var shelfid = CF.TransLabelToShelfId(row.Cells[1].Text);
                        POS_Library.DB.Storage item = new POS_Library.DB.Storage();
                        item.ShelfId = shelfid;
                        item.Area = shelfid.Substring(0, 1);
                        item.Row = int.Parse(shelfid.Substring(1, 2));
                        item.Base = int.Parse(shelfid.Substring(3, 2));
                        item.Layer = int.Parse(shelfid.Substring(5, 2));
                        item.Grid = int.Parse(shelfid.Substring(7, 2));
                        item.StorageTypeId = int.Parse(row.Cells[5].Text);
                        BCList.Add(item);
                    }
                }
                bool result = false;
                var p = new POS_Library.Public.BarcodePrint();
                var radio = int.Parse(radioStore.SelectedValue);
                var destination = dropMachine.SelectedValue;
                
                //解析DropDownList文字，取得machineId
                var machineId = POS_Library.Public.Utility.GetMachineId(destination);
                if (radio == store)
                {
                    PosPrint posPrint = new PosPrint();
                    result = p.PrintShelfBarcode(posPrint, BCList, machineId, false, _areaId);
                }
                else
                {
                    Printer printer = new Printer();
                    result = p.PrintShelfBarcode(printer, BCList, machineId, false, _areaId);
                }

                if (result)
                {
                    Response.Write("列印成功！");
                }
                else
                {
                    Response.Write("列印失敗！");
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 主功能-執行列印

        #region 副功能-TypeToName

        protected string GetStorageType(int value)
        {
            try
            {
                string name = "";

                switch (value)
                {
                    case 0: name = "普通"; break;
                    case 1: name = "散貨"; break;
                    case 2: name = "補貨"; break;
                    case 3: name = "過季"; break;
                    case 4: name = "問題"; break;
                    case 5: name = "不良"; break;
                    case 6: name = "標準暫存"; break;
                    case 7: name = "不良暫存"; break;
                    case 8: name = "問題暫存"; break;
                    case 9: name = "打銷"; break;
                    case 10: name = "無貨"; break;
                    case 11: name = "出貨暫存"; break;
                    case 14: name = "海運暫存"; break;
                    case 15: name = "換貨暫存"; break;
                    case 16: name = "散貨暫存"; break;
                    case 17: name = "調回暫存"; break;
                    case 18: name = "預購暫存"; break;
                    case 19: name = "調出暫存"; break;
                    case 20: name = "展售"; break;
                }

                return name;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion 副功能-TypeToName

        #region 篩選

        /// <summary>
        /// 篩選
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Filter_Click(object sender, EventArgs e)
        {
            Search();
        }

        #endregion 篩選

        protected void radioStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            var radio = radioStore.SelectedValue;
            var machines = POS_Library.Public.Utility.GetMachine(int.Parse(radio), "新儲位");
             dropMachine.Items.Clear();
            foreach (var item in machines)
            {
                dropMachine.Items.Add(POS_Library.Public.Utility.GetDropMachineName(item.Name, item.MachineID, item.MachineBTW));
            }
        }
    }
}