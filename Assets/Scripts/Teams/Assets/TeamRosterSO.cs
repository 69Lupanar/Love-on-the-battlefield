using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Teams
{

    /// <summary>
    /// Représente une équipe de joueurs.
    /// On sépare les SOs de l'équipe et des joueurs pour nous permettre
    /// d'assigner différents joueurs à de mêmes équipes
    /// et d'avoir différentes compositions en fonction de la progression de l'histoire.
    /// </summary>
    [CreateAssetMenu(fileName = "New Team Roster", menuName = "Scriptable Objects/Teams/Team Roster")]
    public class TeamRosterSO : ScriptableObject
    {
        #region Propriétés

        /// <summary>
        /// Les données de l'équipe
        /// </summary>
        [HideInInspector]
        public TeamData TeamData;

        /// <summary>
        /// Les joueurs de l'équipe
        /// </summary>
        [HideInInspector]
        public TeamCompositionData CompositionData;

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("Les données de l'équipe")]
        private TeamSO Team;

        [SerializeField]
        [Tooltip("Joueurs ppaux de l'équipe")]
        private List<CharacterSO> MainCharacters;

        [SerializeField]
        [Tooltip("Joueurs remplaçants de l'équipe")]
        private List<CharacterSO> Substitutes;

        #endregion

        #region Méthodes Unity

#if UNITY_EDITOR

        /// <summary>
        /// Appelée quand une valeur change dans l'inspecteur
        /// </summary>
        private void OnValidate()
        {
            TeamData = Team.Data;

            if (CompositionData.MainCharacters != null)
            {
                CompositionData.MainCharacters.Clear();
                CompositionData.Substitutes.Clear();
                CompositionData.MainCharacters.AddRange(MainCharacters.Select(character => character.Data));
                CompositionData.Substitutes.AddRange(Substitutes.Select(character => character.Data));
            }
            else
            {
                CompositionData = new TeamCompositionData(MainCharacters.Select(character => character.Data), Substitutes.Select(character => character.Data));
            }
        }

#endif

        #endregion
    }
}