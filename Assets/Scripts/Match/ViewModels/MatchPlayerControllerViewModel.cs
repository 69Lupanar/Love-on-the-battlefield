using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le comportement des joueurs et ballons
    /// </summary>
    internal sealed class MatchPlayerControllerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        internal List<MatchCharacterData> Allies { get; private set; } = new();

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal List<MatchCharacterData> Enemies { get; private set; } = new();

        /// <summary>
        /// L'ID du perso contrôlé par le joueur
        /// </summary>
        internal int ActivePlayerIndex { get; set; }

        /// <summary>
        /// L'ID du perso allié actuellement sélectionné comme cible
        /// lors du changement de contrôle
        /// (-1 si aucun)
        /// </summary>
        internal int CurAllyTargetForSwapIndex { get; set; }

        /// <summary>
        /// true si le joueur est en cours de changement de personnage
        /// </summary>
        internal bool IsSwappingCharacter { get; set; }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Crée les données des entités en jeu
        /// </summary>
        /// <param name="nbAllies">Nb d'alliés à instancier</param>
        /// <param name="nbEnemies">Nb d'ennemis à instancier</param>
        internal void SetEntities(int nbAllies, int nbEnemies)
        {
            Allies.Clear();
            Enemies.Clear();

            for (int i = 0; i < nbAllies; ++i)
            {
                Allies.Add(new MatchCharacterData());
            }

            for (int i = 0; i < nbEnemies; ++i)
            {
                Enemies.Add(new MatchCharacterData());
            }

        }

        /// <summary>
        /// Réinitialise les données des persos pour une nouvelle manche
        /// </summary>
        internal void ResetController()
        {
            CancelSwap();

            for (int i = 0; i < Allies.Count; ++i)
            {
                Allies[i] = ResetPlayer(Allies[i]);
            }

            for (int i = 0; i < Enemies.Count; ++i)
            {
                Enemies[i] = ResetPlayer(Enemies[i]);
            }
        }

        /// <summary>
        /// Obtient la direction de l'axe vers où détecter un allié pour le changement de contrôle
        /// </summary>
        /// <param name="activePlayerPos">Position du joueur actif</param>
        /// <param name="swapCharacterAxis">La direction de la ligne sur l'axe XZ</param>
        /// <param name="origin">Début du rayon</param>
        /// <param name="dir">Direction du rayon</param>
        internal void GetSwapDirectionXZ(Vector3 activePlayerPos, Vector2 swapCharacterAxis, out Vector3 origin, out Vector3 dir)
        {
            origin = activePlayerPos + Vector3.up;
            dir = new Vector3(swapCharacterAxis.x, 0f, swapCharacterAxis.y);
        }

        /// <summary>
        /// Echange le contrôle d'alliés entre l'IA et le joueur
        /// </summary>
        /// <param name="newActivePlayerIndex">Index du nouveau joueur actif (-1 si aucun)</param>
        internal void SwapControl(int newActivePlayerIndex)
        {
            // On passe le contrôle à l'allié sélectionné
            IsSwappingCharacter = false;

            if (newActivePlayerIndex > -1)
            {
                ActivePlayerIndex = newActivePlayerIndex;
                CurAllyTargetForSwapIndex = -1;
            }
        }

        /// <summary>
        /// Annule le changement de perso
        /// </summary>
        internal void CancelSwap()
        {
            IsSwappingCharacter = false;
            CurAllyTargetForSwapIndex = -1;
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Réinitialise les données du perso pour une nouvelle manche
        /// </summary>
        private MatchCharacterData ResetPlayer(MatchCharacterData matchCharacterData)
        {
            return matchCharacterData;
        }

        #endregion
    }
}