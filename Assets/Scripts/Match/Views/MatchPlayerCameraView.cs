using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déplacement de la caméra
    /// </summary>
    internal sealed class MatchPlayerCameraView : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// true si aucun match n'est en cours
        /// </summary>
        internal bool MatchIsOver { get; set; } = true;

        #endregion

        #region Inspecteur

        [SerializeField]
        [Tooltip("Position que doit suivre la caméra")]
        private Transform _cameraTarget;

        #endregion

        #region Instance

        /// <summary>
        /// Le MatchManagerViewModel
        /// </summary>
        private MatchManagerViewModel _managerVM;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _managerVM = FindAnyObjectByType<MatchManagerViewModel>();
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (!MatchIsOver)
            {
                MatchCharacterController activePlayer = _managerVM.Allies[_managerVM.ActivePlayerIndex];

                if (_managerVM.CurAllyTargetForSwapIndex > -1)
                {
                    MatchCharacterController curAllyTargetForSwap = _managerVM.Allies[_managerVM.CurAllyTargetForSwapIndex];

                    Vector3 total = activePlayer.transform.position + curAllyTargetForSwap.transform.position;
                    total /= 2f;
                    _cameraTarget.position = new Vector3(total.x, _cameraTarget.position.y, total.z);
                }
                else
                {
                    _cameraTarget.position = activePlayer.transform.position;
                }
            }
        }

        #endregion
    }
}