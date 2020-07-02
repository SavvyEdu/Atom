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
        [SerializeField] private AudioClip particleDeselectSFX;
        [SerializeField] private AudioClip shellSFX;

        private Atom atom;
        private AudioSource[] sources = null;

        private void Awake()
        {
            atom = GetComponent<Atom>();
            sources = GetComponents<AudioSource>();
            sources[0].clip = radioactiveSFX;
            sources[1].clip = particleSelectSFX;
            sources[2].clip = shellSFX;

            Particle.SFX_Select = ParticleSelect;
            Particle.SFX_Deselect = ParticleDeselect;

            Atom.SFX_Shell = ShellChange;

            Adjust(); //only needed here right now, settings are in menu
        }

        private void Adjust()
        {
            sources[0].volume = Settings.MUTE ? 0 : 0.1f * Settings.SFX_VOLUME;
            sources[1].volume = Settings.MUTE ? 0 : 0.05f * Settings.SFX_VOLUME;
            sources[2].volume = Settings.MUTE ? 0 : 0.1f * Settings.SFX_VOLUME;
        }

        private void ParticleSelect()
        {
            if (!sources[1].isPlaying)
                sources[1].PlayOneShot(particleSelectSFX);
        } 
        private void ParticleDeselect()
        {
            if (!sources[1].isPlaying)
                sources[1].PlayOneShot(particleDeselectSFX);
        }
        private void ShellChange()
        {
            if (!sources[2].isPlaying)
                sources[2].PlayOneShot(shellSFX);
        }

        private void Update()
        {
            if (atom.Element != null && Settings.SHAKE)
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
