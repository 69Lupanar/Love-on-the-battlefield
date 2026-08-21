using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Teams
{
    /// <summary>
    /// Réprésente une combinaison de joueurs spécifiques (ppaux et remplaçants).
    /// Cela nous permet d'enregistrer une composition pour une équipe
    /// afin d'éviter de la renseigner à chaque fois
    /// </summary>
    [Serializable]
    public struct TeamCompositionData
    {
        [SerializeField]
        [Tooltip("Joueurs ppaux de l'équipe")]
        public List<CharacterSO> MainCharacters;

        [SerializeField]
        [Tooltip("Joueurs remplaçants de l'équipe")]
        public List<CharacterSO> Substitutes;
    }
}