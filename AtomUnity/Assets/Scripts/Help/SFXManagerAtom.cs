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
        [SerializeField] private AudioClip particleSelectSFX;

        private Atom atom;
        private AudioSource[] sources = null;

        private void Awake()
        {
            atom = GetComponent<Atom>();
            sources = GetComponents<AudioSource>();
            sources[0].clip = radioactiveSFX;
            sources[1].clip = particleSelectSFX;

            Particle.SFX = ParticleSelect;

            Adjust();
        }

        private void Adjust()
        {
            sources[0].volume = Settings.MUTE ? 0 : 0.1f * Settings.SFX_VOLUME;
            sources[1].volume = Settings.MUTE ? 0 : 0.05f * Settings.SFX_VOLUME;
        }

        private void ParticleSelect()
        {
            if (sources[1].isPlaying) { sources[1].Stop(); }
            sources[1].Play();
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
                    if (!sources[0].isPlaying)
                    {
                        sources[0].Play();
                    }
                }
                else
                {
                    sources[0].Stop();   
                }
            }
        }
    }
}
