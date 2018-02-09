using BulletinBridge.Messages.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge
{
    public static class UdpManager
    {
        static IPAddress IpAdress { get; set; }
        static int RemotePort { get; set; }
        static int LocalPort { get; set; }

        public static void Set(string remoteIp, int remotePort, int localPort)
        {
            IpAdress = IPAddress.Parse(remoteIp);
            RemotePort = remotePort;
            LocalPort = localPort;
        }

        public static void Send(MessageBase message)
        {
            var sender = new UdpClient();
            try
            {
                // Создаем endPoint по информации об удаленном хосте
                var endPoint = new IPEndPoint(IpAdress, RemotePort);
                // Преобразуем данные в массив байтов
                var ser = new DataContractSerializer(message.GetType());
                using (var ms = new MemoryStream())
                {
                    ser.WriteObject(ms, message);
                    var f = ms.ToArray();
                    byte[] bytes = f;
                    // Отправляем данные
                    sender.Send(bytes, bytes.Length, endPoint);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdManager -- " + ex.ToString());
            }
            finally
            {
                sender.Close();
            }
        }

        public static void Receive(Func<MessageBase, MessageBase> afterReceived)
        {
            // Создаем UdpClient для чтения входящих данных
            var udp = new UdpClient(LocalPort);
            try
            {
                IPEndPoint remoteIpEndPoint = null;
                while (true)
                {
                    // Ожидание дейтаграммы
                    byte[] receiveBytes = udp.Receive(
                       ref remoteIpEndPoint);
                    // Преобразуем и отображаем данные
                    var ser = new DataContractSerializer(typeof(MessageBase));
                    using (var ms = new MemoryStream(receiveBytes))
                    {
                        var message = (MessageBase)ser.ReadObject(ms);
                        //ServiceRouter.ExecuteRouting(message);
                        if (afterReceived != null)
                            afterReceived(message);  
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdManager -- " + ex.ToString());
            }
            finally
            {
                udp.Close();
            }
        }


    }
}
