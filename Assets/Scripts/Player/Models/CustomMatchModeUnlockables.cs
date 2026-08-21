using System.Collections.Generic;
using Assets.Scripts.Teams;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Contient tous les objets débloqués par le joueurs au cours de sa progression,
    /// réservés au mode Match Personnalisé
    /// </summary>
    public static class CustomMatchModeUnlockables
    {
        /// <summary>
        /// Les équipes débloquées par le joueur
        /// </summary>
        public static List<TeamSO> Teams = new();

        /// <summary>
        /// Les personnages débloquées par le joueur
        /// </summary>
        public static List<CharacterSO> Characters = new();
    }
}