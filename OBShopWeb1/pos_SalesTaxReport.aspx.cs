using OBShopWeb.Poslib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OBShopWeb
{
    public partial class pos_SalesTaxReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        private string _checkInvoiceCreation()
        {
            lblLog.Text = "";

            if (tbxStartDateIssuance.Text.Length == 0 || tbxEndDateIssuance.Text.Length == 0)
                return "[銷貨][錯誤訊息] 未輸入發票起迄日期！";

            if (DateTime.Parse(tbxStartDateIssuance.Text) > DateTime.Parse(tbxEndDateIssuance.Text))
                return "[銷貨][錯誤訊息] 發票起始日期大於發票結束日期！";

            if (tbxUpdStartDateIssuance.Text.Length> 0 && tbxUpdEndDateIssuance.Text.Length>0)
            {
                if(DateTime.Parse(tbxUpdStartDateIssuance.Text) > DateTime.Parse(tbxUpdEndDateIssuance.Text))
                    return "[銷貨][錯誤訊息] 異動起始日期大於異動結束日期！";
            }

            return "";
        }

        private string _checkAllowenceInvoiceCreation()
        {
            lblLog.Text = "";

            if (tbxStartDateAllowence.Text.Length == 0 || tbxEndDateAllowence.Text.Length == 0)
                return "[銷貨][錯誤訊息] 未輸入發票起迄日期！";

            if (DateTime.Parse(tbxStartDateAllowence.Text) > DateTime.Parse(tbxEndDateAllowence.Text))
                return "[銷貨][錯誤訊息] 發票起始日期大於發票結束日期！";

            if (tbxUpdStartDateAllowence.Text.Length > 0 && tbxUpdEndDateAllowence.Text.Length > 0)
            {
                if (DateTime.Parse(tbxUpdStartDateAllowence.Text) > DateTime.Parse(tbxUpdEndDateAllowence.Text))
                    return "[銷貨][錯誤訊息] 異動起始日期大於異動結束日期！";
            }

            return "";

        }

        protected void btnInvoiceTxtOutput_Click(object sender, EventArgs e)
        {
            string startdt = tbxStartDateIssuance.Text;
            string enddt = tbxEndDateIssuance.Text;
            string updStartdt = tbxUpdStartDateIssuance.Text;
            string updEnddt = tbxUpdEndDateIssuance.Text;
            string cancelType = ddlIssuanceCancelType.Text;

            lblLog.Text = _checkInvoiceCreation();
            if (lblLog.Text.Length > 0)
            {return;}

            SalesTax st = new SalesTax();
            st.ExportSalesInvoiceTxtFile(startdt, enddt, cancelType, updStartdt, updEnddt);
            
        }

        protected void btnAllowenceTxtOutput_Click(object sender, EventArgs e)
        {
            string startdt = tbxStartDateAllowence.Text;
            string enddt = tbxEndDateAllowence.Text;
            string updStartdt = tbxUpdStartDateAllowence.Text;
            string updEnddt = tbxUpdEndDateAllowence.Text;
            lblLog.Text = _checkAllowenceInvoiceCreation();
            if (lblLog.Text.Length > 0)
            { return; }
            SalesTax st = new SalesTax();
            st.ExportAllowenceInvoiceTxtFile(startdt, enddt, "cancelonly", updStartdt, updEnddt);
        }

        protected void btnInvoiceCreation_Click(object sender, EventArgs e)
        {
            string startdt = tbxStartDateIssuance.Text;
            string enddt = tbxEndDateIssuance.Text;
            string updStartdt = tbxUpdStartDateIssuance.Text;
            string updEnddt = tbxUpdEndDateIssuance.Text;
            string cancelType = ddlIssuanceCancelType.Text;
            lblLog.Text = _checkInvoiceCreation();
            if (lblLog.Text.Length > 0)
            { return; }

            SalesTax st = new SalesTax();
            st.ExportSalesInvoice(startdt, enddt, cancelType, updStartdt, updEnddt);
        }

        protected void btnAllowenceCreation_Click(object sender, EventArgs e)
        {
            string startdt = tbxStartDateAllowence.Text;
            string enddt = tbxEndDateAllowence.Text;
            string updStartdt = tbxUpdStartDateAllowence.Text;
            string updEnddt = tbxUpdEndDateAllowence.Text;
            lblLog.Text = _checkAllowenceInvoiceCreation();
            if (lblLog.Text.Length > 0)
            { return; }
            SalesTax st = new SalesTax();

            st.ExportAllowencesInvoice(startdt, enddt, "cancelonly", updStartdt, updEnddt);
        }

    }
}