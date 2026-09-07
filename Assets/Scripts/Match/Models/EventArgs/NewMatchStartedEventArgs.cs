using System;
using Assets.Scripts.Teams;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Arguments de l'événement associé
    /// </summary>
    public class NewMatchStartedEventArgs : EventArgs
    {
        #region Propriétéss

        /// <summary>
        /// Paramètres d'un match
        /// </summary>
        internal MatchSettingsData MatchSettings { get; private set; }

        /// <summary>
        /// Equipe alliée
        /// </summary>
        internal TeamRosterSO AllyTeam { get; private set; }

        /// <summary>
        /// Equipe ennemie
        /// </summary>
        internal TeamRosterSO EnemyTeam { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="matchSettings">Paramètres d'un match</param>
        /// <param name="allyTeam">Equipe alliée</param>
        /// <param name="enemyTeam">Equipe ennemie</param>
        public NewMatchStartedEventArgs(MatchSettingsData matchSettings, TeamRosterSO allyTeam, TeamRosterSO enemyTeam)
        {
            MatchSettings = matchSettings;
            AllyTeam = allyTeam;
            EnemyTeam = enemyTeam;
        }

        #endregion
    }
}