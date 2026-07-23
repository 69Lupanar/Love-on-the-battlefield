using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match
    /// </summary>
    internal sealed class MatchManagerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// true si aucun match n'est en cours
        /// </summary>
        internal bool MatchIsOver { get; set; } = true;

        #endregion

        #region Inspecteur

        [field: SerializeField]
        [field: Tooltip("Nombre d'alliés à instancier")]
        internal int NbAllies { get; set; } = 6;

        [field: SerializeField]
        [field: Tooltip("Nombre d'ennemis à instancier")]
        internal int NbEnemies { get; set; } = 6;

        [field: SerializeField]
        [field: Tooltip("Nombre de ballons à instancier")]
        internal int NbBalls { get; set; } = 5;

        #endregion
    }
}