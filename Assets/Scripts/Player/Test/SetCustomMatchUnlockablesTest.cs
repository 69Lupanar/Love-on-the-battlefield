using Assets.Scripts.Teams;
using UnityEngine;

namespace Assets.Scripts.Player.Test
{
    /// <summary>
    /// Script de test pour vérifier le bon fonctionnement de la sélection de membres d'une équipe
    /// </summary>
    public sealed class SetCustomMatchUnlockablesTest : MonoBehaviour
    {
        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            // TAF : Pour les tests, on prend tout dans les Resources.
            // Pour le vrai jeu, utiliser des IDs pour retrouver les ressources correspondantes

            CustomMatchModeUnlockables.Teams.AddRange(GetTeamsUnlockedInCustomMatch());
            CustomMatchModeUnlockables.Characters.AddRange(GetCharactersUnlockedInCustomMatch());
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
        private TeamCharacterSO[] GetCharactersUnlockedInCustomMatch()
        {
            return Resources.LoadAll<TeamCharacterSO>("Teams/Characters");
        }
    }
}