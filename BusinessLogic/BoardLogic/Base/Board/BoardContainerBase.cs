using BusinessLogic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BoardLogic.Base
{
    /// <summary>
    /// Контейнер для работы с уровнем Board
    /// - Управляет жизненным циклом буллетина
    /// - Управляет доступом к борде
    /// </summary>
    internal abstract class BoardContainerBase : ContainerBase<Board>
    {
        public bool HasAuth { get; set; }
        public abstract void Registry();
        public abstract void Auth();
        public abstract void Exit();
        public abstract bool IsBan();
        public abstract bool FillCaptcha();
        public abstract bool IsAccountBlocked();
        public abstract void GetBulletinState(string url);
        public abstract void EditBulletin(Bulletin bulletin);
        public abstract void AddBulletin(Data.Group group, Dictionary<string, string> dictionary);
        public abstract void UpdateBulletin();
        public abstract void CloseBulletin();
        public abstract void GetStats();
        public abstract void GetBulletinId();

        /// <summary>
        /// Загружает все группы и поля Board
        /// </summary>
        public abstract IEnumerable<Data.Group> LoadGroups();

        public abstract string GenerateXlsFromGroup(GroupSignature signature);
        public abstract void AddFromXls();
        public abstract void EditFromXls();
        public abstract void GetBulletins();
    }
}
