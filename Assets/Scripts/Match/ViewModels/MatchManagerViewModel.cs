using Assets.Scripts.Teams;
using Unity.Mathematics;
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
        /// true si la manche en cours utilise la mort subite
        /// </summary>
        internal bool SuddenDeath { get; private set; }

        /// <summary>
        /// Paramètres du match en cours
        /// </summary>
        internal MatchSettingsData MatchSettingsData { get; set; }

        /// <summary>
        /// Composition de l'équipe alliée, gardée en mémoire pour le changement de joueurs à la mi-temps
        /// </summary>
        internal TeamCompositionData AllyTeamComposition { get; set; }

        /// <summary>
        /// Composition de l'équipe ennemie, gardée en mémoire pour le changement de joueurs à la mi-temps
        /// </summary>
        internal TeamCompositionData EnemyTeamComposition { get; set; }

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
        /// Nb d'alliés encore en jeu
        /// </summary>
        internal int NbLiveAllies { get; private set; }

        /// <summary>
        /// Nb d'ennemis encore en jeu
        /// </summary>
        internal int NbLiveEnemies { get; private set; }

        /// <summary>
        /// Temps écoulé depuis le début de la partie
        /// </summary>
        internal int MatchTimer { get; private set; }

        /// <summary>
        /// Durée totale du match
        /// </summary>
        internal int MatchDuration => MatchSettingsData.HalfDuration * 2;

        /// <summary>
        /// Temps écoulé depuis le début de la manche
        /// </summary>
        internal int SetDuration { get; private set; }

        /// <summary>
        /// Temps écoulé depuis le début des prolongations
        /// </summary>
        internal int OvertimeDuration => math.max(0, MatchTimer - MatchDuration);

        /// <summary>
        /// Nb de sets démarrés
        /// </summary>
        internal int CurrentSet { get; private set; }

        /// <summary>
        /// Score des alliés
        /// </summary>
        internal int AlliesScore { get; private set; }

        /// <summary>
        /// Score des ennemis
        /// </summary>
        internal int EnemiesScore { get; private set; }

        #endregion

        #region Méthodes internes

        /// <summary>
        /// Démarre une nouvelle partie
        /// </summary>
        /// <param name="matchSettings">Paramètres d'un match</param>
        /// <param name="allyTeamComposition">Composition de joueurs de l'équipe alliée</param>
        /// <param name="enemyTeamComposition">Composition de joueurs de l'équipe ennemie</param>
        internal void StartNewMatch(MatchSettingsData matchSettings, TeamRosterSO allyTeamComposition, TeamRosterSO enemyTeamComposition)
        {
            MatchTimer = 0;
            CurrentSet = 0;
            AlliesScore = 0;
            EnemiesScore = 0;
            MatchIsOngoing = true;
            MatchSettingsData = matchSettings;
            AllyTeamComposition = allyTeamComposition.Roster;
            EnemyTeamComposition = enemyTeamComposition.Roster;
        }

        /// <summary>
        /// Démarre une nouvelle manche
        /// </summary>
        /// <param name="suddenDeath">true si la manche doit se jouer en mort subite</param>
        internal void StartNewSet(bool suddenDeath = false)
        {
            SuddenDeath = suddenDeath;
            SetDuration = 0;
            ++CurrentSet;
            NbLiveAllies = NbAllies;
            NbLiveEnemies = NbEnemies;
        }

        /// <summary>
        /// Pause la partie
        /// </summary>
        internal void PauseMatch()
        {
            MatchIsOngoing = false;
        }

        /// <summary>
        /// Reprend la partie
        /// </summary>
        internal void ResumeMatch()
        {
            MatchIsOngoing = true;
        }

        /// <summary>
        /// Appelé à chaque seconde
        /// </summary>
        internal void OnTick()
        {
            ++MatchTimer;
            ++SetDuration;
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

        /// <summary>
        /// Vérifie les conditions de victoire pour déterminer l'équipe remportant la manche
        /// </summary>
        /// <returns>ID de l'équipe gagnante de la manche</returns>
        internal TeamID GetSetWinningTeam()
        {
            if (NbLiveAllies == NbEnemies)
                return TeamID.None;
            else if (NbLiveAllies > NbEnemies)
                return TeamID.Ally;
            else
                return TeamID.Enemy;
        }

        /// <summary>
        /// Vérifie les conditions de victoire pour déterminer l'équipe remportant la partie
        /// </summary>
        /// <returns>ID de l'équipe gagnante de la partie</returns>
        internal TeamID GetMatchWinningTeam()
        {
            if (AlliesScore == EnemiesScore)
                return TeamID.None;
            else if (AlliesScore > EnemiesScore)
                return TeamID.Ally;
            else
                return TeamID.Enemy;
        }

        /// <summary>
        /// Met fin à la manche
        /// </summary>
        /// <param name="setWinningTeamID">ID de l'équipe gagnante de la manche</param>
        internal void EndSet(TeamID setWinningTeamID)
        {
            switch (setWinningTeamID)
            {
                case TeamID.Ally:
                    ++AlliesScore;
                    break;
                case TeamID.Enemy:
                    ++EnemiesScore;
                    break;
            }
        }

        /// <summary>
        /// Met fin à la partie
        /// </summary>
        internal void EndMatch()
        {
            MatchIsOngoing = false;
        }

        #endregion
    }
}