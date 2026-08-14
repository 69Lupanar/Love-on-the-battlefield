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
        [Tooltip("Durée d'une moitié de partie en secondes")]
        internal int HalfDuration;

        [SerializeField]
        [Tooltip("Durée d'une manche en secondes")]
        internal int SetDuration;

        #endregion
    }
}