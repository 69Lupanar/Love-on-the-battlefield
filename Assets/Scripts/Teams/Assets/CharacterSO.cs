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
        #region Propriétés

        /// <summary>
        /// Les données du personnage
        /// </summary>
        [HideInInspector]
        public CharacterData Data;

        #endregion

        #region Inspecteur

        [Tooltip("Le modèle 3D du personnage")]
        public Mesh Mesh;

        [Tooltip("Le material appliqué au modèle 3D du personnage")]
        public Material Material;

        [Min(1)]
        [Tooltip("Niveau d'un personnage. Ses stats augmentent quand il gagne un niveau. Détermine aussi le niveau d'intelligence des IAs.")]
        public byte Level;

        [HideInInspector]
        [Tooltip("Expérience du personnage, gagnée en participant à des matchs où d'autres événements de l'histoire.")]
        public uint Experience;

        [Min(1)]
        [Tooltip("Force d'un personnage, représentant sa force de tir et de saut")]
        public byte Strength;

        [Min(1)]
        [Tooltip("Agilité d'un personnage, représentant sa capacité à esquiver et bloquer les tirs")]
        public byte Agility;

        [Min(1)]
        [Tooltip("Endurance d'un personnage, représentant sa capacité totale d'énergie")]
        public byte Endurance;

        [Min(1)]
        [Tooltip("Affection d'un personnage, représentant le taux de succès de son coup spécial ainsi que la vitesse d'exécution de ce dernier")]
        public byte Affection;

        [Min(1)]
        [Tooltip("Force max que peut atteindre le personnage")]
        public byte MaxStrength;

        [Min(1)]
        [Tooltip("Agilité max que peut atteindre le personnage")]
        public byte MaxAgility;

        [Min(1)]
        [Tooltip("Endurance max que peut atteindre le personnage")]
        public byte MaxEndurance;

        [Min(1)]
        [Tooltip("Affection max que peut atteindre le personnage")]
        public byte MaxAffection;

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
            Data.Name = name;
            Data.Appearance.Mesh = Mesh;
            Data.Appearance.Material = Material;

            Level = (byte)math.clamp(Level, 1, Constants.MAX_LEVEL);
            MaxStrength = (byte)math.clamp(MaxStrength, 1, Constants.MAX_STAT_VALUE);
            MaxAgility = (byte)math.clamp(MaxAgility, 1, Constants.MAX_STAT_VALUE);
            MaxEndurance = (byte)math.clamp(MaxEndurance, 1, Constants.MAX_STAT_VALUE);
            MaxAffection = (byte)math.clamp(MaxAffection, 1, Constants.MAX_STAT_VALUE);
            Strength = (byte)math.clamp(Strength, 1, MaxStrength);
            Agility = (byte)math.clamp(Agility, 1, MaxAgility);
            Endurance = (byte)math.clamp(Endurance, 1, MaxEndurance);
            Affection = (byte)math.clamp(Affection, 1, MaxAffection);

            Data.Stats.Level = Level;
            Data.Stats.Experience = DataExtensions.GetExpUntilNextLevel(Level, ExpProgressCurve);
            Data.Stats.MaxStrength = MaxStrength;
            Data.Stats.MaxAgility = MaxAgility;
            Data.Stats.MaxEndurance = MaxEndurance;
            Data.Stats.MaxAffection = MaxAffection;
            Data.Stats.Strength = Strength;
            Data.Stats.Agility = Agility;
            Data.Stats.Endurance = Endurance;
            Data.Stats.Affection = Affection;
        }

#endif

        #endregion
    }
}