﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DUI
{
    public class DUIAnchor : MonoBehaviour
    {
        /// <summary>
        /// Anchors the UI element based on min and max proportion of the parent Anchor
        /// </summary>

        [SerializeField] private Vector2 min;
        [SerializeField] private Vector2 max;

        public Bounds Bounds { get; private set; }
        public Rect Rect { get { return new Rect(Bounds.center, Bounds.size); } }

        DUIAnchor parentAnchor;

        public Vector2[] MinMax
        {
            get { return new Vector2[] { min, max }; }
            set
            {
                min = value[0];
                max = value[1];
                if (parentAnchor == null)
                    SetPosition(new Bounds(Vector2.zero, new Vector2(DUI.cameraWidth, DUI.cameraHeight) * 2));
                else
                    SetPosition(parentAnchor.Bounds);
            }
        }

        private void Awake()
        {
            parentAnchor = transform.parent.GetComponent<DUIAnchor>();
        }

        private void Start()
        {
            //parent node starts the recursive call using the full camera space as bounds
            if (parentAnchor == null)
            {
                //SetPosition(new Bounds(Vector2.zero, new Vector2(DUI.cameraWidth, DUI.cameraHeight) * 2));
            }
        }

        /// <summary>
        /// sets the anchor bounds based on parent bounds
        /// </summary>
        /// <param name="r">parent bounds</param>
        public void SetPosition(Bounds r)
        {

            //calculate new bounds based on the min and max proportions
            Bounds = new Bounds(new Vector2(r.center.x + ((max.x + min.x - 1) / 2) * r.size.x,
                                             r.center.y + ((max.y + min.y - 1) / 2) * r.size.y),
                                new Vector2(r.size.x * Mathf.Abs(max.x - min.x),
                                             r.size.y * Mathf.Abs(max.y - min.y)));

            //move to the new center position
            transform.position = Bounds.center;

            //scale any UI sprites to match the bounds
            SpriteRenderer render = GetComponent<SpriteRenderer>();
            if (render != null)
            {
                render.size = Bounds.size;
            }

            //set the layout for this Anchor
            DUILayout layout = GetComponent<DUILayout>();
            if (layout)
            {
                layout.SetAnchors();
            }

            //recursively call set position on any child objects
            List<DUIAnchor> duias = new List<DUIAnchor>();
            for (int i = 0; i < transform.childCount; i++)
            {
                DUIAnchor a = transform.GetChild(i).GetComponent<DUIAnchor>();
                if(a != null)
                    duias.Add(a);
            }
            foreach (DUIAnchor duia in duias)
            {
                duia.SetPosition(Bounds);
            }
        }

        /// <summary>
        /// Set the new bounds
        /// </summary>
        /// <param name="min">bottom left proportion</param>
        /// <param name="max">top right proportion</param>
        public void SetMinMax(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// draw a cyan collider based on bounds
        /// </summary>
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Bounds.center, Bounds.size);
        }
    }
}
