using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Match
{
    /// <summary>
    /// Gère la création des joueurs
    /// </summary>
    internal sealed class MatchSpawnerViewModel : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// Les persos du joueur
        /// </summary>
        internal List<Transform> AlliesT { get; private set; } = new();

        /// <summary>
        /// Les persos ennemis
        /// </summary>
        internal List<Transform> EnemiesT { get; private set; } = new();

        /// <summary>
        /// Les ballons
        /// </summary>
        internal List<Transform> BallsT { get; private set; } = new();

        #endregion

        #region Inspecteur

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

        #region Méthodes publiques

        /// <summary>
        /// Désactive les joueurs et ballons actifs
        /// </summary>
        internal void DisableActivePlayersAndBalls()
        {
            for (int i = 0; i < AlliesT.Count; ++i)
            {
                AlliesT[i].SetParent(_inactiveAlliesParent);
            }

            for (int i = 0; i < EnemiesT.Count; ++i)
            {
                EnemiesT[i].SetParent(_inactiveEnemiesParent);
            }

            for (int i = 0; i < BallsT.Count; ++i)
            {
                BallsT[i].SetParent(_inactiveBallsParent);
            }
        }

        /// <summary>
        /// Instancie les joueurs et ballons
        /// </summary>
        internal void SpawnPlayersAndBalls(int nbAllies, int nbEnemies, int nbBalls)
        {
            AlliesT.Clear();
            EnemiesT.Clear();
            BallsT.Clear();
            Transform character;
            Transform ball;

            for (int i = 0; i < nbAllies; ++i)
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
                AlliesT.Add(character);
            }

            for (int i = 0; i < nbEnemies; ++i)
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
                EnemiesT.Add(character);
            }

            for (int i = 0; i < nbBalls; ++i)
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
                BallsT.Add(ball);
            }
        }

        /// <summary>
        /// Ramène les joueurs et ballons à leurs positions et rotations d'origine
        /// </summary>
        internal void ResetPlayersAndBallsPoses()
        {
            for (int i = 0; i < AlliesT.Count; ++i)
            {
                AlliesT[i].transform.position = GetSpawnPosition(_inactiveAlliesParent.position, i, AlliesT.Count);
            }

            for (int i = 0; i < EnemiesT.Count; ++i)
            {
                EnemiesT[i].transform.SetPositionAndRotation(GetSpawnPosition(_inactiveEnemiesParent.position, i, EnemiesT.Count), _inactiveEnemiesParent.rotation);
            }

            for (int i = 0; i < BallsT.Count; ++i)
            {
                BallsT[i].transform.SetPositionAndRotation(GetSpawnPosition(_inactiveBallsParent.position, i, BallsT.Count), _inactiveBallsParent.rotation);
            }
        }

        #endregion

        #region Méthodes privées

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