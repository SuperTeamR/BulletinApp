using BulletinClient.Core;
using BulletinClient.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.General
{
    class MainWindowVM : ViewModelBase
    {
        public Forms.MainView.ViewModel CurrentView => DCT.Execute(d => d.ContainerViewModel.GetInstance<Forms.MainView.ViewModel>());
    }
}
