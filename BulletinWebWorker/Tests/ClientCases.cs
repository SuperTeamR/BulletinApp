using BulletinBridge.Data;
using BulletinBridge.Messages.BoardApi;
using BulletinWebWorker.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Tests
{
    public static class ClientCases
    {
        public static void AddBulletins()
        {
            var signature = new GroupSignature("Хобби и отдых", "Спорт и отдых", "Другое");
            var fields = new Dictionary<string, string>
            {
                {"Название объявления", "Мое объявление" },
                {"Описание объявления", "Описание..." }
            };
            var package = new BulletinPackage
            {
                Signature = signature,
                ValueFields = fields
            };
            var request = new RequestBoardApi_AddBulletins
            {
                Objects = new[] { package }
            };

            ClientService.SendMessage<RequestBoardApi_AddBulletins, ResponseBoardApi_AddBulletins>(request, BulletinBridge.Commands.CommandApi.Board_AddBulletins);
        }

    }
}
