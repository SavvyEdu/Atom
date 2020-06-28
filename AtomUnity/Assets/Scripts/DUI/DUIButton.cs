using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DUI
{
    [RequireComponent(typeof(Collider))]
    public class DUIButton : MonoBehaviour
    {
        /// <summary>
        /// sends out events when mouse enters, clicks, and exits DUIanchor bounds
        /// </summary>

        public UnityEvent OnClick; //called when the mouse clicks while in bounds
        public UnityAction<Vector2> OnDrag;
    }
}
