using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace OBShopWeb
{
    public class CreateXLS
    {
        #region 主功能-產生XLS

        /// <summary>
        /// 產生XLS
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="ms"></param>
        /// <param name="filename"></param>
        /// <param name="colnameList"></param>
        /// <param name="gv"></param>
        public void DoCreateXLS(HSSFWorkbook workbook, MemoryStream ms, List<int> colList, GridView gv, bool onlyHeader, string 工作表名稱)
        {
            try
            {
                //參數皆正確才產生
                if (workbook != null && ms != null && colList != null && gv.Rows.Count > 0)
                {
                    // 新增試算表。
                    var sheet = workbook.CreateSheet((工作表名稱 != "") ? 工作表名稱 : "My Sheet");
                    //加Column
                    Row row;

                    #region ●產生標頭Header

                    row = sheet.CreateRow(0);
                    int xlscoli = 0;
                    // 插入欄位Header。
                    for (var i = 0; i < gv.HeaderRow.Cells.Count; i++)
                    {
                        if (colList.Count == 0 || colList.Contains(i))
                        {
                            row.CreateCell(xlscoli).SetCellValue(gv.HeaderRow.Cells[i].Text);
                            xlscoli++;
                        }
                    }

                    #endregion ●產生標頭Header

                    #region ●產生內容

                    // 插入資料值。 /非onlyHeader
                    if (!onlyHeader)
                    {
                        int xlsrowi = 1;

                        for (var j = 0; j < gv.Rows.Count; j++)
                        {
                            //欄位數要歸0
                            xlscoli = 0;

                            row = sheet.CreateRow(xlsrowi++);

                            for (var i = 0; i < gv.HeaderRow.Cells.Count; i++)
                            {
                                if (colList.Count == 0 || colList.Contains(i))
                                {
                                    row.CreateCell(xlscoli).SetCellValue((gv.Rows[j].Cells[i].Text == "&nbsp;") ? "" : gv.Rows[j].Cells[i].Text);
                                    xlscoli++;
                                }
                            }
                        }
                        workbook.Write(ms);
                    }

                    #endregion ●產生內容
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion 主功能-產生XLS
    }
}