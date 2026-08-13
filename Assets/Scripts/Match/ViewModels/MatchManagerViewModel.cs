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
        /// true si un match est en cours
        /// </summary>
        internal bool MatchIsOngoing { get; private set; }

        /// <summary>
        /// Paramètres du match en cours
        /// </summary>
        internal MatchSettingsData MatchSettingsData { get; set; }

        /// <summary>
        /// Nombre d'alliés à instancier
        /// </summary>
        internal int NbAllies => MatchSettingsData.NbAllies;

        /// <summary>
        /// Nombre d'ennemis à instancier
        /// </summary>
        internal int NbEnemies => MatchSettingsData.NbAllies;

        /// <summary>
        /// Nombre de ballons à instancier
        /// </summary>
        internal int NbBalls => MatchSettingsData.NbAllies;

        /// <summary>
        ///  Nb d'alliés encore en jeu
        /// </summary>
        internal int NbLiveAllies { get; private set; }

        /// <summary>
        ///  Nb d'ennemis encore en jeu
        /// </summary>
        internal int NbLiveEnemies { get; private set; }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Démarre une nouvelle partie
        /// </summary>
        /// <param name="matchSettings">Paramètres d'un match</param>
        internal void StartNewMatch(MatchSettingsData matchSettings)
        {
            MatchIsOngoing = true;
            MatchSettingsData = matchSettings;
        }

        /// <summary>
        /// Démarre une nouvelle manche
        /// </summary>
        internal void StartNewSet()
        {
            NbLiveAllies = NbAllies;
            NbLiveEnemies = NbEnemies;
        }

        /// <summary>
        /// Appelée quand un perso est éliminé
        /// </summary>
        /// <param name="characterIsAlly">true si c'est un allié</param>
        internal void OnCharacterEliminated(bool characterIsAlly)
        {
            if (characterIsAlly)
            {
                --NbLiveAllies;
            }
            else
            {
                --NbLiveEnemies;
            }
        }

        #endregion
    }
}