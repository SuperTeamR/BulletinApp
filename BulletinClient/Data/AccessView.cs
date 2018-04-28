using BulletinBridge.Data;
using FessooFramework.Tools.Helpers;

namespace BulletinClient.Data
{
    public class AccessView
    {
        string Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string State { get; set; }

        public AccessView()
        {

        }
        public AccessView(AccessCache package)
        {
            Login = package.Login;
            Password = package.Password;
            State = TranslateState(EnumHelper.GetValue<AccessState>(package.State));
        }

        string TranslateState(AccessState state)
        {
            switch (state)
            {
                case AccessState.Created:
                    return "Создана";
                case AccessState.Activated:
                    return "Подключена";
                case AccessState.Blocked:
                    return "Заблокирована";
                case AccessState.Banned:
                    return "Забанена";
                case AccessState.DemandPay:
                    return "Заблокирована";
                case AccessState.Closed:
                    return "Закрыта";
                case AccessState.Unchecked:
                    return "Не проверена";
                case AccessState.Checking:
                    return "Проверяется";
                case AccessState.Error:
                    return "Ошибка";
                default:
                    return string.Empty;
            }
        }
    }
}