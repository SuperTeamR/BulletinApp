using BulletinBridge.Data;
using FessooFramework.Tools.DCT;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XlsIntegration
{
    public static class XlsParser
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Парсит xls из стрима </summary>
        ///
        /// <remarks>   SV Milovanov, 13.02.2018. </remarks>
        ///
        /// <param name="stream">   The stream. </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process parse from XLS in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public static IEnumerable<XlsPackage> ParseFromXls(Stream stream)
        {
            var result = Enumerable.Empty<XlsPackage>().ToList();
            DCT.Execute(d =>
            {
                using (var package = new ExcelPackage(stream))
                {
                    var sheet = package.Workbook.Worksheets.FirstOrDefault();
                    List<string> columnNames = new List<string>();
                    foreach (var firstRowCell in sheet.Cells[sheet.Dimension.Start.Row, sheet.Dimension.Start.Column + 1, 1, sheet.Dimension.End.Column])
                        columnNames.Add(firstRowCell.Text);

                    var bulletinCount = 0;
                    var endOfRows = false;
                    while (!endOfRows)
                    {
                        var dictionary = new Dictionary<string, string>();
                        var hasAnyValue = false;
                        var urlCell = sheet.Cells[bulletinCount + 2, 1];
                        var url = urlCell.Value as string;
                        for (var i = 0; i < columnNames.Count; i++)
                        {
                            var header = sheet.Cells[1, i + 2];
                            var cell = sheet.Cells[bulletinCount + 2, i + 2];
                            var key = header.Value as string;
                            var v = cell.Value != null ? cell.Value.ToString() : string.Empty;
                            if (!string.IsNullOrEmpty(v))
                                hasAnyValue = true;
                            dictionary.Add(key, v);
                        }
                        if (hasAnyValue)
                        {
                            result.Add(new XlsPackage
                            {
                                Url = url,
                                Fields = dictionary,
                            });
                            bulletinCount++;
                        }
                        else
                        {
                            endOfRows = true;
                        }
                    }
                }
            });
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Создать xls с выбранными заголовками </summary>
        ///
        /// <remarks>   SV Milovanov, 13.02.2018. </remarks>
        ///
        /// <param name="headers">  The headers. </param>
        ///
        /// <returns>   A new array of byte. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static byte[] CreateXls(IEnumerable<string> headers)
        {
            var result = Enumerable.Empty<byte>().ToArray();
            DCT.Execute(d =>
            {
                var array = headers.ToArray();
                using (var xls = new ExcelPackage())
                {
                    var worksheet = xls.Workbook.Worksheets.Add("Мои объявления");
                    for (var i = 0; i < array.Length; i++)
                    {
                        var cell = worksheet.Cells[1, i + 1];
                        cell.Style.Font.Size = 14;
                        cell.Value = array[i];
                        cell.AutoFitColumns();
                    }
                    result = ReadFully(xls.Stream);
                }
            });
            return result;
        }


        static byte[] ReadFully(Stream input)
        {
            var result = Enumerable.Empty<byte>().ToArray();
            DCT.Execute(d =>
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    result = ms.ToArray();
                }
            });
            return result;
        }
    }
}
