using System;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Données d'un personnage
    /// </summary>
    [Serializable]
    public class CharacterData
    {
        [Tooltip("Le nom du personnage")]
        public string Name;

        [Tooltip("L'apparence du personnage")]
        public MatchCharacterSkinData Appearance;

        [Tooltip("Les stats du personnage")]
        public CharacterStatsData Stats;

        [Tooltip("Courbe de progression du gain d'expérience du personnage")]
        public AnimationCurve ExpProgressCurve;
    }
}