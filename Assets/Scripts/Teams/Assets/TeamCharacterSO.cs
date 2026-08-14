using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Représente les caractéristiques d'un personnage
    /// utilisé lors d'un match (stats, apparence, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "New Team Character", menuName = "Scriptable Objects/Teams/Team Character")]
    public sealed class TeamCharacterSO : ScriptableObject
    {
        #region Inspecteur

        [Tooltip("Le nom du personnage")]
        public string Name;

        [Tooltip("L'apparence du personnage")]
        public MatchCharacterSkinData Appearance;

        [Tooltip("Les stats du personnage")]
        public CharacterStatsData Stats;

        [Tooltip("Courbe de progression du gain d'expérience du personnage")]
        public AnimationCurve ExpProgressCurve;

        #endregion

        #region Méthodes Unity

#if UNITY_EDITOR

        /// <summary>
        /// Appelée quand une valeur change dans l'inspecteur
        /// </summary>
        private void OnValidate()
        {
            Name = name;
            Stats.Level = math.clamp(Stats.Level, 1, Constants.MAX_LEVEL);
            Stats.Strength = math.clamp(Stats.Strength, 1, Stats.MaxStrength);
            Stats.Agility = math.clamp(Stats.Agility, 1, Stats.MaxAgility);
            Stats.Endurance = math.clamp(Stats.Endurance, 1, Stats.MaxEndurance);
            Stats.Affection = math.clamp(Stats.Affection, 1, Stats.MaxAffection);
        }

#endif

        #endregion
    }
}