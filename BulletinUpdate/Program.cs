using FessooFramework.Tools.Helpers;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BulletinUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SendMessage("Start update BulletinDriver");
            try
            {
                DownloadFile();
            }
            catch (Exception ex)
            {
                ConsoleHelper.SendException(ex);
            }
            //ConsoleHelper.SendMessage("Complete update BulletinDriver");
            Console.ReadLine();
        }

        static void DownloadFile()
        {
            ConsoleHelper.SendMessage("Download file started BulletinDriver");

            var client = new WebClient();
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                       @"\Update\";
            var name = $"{DateTime.Now.Year}_{DateTime.Now.Month}_{DateTime.Now.Day}";
            var file = "";
            var i = 1;
            while (true)
            {
                file = Path.Combine(path,$"{name}_{i}.zip");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!File.Exists(file))
                    break;
                i += 1;
            }
            ConsoleHelper.SendMessage("File name generate complete - " + file);

            client.DownloadFileAsync(new Uri("http://176.111.73.51:44444/Apps/BulletinWebDriver.zip"), file);
            client.DownloadFileCompleted += (a, b) => Unpackage(file);
        }

        static void Unpackage(string path)
        {
            try
            {
                CloseAll();
                UnZip(path);
                Process.Start(@"BulletinWebDriver\BulletinWebDriver.exe");
            }
            catch (Exception ex)
            {
                ConsoleHelper.SendException(ex);
            }
        }

        static void CloseAll()
        {
            ConsoleHelper.SendMessage("Close application start");

            var driver = Process.GetProcessesByName("BulletinWebDriver");
            foreach (var d in driver)
                d.Kill();

            var geckodrivers = Process.GetProcessesByName("geckodriver");
            foreach (var d in geckodrivers)
                d.Kill();

            var fireFox = Process.GetProcessesByName("Firefox");
            foreach (var d in fireFox)
                d.Kill();

            ConsoleHelper.SendMessage("Close application complete");

        }
        static void UnZip(string path)
        {
            try
            {
                using (var zip = ZipFile.Read(path))
                {
                    foreach (var e in zip)
                    {
                        try//Allowed
                        {
                            e.Extract(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ExtractExistingFileAction.OverwriteSilently);
                        }
                        catch (Exception ex)
                        {
                            try//Allowed
                            {
                                var owerwrite = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                    e.FileName);
                                var fi = new FileInfo(owerwrite);
                                if (fi.Exists)
                                {
                                    File.Move(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                        e.FileName), Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                        e.FileName + ".oldest"));
                                    e.Extract(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ExtractExistingFileAction.OverwriteSilently);

                                }
                            }
                            catch (Exception exception)
                            {
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.SendException(ex);
            }
        }
    }
}
