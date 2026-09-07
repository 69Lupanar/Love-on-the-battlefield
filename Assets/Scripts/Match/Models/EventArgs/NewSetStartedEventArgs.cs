using System;
using Assets.Scripts.Teams;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Arguments de l'événement associé
    /// </summary>
    public class NewSetStartedEventArgs : EventArgs
    {
        #region Propriétéss

        /// <summary>
        /// true si la manche est en mort subite
        /// </summary>
        internal bool SuddenDeath { get; private set; }

        /// <summary>
        /// Données de l'équipe alliée
        /// </summary>
        internal TeamData AllyTeamData { get; private set; }

        /// <summary>
        /// Données de l'équipe ennemie
        /// </summary>
        internal TeamData EnemyTeamData { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="suddenDeath">true si la manche est en mort subite</param>
        /// <param name="allyTeamData">Données de l'équipe alliée</param>
        /// <param name="enemyTeamData">Données de l'équipe ennemie</param>
        public NewSetStartedEventArgs(bool suddenDeath, TeamData allyTeamData, TeamData enemyTeamData)
        {
            SuddenDeath = suddenDeath;
            AllyTeamData = allyTeamData;
            EnemyTeamData = enemyTeamData;
        }

        #endregion
    }
}