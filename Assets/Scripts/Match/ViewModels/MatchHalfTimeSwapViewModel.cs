using Assets.Scripts.Teams;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Assigne les joueurs disponibles aux équipes à jouer
    /// </summary>
    internal sealed class MatchHalfTimeSwapViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Alliés
        /// </summary>
        internal TeamCompositionData AllyTeamComposition { get; private set; }

        /// <summary>
        /// Ennemis
        /// </summary>
        internal TeamCompositionData EnemyTeamComposition { get; private set; }

        #endregion

        #region Instance

        /// <summary>
        /// true si les équipes doivent avoir un nombre exact de joueurs actifs
        /// </summary>
        private bool _enforceStrictCountLimit = true;

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="allyTeamComposition">Alliés</param>
        /// <param name="enemyTeamComposition">Ennemis</param>
        /// <param name="enforceStrictCountLimit">true si les équipes doivent avoir un nombre exact de joueurs actifs</param>
        internal void SetTeams(TeamCompositionData allyTeamComposition, TeamCompositionData enemyTeamComposition, bool enforceStrictCountLimit = true)
        {
            _enforceStrictCountLimit = enforceStrictCountLimit;
            AllyTeamComposition = allyTeamComposition;
            EnemyTeamComposition = enemyTeamComposition;
        }

        #endregion
    }
}