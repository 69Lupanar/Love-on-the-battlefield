using System.Collections.Generic;
using Assets.Scripts.Teams;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Assigne les joueurs disponibles aux équipes à jouer
    /// </summary>
    internal sealed class MatchHalfTimeSwapViewModel : MonoBehaviour
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

        #endregion

        #region Instance

        /// <summary>
        /// true si les équipes doivent avoir un nombre exact de joueurs actifs
        /// </summary>
        private bool _enforceStrictCountLimit = true;

        /// <summary>
        /// Le nb d'alliés actifs à l'ouverture du menu de sélection
        /// </summary>
        private int _allyMainCompositionCount;

        /// <summary>
        /// Le nb d'ennemis actifs à l'ouverture du menu de sélection
        /// </summary>
        private int _enemyMainCompositionCount;

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Assigne les compositions d'équipe par défaut
        /// </summary>
        /// <param name="allyTeamComposition">Alliés</param>
        /// <param name="enemyTeamComposition">Ennemis</param>
        /// <param name="enforceStrictCountLimit">true si les équipes doivent avoir un nombre exact de joueurs actifs</param>
        internal void SetTeams(TeamCompositionData allyTeamComposition, TeamCompositionData enemyTeamComposition, bool enforceStrictCountLimit = true)
        {
            _enforceStrictCountLimit = enforceStrictCountLimit;
            _allyMainCompositionCount = allyTeamComposition.MainCharacters.Count;
            _enemyMainCompositionCount = enemyTeamComposition.MainCharacters.Count;
            AllyTeamComposition = allyTeamComposition;
            EnemyTeamComposition = enemyTeamComposition;
        }

        /// <summary>
        /// Echange 2 persos de place
        /// </summary>
        /// <param name="oldListIndex">L'ID du groupe du perso à déplacer</param>
        /// <param name="newListIndex">L'ID du groupe du perso cible</param>
        /// <param name="draggedSiblingIndex">L'ID du perso à déplacer</param>
        /// <param name="targetSiblingIndex">L'ID du perso cible</param>
        internal void SwapCharacters(int oldListIndex, int newListIndex, int draggedSiblingIndex, int targetSiblingIndex)
        {
            List<CharacterData> oldGroupList = oldListIndex == 0 ? AllyTeamComposition.MainCharacters :
                                               oldListIndex == 1 ? AllyTeamComposition.Substitutes :
                                               oldListIndex == 2 ? EnemyTeamComposition.MainCharacters :
                                               oldListIndex == 3 ? EnemyTeamComposition.Substitutes :
                                               null;
            List<CharacterData> newGroupList = newListIndex == 0 ? AllyTeamComposition.MainCharacters :
                                               newListIndex == 1 ? AllyTeamComposition.Substitutes :
                                               newListIndex == 2 ? EnemyTeamComposition.MainCharacters :
                                               newListIndex == 3 ? EnemyTeamComposition.Substitutes :
                                               null;

            (oldGroupList[draggedSiblingIndex], newGroupList[targetSiblingIndex]) = (newGroupList[targetSiblingIndex], oldGroupList[draggedSiblingIndex]);
        }

        /// <summary>
        /// Déplace un perso d'une liste à une autre
        /// </summary>
        /// <param name="oldListIndex">L'ID du groupe du perso à déplacer</param>
        /// <param name="newListIndex">L'ID du groupe du perso cible</param>
        /// <param name="siblingIndex">L'ID du perso à déplacer</param>
        internal void AddCharacterToList(int oldListIndex, int newListIndex, int siblingIndex)
        {
            List<CharacterData> oldGroupList = oldListIndex == 0 ? AllyTeamComposition.MainCharacters :
                                               oldListIndex == 1 ? AllyTeamComposition.Substitutes :
                                               oldListIndex == 2 ? EnemyTeamComposition.MainCharacters :
                                               oldListIndex == 3 ? EnemyTeamComposition.Substitutes :
                                               null;
            List<CharacterData> newGroupList = newListIndex == 0 ? AllyTeamComposition.MainCharacters :
                                               newListIndex == 1 ? AllyTeamComposition.Substitutes :
                                               newListIndex == 2 ? EnemyTeamComposition.MainCharacters :
                                               newListIndex == 3 ? EnemyTeamComposition.Substitutes :
                                               null;

            CharacterData character = oldGroupList[siblingIndex];
            oldGroupList.RemoveAt(siblingIndex);
            newGroupList.Add(character);
        }

        /// <summary>
        /// Fait tourner les membres de l'équipe ennemie.
        /// Le changement est fait aléatoirement par le jeu
        /// en s'adaptant à la formation du joueur
        /// ou en fonction de la progression dans l'histoire (à déterminer)
        /// </summary>
        internal void SwapEnemies()
        {
            //TAF : Faire la rotation
        }

        /// <summary>
        /// Vérifie que les équipes sont valides pour la 2nde mi-temps
        /// </summary>
        /// <param name="nbMaxAllies">Nb d'alliés max pouvant être actifs</param>
        /// <param name="nbMaxEnemies">Nb d'ennemis max pouvant être actifs</param>
        /// <returns>Un Code d'erreur si les équipes ne sont pas valides (-1 si aucune erreur)</returns>
        internal int CheckTeamCompositions(int nbMaxAllies, int nbMaxEnemies)
        {
            if (_enforceStrictCountLimit)
            {
                if (_allyMainCompositionCount != AllyTeamComposition.MainCharacters.Count)
                {
                    return 0;
                }
                if (_enemyMainCompositionCount != EnemyTeamComposition.MainCharacters.Count)
                {
                    return 1;
                }
            }
            else
            {
                if (AllyTeamComposition.MainCharacters.Count > nbMaxAllies)
                {
                    return 0;
                }
                if (EnemyTeamComposition.MainCharacters.Count > nbMaxEnemies)
                {
                    return 1;
                }
            }

            return -1;
        }

        #endregion
    }
}