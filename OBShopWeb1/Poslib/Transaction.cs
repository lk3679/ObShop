using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace OBShopWeb.Poslib
{
    public class Transaction
    {

        public static TransactionData get_pos_transaction_list(string vip_id, string mobile, string start_date, string end_date)
        {
            string sql = "";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("vip_id", vip_id);
            param.Add("mobile", mobile);
            param.Add("start_date", start_date);
            param.Add("end_date", end_date);

            if (vip_id != "")
                sql = "select * from XPPOS.[dbo].Transation where VipNo = @vip_id order by TrDate desc ";
            else if(mobile!="")
                sql = "select T.* from XPPOS.[dbo].Transation T inner join pos_vip on VipNo = vip_id  where mobile = @mobile order by TrDate desc";
            else
                sql = "select * from XPPOS.[dbo].Transation where TrDate >= @start_date and TrDate <= @end_date order by TrNo";

            TransactionData TD = new TransactionData();
            TD.transaction_list = DB.DBQuery(sql, param, "orangebear");
            foreach (DataRow row in TD.transaction_list.Rows)
            {
                TD.amount += int.Parse(row["TrAmount"].ToString());
                TD.cash +=row["TrCash"].ToString()==""?0: int.Parse(row["TrCash"].ToString());
                TD.credit += row["TrCreditCard"].ToString()==""?0:int.Parse(row["TrCreditCard"].ToString());
                TD.voucher += int.Parse(row["TrVoucher"].ToString());
            }

            return TD;
        }

        public static DataTable get_pos_transaction(string transaction_id)
        {
            string sql="select S.*, S.序號 id, I.ItemMark from XPPOS.[dbo].Transation T ";
            sql+="inner join XPPOS.[dbo].SaleItem S on T.TrNo = S.TrNo ";
            sql+="inner join XPPOS.[dbo].Item I on S.ItemNo = I.ItemNo ";
            sql+="where T.TrNo = @transaction_id ";

             Dictionary<string, object> param = new Dictionary<string, object>();
             param.Add("transaction_id", transaction_id);
            DataTable dt=DB.DBQuery(sql,param,"orangebear");
            return dt;
        }

        public static bool delete_pos_transaction(string transaction_id)
        {
            string sql = "delete from XPPOS.[dbo].SaleItem where TrNo =@transaction_id;  ";
            sql += "delete from XPPOS.[dbo].Transation where TrNo =@transaction_id; ";
            Dictionary<string, object> param = new Dictionary<string, object>();
            param.Add("transaction_id", transaction_id);
            return DB.DBNonQuery(sql, param, "orangebear");
        }
    }

    public class TransactionData
    {
        public int amount;
        public int cash;
        public int credit;
        public int voucher;
        public DataTable transaction_list;

    }
}