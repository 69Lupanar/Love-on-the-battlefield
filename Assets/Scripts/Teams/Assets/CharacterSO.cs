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

        [SerializeField]
        [Tooltip("Le modèle 3D du personnage")]
        private Mesh Mesh;

        [SerializeField]
        [Tooltip("Le material appliqué au modèle 3D du personnage")]
        private Material Material;

        [SerializeField]
        [Min(1)]
        [Tooltip("Niveau d'un personnage. Ses stats augmentent quand il gagne un niveau. Détermine aussi le niveau d'intelligence des IAs.")]
        private int Level;

        [SerializeField]
        [HideInInspector]
        [Tooltip("Expérience du personnage, gagnée en participant à des matchs où d'autres événements de l'histoire.")]
        private int Experience;

        [SerializeField]
        [Min(1)]
        [Tooltip("Force d'un personnage, représentant sa force de tir et de saut")]
        private int Strength;

        [SerializeField]
        [Min(1)]
        [Tooltip("Agilité d'un personnage, représentant sa capacité à esquiver et bloquer les tirs")]
        private int Agility;

        [SerializeField]
        [Min(1)]
        [Tooltip("Endurance d'un personnage, représentant sa capacité totale d'énergie")]
        private int Endurance;

        [SerializeField]
        [Min(1)]
        [Tooltip("Affection d'un personnage, représentant le taux de succès de son coup spécial ainsi que la vitesse d'exécution de ce dernier")]
        private int Affection;

        [SerializeField]
        [Min(1)]
        [Tooltip("Force max que peut atteindre le personnage")]
        private int MaxStrength;

        [SerializeField]
        [Min(1)]
        [Tooltip("Agilité max que peut atteindre le personnage")]
        private int MaxAgility;

        [SerializeField]
        [Min(1)]
        [Tooltip("Endurance max que peut atteindre le personnage")]
        private int MaxEndurance;

        [SerializeField]
        [Min(1)]
        [Tooltip("Affection max que peut atteindre le personnage")]
        private int MaxAffection;

        [SerializeField]
        [Tooltip("Courbe de progression du gain d'expérience du personnage")]
        private AnimationCurve ExpProgressCurve;

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

            Level = math.clamp(Level, 1, TeamConstants.MAX_LEVEL);
            MaxStrength = math.clamp(MaxStrength, 1, TeamConstants.MAX_STAT_VALUE);
            MaxAgility = math.clamp(MaxAgility, 1, TeamConstants.MAX_STAT_VALUE);
            MaxEndurance = math.clamp(MaxEndurance, 1, TeamConstants.MAX_STAT_VALUE);
            MaxAffection = math.clamp(MaxAffection, 1, TeamConstants.MAX_STAT_VALUE);
            Strength = math.clamp(Strength, 1, MaxStrength);
            Agility = math.clamp(Agility, 1, MaxAgility);
            Endurance = math.clamp(Endurance, 1, MaxEndurance);
            Affection = math.clamp(Affection, 1, MaxAffection);

            Data.Stats.Level = (byte)Level;
            Data.Stats.Experience = DataExtensions.GetExpUntilNextLevel((uint)Level, ExpProgressCurve);
            Data.Stats.MaxStrength = (byte)MaxStrength;
            Data.Stats.MaxAgility = (byte)MaxAgility;
            Data.Stats.MaxEndurance = (byte)MaxEndurance;
            Data.Stats.MaxAffection = (byte)MaxAffection;
            Data.Stats.Strength = (byte)Strength;
            Data.Stats.Agility = (byte)Agility;
            Data.Stats.Endurance = (byte)Endurance;
            Data.Stats.Affection = (byte)Affection;
        }

#endif

        #endregion
    }
}