﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace BulletinClient.Forms.MainView
{

    public static class ClipboardHelper2
    {
        public delegate string[] ParseFormat(string value);

        public static List<string[]> ParseClipboardData()
        {
            List<string[]> clipboardData = new List<string[]>();

            // get the data and set the parsing method based on the format
            // currently works with CSV and Text DataFormats            
            IDataObject dataObj = System.Windows.Clipboard.GetDataObject();

            if (dataObj != null)
            {
                string[] formats = dataObj.GetFormats(true);
                if (formats.Contains(DataFormats.CommaSeparatedValue))
                {
                    string clipboardString = (string)dataObj.GetData(DataFormats.CommaSeparatedValue);
                    {
                        // EO: Subject to error when a CRLF is included as part of the data but it work for the moment and I will let it like it is
                        // WARNING ! Subject to errors
                        string[] lines = clipboardString.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                        string[] lineValues;
                        foreach (string line in lines)
                        {
                            lineValues = CsvHelper.ParseLineCommaSeparated(line);
                            if (lineValues != null)
                            {
                                clipboardData.Add(lineValues);
                            }
                        }
                    }
                }
                else if (formats.Contains(DataFormats.Text))
                {
                    string clipboardString = (string)Clipboard.GetData(DataFormats.Text);
                    //(string)dataObj.GetData(DataFormats.Text, true);
                    clipboardData = CsvHelper.ParseText(clipboardString);
                }
            }

            return clipboardData;
        }
    }
}