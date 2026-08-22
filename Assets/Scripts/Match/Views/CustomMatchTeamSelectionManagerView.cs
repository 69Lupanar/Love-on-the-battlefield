using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Assigne les joueurs disponibles aux équipes à jouer
    /// </summary>
    [RequireComponent(typeof(CustomMatchTeamSelectionManagerViewModel))]
    public sealed class CustomMatchTeamSelectionManagerView : MonoBehaviour
    {
        #region Inspecteur

        [SerializeField]
        [Tooltip("Si true, les équipes doivent avoir un nombre exact de joueurs ppaux. Si false, elles peuvent en avoir moins (min. 1)")]
        private bool _enforceStrictCharacterLimit = false;

        [SerializeField]
        [Tooltip("Préfab des labels glissables/déposables dans l'interface")]
        private GameObject _draggableLabelPrefab;

        [SerializeField]
        [Tooltip("Parent des noms des joueurs ppaux")]
        private Transform _allyMainParent;

        [SerializeField]
        [Tooltip("Parent des noms des joueurs remplaçants")]
        private Transform _allySubstituteParent;

        [SerializeField]
        [Tooltip("Parent des noms des joueurs ppaux")]
        private Transform _enemyMainParent;

        [SerializeField]
        [Tooltip("Parent des noms des joueurs remplaçants")]
        private Transform _enemySubstituteParent;

        [SerializeField]
        [Tooltip("Parent des noms des joueurs en réserve")]
        private Transform _reserveParent;

        #endregion

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