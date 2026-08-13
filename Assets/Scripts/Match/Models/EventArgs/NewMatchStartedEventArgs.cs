using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Arguments de l'événement associé
    /// </summary>
    public class NewMatchStartedEventArgs : EventArgs
    {
        #region Propriétéss

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        internal List<Transform> AlliesT { get; private set; }

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal List<Transform> EnemiesT { get; private set; }

        /// <summary>
        /// Les ballons
        /// </summary>
        internal List<Transform> BallsT { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="alliesT">Alliés instanciés</param>
        /// <param name="enemiesT">Ennemis instanciés</param>
        /// <param name="ballsT">Ballons instanciés</param>
        public NewMatchStartedEventArgs(List<Transform> alliesT, List<Transform> enemiesT, List<Transform> ballsT)
        {
            AlliesT = alliesT;
            EnemiesT = enemiesT;
            BallsT = ballsT;
        }

        #endregion
    }
}