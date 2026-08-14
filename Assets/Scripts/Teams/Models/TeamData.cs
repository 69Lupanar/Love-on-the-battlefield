using System;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Données d'une équipe
    /// </summary>
    [Serializable]
    public struct TeamData
    {
        [Tooltip("Le nom de l'équipe")]
        public string Name;

        [Tooltip("Sprite du logo de l'équipe")]
        public Sprite LogoSprite;

        [Tooltip("Couleur de l'équipe")]
        public Color Color;
    }
}