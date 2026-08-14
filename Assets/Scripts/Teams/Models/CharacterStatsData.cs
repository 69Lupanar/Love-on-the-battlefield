using System;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Représente les stats de chaque personnage jouable,
    /// convertis en données de mouvement lors d'un match
    /// </summary>
    [Serializable]
    public struct CharacterStatsData
    {
        [Min(1)]
        [Tooltip("Niveau d'un personnage. Ses stats augmentent quand il gagne un niveau. Détermine aussi le niveau d'intelligence des IAs.")]
        public int Level;

        [HideInInspector]
        [Tooltip("Expérience du personnage, gagnée en participant à des matchs où d'autres événements de l'histoire.")]
        public int Experience;

        [Min(1)]
        [Tooltip("Force d'un personnage, représentant sa force de tir et de saut")]
        public int Strength;

        [Min(1)]
        [Tooltip("Agilité d'un personnage, représentant sa capacité à esquiver et bloquer les tirs")]
        public int Agility;

        [Min(1)]
        [Tooltip("Endurance d'un personnage, représentant sa capacité totale d'énergie")]
        public int Endurance;

        [Min(1)]
        [Tooltip("Affection d'un personnage, représentant le taux de succès de son coup spécial ainsi que la vitesse d'exécution de ce dernier")]
        public int Affection;

        [Min(1)]
        [Tooltip("Force max que peut atteindre le personnage")]
        public int MaxStrength;

        [Min(1)]
        [Tooltip("Agilité max que peut atteindre le personnage")]
        public int MaxAgility;

        [Min(1)]
        [Tooltip("Endurance max que peut atteindre le personnage")]
        public int MaxEndurance;

        [Min(1)]
        [Tooltip("Affection max que peut atteindre le personnage")]
        public int MaxAffection;

    }
}