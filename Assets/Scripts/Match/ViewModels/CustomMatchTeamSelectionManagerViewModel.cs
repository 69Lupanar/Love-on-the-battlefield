using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly List<CharacterData> _reserve = new();

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="allyTeamComposition">Alliés</param>
        internal void SetAllyTeam(TeamCompositionData allyTeamComposition)
        {
            AllyTeamComposition = allyTeamComposition;
        }

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="enemyTeamComposition">Ennemis</param>
        internal void SetEnemyTeam(TeamCompositionData enemyTeamComposition)
        {
            EnemyTeamComposition = enemyTeamComposition;
        }

        /// <summary>
        /// Assigne les personnages en réserve
        /// </summary>
        /// <param name="characters">Les personnages débloqués par le joueur</param>
        internal void SetReserveCharacters(IEnumerable<CharacterData> characters)
        {
            _reserve.Clear();
            _reserve.AddRange(characters);
        }

        #endregion
    }
}