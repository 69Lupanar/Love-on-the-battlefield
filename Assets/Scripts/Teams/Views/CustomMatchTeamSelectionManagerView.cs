using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Assigne les joueurs disponibles aux équipes à jouer
    /// </summary>
    [RequireComponent(typeof(CustomMatchTeamSelectionManagerViewModel))]
    public sealed class CustomMatchTeamSelectionManagerView : MonoBehaviour
    {
        #region Méthodes publiques

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="allyRoster">Alliés</param>
        /// <param name="enemyRoster">Ennemis</param>
        public void SetDefaultRosters(TeamRosterSO allyRoster, TeamRosterSO enemyRoster)
        {

        }

        #endregion
    }
}