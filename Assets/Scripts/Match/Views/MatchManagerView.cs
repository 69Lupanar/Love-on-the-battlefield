using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match
    /// </summary>
    [RequireComponent(typeof(MatchManagerViewModel))]
    public class MatchManagerView : MonoBehaviour
    {
        #region Instance

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchManagerViewModel _vm;

        /// <summary>
        /// Le spawner des joueurs et ballons
        /// </summary>
        private MatchSpawnerView _spawner;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            _vm = GetComponent<MatchManagerViewModel>();
            _spawner = FindAnyObjectByType<MatchSpawnerView>();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Appelée par le bouton Start New Match
        /// </summary>
        public void OnStartNewMatchBtnClick()
        {
        }

        #endregion

        #region Méthodes privées

        #endregion
    }
}