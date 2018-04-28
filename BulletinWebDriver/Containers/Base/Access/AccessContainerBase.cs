using BulletinBridge.Data;
using System;

namespace BulletinWebWorker.Containers.Base.Access
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Управляет учетной записью на борде </summary>
    ///
    /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------
    abstract class AccessContainerBase
    {
        public abstract Guid Uid { get; }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Пытается авторизоваться </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="access">   Пакет авторизации </param>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------
        public abstract bool TryAuth(AccessCache access);
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Авторизация на борде </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------
        protected abstract bool Auth();
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Выход из профиля на борде </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------
        protected abstract void Exit();
    }
}