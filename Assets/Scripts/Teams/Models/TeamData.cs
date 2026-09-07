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
        /// <summary>
        /// Le nom de l'équipe
        /// </summary>
        public string Name;

        /// <summary>
        /// Sprite du logo de l'équipe
        /// </summary>
        public Sprite LogoSprite;

        /// <summary>
        /// Couleur de l'équipe
        /// </summary>
        public Color Color;

        /// <summary>
        /// Material du ballon de l'équipe
        /// </summary>
        public Material BallMaterial;

        /// <summary>
        /// Material du ballon de l'équipe en mort subite
        /// </summary>
        public Material BallMaterialSuddenDeath;
    }
}