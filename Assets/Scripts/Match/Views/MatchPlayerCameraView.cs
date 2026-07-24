using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déplacement de la caméra
    /// </summary>
    internal sealed class MatchPlayerCameraView : MonoBehaviour
    {
        #region Inspecteur

        [SerializeField]
        [Tooltip("Position que doit suivre la caméra")]
        private Transform _cameraTarget;

        #endregion

        #region Instance

        /// <summary>
        /// Le MatchPlayerControllerViewModel
        /// </summary>
        private MatchPlayerControllerViewModel _playerVM;

        /// <summary>
        /// Le MatchManagerViewModel
        /// </summary>
        private MatchManagerViewModel _matchVM;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _playerVM = FindAnyObjectByType<MatchPlayerControllerViewModel>();
            _matchVM = FindAnyObjectByType<MatchManagerViewModel>();
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (!_matchVM.MatchIsOver)
            {
                MatchCharacterController activePlayer = _playerVM.Allies[_playerVM.ActivePlayerIndex];

                if (_playerVM.CurAllyTargetForSwapIndex > -1)
                {
                    MatchCharacterController curAllyTargetForSwap = _playerVM.Allies[_playerVM.CurAllyTargetForSwapIndex];

                    Vector3 avg = (activePlayer.transform.position + curAllyTargetForSwap.transform.position) / 2f;
                    _cameraTarget.position = new Vector3(avg.x, _cameraTarget.position.y, avg.z);
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