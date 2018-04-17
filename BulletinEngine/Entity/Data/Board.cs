using FessooFramework.Objects.Data;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Entity.Data
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
            new EntityObjectALMConfiguration<Board, BoardState>(BoardState.Active, BoardState.Checked, SetValueDefault),
            new EntityObjectALMConfiguration<Board, BoardState>(BoardState.Checked, BoardState.Changed, SetValueDefault),
            new EntityObjectALMConfiguration<Board, BoardState>(BoardState.Checked, BoardState.Active, SetValueDefault),
            new EntityObjectALMConfiguration<Board, BoardState>(BoardState.Changed, BoardState.Active, SetValueDefault),
        };
        protected override int GetStateValue(BoardState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override Board SetValueDefault(Board arg1, Board arg2)
        {
            arg1.Name = arg2.Name;

            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<Board>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<Board>>();
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
