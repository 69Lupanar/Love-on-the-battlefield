using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Utilities.Views
{
    /// <summary>
    /// Elément d'UI pouvant être glissé et déposé par l'utilisateur
    /// </summary>
    public abstract class DraggableUIElement : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler
    {
        #region Evénements

        /// <summary>
        /// Appelée quand on commence à glisser l'objet
        /// </summary>
        public EventHandler OnDragStartedEvent { get; set; }

        /// <summary>
        /// Appelée quand on dépose l'objet
        /// </summary>
        public EventHandler OnDroppedEvent { get; set; }

        #endregion

        #region Propriétés

        /// <summary>
        /// Le dernier parent de cet objet avant d'être déplacé
        /// </summary>
        public Transform LastParent { get; set; }

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            LastParent = transform.parent;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        ///  Appelée quand on commence à glisser l'objet
        /// </summary>
        /// <param name="_"></param>
        public void OnPointerDown(PointerEventData _)
        {
            OnDragStartedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///  Appelée quand on glisse l'objet
        /// </summary>
        /// <param name="eventData">Contexte</param>
        public void OnPointerMove(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        /// <summary>
        ///  Appelée quand on dépose l'objet
        /// </summary>
        /// <param name="_"></param>
        public void OnPointerUp(PointerEventData _)
        {
            OnDroppedEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}