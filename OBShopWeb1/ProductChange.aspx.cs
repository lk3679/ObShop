using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using OBShopWeb.publics;
using POS_Library.Public;
using System.Web.Configuration;
using OBShopWeb.Poslib;

namespace OBShopWeb
{
    public partial class ProductChange : System.Web.UI.Page
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
                        var machines = POS_Library.Public.Utility.GetMachine(store).Where(x => x.Name == "銷售產品");
                        foreach (var item in machines)
                        {
                            DDL_destination_ticket.Items.Add(Utility.GetDropMachineName(item.Name, item.MachineID, item.MachineBTW));
                        }
                        Default();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void Default()
        {
            var pDa = new POS_Library.ShopPos.ProductChangeDA();
            //取得有異動產品價格
            var result = pDa.GetPriceProductChange();
            var productPrint = result.Where(x => x.Print).ToList();
            //更新不須異動異動
            var printSkus = productPrint.Select(x => x.ProductId).ToList();
            pDa.SetPriceProductChange(printSkus);
            if (productPrint.Any())
            {
                btnPrintShelf.Visible = true;
                int xx = 1;
                var BLList = productPrint.Select(x => new
                {
                    序號 = xx++,
                    儲位位置 = x.ShelfName,
                    產品編號 = x.ProductId,
                    原售價 = x.OriginalPrice,
                    異動售價 = x.NewPrice,
                    數量 = x.Quantity,
                    建立時間 = x.CreateDate
                }).ToList();
                gv_product_id.DataSource = BLList;
                gv_product_id.DataBind();

                for (var i = 0; i < BLList.Count; i++)
                {
                    GridViewRow row = gv_product_id.Rows[i];
                    row.Cells[0].Text = BLList[i].序號.ToString();
                    row.Cells[1].Text = BLList[i].儲位位置;
                    row.Cells[2].Text = BLList[i].產品編號;
                    row.Cells[3].Text = BLList[i].原售價.ToString();
                    row.Cells[4].Text = BLList[i].異動售價.ToString();
                    DropDownList ddl = row.Cells[5].FindControl("DDL_Quantity") as DropDownList;
                    ddl.SelectedValue = BLList[i].數量.ToString();
                    row.Cells[6].Text = BLList[i].建立時間.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            else
            {
                lbl_Message.Text = "無資料";
            }
        }

        protected void btnPrintShelf_Click(object sender, EventArgs e)
        {
            try
            {
                var destination = DDL_destination_ticket.SelectedValue;
                //解析DropDownList文字，取得machineId
                var machineId = Utility.GetMachineId(destination);

                var pDa = new POS_Library.ShopPos.ProductChangeDA();
                var result = pDa.GetPriceProductChange();
                var printProduct = result.Where(x => x.Print).ToList();
                var barcodeLabels = new List<BarcodePrint.BarcodeLabl>();
                var ckPrint = true;
                if (printProduct.Any())
                { 
                    foreach (var item in printProduct)
                    {
                        var barcodeLabel = new BarcodePrint.BarcodeLabl();
                        barcodeLabel.ProductId = item.ProductId;
                        barcodeLabel.Barcode = item.Barcode;
                        barcodeLabel.Quantity = item.Quantity.ToString();
                        barcodeLabel.Price = item.NewPrice.ToString();
                        barcodeLabels.Add(barcodeLabel);

                        var printer = new PosPrint();
                        var p = new BarcodePrint();
                        var resultPrint = p.PrintBarcode(printer, machineId, barcodeLabels);
                        if (resultPrint)
                        {
                            pDa.SetPriceProductChange(item);
                        }
                        else
                        {
                            ckPrint = false;
                        }
                    }
                    if (!ckPrint)
                    {
                        lbl_Message.Text = "有部分條碼列印失敗！請繼續列印！";
                    }
                    else
                    {
                        lbl_Message.Text = "列印完成！";
                    }
                }
                else
                {
                    lbl_Message.Text = "沒有需要列印之產品！";
                }
                Default();
            }
            catch (Exception ex)
            {
                lbl_Message.Text = "列印失敗！請繼續列印！" + ex.Message;
            }
        }
    }
}