using Assets.Scripts.Teams;
using UnityEngine;

namespace Assets.Scripts.Match.Test
{
    /// <summary>
    /// AU lancement de la scène, assigne les équipes par défaut
    /// qui seront ensuite passées à la scène de match
    /// </summary>
    public class MatchSettingsSetTeamsTest : MonoBehaviour
    {
        /// <summary>
        /// Composition de l'équipe alliée
        /// </summary>
        [SerializeField]
        [Tooltip("Composition de l'équipe alliée")]
        private TeamRosterSO _allyTeam;

        /// <summary>
        /// Composition de l'équipe ennemie
        /// </summary>
        [SerializeField]
        [Tooltip("Composition de l'équipe ennemie")]
        private TeamRosterSO _enemyTeam;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            FindAnyObjectByType<MatchSettingsView>().SetTeams(_allyTeam, _enemyTeam);
        }
    }
}