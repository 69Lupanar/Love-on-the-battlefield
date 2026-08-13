using System;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Paramètres d'un match
    /// </summary>
    [Serializable]
    public struct MatchSettingsData
    {
        #region Inspecteur

        [SerializeField]
        [Tooltip("Nombre d'alliés à instancier")]
        internal int NbAllies;

        [SerializeField]
        [Tooltip("Nombre d'ennemis à instancier")]
        internal int NbEnemies;

        [SerializeField]
        [Tooltip("Nombre de ballons à instancier")]
        internal int NbBalls;

        [SerializeField]
        [Tooltip("Durée d'une partie en secondes")]
        internal int MatchDuration;

        [SerializeField]
        [Tooltip("Durée d'une manche en secondes")]
        internal int SetDuration;

        #endregion
    }
}