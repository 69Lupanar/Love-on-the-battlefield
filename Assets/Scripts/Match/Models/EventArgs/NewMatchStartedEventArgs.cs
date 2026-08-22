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
        /// Composition de joueurs de l'équipe alliée
        /// </summary>
        internal TeamCompositionData AllyTeamComposition { get; private set; }

        /// <summary>
        /// Composition de joueurs de l'équipe ennemie
        /// </summary>
        internal TeamCompositionData EnemyTeamComposition { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="matchSettings">Paramètres d'un match</param>
        /// <param name="allyTeamComposition">Composition de joueurs de l'équipe alliée</param>
        /// <param name="enemyTeamComposition">Composition de joueurs de l'équipe ennemie</param>
        public NewMatchStartedEventArgs(MatchSettingsData matchSettings, TeamRosterSO allyTeamComposition, TeamRosterSO enemyTeamComposition)
        {
            MatchSettings = matchSettings;
            AllyTeamComposition = allyTeamComposition.Roster;
            EnemyTeamComposition = enemyTeamComposition.Roster;
        }

        #endregion
    }
}