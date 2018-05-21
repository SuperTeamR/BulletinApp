using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Models
{
    public class Email : EntityObjectALM<Email, DefaultState>
    {
        #region Properties

        public string Title { get; set; }
        public string Body { get; set; }
        public string Destination { get; set; }

        #endregion
        protected override IEnumerable<EntityObjectALMCreator<Email>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<Email>>();

        protected override int GetStateValue(DefaultState state)
        {
            return (int)state;
        }

        protected override Email SetValueDefault(Email arg1, Email arg2)
        {
            arg1.Title = arg2.Title;
            arg1.Body = arg2.Body;
            arg1.Destination = arg2.Destination;
            return arg1;
        }
    }
}
