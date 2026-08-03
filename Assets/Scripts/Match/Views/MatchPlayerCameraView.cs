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
        private MatchPlayerControllerView _playerV;

        /// <summary>
        /// Le MatchManagerViewModel
        /// </summary>
        private MatchManagerView _matchV;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _playerV = FindAnyObjectByType<MatchPlayerControllerView>();
            _matchV = FindAnyObjectByType<MatchManagerView>();
        }

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (!_matchV.MatchIsOver)
            {
                MatchCharacterControllerView activePlayer = _playerV.Allies[_playerV.ActivePlayerIndex];

                if (_playerV.CurAllyTargetForSwapIndex > -1)
                {
                    MatchCharacterControllerView curAllyTargetForSwap = _playerV.Allies[_playerV.CurAllyTargetForSwapIndex];

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