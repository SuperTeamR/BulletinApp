using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XlsIntegration
{
    public static class XlsParser
    {

        public static void ParseFromXls(Stream stream)
        {
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.FirstOrDefault();
                List<string> columnNames = new List<string>();
                foreach (var firstRowCell in sheet.Cells[sheet.Dimension.Start.Row, sheet.Dimension.Start.Column + 1, 1, sheet.Dimension.End.Column])
                    columnNames.Add(firstRowCell.Text);

                var endOfRows = false;
                while(!endOfRows)
                {
                    for(var i = 0; i < columnNames.Count; i++)
                    {


                    }
                }
            }
        }
    }
}
