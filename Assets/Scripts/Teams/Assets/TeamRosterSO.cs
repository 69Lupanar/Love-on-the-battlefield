using Assets.Scripts.Teams;
using UnityEngine;

/// <summary>
/// Représente une équipe de joueurs
/// </summary>
[CreateAssetMenu(fileName = "New Team Roster", menuName = "Scriptable Objects/Teams/Team Roster")]
public class TeamRosterSO : ScriptableObject
{
    #region Inspecteur

    [Tooltip("Les données de l'équipe")]
    public TeamData Data;

    [Tooltip("Les personnages disponibles dans cette équipe")]
    public TeamCharacterSO[] Roster;

    #endregion

    #region Méthodes Unity

#if UNITY_EDITOR

    /// <summary>
    /// Appelée quand une valeur change dans l'inspecteur
    /// </summary>
    private void OnValidate()
    {
        Data.Name = name;
    }

#endif

    #endregion
}
