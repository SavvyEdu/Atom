using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atom
{
    public class Electron : Particle
    {
        /// <summary>
        /// Handles the behavior of electron particles in the atom
        /// </summary>
        

        public Renderer Render { get; private set; }

        //get and set the radius in Unity Units
        public new float Radius
        {
            get { return transform.localScale.x / 2; }
            set {
                transform.localScale = Vector3.one * 2.0f * value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Render = GetComponent<Renderer>();
            Radius = 0.25f;
        }

        protected override void PickUpParticle()
        {
            //check the the electron is part of the atom and can be removed
            if (inAtom && atom.RemoveElectron(this))
            {
                base.PickUpParticle();
            }
            else
            {
                atom.RemoveExcessParticle(this);
            }
        }

        protected override void DropParticle()
        {
            //electron must be added when Next Shell exists and isn't Full
            bool mustAdd = false; 
            //check if must be added to pBlock of Next Shell
            if(atom.OuterShell && atom.OuterShell.NextShell)
            {
                mustAdd = !atom.OuterShell.NextShell.pBlockFull;
                //check if must be added to dBlock of Next Next Shell
                if(!mustAdd && atom.OuterShell.NextShell.NextShell)
                {
                    mustAdd = !atom.OuterShell.NextShell.NextShell.dBlockFull;
                    //check if must be added to fBlock of Next Next Next Shell
                    if (!mustAdd && atom.OuterShell.NextShell.NextShell.NextShell)
                    {
                        mustAdd = !atom.OuterShell.NextShell.NextShell.NextShell.fBlockFull;
                    }
                }
            }

            //check not already part of atom, within atom bounds, and can actually be added
            if (!inAtom && (!atom.Interactable || atom.Contains(transform.position) || mustAdd ) && atom.OuterShell.AddParticle(this))
            {
                base.DropParticle();
            }
            //electron out of bounds or cound not be added 
            else
            {
                atom.AddExcessParticle(this);
            }
        }
    }
}
