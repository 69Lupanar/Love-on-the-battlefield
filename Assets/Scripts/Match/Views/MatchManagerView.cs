using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère le déroulement d'un match de dodgeball
    /// </summary>
    [RequireComponent(typeof(MatchManagerViewModel))]
    public class MatchManagerView : MonoBehaviour
    {
        #region Inspecteur
        [Header("UI")]
        [Space(10)]

        [SerializeField]
        [Tooltip("InputField du nb d'alliés")]
        private TMP_InputField _nbAlliesField;

        [SerializeField]
        [Tooltip("InputField du nb d'ennemis")]
        private TMP_InputField _nbEnemiesField;

        [SerializeField]
        [Tooltip("InputField du nb de ballons")]
        private TMP_InputField _nbBallsField;

        [Space(10)]
        [Header("Spawner")]
        [Space(10)]

        [SerializeField]
        [Tooltip("Prefab des alliés dirigés par le joueur")]
        private GameObject _allyPrefab;

        [SerializeField]
        [Tooltip("Prefab des ennemis dirigés par l'IA")]
        private GameObject _enemyPrefab;

        [SerializeField]
        [Tooltip("Prefab des ballons")]
        private GameObject _ballPrefab;

        [SerializeField]
        [Tooltip("Point de placement des alliés en début de partie")]
        private Transform _inactiveAlliesParent;

        [SerializeField]
        [Tooltip("Point de placement des ennemis en début de partie")]
        private Transform _inactiveEnemiesParent;

        [SerializeField]
        [Tooltip("Point de placement des ballons en début de partie")]
        private Transform _inactiveBallsParent;

        [SerializeField]
        [Tooltip("Espacement entre les objets lors de leur création")]
        private float _spawnSpacing = 1.5f;

        #endregion

        #region Instance

        /// <summary>
        /// Le ViewModel
        /// </summary>
        private MatchManagerViewModel _vm;

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        public Transform[] AlliesT { get; private set; }

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        public Transform[] EnemiesT { get; private set; }

        /// <summary>
        /// Les ballons
        /// </summary>
        public Transform[] BallsT { get; private set; }

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Awake()
        {
            _vm = GetComponent<MatchManagerViewModel>();
            _nbAlliesField.SetTextWithoutNotify(_vm.NbAllies.ToString());
            _nbEnemiesField.SetTextWithoutNotify(_vm.NbEnemies.ToString());
            _nbBallsField.SetTextWithoutNotify(_vm.NbBalls.ToString());
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbAlliesInputFieldEndEdit(string str)
        {
            _vm.NbAllies = math.max(1, int.Parse(str));
            _nbAlliesField.SetTextWithoutNotify(_vm.NbAllies.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbEnemiesInputFieldEndEdit(string str)
        {
            _vm.NbEnemies = math.max(1, int.Parse(str));
            _nbEnemiesField.SetTextWithoutNotify(_vm.NbEnemies.ToString());
        }

        /// <summary>
        /// Appelée par l'InputField
        /// </summary>
        public void OnNbBallsInputFieldEndEdit(string str)
        {
            _vm.NbBalls = math.max(1, int.Parse(str));
            _nbBallsField.SetTextWithoutNotify(_vm.NbBalls.ToString());
        }

        /// <summary>
        /// Appelée par le bouton Start New Match
        /// </summary>
        public void OnStartNewMatchBtnClick()
        {
            // Nettoie la scène si on a déjà lancé un match

            if (AlliesT != null)
            {
                _vm.DisableActivePlayersAndBalls();
                DisableActivePlayersAndBalls();
            }

            AllocateArrays();
            SpawnPlayersAndBalls();
            ResetPlayersAndBallsPoses();

            _vm.AllocateArrays();
            _vm.SetPlayersAndBalls(AlliesT, EnemiesT, BallsT);
            _vm.SetActivePlayer(_vm.ActivePlayerIndex);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Désactive les joueurs et ballons actifs
        /// </summary>
        private void DisableActivePlayersAndBalls()
        {
            for (int i = 0; i < AlliesT.Length; ++i)
            {
                AlliesT[i].SetParent(_inactiveAlliesParent);
            }

            for (int i = 0; i < EnemiesT.Length; ++i)
            {
                EnemiesT[i].SetParent(_inactiveEnemiesParent);
            }

            for (int i = 0; i < BallsT.Length; ++i)
            {
                BallsT[i].SetParent(_inactiveBallsParent);
            }
        }

        /// <summary>
        /// Alloue les tables pour les persos et les ballons
        /// </summary>
        public void AllocateArrays()
        {
            AlliesT = new Transform[_vm.NbAllies];
            EnemiesT = new Transform[_vm.NbEnemies];
            BallsT = new Transform[_vm.NbBalls];
        }

        /// <summary>
        /// Instancie les joueurs et ballons
        /// </summary>
        private void SpawnPlayersAndBalls()
        {
            Transform character;
            Transform ball;

            for (int i = 0; i < _vm.NbAllies; ++i)
            {
                if (_inactiveAlliesParent.childCount > 0)
                {
                    character = _inactiveAlliesParent.GetChild(0);
                }
                else
                {
                    character = Instantiate(_allyPrefab, transform).transform;
                }

                character.SetParent(transform);
                AlliesT[i] = character;
            }

            for (int i = 0; i < _vm.NbEnemies; ++i)
            {
                if (_inactiveEnemiesParent.childCount > 0)
                {
                    character = _inactiveEnemiesParent.GetChild(0);
                }
                else
                {
                    character = Instantiate(_enemyPrefab, transform).transform;
                }

                character.SetParent(transform);
                EnemiesT[i] = character;
            }

            for (int i = 0; i < _vm.NbBalls; ++i)
            {
                if (_inactiveBallsParent.childCount > 0)
                {
                    ball = _inactiveBallsParent.GetChild(0);
                }
                else
                {
                    ball = Instantiate(_ballPrefab, transform).transform;
                }

                ball.SetParent(transform);
                BallsT[i] = ball;
            }
        }

        /// <summary>
        /// Ramène les joueurs et ballons à leurs positions et rotations d'origine
        /// </summary>
        private void ResetPlayersAndBallsPoses()
        {
            for (int i = 0; i < _vm.NbAllies; ++i)
            {
                AlliesT[i].transform.SetPositionAndRotation(GetSpawnPosition(_inactiveAlliesParent.position, i, _vm.NbAllies), _inactiveAlliesParent.rotation);
            }

            for (int i = 0; i < _vm.NbEnemies; ++i)
            {
                EnemiesT[i].transform.SetPositionAndRotation(GetSpawnPosition(_inactiveEnemiesParent.position, i, _vm.NbEnemies), _inactiveEnemiesParent.rotation);
            }

            for (int i = 0; i < _vm.NbBalls; ++i)
            {
                BallsT[i].transform.SetPositionAndRotation(GetSpawnPosition(_inactiveBallsParent.position, i, _vm.NbBalls), _inactiveBallsParent.rotation);
            }
        }

        /// <summary>
        /// Calcule la position où placer l'objet
        /// </summary>
        /// <param name="origin">La position d'origine</param>
        /// <param name="i">Index de l'élément actuel</param>
        /// <param name="max">Nb d'objets total à créer</param>
        /// <returns>La position de l'objet dans la scène</returns>
        private Vector3 GetSpawnPosition(Vector3 origin, int i, int max)
        {
            float startPosX = -Mathf.Floor(max / 2f) * _spawnSpacing + (max % 2 == 0 ? _spawnSpacing / 2f : 0f) + _spawnSpacing * i;
            origin.x += startPosX;
            return origin;
        }

        #endregion
    }
}