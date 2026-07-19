using System;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Données de mouvement d'un personnage lors d'un match
    /// </summary>
    [Serializable]
    public struct MatchCharacterMovementData
    {
        #region Instance

        [Tooltip("Vitesse de mouvement")]
        public float MoveSpeed;

        [Tooltip("Vitesse d'esquive")]
        public float DodgeSpeed;

        [Tooltip("Intervalle de force de saut")]
        public Vector2 JumpForceInterval;

        [Tooltip("Intervalle de force de tir")]
        public Vector2 FireForceInterval;

        [Tooltip("Durée d'esquive")]
        public float DodgeDuration;

        [Tooltip("Durée de blocage")]
        public float BlockDuration;

        [Tooltip("Coût en énergie d'utilisation de l'esquive")]
        public float DodgeEnergyCost;

        [Tooltip("Coût en énergie d'utilisation du blocage")]
        public float BlockEnergyCost;

        [Tooltip("Coût en énergie de chargement du saut chaque frame")]
        public float JumpChargeEnergyCostPerFrame;

        [Tooltip("Coût en énergie de chargement du saut chaque frame")]
        public float FireChargeEnergyCostPerFrame;

        #endregion
    }
}