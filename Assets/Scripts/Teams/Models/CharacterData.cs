using System;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Données d'un personnage
    /// </summary>
    [Serializable]
    public struct CharacterData
    {
        /// <summary>
        /// Le nom du personnage
        /// </summary>
        public string Name;

        /// <summary>
        /// L'apparence du personnage
        /// </summary>
        public MatchCharacterSkinData Appearance;

        /// <summary>
        /// Les stats du personnage
        /// </summary>
        public CharacterStatsData Stats;

        /// <summary>
        /// Courbe de progression du gain d'expérience du personnage
        /// </summary>
        public AnimationCurve ExpProgressCurve;
    }
}