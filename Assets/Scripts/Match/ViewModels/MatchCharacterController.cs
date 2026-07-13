using Assets.Scripts.Match;
using UnityEngine;

/// <summary>
/// Gère les déplacements du personnage
/// </summary>
[RequireComponent(typeof(PlayerInput), typeof(AIInput))]
public class MatchCharacterController : MonoBehaviour
{
    #region Propriétés

    /// <summary>
    /// true si c'est un allié du joueur
    /// </summary>
    public bool IsAlly { get; set; }

    #endregion

    #region Inspecteur

    [SerializeField]
    [Tooltip("Commandes du joueur")]
    private PlayerInput _playerInput;

    [SerializeField]
    [Tooltip("Commandes de l'IA")]
    private AIInput _aiInput;

    [SerializeField]
    [Tooltip("Emplacement de la balle quand tenue par le joueur")]
    private Transform _ballHoldingPos;

    #endregion

    #region Instance

    /// <summary>
    /// Commandes actives du personnage
    /// </summary>
    private ICharacterInput _activeInput;

    #endregion

    #region Méthodes publiques

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
        if (enable)
        {
            _playerInput.Enable();
            _aiInput.Enable();
        }
        else
        {
            _playerInput.Disable();
            _aiInput.Disable();
        }
    }

    #endregion
}
