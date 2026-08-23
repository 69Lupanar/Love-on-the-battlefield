using Assets.Scripts.Teams;
using UnityEngine;

/// <summary>
/// Représente une équipe de joueurs
/// </summary>
[CreateAssetMenu(fileName = "New Team", menuName = "Scriptable Objects/Teams/Team")]
public class TeamSO : ScriptableObject
{
    #region Propriétés

    /// <summary>
    /// Les données de l'équipe
    /// </summary>
    [HideInInspector]
    public TeamData Data;

    #endregion

    #region Inspecteur

    [SerializeField]
    [Tooltip("Sprite du logo de l'équipe")]
    private Sprite LogoSprite;

    [SerializeField]
    [Tooltip("Couleur de l'équipe")]
    private Color Color;

    #endregion

    #region Méthodes Unity

#if UNITY_EDITOR

    /// <summary>
    /// Appelée quand une valeur change dans l'inspecteur
    /// </summary>
    private void OnValidate()
    {
        Data.Name = name;
        Data.LogoSprite = LogoSprite;
        Data.Color = Color;
    }

#endif

    #endregion
}
