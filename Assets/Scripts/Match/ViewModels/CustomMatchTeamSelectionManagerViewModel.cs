using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Assets.Scripts.Teams;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Assigne les joueurs disponibles aux équipes à jouer
    /// </summary>
    internal sealed class CustomMatchTeamSelectionManagerViewModel : MonoBehaviour
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

        /// <summary>
        /// Liste des persos en réserve
        /// </summary>
        internal ReadOnlyCollection<CharacterData> Reserve => _reserve.AsReadOnly();

        #endregion

        #region Instance

        /// <summary>
        /// Liste des persos en réserve
        /// </summary>
        private List<CharacterData> _reserve = new();

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="allyTeamComposition">Alliés</param>
        internal void SetAllyTeam(TeamCompositionData allyTeamComposition)
        {
            // On en crée une copie car on veut pouvoir changer les joueurs
            // sans toucher à l'asset de base
            AllyTeamComposition = allyTeamComposition.Clone();
        }

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="enemyTeamComposition">Ennemis</param>
        internal void SetEnemyTeam(TeamCompositionData enemyTeamComposition)
        {
            // On en crée une copie car on veut pouvoir changer les joueurs
            // sans toucher à l'asset de base
            EnemyTeamComposition = enemyTeamComposition.Clone();
        }

        /// <summary>
        /// Assigne les personnages en réserve
        /// </summary>
        /// <param name="characters">Les personnages débloqués par le joueur</param>
        internal void SetReserveCharacters(IEnumerable<CharacterData> characters)
        {
            _reserve.Clear();

            // On vérifie que les personnages à ajouter ne sont pas déjà assignés à une équipe.
            // Si ce n'est pas le cas, on les ajoute à la réserve.

            _reserve.AddRange(characters.Where(character => !AllyTeamComposition.MainCharacters.Contains(character) &&
                                                            !AllyTeamComposition.Substitutes.Contains(character) &&
                                                            !EnemyTeamComposition.MainCharacters.Contains(character) &&
                                                            !EnemyTeamComposition.Substitutes.Contains(character)));
        }

        #endregion
    }
}