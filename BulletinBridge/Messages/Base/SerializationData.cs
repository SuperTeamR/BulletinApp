using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.Base
{
    [DataContract]
    public class SerializationData
    {
        #region Property
        [DataMember]
        public byte[] Bytes { get; set; }
        [DataMember]
        public string AssemblyQualifiedName { get; set; }
        #endregion
        #region Constructor
        public SerializationData(object obj)
        {
            Serialize(obj);
        }
        #endregion
        #region Methods
        private void Serialize(object obj)
        {
            AssemblyQualifiedName = obj.GetType().AssemblyQualifiedName.ToString();
            var ser = new DataContractSerializer(obj.GetType());
            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);
                var f = ms.ToArray();
                Bytes = f;
            }
        }
        public T Deserialize<T>()
        {
            var ser = new DataContractSerializer(Type.GetType(AssemblyQualifiedName));
            using (var ms = new MemoryStream(Bytes))
                return (T)ser.ReadObject(ms);
        }
        #endregion
    }
}
