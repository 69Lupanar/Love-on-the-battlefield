using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Utilities.Views
{
    /// <summary>
    /// Elément d'UI pouvant être glissé et déposé par l'utilisateur
    /// </summary>
    public abstract class DraggableUIElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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

        /// <summary>
        /// Le dernier ID de cet objet avant d'être déplacé
        /// </summary>
        public int LastSiblingIndex { get; set; }

        #endregion

        #region Instance

        /// <summary>
        /// true si l'élément est manipulé
        /// </summary>
        protected bool _isDragging;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Init
        /// </summary>
        private void Start()
        {
            LastParent = transform.parent;
            LastSiblingIndex = transform.GetSiblingIndex();
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        ///  Appelée quand on commence à glisser l'objet
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            OnDragStartedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///  Appelée quand on glisse l'objet
        /// </summary>
        /// <param name="eventData">Contexte</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragging)
            {
                transform.position += new Vector3(eventData.delta.x, eventData.delta.y);
            }
        }

        /// <summary>
        ///  Appelée quand on dépose l'objet
        /// </summary>
        /// <param name="_"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            OnDroppedEvent?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}