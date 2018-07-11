using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using OBShopWeb.PDA;
using OBShopWeb.publics;
using POS_Library.Public;
using POS_Library.ShopPos;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class PrintBarCode : System.Web.UI.Page
    {
        private string account;

        //儲位所在地
        static private int _areaId = int.Parse(Area.WmsAreaXml("Area"));
        int store = POS_Library.Public.Utility.GetStore(_areaId);

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["Account"] == null)
                {
                    Response.Write(" <script> parent.document.location= 'logout.aspx' </script> ");
                    Response.End();
                }
                else
                {
                    account = Session["Account"].ToString();
                    if (!IsPostBack)
                    {
                        //加入DropDownList
                        var machines = POS_Library.Public.Utility.GetMachine(store);
                        foreach (var item in machines)
                        {
                            DDL_destination_ticket.Items.Add(Utility.GetDropMachineName(item.Name, item.MachineID, item.MachineBTW));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private enum EnumType
        {
            SearchProduct,
            SearchTicket,
        }

        protected void radioBtnType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbl_Message.Text = "";
            int type = int.Parse(radioBtnType.SelectedValue);
            switch (type)
            {
                case (int)EnumType.SearchProduct:
                    panelProduct.Visible = true;
                    panelTicket.Visible = false;
                    txt_productId.Text = "";
                    DDL_destination_ticket.Items.Clear();
                    lsbList.Items.Clear();
                    var product = Utility.GetMachine(store);
                    foreach (var item in product)
                    {
                        DDL_destination_ticket.Items.Add(Utility.GetDropMachineName(item.Name, item.MachineID, item.MachineBTW));
                    }
                    break;

                case (int)EnumType.SearchTicket:
                    panelProduct.Visible = false;
                    panelTicket.Visible = true;

                    DDL_destination_ticket.Items.Clear();
                    var product2 = Utility.GetMachine(store).Where(x => x.Name == "銷售產品" || x.Name == "產品").ToList();
                    foreach (var item in product2)
                    {
                        DDL_destination_ticket.Items.Add(Utility.GetDropMachineName(item.Name, item.MachineID, item.MachineBTW));
                    }
                    break;
            }
        }

        #region 產品

        protected void txt_productId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                var sp = new ShelfProcess();
                var str_input = txt_productId.Text.Trim().ToUpper();
                var quantity = int.Parse(DDL_Quantity.SelectedValue);
                var productId = string.Empty;
                var productBarcode = string.Empty;
                CheckFormat CF = new CheckFormat();
                //檢查輸入是否為條碼
                if (CF.CheckID(str_input, CheckFormat.FormatName.Product))
                {
                    //檢查是否為有效條碼
                    string chProduct = sp.GetProductNum(txt_productId.Text);
                    if (string.IsNullOrEmpty(chProduct))
                    {
                        lbl_Message.Text = "掃入條碼有誤！";
                        return;
                    }
                    else
                    {
                        productId = chProduct;
                        string chBarcode = sp.GetProductPriceBarcode(chProduct);
                        productBarcode = chBarcode;
                    }
                }
                else
                {
                    //檢查是否為有效產品
                    string chBarcode = sp.GetProductPriceBarcode(str_input);
                    if (string.IsNullOrEmpty(chBarcode))
                    {
                        lbl_Message.Text = "掃入條碼有誤！";
                        return;
                    }
                    else
                    {
                        productId = str_input;
                        productBarcode = chBarcode;
                    }
                }
                for (int i = 0; i < quantity; i++)
                {
                    ListItem product = new ListItem();
                    product.Text = productId;
                    product.Value = productBarcode;
                    lsbList.Items.Add(product);
                }

                txt_productId.Text = "";
                lbl_Message.Text = "";
                btnPrintProduct.Visible = true;

                txt_productId.Focus();
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        /// <summary>
        /// 列印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPrintProduct_Click(object sender, EventArgs e)
        {
            try
            {
                var destination = DDL_destination_ticket.SelectedValue; 
                    //解析DropDownList文字，取得machineId
                var machineId = Utility.GetMachineId(destination);
                var bars = new List<BarcodePrint.BarcodeLabl>();
                if (lsbList.Items.Count > 0)
                { 
                    for (int i = 0; i < lsbList.Items.Count; i++)
                    {
                        var bar = new BarcodePrint.BarcodeLabl();
                        bar.ProductId = lsbList.Items[i].Text;
                        bar.Barcode = lsbList.Items[i].Value.Split('|')[0];
                        bar.Price = lsbList.Items[i].Value.Split('|')[1];
                        bars.Add(bar);
                    }
                    var barcodeLabels = bars.GroupBy(x => new { x.ProductId, x.Barcode, x.Price }).Select(x => new BarcodePrint.BarcodeLabl
                    {
                        ProductId = x.Key.ProductId,
                        Barcode = x.Key.Barcode,
                        Quantity = x.Count().ToString(),
                        Price = machineId=="P_SaleProduct"?x.Key.Price:""
                    }).ToList();
                    if (!barcodeLabels.Any())
                    {
                        lbl_Message.Text = "系統無此條碼！";
                        return;
                    }

                    var printer = new PosPrint();
                    var p = new BarcodePrint();
                    var result = p.PrintBarcode(printer, machineId, barcodeLabels);
                    string showMessage = string.Empty;
                    if (result)
                    {
                        showMessage = "<script>alert('列印成功!');</script>";
                    }
                    else
                    {
                        showMessage = "<script>alert('列印失敗!');</script>";
                    }
                    txt_productId.Text = "";
                    lsbList.Items.Clear();
                    Page.RegisterClientScriptBlock("checkinput", @showMessage);
                }
                else
                {
                    lbl_Message.Text = "沒有輸入任何要印的產品！";
                }
            }
            catch (Exception ex)
            {
                Response.Write("系統發生錯誤 " + ex.Message);
            }
        }

        #endregion 產品

        #region 輸入補貨/展售儲位

        protected void txt_Shelf_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lbl_Message.Text = "";
                var sp = new ShelfProcess();
                var input = txt_Shelf.Text.Trim();
                int? shelfType = sp.CheckStorage(input, _areaId);
                if (shelfType != (int)Utility.StorageType.補貨儲位 && shelfType != (int)Utility.StorageType.展售儲位)
                {
                    lbl_Message.Text = "此儲位只能為補貨/展售儲位！";
                    gv_product_id.Visible = false;
                    btnPrintShelf.Visible = false;
                }
                else
                {
                    var shipDa = new POS_Library.ShopPos.ShipInDA();
                    int xx = 1;
                    var BLList = shipDa.GetShelfDetails(input).Select(x => new
                    {
                        序號 = xx++,
                        產品編號 = x.ProductId,
                        產品條碼 = x.Barcode,
                        數量 = x.Quantity,
                        價格 = x.Price
                    }).ToList();
                    if (BLList.Any())
                    {
                        gv_product_id.DataSource = BLList;
                        gv_product_id.DataBind();

                        for (var i = 0; i < BLList.Count; i++)
                        {
                            GridViewRow row = gv_product_id.Rows[i];
                            row.Cells[0].Text = BLList[i].序號.ToString();
                            row.Cells[1].Text = BLList[i].產品編號;
                            row.Cells[2].Text = BLList[i].產品條碼;
                            DropDownList ddl = row.Cells[3].FindControl("DDL_Quantity") as DropDownList;
                            ddl.SelectedValue = BLList[i].數量;
                            row.Cells[4].Text = BLList[i].價格;
                        }
                        gv_product_id.Visible = true;
                        btnPrintShelf.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                lbl_Message.Text = "請輸入正確的資訊！";
                gv_product_id.Visible = false;
                btnPrintShelf.Visible = false;
            }
        }

        /// <summary>
        /// 列印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPrintShelf_Click(object sender, EventArgs e)
        {
            var destination = DDL_destination_ticket.SelectedValue;
            //取得machineId
            var machineId = Utility.GetMachineId(destination);
            var barcodeLabels = new List<BarcodePrint.BarcodeLabl>();
            //取checkbox有勾的
            foreach (GridViewRow row in gv_product_id.Rows)
            {
                var BL = new BarcodePrint.BarcodeLabl();
                BL.Id = row.Cells[0].Text;
                BL.ProductId = row.Cells[1].Text;
                BL.Barcode = row.Cells[2].Text;
                DropDownList ddl = row.Cells[3].FindControl("DDL_Quantity") as DropDownList;
                BL.Quantity = ddl.SelectedValue;
                BL.Price = machineId == "P_SaleProduct" ? row.Cells[4].Text : "";
                //放入List
                barcodeLabels.Add(BL);
            }

            string showMessage = string.Empty;
            if (barcodeLabels.Count > 0)
            {
                var printer = new PosPrint();
                var p = new BarcodePrint();
                var result = p.PrintBarcode(printer, machineId, barcodeLabels);
                if (result)
                {
                    showMessage = "<script>alert('列印成功!');</script>";
                }
                else
                {
                    showMessage = "<script>alert('列印失敗!');</script>";
                }
            }
            else
            {
                showMessage = "<script>alert('沒有選擇資料!');</script>";
            }

            Page.RegisterClientScriptBlock("checkinput", @showMessage);
        }

        #endregion 輸入補貨/展售儲位
    }
}