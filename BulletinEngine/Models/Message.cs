using BulletinBridge.Models;
using BulletinEngine.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Models
{
    public class Message : EntityObjectALM<Message, DefaultState>
    {
        public Guid AccessId { get; set; }
        public string Text { get; set; }
        public string IsAnswered { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Url { get; set; }

        #region ALM
        protected override IEnumerable<EntityObjectALMConfiguration<Message, DefaultState>> Configurations => Enumerable.Empty<EntityObjectALMConfiguration<Message, DefaultState>>();

        protected override int GetStateValue(DefaultState state)
        {
            return (int)state;
        }

        protected override Message SetValueDefault(Message arg1, Message arg2)
        {
            arg1.AccessId = arg2.AccessId;
            arg1.Text = arg2.Text;
            arg1.PublicationDate = arg2.PublicationDate;
            arg1.IsAnswered = arg2.IsAnswered;
            return arg1;
        }
        #endregion

        #region Creators
        protected override IEnumerable<EntityObjectALMCreator<Message>> CreatorsService => new[]
        {
            EntityObjectALMCreator<Message>.New<MessageCache>(ToCache, ToEntity, new Version(1,0,0,0)),
        };
        private Message ToEntity(MessageCache cache, Message entity)
        {
            entity.AccessId = cache.AccessId;
            entity.Text = cache.Text;
            entity.PublicationDate = cache.PublicationDate;

            return entity;
        }
        internal static MessageCache ToCache(Message obj)
        {
            var result = new MessageCache();
            result.AccessId = obj.AccessId;
            result.Text = obj.Text;
            result.PublicationDate = obj.PublicationDate;

            return result;
        }
        #endregion

        public override IEnumerable<EntityObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<EntityObject> obj = null, IEnumerable<Guid> id = null)
        {
            var result = Enumerable.Empty<EntityObject>();
            BCT.Execute(d =>
            {
                var entities = Enumerable.Empty<Message>();
                if (obj.Any())
                    entities = obj.Select(q => (Message)q).ToArray();
                switch (code)
                {
                    case "Save":
                        result = obj;
                        d.SaveChanges();
                        break;
                    case "All":
                        break;
                }
            });
            return result;
        }

        public override IEnumerable<TDataModel> _CacheSave<TDataModel>(IEnumerable<TDataModel> objs)
        {
            var result = Enumerable.Empty<TDataModel>();
            BCT.Execute(d =>
            {
                foreach(var obj in objs)
                {
                    var msg = d.BulletinDb.Messages.FirstOrDefault(q => q.Id == obj.Id);
                    if(msg == null)
                    {
                        var newMsg = obj as Message;
                        newMsg.StateEnum = DefaultState.Created;

                    }
                }
                d.SaveChanges();
                result = objs;
            });
            return result;
        }
    }
}
