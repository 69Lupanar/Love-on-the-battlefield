using Assets.Scripts.Match;
using UnityEngine;

/// <summary>
/// Gère les déplacements du personnage
/// </summary>
[RequireComponent(typeof(AIInput), typeof(PlayerInput))]
public class MatchCharacterController : MonoBehaviour
{
    #region Propriétés

    /// <summary>
    /// true si c'est un allié du joueur
    /// </summary>
    public bool IsAlly { get; private set; }

    #endregion

    #region Inspecteur

    [SerializeField]
    [Tooltip("Emplacement de la balle quand tenue par le joueur")]
    private Transform _ballHoldingPos;

    #endregion

    #region Instance

    /// <summary>
    /// Commandes du joueur
    /// </summary>
    private ICharacterInput _playerInput;

    /// <summary>
    /// Commandes de l'IA
    /// </summary>
    private ICharacterInput _aiInput;

    /// <summary>
    /// Commandes actives du personnage
    /// </summary>
    private ICharacterInput _activeInput;

    #endregion

    #region Méthodes Unity

    /// <summary>
    /// init
    /// </summary>
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _aiInput = GetComponent<AIInput>();
    }

    #endregion

    #region Méthodes publiques

    /// <summary>
    /// Assigne le personnage à une équipe
    /// </summary>
    /// <param name="isAlly">true si c'est un allié du joueur</param>
    public void SetTeam(bool isAlly)
    {
        IsAlly = isAlly;
    }

    /// <summary>
    /// Donne le contrôle du perso au joueur
    /// </summary>
    public void GiveControlToPlayer()
    {
        _activeInput = _playerInput;
    }

    /// <summary>
    /// Donne le contrôle du perso à l'IA
    /// </summary>
    public void GiveControlToAI()
    {
        _activeInput = _aiInput;
    }

    /// <summary>
    /// Active ou non les commandes du personnages
    /// </summary>
    public void EnableInput(bool enable)
    {
        //TAF
    }

    #endregion
}
