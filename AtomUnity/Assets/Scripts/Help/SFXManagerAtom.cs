using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Atom
{

    [RequireComponent(typeof(Atom))]
    public class SFXManagerAtom : MonoBehaviour
    {
        [SerializeField] private AudioClip radioactiveSFX;

        private Atom atom;
        private AudioSource source = null;

        private void Awake()
        {
            atom = GetComponent<Atom>();
            source = GetComponent<AudioSource>();
            source.clip = radioactiveSFX;
        }

        private void Adjust()
        {
            source.volume = Settings.MUTE ? 0 : 0.1f * Settings.SFX_VOLUME;
        }


        private void Update()
        {
            if (atom.Element != null)
            {
                //check for radioactive isotope, and not already playing
                Isotope isotope = atom.Element.GetIsotope(atom.Nucleus.Mass);
                if (isotope != null && !isotope.Stable)
                {
                    //make sure source isn't already looping
                    if (!source.isPlaying)
                    {
                        source.Play();
                    }
                }
                else
                {
                    source.Stop();   
                }
            }
        }
    }
}
