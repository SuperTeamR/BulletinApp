using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BulletinClient.ViewModels;

namespace BulletinClient.Views
{
    /// <summary>
    /// Логика взаимодействия для ModalV.xaml
    /// </summary>
    public partial class ModalV : UserControl
    {
        public ModalV()
        {
            InitializeComponent();
        }

        private void grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var viewmodel = DataContext as ModalVM;
            if (viewmodel != null)
                viewmodel.CommandDialogClose.Execute(true);
            //DialogsDispacher.ClearContentDialog();
        }
    }
}
