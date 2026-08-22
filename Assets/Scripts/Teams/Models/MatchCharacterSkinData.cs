using System;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Représente l'apparence du perso lors d'un match
    /// </summary>
    [Serializable]
    public struct MatchCharacterSkinData
    {
        /// <summary>
        /// Le modèle 3D du personnage
        /// </summary>
        public Mesh Mesh;

        /// <summary>
        /// Le material appliqué au modèle 3D du personnage
        /// </summary>
        public Material Material;
    }
}