using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Représente les données d'un personnage (stats, apparence, etc.)
    /// </summary>
    [CreateAssetMenu(fileName = "New Character", menuName = "Scriptable Objects/Teams/Character")]
    public sealed class CharacterSO : ScriptableObject
    {
        #region Inspecteur

        [Tooltip("Les données du personnage")]
        public CharacterData Data;

        #endregion

        #region Méthodes Unity

#if UNITY_EDITOR

        /// <summary>
        /// Appelée quand une valeur change dans l'inspecteur
        /// </summary>
        private void OnValidate()
        {
            Data.Name = name;
            Data.Stats.Level = math.clamp(Data.Stats.Level, 1, Constants.MAX_LEVEL);
            Data.Stats.MaxStrength = math.clamp(Data.Stats.MaxStrength, 1, Constants.MAX_STAT_VALUE);
            Data.Stats.MaxAgility = math.clamp(Data.Stats.MaxAgility, 1, Constants.MAX_STAT_VALUE);
            Data.Stats.MaxEndurance = math.clamp(Data.Stats.MaxEndurance, 1, Constants.MAX_STAT_VALUE);
            Data.Stats.MaxAffection = math.clamp(Data.Stats.MaxAffection, 1, Constants.MAX_STAT_VALUE);
            Data.Stats.Strength = math.clamp(Data.Stats.Strength, 1, Data.Stats.MaxStrength);
            Data.Stats.Agility = math.clamp(Data.Stats.Agility, 1, Data.Stats.MaxAgility);
            Data.Stats.Endurance = math.clamp(Data.Stats.Endurance, 1, Data.Stats.MaxEndurance);
            Data.Stats.Affection = math.clamp(Data.Stats.Affection, 1, Data.Stats.MaxAffection);
        }

#endif

        #endregion
    }
}