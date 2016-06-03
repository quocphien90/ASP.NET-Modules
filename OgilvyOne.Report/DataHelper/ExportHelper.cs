using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OgilvyOne.Report.DataHelper
{
    public class ExportHelper
    {
        private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName)
        {
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[1];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Tahoma"; //Default Font name for whole sheet

            return ws;
        }

        /// <summary>
        /// Sets the workbook properties and adds a default sheet.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        private static void SetWorkbookProperties(ExcelPackage p, string title)
        {
            //Here setting some document properties
            p.Workbook.Properties.Author = "Dsquare";
            p.Workbook.Properties.Title = title;


        }

        private static void CreateHeader(ExcelWorksheet ws, ref int rowIndex, ref int colIndex, DataTable dt)
        {
            colIndex = 1;
            foreach (DataColumn dc in dt.Columns) //Creating Headings
            {
                CreateHeader(ws, rowIndex, colIndex, dc.ColumnName);
                colIndex++;
            }
        }

        private static ExcelRange CreateHeader(ExcelWorksheet ws, int rowIndex, int colIndex, string Name)
        {
            var cell = ws.Cells[rowIndex, colIndex];

            //Setting the background color of header cells to Gray
            var fill = cell.Style.Fill;
            fill.PatternType = ExcelFillStyle.Solid;
            fill.BackgroundColor.SetColor(Color.FromArgb(200, 200, 200));

            //Setting Top/left,right/bottom borders.
            var border = cell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;

            //Setting Value in cell
            cell.Value = Name;
            return cell;
        }

        private static void CreateData(ExcelWorksheet ws, ref int rowIndex, DataTable dt)
        {
            int colIndex = 0;
            foreach (DataRow dr in dt.Rows) // Adding Data into rows
            {
                colIndex = 1;
                rowIndex++;

                foreach (DataColumn dc in dt.Columns)
                {
                    var cell = ws.Cells[rowIndex, colIndex];

                    //Setting Value in cell
                    cell.Value = dr[dc.ColumnName].ToString();

                    //Setting borders of cell
                    var border = cell.Style.Border;
                    border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                    colIndex++;
                }
            }
        }

        /// <summary>
        /// Adds the custom shape.
        /// </summary>
        /// <param name="ws">Worksheet</param>
        /// <param name="colIndex">Column Index</param>
        /// <param name="rowIndex">Row Index</param>
        /// <param name="shapeStyle">Shape style</param>
        /// <param name="text">Text for the shape</param>
        private static void AddCustomShape(ExcelWorksheet ws, int colIndex, int rowIndex, eShapeStyle shapeStyle, string text)
        {
            ExcelShape shape = ws.Drawings.AddShape("cs" + rowIndex.ToString() + colIndex.ToString(), shapeStyle);
            shape.From.Column = colIndex;
            shape.From.Row = rowIndex;
            shape.From.ColumnOff = Pixel2MTU(5);
            shape.SetSize(100, 100);
            shape.RichText.Add(text);
        }

        /// <summary>
        /// Adds the image in excel sheet.
        /// </summary>
        /// <param name="ws">Worksheet</param>
        /// <param name="colIndex">Column Index</param>
        /// <param name="rowIndex">Row Index</param>
        /// <param name="filePath">The file path</param>
        private static void AddImage(ExcelWorksheet ws, int columnIndex, int rowIndex, string filePath)
        {
            //How to Add a Image using EP Plus
            Bitmap image = new Bitmap(filePath);
            ExcelPicture picture = null;
            if (image != null)
            {
                picture = ws.Drawings.AddPicture("pic" + rowIndex.ToString() + columnIndex.ToString(), image);
                picture.From.Column = columnIndex;
                picture.From.Row = rowIndex;
                picture.From.ColumnOff = Pixel2MTU(2); //Two pixel space for better alignment
                picture.From.RowOff = Pixel2MTU(2);//Two pixel space for better alignment
                picture.SetSize(100, 100);
            }
        }

        /// <summary>
        /// Adds the comment in excel sheet.
        /// </summary>
        /// <param name="ws">Worksheet</param>
        /// <param name="colIndex">Column Index</param>
        /// <param name="rowIndex">Row Index</param>
        /// <param name="comments">Comment text</param>
        /// <param name="author">Author Name</param>
        private static void AddComment(ExcelWorksheet ws, int colIndex, int rowIndex, string comment, string author)
        {
            //Adding a comment to a Cell
            var commentCell = ws.Cells[rowIndex, colIndex];
            commentCell.AddComment(comment, author);
        }

        /// <summary>
        /// Pixel2s the MTU.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <returns></returns>
        public static int Pixel2MTU(int pixels)
        {
            int mtus = pixels * 9525;
            return mtus;
        }

        private static string GetDisplay(PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), true);
            if (attr.Length > 0)
            {
                return ((System.ComponentModel.DataAnnotations.DisplayAttribute)attr[0]).GetName();
            }
            return propertyInfo.Name;
        }
        public static void Export<T>(System.IO.Stream outputStream, IEnumerable<T> data)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                SetWorkbookProperties(p, "Export");
                //Create a sheet
                ExcelWorksheet ws = CreateSheet(p, "Export Sheet");

                int rowIndex = 1;
                var listPros = typeof(T).GetProperties();
                int colIndex = 1;
                foreach (var pro in listPros)
                {
                    CreateHeader(ws, rowIndex, colIndex, GetDisplay(pro));
                    colIndex++;
                }
                var endCol = colIndex;
                rowIndex++;
                colIndex = 1;
                var range = ws.Cells[rowIndex, colIndex];
                range.LoadFromCollection(data);
                //foreach (var item in data)
                //{
                //	colIndex = 1;
                //	rowIndex++;
                //	foreach (var pro in listPros)
                //	{
                //		var cell = ws.Cells[rowIndex, colIndex];
                //		//Setting Value in cell
                //		cell.Value = pro.GetValue(item, null);

                //		colIndex++;
                //	}
                //}
                ws.Cells[1, 1, data.Count() + 1, colIndex].AutoFitColumns();
                colIndex = 1;
                foreach (var pro in listPros)
                {
                    var cell = ws.Cells[2, colIndex, data.Count() + 1, colIndex];
                    if (pro.PropertyType == typeof(DateTime))
                    {
                        cell.Style.Numberformat.Format = "dd/mm/yyyy";
                    }
                    ws.Cells[1, colIndex, data.Count() + 1, colIndex].AutoFitColumns();
                    colIndex++;
                }
                p.SaveAs(outputStream);
            }
        }
        public static void Export(System.IO.Stream outputStream, DataTable data)
        {
            using (ExcelPackage p = new ExcelPackage())
            {
                //set the workbook properties and add a default sheet in it
                SetWorkbookProperties(p, "Export");
                //Create a sheet
                ExcelWorksheet ws = CreateSheet(p, "Export Data");

                int rowIndex = 1;
                int colIndex = 1;

                CreateHeader(ws, ref rowIndex, ref colIndex, data);
                rowIndex++;
                colIndex = 1;

                var range = ws.Cells[rowIndex, colIndex];
                range.LoadFromDataTable(data, false);

                //CreateData(ws, ref rowIndex, dt);

                colIndex = 1;
                for (var i = 0; i < data.Columns.Count; i++)
                {
                    var col = data.Columns[i];
                    var cell = ws.Cells[2, colIndex, data.Rows.Count + 1, colIndex];
                    if (col.DataType == typeof(DateTime))
                    {
                        cell.Style.Numberformat.Format = "dd/mm/yyyy";
                    }
                    ws.Cells[1, colIndex, data.Rows.Count + 1, colIndex].AutoFitColumns();
                    colIndex++;
                }

                p.SaveAs(outputStream);
            }
        }
    }

}