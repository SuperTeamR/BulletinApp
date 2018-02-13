using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BulletinClient.Helpers
{
    static class WindowHelper
    {
        private static readonly double WINDOW_MINHEIGHT = 700;
        private static readonly double WINDOW_MINWIDTH = 1000;

        public static MainWindow Window
        {
            get { return _w = _w ?? CreateWindow(); }
        }

        static MainWindow _w; 

        static MainWindow CreateWindow()
        {
            _w = new MainWindow();
            Application.Current.MainWindow = _w;
            var viewModel = new General.MainWindowVM();
            _w.DataContext = viewModel;
            _w.MinHeight = WINDOW_MINHEIGHT;
            _w.MinWidth = WINDOW_MINWIDTH;
            _w.Show();
            return _w;
        }
    }
}
