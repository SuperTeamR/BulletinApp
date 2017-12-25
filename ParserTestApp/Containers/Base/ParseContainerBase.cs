using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Containers.Base
{
    public abstract class ParseContainerBase : IParseContainer
    {
        #region Property
        public abstract string StartUrl { get; }
        public abstract string LoginUrl { get; }
        public abstract string Login { get; }
        public abstract string Password { get; }
        #endregion
        #region Constructor
        #endregion
        #region Methods
        public bool Execute()
        {
            var result = false;
            _DCT.Execute(data =>
            {
                Authorization();
            }, _DCTGroup.ParseContainerBase);
            return result;
        }
        public abstract void Authorization();
        #endregion
    }
}
