using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atom
{
    public class Shell : MonoBehaviour
    {
        [SerializeField] private float particleSpeed; //magnitude of force to get into orbit
        [SerializeField] private float orbitSpeed; //magnitude of orbital force

        private List<Particle> particles; //list of all the particles in this shell
        private float seperationDistance; //how far apart each electron should be
        private float scale = 1;
        private float radius;

        //TODO: replace with constants
        private Color sBlockColor = new Color(0,0.3f, 0.3f);
        private Color pBlockColor = new Color(0, 0.7f, 0.7f);
        private Color dBlockColor = new Color(0, 1f, 0.5f);
        private Color fBlockColor = new Color(0.2f, 0.5f, 0.9f);

        public float Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                CalcSeperationDistance();
            }
        }//desired orbital radius

        public int ElectronCount => particles.Count; 
        public Particle[] Particles => particles.ToArray();
        public bool sBlockFull => ElectronCount >= 2;
        public bool pBlockFull => ElectronCount >= 8 || MaxParticles <= 2;
        public bool dBlockFull => ElectronCount >= 18 || MaxParticles <= 8;
        public bool fBlockFull => ElectronCount >= 32 || MaxParticles <= 18;
        public Shell NextShell { get; set; }
        public int MaxParticles { get; set; }

        public float Scale
        {
            set
            {
                scale = value;
                foreach (Electron particle in particles)
                {
                    particle.Radius = scale / 4;
                }
            }
        }


        private void Awake()
        {
            particles = new List<Particle>();
        }

        /// <summary>
        /// Add a particle to this shell
        /// </summary>
        /// <param name="particle">Particle to be added</param>
        /// <returns>true if sucessfully added</returns>
        public bool AddParticle(Particle particle)
        {
            //0 recursively fill in electrons in prev that MUST be there
            if (NextShell)
            {
                if (!NextShell.pBlockFull || !NextShell.sBlockFull)
                {
                    return NextShell.AddParticle(particle);
                }
            }

            //1 Fill own sBlock
            if (!sBlockFull)
            {
                Add(particle);
                return true;
            }

            if(NextShell)
            {
                //2 Fill n-2 fBlock
                if(NextShell.NextShell && (!NextShell.NextShell.fBlockFull || 
                                           !NextShell.NextShell.dBlockFull || 
                                           !NextShell.NextShell.pBlockFull || 
                                           !NextShell.NextShell.sBlockFull))
                {
                    NextShell.NextShell.Add(particle);
                    return true;
                }

                //3 Fill n-1 dBlock
                if (!NextShell.dBlockFull || 
                    !NextShell.pBlockFull || 
                    !NextShell.sBlockFull)
                {
                    NextShell.Add(particle);
                    return true;
                }
            }

            //4 Fill own blocks
            if(!pBlockFull)
            {
                Add(particle);
                return true;
            }

            return false;
        }

        //helper add function for actually adding the particle
        private void Add(Particle particle)
        {
            //add the particle
            particles.Add(particle);
            particle.transform.SetParent(transform);
            particle.Radius = scale / 4;

            //calculate the new seperation distance
            CalcSeperationDistance();
            ColorParticles();
        }

        /// <summary>
        /// Removes a particle from this shell
        /// </summary>
        /// <param name="particle">Particle to remove</param>
        /// <returns></returns>
        public bool RemoveParticle(Particle particle)
        {
            //make sure the particle is an electron and actually in this shell
            if (particle is Electron && particles.Contains(particle))
            {
                particles.Remove(particle);
                particle.transform.SetParent(null);

                //calculate the new seperation distance
                CalcSeperationDistance();
                return true;
            }
            //not in shell, check the next one
            else if (NextShell != null)
            {
                //recursively check if particle in next shell
                if (NextShell.RemoveParticle(particle))
                {
                    //succeeded in removing particle (only take from sblock of Next pblock needs)
                    if (ElectronCount > 2 || (!NextShell.pBlockFull && ElectronCount > 0))
                    {
                        //replace the removed partcicle with one from this shell
                        Particle transferParticle = particles[0];
                        particles.Remove(transferParticle);
                        NextShell.Add(transferParticle);

                        CalcSeperationDistance();
                    }
                    return true;
                }
            }
            return false;
        }

        //Recolors the particles
        private void ColorParticles()
        {
            for(int i = 0, len = ElectronCount; i< len; i++)
            {
                if (i < 2) particles[i].GetComponent<Renderer>().material.color = sBlockColor;
                else if (i < 8) particles[i].GetComponent<Renderer>().material.color = pBlockColor;
                else if (i < 18) particles[i].GetComponent<Renderer>().material.color = dBlockColor;
                else particles[i].GetComponent<Renderer>().material.color = fBlockColor;
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0, len = ElectronCount; i < len; i++)
            {
                //calculate force to get into orbit
                Vector3 diffRadius = transform.position - particles[i].PhysicsObj.Position;
                Vector2 forceToRadius = diffRadius.normalized * (diffRadius.magnitude - Radius) * particleSpeed;

                //calculate force to maintain orbit
                Vector2 forceToOrbit = new Vector2(-diffRadius.y, diffRadius.x).normalized * orbitSpeed * scale;

                //apply forces to the particles
                particles[i].PhysicsObj.AddForce(forceToRadius + forceToOrbit);

                for (int j = 0; j < i; j++)
                {

                    //find the distance between particles
                    Vector2 diffOther = particles[i].PhysicsObj.Position - particles[j].PhysicsObj.Position;

                    //rare occurance, but seperate from identical other
                    if (diffOther.sqrMagnitude < 0.001) { particles[i].PhysicsObj.AddForce(Random.insideUnitSphere); }

                    //calculate the amount of overlap
                    float overlap = diffOther.magnitude - seperationDistance;
                    if (overlap < 0)
                    {
                        //add force to seperate
                        Vector2 forceToSeperate = diffOther.normalized * overlap * particleSpeed;
                        //apply forces to the particles
                        particles[i].PhysicsObj.AddForce(-forceToSeperate);
                        particles[j].PhysicsObj.AddForce(forceToSeperate);

                    }

                }
            }
        }

        public void TrimElectrons(int num)
        {
            if (num > 0)
            {
                Particle[] pA = particles.ToArray(); // copy to array so list can be mutated
                foreach (Particle particle in pA)
                {
                    RemoveParticle(particle);
                    particle.OnDeselect?.Invoke();

                    if (--num <= 0)
                        return;
                }

                //trim remaining from next shell
                NextShell.TrimElectrons(num);
            }
        }

        /// <summary>
        /// calculates the distance between points when n points are equally spaced on peremiter of circle
        /// </summary>
        /// <param name="n">number of points on circle</param>
        /// <returns>distance between points</returns>
        private void CalcSeperationDistance()
        {
            seperationDistance = 2 * Radius * Mathf.Sin(Mathf.PI / ElectronCount);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}
