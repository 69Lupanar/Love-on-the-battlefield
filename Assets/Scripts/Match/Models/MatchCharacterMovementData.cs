using System;
using Assets.Scripts.Teams;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Données de mouvement d'un personnage lors d'un match
    /// </summary>
    [Serializable]
    public struct MatchCharacterMovementData
    {
        #region Inspecteur

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
        public float JumpChargeSpeed;

        [Tooltip("Coût en énergie de chargement du saut chaque frame")]
        public float FireChargeSpeed;

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="characterStats">Les stats générales d'un personnage</param>
        /// <param name="min">Valeurs minimales possibles</param>
        /// <param name="max">Valeurs maximales possibles</param>
        public MatchCharacterMovementData(CharacterStatsData characterStats, MatchCharacterMovementData min, MatchCharacterMovementData max)
        {
            // On fait un lerp entre les 2 valeurs possibles en fonction des stats du joueur.
            // On retire -1 à l'évaluation pour s'assurer qu'on tombe bien sur la valeur min
            // si une stat est à 1

            MoveSpeed = math.lerp(min.MoveSpeed, max.MoveSpeed, ((float)characterStats.Strength - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
            DodgeSpeed = math.lerp(min.DodgeSpeed, max.DodgeSpeed, ((float)characterStats.Agility - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
            JumpForceInterval = Vector2.Lerp(min.JumpForceInterval, max.JumpForceInterval, ((float)characterStats.Strength - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
            FireForceInterval = Vector2.Lerp(min.FireForceInterval, max.FireForceInterval, ((float)characterStats.Strength - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
            DodgeDuration = math.lerp(min.DodgeDuration, max.DodgeDuration, ((float)characterStats.Agility - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
            BlockDuration = math.lerp(min.BlockDuration, max.BlockDuration, ((float)characterStats.Agility - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
            DodgeEnergyCost = math.lerp(min.DodgeEnergyCost, max.DodgeEnergyCost, ((float)characterStats.Endurance - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
            BlockEnergyCost = math.lerp(min.BlockEnergyCost, max.BlockEnergyCost, ((float)characterStats.Endurance - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
            JumpChargeSpeed = math.lerp(min.JumpChargeSpeed, max.JumpChargeSpeed, ((float)characterStats.Endurance - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
            FireChargeSpeed = math.lerp(min.FireChargeSpeed, max.FireChargeSpeed, ((float)characterStats.Endurance - 1) / ((float)Constants.MAX_STAT_VALUE - 1));
        }

        #endregion
    }
}