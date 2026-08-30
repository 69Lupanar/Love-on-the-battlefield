using System;
using Assets.Scripts.Teams;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Arguments de l'événement associé
    /// </summary>
    public class HalfTimeEndedEventArgs : EventArgs
    {
        #region Propriétéss

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
        /// <param name="allyTeamComposition">Composition de joueurs de l'équipe alliée</param>
        /// <param name="enemyTeamComposition">Composition de joueurs de l'équipe ennemie</param>
        public HalfTimeEndedEventArgs(TeamCompositionData allyTeamComposition, TeamCompositionData enemyTeamComposition)
        {
            AllyTeamComposition = allyTeamComposition;
            EnemyTeamComposition = enemyTeamComposition;
        }

        #endregion
    }
}