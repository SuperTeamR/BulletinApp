using System;
using System.Collections;
using System.Collections.Generic;
using BulletinClient.ViewModels;

namespace BulletinClient.Helpers
{
    public class ModalWindow
    {
        public object Content { get; set; }

        public ModalWindow(object content)
        {
            Content = content;
        }
    }


    public static class ModalHelper
    {
        private static ModalVM modalVM;
        public static Action DialogChangeAction { get; set; }

        public static void Initialize(ModalVM modalVM)
        {
            ModalHelper.modalVM = modalVM;
        }

        public static void OpenDialog<T>()
            where T : new()
        {
            OpenDialog(new T());
        }

        public static void OpenDialog(object content)
        {
            var modal = new ModalWindow(content);
            modalVM.SetContent(modal);
            if (DialogChangeAction != null) DialogChangeAction();
        }
        public static void CloseDialog()
        {
            modalVM.SetContent(null);
            if (DialogChangeAction != null) DialogChangeAction();
        }
    }
}