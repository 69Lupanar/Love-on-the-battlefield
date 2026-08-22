using System.Collections.Generic;
using Assets.Scripts.Teams;
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Contient tous les objets débloqués par le joueurs au cours de sa progression,
    /// réservés au mode Match Personnalisé
    /// </summary>
    public static class CustomMatchModeUnlockables
    {
        #region Statiques

        /// <summary>
        /// Les équipes débloquées par le joueur
        /// </summary>
        public static List<TeamRosterSO> Teams = new();

        /// <summary>
        /// Les personnages débloquées par le joueur
        /// </summary>
        public static List<CharacterSO> Characters = new();

        #endregion

        #region Méthodes publiques statiques

        // TAF : Pour les tests, on prend tout dans les Resources.
        // Pour le vrai jeu, utiliser des IDs pour retrouver les ressources correspondantes

        /// <summary>
        /// Obtient la liste des équipes débloquées par le joueur
        /// </summary>
        /// <returns>La liste des équipes débloquées par le joueur</returns>
        public static TeamRosterSO[] GetRostersUnlockedInCustomMatch()
        {
            return Resources.LoadAll<TeamRosterSO>("Rosters");
        }

        /// <summary>
        /// Obtient la liste des persos débloquées par le joueur
        /// </summary>
        /// <returns>La liste des persos débloquées par le joueur</returns>
        public static CharacterSO[] GetCharactersUnlockedInCustomMatch()
        {
            return Resources.LoadAll<CharacterSO>("Characters");
        }
        #endregion
    }
}