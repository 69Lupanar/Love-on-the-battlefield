using Assets.Scripts.Teams;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Gère le contenu débloqué par le joueur au cours de sa progression
    /// </summary>
    public static class PlayerUnlockablesManager
    {
        /// <summary>
        /// Obtient la liste des équipes débloquées par le joueur
        /// </summary>
        /// <returns>La liste des équipes débloquées par le joueur</returns>
        public static TeamRosterSO[] GetUnlockedTeams()
        {
            // TAF : Pour les tests, on prend tout dans les Resources.
            // Pour le vrai jeu, utiliser des IDs pour retrouver les équipes correspondantes

            return Resources.LoadAll<TeamRosterSO>("Teams/Rosters");
        }

        /// <summary>
        /// Obtient la liste des persos débloquées par le joueur
        /// </summary>
        /// <returns>La liste des persos débloquées par le joueur</returns>
        public static TeamCharacterSO[] GetUnlockedCharacters()
        {
            // TAF : Pour les tests, on prend tout dans les Resources.
            // Pour le vrai jeu, utiliser des IDs pour retrouver les persos correspondants

            return Resources.LoadAll<TeamCharacterSO>("Teams/Characters");
        }
    }
}