using System;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Représente les stats de chaque personnage jouable,
    /// convertis en données de mouvement lors d'un match
    /// </summary>
    [Serializable]
    public struct CharacterStatsData
    {
        /// <summary>
        /// Niveau d'un personnage. Ses stats augmentent quand il gagne un niveau. Détermine aussi le niveau d'intelligence des IAs.
        /// </summary>
        public byte Level;

        /// <summary>
        /// Expérience du personnage, gagnée en participant à des matchs où d'autres événements de l'histoire.
        /// </summary>
        public uint Experience;

        /// <summary>
        /// Force d'un personnage, représentant sa force de tir et de saut
        /// </summary>
        public byte Strength;

        /// <summary>
        /// Agilité d'un personnage, représentant sa capacité à esquiver et bloquer les tirs
        /// </summary>
        public byte Agility;

        /// <summary>
        /// Endurance d'un personnage, représentant sa capacité totale d'énergie
        /// </summary>
        public byte Endurance;

        /// <summary>
        /// Affection d'un personnage, représentant le taux de succès de son coup spécial ainsi que la vitesse d'exécution de ce dernier
        /// </summary>
        public byte Affection;

        /// <summary>
        /// Force max que peut atteindre le personnage
        /// </summary>
        public byte MaxStrength;

        /// <summary>
        /// Agilité max que peut atteindre le personnage
        /// </summary>
        public byte MaxAgility;

        /// <summary>
        /// Endurance max que peut atteindre le personnage
        /// </summary>
        public byte MaxEndurance;

        /// <summary>
        /// Affection max que peut atteindre le personnage
        /// </summary>
        public byte MaxAffection;
    }
}