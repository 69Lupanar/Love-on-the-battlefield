using Assets.Scripts.Teams;
using UnityEngine;

namespace Assets.Scripts.Player.Test
{
    /// <summary>
    /// Script de test pour vérifier le bon fonctionnement de la sélection de membres d'une équipe
    /// </summary>
    public sealed class SetCustomMatchUnlockablesTest : MonoBehaviour
    {
        #region Inspecteur

        [Tooltip("L'équipe par défaut du joueur pour ce test")]
        public TeamRosterSO DefaultAllyTeam;

        [Tooltip("L'équipe par défaut de l'IA ennemie pour ce test")]
        public TeamRosterSO DefaultEnemyTeam;

        #endregion

        #region Instance

        /// <summary>
        /// La vue
        /// </summary>
        private CustomMatchTeamSelectionManagerView _view;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _view = FindAnyObjectByType<CustomMatchTeamSelectionManagerView>();

            // TAF : Pour les tests, on prend tout dans les Resources.
            // Pour le vrai jeu, utiliser des IDs pour retrouver les ressources correspondantes

            CustomMatchModeUnlockables.Teams.AddRange(GetTeamsUnlockedInCustomMatch());
            CustomMatchModeUnlockables.Characters.AddRange(GetCharactersUnlockedInCustomMatch());
            _view.SetDefaultRosters(DefaultAllyTeam, DefaultEnemyTeam);

        }

        #endregion

        /// <summary>
        /// Obtient la liste des équipes débloquées par le joueur
        /// </summary>
        /// <returns>La liste des équipes débloquées par le joueur</returns>
        private TeamSO[] GetTeamsUnlockedInCustomMatch()
        {
            return Resources.LoadAll<TeamSO>("Teams/Rosters");
        }

        /// <summary>
        /// Obtient la liste des persos débloquées par le joueur
        /// </summary>
        /// <returns>La liste des persos débloquées par le joueur</returns>
        private CharacterSO[] GetCharactersUnlockedInCustomMatch()
        {
            return Resources.LoadAll<CharacterSO>("Teams/Characters");
        }
    }
}