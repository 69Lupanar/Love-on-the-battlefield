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
        [Tooltip("Le modèle 3D du personnage")]
        public Mesh Mesh;

        [Tooltip("Le material appliqué au modèle 3D du personnage")]
        public Material Material;
    }
}