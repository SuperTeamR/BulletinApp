using FessooFramework.Objects.Data;
using System.Collections.Generic;
using System.Linq;

namespace BulletinExample.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Борда буллетинов. </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class Board : EntityObjectALM<Board, BoardState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Наименование борды. </summary>
        ///
        /// <value> The name. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Name { get; set; }


        #endregion

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<Board, BoardState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<Board, BoardState>(BoardState.Active, BoardState.Checked, Checked),
            new EntityObjectALMConfiguration<Board, BoardState>(BoardState.Checked, BoardState.Changed, Changed),
            new EntityObjectALMConfiguration<Board, BoardState>(BoardState.Checked, BoardState.Active, Active),
            new EntityObjectALMConfiguration<Board, BoardState>(BoardState.Changed, BoardState.Active, Active),
        };
        protected override IEnumerable<BoardState> DefaultState => new[] { BoardState.Error };

        protected override IEnumerable<EntityObjectALMCreator<Board>> CreatorsService => throw new System.NotImplementedException();

        protected override int GetStateValue(BoardState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        private Board Active(Board arg1, Board arg2)
        {
            arg1.Name = arg2.Name;

            return arg1;
        }

        private Board Changed(Board arg1, Board arg2)
        {
            arg1.Name = arg2.Name;

            return arg1;
        }

        private Board Checked(Board arg1, Board arg2)
        {
            arg1.Name = arg2.Name;

            return arg1;
        }
        #endregion

    }

    public enum BoardState
    {
        Active = 0,
        Checked = 1,
        Changed = 3,
        Error = 99,
    }
}
