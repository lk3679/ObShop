using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using POS_Library.ShopPos;
using POS_Library.ShopPos.DataModel;
using POS_Library.Public;
using OBShopWeb.Poslib;

namespace OBShopWeb.PDA
{
    public partial class CreateStorageNew : System.Web.UI.Page
    {
        #region 宣告

        //儲位所在地
        private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        String account;

        ShelfProcess sp = new ShelfProcess();
        CheckFormat CF = new CheckFormat();

        string oldStorageId = "";

        protected class StorageData
        {
            public string 儲位名稱 { get; set; }
            public string 儲位類別 { get; set; }
            public int 儲位容量 { get; set; }
            public string 撿貨群組 { get; set; }
        }

        #endregion

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

                    if (!IsPostBack)
                    {
                        SetDDL(DDL_D1);
                        SetDDL(DDL_D2);
                        SetDDL(DDL_D3);
                        SetDDL(DDL_D4);
                        this.txt_Number.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { return false; }");
                        this.txt_StartStorage.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { return false; }");
                        this.txt_Volume.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { return false; }");
                        this.txtGroupName.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { return false; }");
                    }
                    txtGroupName.Text = DDL_Group1.SelectedValue + DDL_Group2.SelectedValue;
                    lbl_ShlefId.Text = CF.TransShelfIdToLabel(DDL_Area.Text + DDL_D1.Text + DDL_D2.Text + DDL_D3.Text + DDL_D4.Text);
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
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
        }

        #endregion

        #region 主功能-新增儲位

