using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère les déplacements du personnage
    /// </summary>
    internal sealed class MatchCharacterControllerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// true si c'est un allié du joueur
        /// </summary>
        internal bool IsAlly { get; set; }

        /// <summary>
        /// true si le perso porte un ballon
        /// </summary>
        internal bool IsHoldingABall { get; set; }

        /// <summary>
        /// true si le perso est éliminé
        /// </summary>
        internal bool IsEliminated { get; set; }

        /// <summary>
        /// Le dernier adversaire ciblé par le joueur
        /// </summary>
        internal int LastOpponentTargetIndex { get; set; }

        /// <summary>
        /// Energie du joueur
        /// </summary>
        internal float Energy { get; set; }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Réinitialise le perso pour une nouvelle manche
        /// </summary>
        internal void ResetPlayer()
        {
            IsEliminated = false;
            Energy = 1f;
            LastOpponentTargetIndex = -1;
        }

        /// <summary>
        /// Charge un tir
        /// </summary>
        /// <param name="fireChargeSpeed">Vitesse de charge du tir</param>
        /// <param name="deltaTime">Durée d'une frame</param>
        internal void ChargeShot(float fireChargeSpeed, float deltaTime)
        {
            if (Energy > 0f)
            {
                Energy -= deltaTime * fireChargeSpeed;
            }
        }

        /// <summary>
        /// Tire le ballon
        /// </summary>
        internal void Shoot()
        {
            IsHoldingABall = false;
            Energy = 1f;
        }

        /// <summary>
        /// Récupère le ballon
        /// </summary>
        /// <param name="ball">Le ballon</param>
        internal void PickUpBall()
        {
            IsHoldingABall = true;
        }

        #endregion
    }
}