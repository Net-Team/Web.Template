using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Core.Xls
{
    /// <summary>
    /// 表示xls文档
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class XlsDoc<TModel> : IEnumerable<XlsSheet<TModel>> where TModel : class, new()
    {
        /// <summary>
        /// 表格
        /// </summary>
        private readonly List<XlsSheet<TModel>> sheets;

        /// <summary>
        /// 获取文件名
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// 获取表格数量
        /// </summary>
        public int SheetsCount => this.sheets.Count;

        /// <summary>
        /// 通过索引获取表格
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public XlsSheet<TModel> this[int index] => this.sheets[index];

        /// <summary>
        /// xls文档
        /// </summary>
        /// <param name="xls">xls文件路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public XlsDoc(string xls)
        {
            using var dataset = XlsReader.ReadAsDataSet(xls);
            this.sheets = this.Parse(dataset).ToList();
        }

        /// <summary>
        /// 解析文档
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private IEnumerable<XlsSheet<TModel>> Parse(DataSet dataSet)
        {
            for (var i = 0; i < dataSet.Tables.Count; i++)
            {
                var table = dataSet.Tables[i];
                yield return new XlsSheet<TModel>(table);
            }
        }

        /// <summary>
        /// 返回迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<XlsSheet<TModel>> GetEnumerator()
        {
            return this.sheets.GetEnumerator();
        }

        /// <summary>
        /// 返回迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// xls读取器
        /// </summary>
        private static class XlsReader
        {
            /// <summary>
            /// 读取xls为dataSet
            /// </summary>
            /// <param name="xls"></param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="FileNotFoundException"></exception>
            /// <returns></returns>
            public static DataSet ReadAsDataSet(string xls)
            {
                if (xls.IsNullOrEmpty())
                {
                    throw new ArgumentNullException(nameof(xls));
                }
                if (File.Exists(xls) == false)
                {
                    throw new FileNotFoundException(xls);
                }

                var sheets = ReadAsSheets(xls);
                var dataSet = new DataSet();
                foreach (var sh in sheets)
                {
                    var table = ReadAsTable(sh);
                    dataSet.Tables.Add(table);
                }
                return dataSet;
            }

            /// <summary>
            /// 读取xls为ISheet
            /// </summary>
            /// <param name="xls"></param>
            /// <returns></returns>
            private static ISheet[] ReadAsSheets(string xls)
            {
                var sheets = new List<ISheet>();
                var isXls = Path.GetExtension(xls).EqualsIgnoreCase(".xls");
                using (var fs = new FileStream(xls, FileMode.Open, FileAccess.Read))
                {
                    var wb = isXls ? (IWorkbook)new HSSFWorkbook(fs) : new XSSFWorkbook(fs);
                    for (var i = 0; i < wb.NumberOfSheets; i++)
                    {
                        sheets.Add(wb.GetSheetAt(i));
                    }
                }
                return sheets.ToArray();
            }

            /// <summary>
            /// 读取sheet为Table
            /// </summary>
            /// <param name="sheet"></param>
            /// <returns></returns>
            private static DataTable ReadAsTable(ISheet sheet)
            {
                var table = new DataTable();
                table.TableName = sheet.SheetName;
                table.Rows.Clear();
                table.Columns.Clear();

                var headerRow = sheet.GetRow(0);
                if (headerRow == null)
                {
                    return table;
                }

                int colCount = headerRow.LastCellNum;
                for (var c = 0; c < colCount; c++)
                {
                    table.Columns.Add(headerRow.GetCell(c).ToString());
                }

                var i = 1;
                var currentRow = sheet.GetRow(i);
                while (currentRow != null)
                {
                    var dr = table.NewRow();
                    for (var j = 0; j < currentRow.Cells.Count; j++)
                    {
                        var cell = currentRow.GetCell(j);
                        if (cell != null)
                        {
                            switch (cell.CellType)
                            {
                                case CellType.Numeric:
                                    dr[j] = DateUtil.IsCellDateFormatted(cell)
                                        ? cell.DateCellValue.ToString(CultureInfo.InvariantCulture)
                                        : cell.NumericCellValue.ToString(CultureInfo.InvariantCulture);
                                    break;

                                case CellType.Blank:
                                    dr[j] = null;
                                    break;

                                case CellType.Boolean:
                                    dr[j] = cell.BooleanCellValue;
                                    break;

                                default:
                                    dr[j] = cell.StringCellValue;
                                    break;
                            }
                        }
                    }
                    table.Rows.Add(dr);
                    i++;
                    currentRow = sheet.GetRow(i);
                }
                return table;
            }

        }
    }
}