        /// <summary>
        /// 執行新增儲位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Execution_Click(object sender, EventArgs e)
        {
            try
            {
                if (CF.CheckID(txt_Number.Text.Trim(), CheckFormat.FormatName.Storage))
                {
                    lbl_Message.Text = "儲位名稱錯誤！";
                }
                else if (!Regex.IsMatch(txt_Number.Text.Trim(), @"^\d"))
                {
                    lbl_Message.Text = "數量不為數字！";
                }
                else if (!Regex.IsMatch(txt_Volume.Text.Trim(), @"^\d"))
                {
                    lbl_Message.Text = "容量不為數字！";
                }
                else if (!Regex.IsMatch(txtGroupName.Text.Trim(), @"^[A-Z][1-9]$"))
                {
                    lbl_Message.Text = "群組不為A-Z加一碼數字！";
                }
                else
                {
                    List<StorageData> list = new List<StorageData>();
                    String temp = "";

                    FreeCreateNew(list);

                    gv_List.DataSource = list;
                    gv_List.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 隨意產生(2013-0805新增)(2013-0808增加統一格式)
        /// </summary>
        void FreeCreateNew(List<StorageData> list)
        {
            //規則產生
            if (CB_OneFormat.Checked)
            {
                var startId = CF.TransLabelToShelfId(lbl_ShlefId.Text).Substring(0, 5);
                int startnum1 = int.Parse(CF.TransLabelToShelfId(lbl_ShlefId.Text).Substring(5, 2));
                int startnum2 = int.Parse(CF.TransLabelToShelfId(lbl_ShlefId.Text).Substring(7, 2));

                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < int.Parse(txt_Number.Text.Trim()); i++)
                    {
                        StorageData data = new StorageData();

                        data.儲位容量 = int.Parse(txt_Volume.Text.Trim());
                        data.儲位類別 = ddl_Type.Text.Trim();
                        data.撿貨群組 = txtGroupName.Text.Trim();
                        data.儲位名稱 = CF.TransShelfIdToLabel(startId + (startnum1 + j).ToString("00") + (startnum2 + i).ToString("00"));
                        list.Add(data);
                    }
                }
            }
            //隨意產生
            else
            {
                var startId = CF.TransLabelToShelfId(lbl_ShlefId.Text).Substring(0, 7);
                int startnum = int.Parse(CF.TransLabelToShelfId(lbl_ShlefId.Text).Substring(7, 2));

                for (int i = 0; i < int.Parse(txt_Number.Text.Trim()); i++)
                {
                    StorageData data = new StorageData();

                    data.儲位容量 = int.Parse(txt_Volume.Text.Trim());
                    data.儲位類別 = ddl_Type.Text.Trim();
                    data.撿貨群組 = txtGroupName.Text.Trim();
                    data.儲位名稱 = CF.TransShelfIdToLabel(startId + (startnum + i).ToString("00"));
                    list.Add(data);
                }
            }
        }

        #endregion

        #region 主功能-新增儲位(舊)

        /// <summary>
        /// 產生17 or 34個儲位為同個貨架(2013-0123新增) 11~44~51__61~94~A1
        /// </summary>
        void SuperCreate(List<StorageData> list, int turn)
        {
            for (int i = 0; i < 17; i++)
            {
                StorageData data = new StorageData();

                if (i == 0 || (i + 1) % 17 != 0)
                {
                    if (string.IsNullOrEmpty(oldStorageId))
                    {
                        if (turn == 1)
                        {
                            data.儲位名稱 = txt_StartStorage.Text.Trim();
                        }
                        else
                        {
                            data.儲位名稱 = oldStorageId.Substring(0, 4) + "61";
                        }
                    }
                    else
                    {
                        data.儲位名稱 = GetNewStorageId(oldStorageId, 5);
                    }
                    oldStorageId = data.儲位名稱;
                }
                else if ((i + 1) == 17)
                {
                    if (turn == 1)
                    {
                        data.儲位名稱 = oldStorageId.Substring(0, 4) + "51";
                    }
                    else
                    {
                        data.儲位名稱 = oldStorageId.Substring(0, 4) + "A1";
                    }
                }

                data.儲位容量 = int.Parse(txt_Volume.Text.Trim());
                data.儲位類別 = ddl_Type.Text.Trim();
                data.撿貨群組 = txtGroupName.Text.Trim();

                list.Add(data);
            }
        }

        /// <summary>
        /// 隨意產生(2013-0130新增)
        /// </summary>
        void FreeCreate(List<StorageData> list)
        {
            for (int i = 0; i < int.Parse(txt_Number.Text.Trim()); i++)
            {
                StorageData data = new StorageData();

                if (string.IsNullOrEmpty(oldStorageId))
                {
                    data.儲位名稱 = txt_StartStorage.Text.Trim();
                }
                else
                {
                    data.儲位名稱 = GetNewStorageId(oldStorageId, 5);
                }

                data.儲位容量 = int.Parse(txt_Volume.Text.Trim());
                data.儲位類別 = ddl_Type.Text.Trim();
                data.撿貨群組 = txtGroupName.Text.Trim();
                oldStorageId = data.儲位名稱;
                list.Add(data);
            }
        }

        /// <summary>
        /// 產生新儲位ID
        /// </summary>
        /// <param name="storageId"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        protected string GetNewStorageId(string storageId, int num)
        {
            try
            {
                //無限制產生到A
                Char Aor4 = '4';
                if (CB_A.Checked == true)
                    Aor4 = 'A';

                string newStorageId = "";

                if (storageId[num] == Aor4)
                {
                    if (num == storageId.Length - 1)
                    {
                        storageId = storageId.Substring(0, num) + "1";
                    }
                    else
                    {
                        storageId = storageId.Substring(0, num) + "1" + storageId.Substring(num + 1, storageId.Length - (num + 1));
                    }

                    newStorageId = GetNewStorageId(storageId, --num);
                }
                else
                {
                    if (num == storageId.Length - 1)
                    {
                        newStorageId = storageId.Substring(0, num) + (int.Parse(storageId[num].ToString()) + 1 < 10 ? (int.Parse(storageId[num].ToString()) + 1).ToString() : "A");
                    }
                    else
                    {
                        newStorageId = storageId.Substring(0, num) + (int.Parse(storageId[num].ToString()) + 1 < 10 ? (int.Parse(storageId[num].ToString()) + 1).ToString() : "A") + storageId.Substring(num + 1, storageId.Length - (num + 1));
                    }
                }

                return newStorageId.Substring(0, 6);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region 主功能-儲存

        /// <summary>
        /// 儲存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                List<StorageDM> list = new List<StorageDM>();

                //判斷儲位名稱為空(2013-0410新增)-----------------------------------
                bool empty = false;

                foreach (GridViewRow row in gv_List.Rows)
                {
                    var txtName = row.Cells[0].FindControl("txt_StorageId") as TextBox;
                    empty = txtName.Text.Trim() == "" ? true : false;
                }
                //-----------------------------------------------------------------
                if (empty)
                {
                    lbl_Message.Text = "儲位名稱不可為空！";
                }
                else
                {
                    MsgStatus result;
                    bool resultAll = false;

                    List<string> errorList = new List<string>();

                    foreach (GridViewRow row in gv_List.Rows)
                    {
                        var txtName = CF.TransLabelToShelfId((row.Cells[0].FindControl("txt_StorageId") as TextBox).Text.Trim());
                        var ddlTemp = (row.Cells[1].FindControl("ddl_StorageType") as DropDownList).SelectedValue;
                        var txtVolume = (row.Cells[2].FindControl("txt_StorageVolume") as TextBox).Text.Trim();
                        var txtGroupName = (row.Cells[3].FindControl("txt_StorageGroupName") as TextBox).Text.Trim();
                        int floor = 1;
                        string Area = txtName.Substring(0,1);
                        if (Area =="Z")
                        {
                            lbl_Message.Text = "目前Z儲位不開放新增！";
                            return;
                        }
                        int 排 = int.Parse(txtName.Substring(1,2));
                        int 座 = int.Parse(txtName.Substring(3,2));
                        int 層 = int.Parse(txtName.Substring(5,2));
                        int 格 = int.Parse(txtName.Substring(7,2));
                        int 儲位類別 = int.Parse(ddlTemp);
                        int 材積 = int.Parse(txtVolume);
                        String 列印群組 = txtGroupName;

                        result = sp.SaveStorageList(txtName, floor, Area, 排, 座, 層, 格, 儲位類別, 材積, 列印群組, _areaId);

                        resultAll = (result.Result == "0") ? false : true;

                        if (!resultAll)
                            errorList.Add(txtName);
                    }


                    if (!resultAll)
                    {
                        lbl_Message.Text = String.Join(",", errorList) + " 建立失敗！";
                    }
                    else
                    {
                        lbl_Message.Text = "成功！";
                        gv_List.Controls.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion

        #region gv_List_RowDataBound

        /// <summary>
        /// gv_List_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gv_List_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                //若為DataRow
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var ddlTemp = e.Row.Cells[1].FindControl("ddl_StorageType") as DropDownList;
                    ddlTemp.SelectedIndex = ddl_Type.SelectedIndex;
                    var txtName = e.Row.Cells[0].FindControl("txt_StorageId") as TextBox;
                    txtName.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { return false; }");
                    var txtVolume = e.Row.Cells[2].FindControl("txt_StorageVolume") as TextBox;
                    txtVolume.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { return false; }");
                    var txtGroupName = e.Row.Cells[3].FindControl("txt_StorageGroupName") as TextBox;
                    txtGroupName.Attributes.Add("onkeypress", "if( event.keyCode == 13 ) { return false; }");
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