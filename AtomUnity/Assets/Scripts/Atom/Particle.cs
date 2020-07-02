using UnityEngine;
using UnityEngine.Events;
using Atom.Physics;
using DUI;
using System;

namespace Atom
{
    [RequireComponent(typeof(PhysicsObject))]
    [RequireComponent(typeof(DUIButton))]
    public abstract class Particle : MonoBehaviour
    {
        private DUIButton sphereButton; //ref to attached DUI sphere collider 
        private const int releaseSpeed = 20;

        protected bool inAtom = false; //internally true when part of the atom
        protected bool selected = false; //true when the particle is currently selected

        public UnityEvent OnSelect; //called when the particle is first selected 
        public UnityEvent OnDeselect; //called when the particle is released from selection

        protected static Atom atom; //static ref to the Atom

        public static Action SFX_Select;
        public static Action SFX_Deselect;

        //get and set the radius in Unity Units
        private float radius;
        public float Radius {
            get { return radius; }
            set {
                radius = value;
                transform.localScale = Vector3.one * 2.0f * radius; 
            }
        }

        public PhysicsObject PhysicsObj { get; private set; }

        protected virtual void Awake()
        {
            //find components
            PhysicsObj = GetComponent<PhysicsObject>();
            sphereButton = GetComponent<DUIButton>();

            //set spherebutton to have same radius as particle
            //sphereButton.Radius = Mathf.Max(0.5f, Radius);

            //get the satic reference to the atom
            if(atom == null)
            {
                atom = FindObjectOfType<Atom>();
                if (atom == null)
                {
                    throw new System.Exception("An Atom class is needed");
                }
            }

            //hook up events 
            sphereButton.OnClick.AddListener(Select);
            OnSelect.AddListener(Select);
            OnDeselect.AddListener(Deselect);
        }

        protected virtual void Update()
        {
            //behavior when particle is selected by the user
            if (selected)
            {
                //move to mouse position 
                transform.position = DUI.DUI.inputPos + Vector3.back;

                //call deselect when intut released
                if (Input.GetMouseButtonUp(0))
                {
                    OnDeselect?.Invoke();
                }
            }
        }

        protected void Select()
        {
            SFX_Select?.Invoke();

            //run the pickup particle behavior
            PickUpParticle();

            selected = true;
        }

        protected void Deselect()
        {
            SFX_Deselect?.Invoke();

            //run the drop particle behavior
            DropParticle();

            PhysicsObj.AddForce((DUI.DUI.inputPos - DUI.DUI.inputPosPrev) * releaseSpeed);
            selected = false;
        }

        /// <summary>
        /// Behavior for when the particle is dropped into the atom
        /// </summary>
        protected virtual void DropParticle()
        {
            inAtom = true;
        }

        /// <summary>
        /// Behavior for when the particle is picked up out of the atom
        /// </summary>
        protected virtual void PickUpParticle()
        {
            inAtom = false;
        }

    }
}
