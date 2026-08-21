using Assets.Scripts.Teams;
using UnityEngine;

/// <summary>
/// Représente une équipe de joueurs.
/// On sépare les SOs de l'équipe et des joueurs pour nous permettre
/// d'assigner différents joueurs à de mêmes équipes
/// et d'avoir différentes compositions en fonction de la progression de l'histoire.
/// </summary>
[CreateAssetMenu(fileName = "New Team Roster", menuName = "Scriptable Objects/Teams/Team Roster")]
public class TeamRosterSO : ScriptableObject
{
    #region Inspecteur

    [Tooltip("Les données de l'équipe")]
    public TeamSO Team;

    [Tooltip("Les joueurs de l'équipe")]
    public TeamCompositionData Roster;

    #endregion
}
