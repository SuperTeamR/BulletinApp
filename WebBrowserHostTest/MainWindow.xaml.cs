using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebBrowserHostTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            (MyWebBrowser.Child as System.Windows.Forms.WebBrowser).Navigate(Constants.InitialUrl);
        }

        private void wbWinForms_DocumentTitleChanged(object sender, EventArgs e)
        {
            this.Title = (sender as System.Windows.Forms.WebBrowser).DocumentTitle;
        }

        void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var webBrowser = (MyWebBrowser.Child as System.Windows.Forms.WebBrowser);
            var buttons = webBrowser.Document.GetElementsByTagName("button").Cast<HtmlElement>();
            var pack = "Продолжить с пакетом «Обычная продажа»";
            var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
            if (button != null)
                button.InvokeMember("click");
        }
    }
}
