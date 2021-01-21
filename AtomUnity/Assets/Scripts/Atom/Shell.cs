using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atom.Util;
using System.Linq;

namespace Atom
{
    public class Shell : MonoBehaviour
    {
        private const float ALIGNMENT_SPEED = 2.0f; //magnitude of force to get into orbit
        private const float ORBIT_SPEED = 0.05f; //magnitude of orbital force
        private const float SEPERATION_SPEED = 5.0f; //magnitude of speration force

        private List<Particle> particles; //list of all the particles in this shell
        private float seperationDistance; //how far apart each electron should be
        private float scale = 1;
        private float radius; //desired orbital radius

        private CircleDraw circleDraw;

        public float Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                CalcSeperationDistance();
                circleDraw.Draw(radius, 0.1f * scale);
            }
        }

        public int ElectronCount => particles.Count; //quick ref for particles.Count
        public Particle[] Particles => particles.ToArray();
        public bool sBlockFull => ElectronCount >= 2; //checks s block full
        public bool pBlockFull => (ElectronCount >= 8 || MaxParticles <= 2) && sBlockFull; //checks p s blocks full
        public bool dBlockFull => (ElectronCount >= 18 || MaxParticles <= 8) && pBlockFull; //checks d p s blocks full
        public bool fBlockFull => (ElectronCount >= 32 || MaxParticles <= 18) && dBlockFull; //check f d p s blocks full
        
        /// <summary> The shell directly below this one </summary>
        public Shell NextShell { get; set; } 
        public int MaxParticles { get; set; }

        public float Scale
        {
            set
            {
                scale = value;
                foreach (Electron particle in particles)
                    particle.Radius = scale / 4;
            }
        }

        private void Awake()
        {
            particles = new List<Particle>();
            circleDraw = GetComponent<CircleDraw>();
        }

        /// <summary>
        /// Figures out where to Add a particle
        /// </summary>
        /// <param name="particle">Particle to be added</param>
        /// <returns>true if sucessfully added</returns>
        public bool AddParticle(Particle particle)
        {
            //0 recursively fill in electrons in PREVIOUS LEVEL that MUST be there
            if (NextShell)
            {
                if (!NextShell.pBlockFull)
                {
                    return NextShell.AddParticle(particle);
                }
            }

            //1 Fill shell sBlock
            if (!sBlockFull)
            {
                Add(particle);
                return true;
            }

            if(NextShell)
            {
                //2 Fill shell-2 fBlock
                if(NextShell.NextShell && !NextShell.NextShell.fBlockFull)
                {
                    NextShell.NextShell.Add(particle);
                    return true;
                }

                //3 Fill shell-1 dBlock
                if (!NextShell.dBlockFull) {
                    NextShell.Add(particle);
                    return true;
                }
            }

            //4 Fill shell blocks
            if(!pBlockFull)
            {
                Add(particle);
                return true;
            }

            //No open place for electron
            return false;
        }

        //helper add function for actually adding the particle into a shell
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
            if (particles.Contains(particle))
            {
                //remove particle from this layer
                Remove(particle);
                particle.transform.SetParent(null);

                FallUp();
                return true;
            }
            //not in shell, check the next one
            else if (NextShell != null)
            {
                //recursively check if particle in next shell
                if (NextShell.RemoveParticle(particle))
                {
                    //make sure there are electrons AND NOT (sBlock into dBlock OR pBlock into fBlock)
                    if (ElectronCount > 0 && !((ElectronCount <= 2 && NextShell.pBlockFull) || (ElectronCount <= 8 && NextShell.dBlockFull))) {
                        //replace the removed partcicle with one from this shell
                        TransferParticle(this, NextShell);
                        FallUp(); 
                    }
                    return true;
                }
            }
            return false;
        }

        //helper remove function for actually removing particle
        private void Remove(Particle particle)
        {
            particles.Remove(particle);
            //calculate the new seperation distance
            CalcSeperationDistance();
            ColorParticles();
        }

        // helper function for moving electrons back up the atom
        void FallUp()
        {
            //check if sBlock not full (should only occur on outer shell)
            if (!sBlockFull && NextShell != null) //
            {
                //raise shell-1 dBlock into sBlock
                if (NextShell.ElectronCount > 8)
                    TransferParticle(NextShell, this);
                //dBlock empty raise shell-2 fBlock into sBlcok
                else if (NextShell.NextShell != null && NextShell.NextShell.ElectronCount > 18)
                    TransferParticle(NextShell.NextShell, this);
            }
        }

        /// <summary> Helper funtion for moving particles between shells </summary>
        /// <param name="from">Shell to take a particle from</param>
        /// <param name="to">Shell to add particle to</param>
        private void TransferParticle(Shell from, Shell to)
        {
            Particle transferParticle = from.particles[0];
            from.Remove(transferParticle);
            to.Add(transferParticle);
        }

        //Recolors the particles
        private void ColorParticles()
        {
            if (Settings.COLOR)
            {
                Color c;
                Renderer rend;
                for (int i = 0, len = ElectronCount; i < len; i++)
                {
                    rend = (particles[i] as Electron).Render;

                    if (i < 2) c = BlockTypeUtil.ColorFromBlock(BlockType.sBlock);
                    else if (i < 8) c = BlockTypeUtil.ColorFromBlock(BlockType.pBlock);
                    else if (i < 18) c = BlockTypeUtil.ColorFromBlock(BlockType.dBlock);
                    else c = BlockTypeUtil.ColorFromBlock(BlockType.fBlock);

                    rend.material.SetColor("_Color", c);
                    rend.material.SetColor("_EmissionColor", c/4);
                }
            }
        }

        private void SortParticles()
        {
            float GetAngle(Particle p)
            {
                float x1 = particles[0].transform.position.x;
                float y1 = particles[0].transform.position.y;
                float x2 = p.transform.position.x;
                float y2 = p.transform.position.y;

                float dot = x1 * x2 + y1 * y2; //dot product
                float det = x1 * y2 - y1 * x2; //determinatnt

                return Mathf.Atan2(det, dot); //angle
            }

            //List<Particle> sortedParticles = particles.OrderBy((p) => Vector2.Angle(Vector2.right, p.transform.position)).ToList();
            particles = particles.OrderBy(p => GetAngle(p)).ToList();
        }

        private void FixedUpdate()
        {
            bool sort = false;

            for (int i = 0, len = ElectronCount; i < len; i++)
            {
                //calculate force to get into orbit
                Vector2 diffRadius = transform.position - particles[i].PhysicsObj.Position;
                Vector2 forceToRadius = diffRadius.normalized * (diffRadius.sqrMagnitude - Radius*Radius) * ALIGNMENT_SPEED;
                particles[i].PhysicsObj.AddForce(forceToRadius);

                //calculate force to maintain orbit
                if (Settings.ORBIT)
                {
                    Vector2 forceToOrbit = new Vector2(-diffRadius.y, diffRadius.x).normalized * ORBIT_SPEED * scale;
                    particles[i].PhysicsObj.AddForce(forceToOrbit);
                }

                //calculate force to z = 0;
                Vector3 forceToZ = Vector3.back * particles[i].PhysicsObj.Position.z;
                particles[i].PhysicsObj.AddForce(forceToZ);

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
                        Vector2 forceToSeperate = diffOther.normalized * overlap * SEPERATION_SPEED ;
                        //apply forces to the particles
                        particles[i].PhysicsObj.AddForce(-forceToSeperate);
                        particles[j].PhysicsObj.AddForce(forceToSeperate);
                    }
                }

                sort = sort || particles[i].PhysicsObj.Velocity.sqrMagnitude > 0.1;
            }

            if (sort) { SortParticles(); }
        }

        /// <summary> Removes the specified number of electrons (Recursive) </summary>
        /// <param name="num"></param>
        public void TrimElectrons(int num)
        {
            if (num <= 0) return;

            Particle particle;
            for(; num > 0; num--)
            {
                particle = particles[0]; //maitains ref after particle is removed
                
                //Run removal logic
                RemoveParticle(particle);
                particle.OnDeselect?.Invoke();
            }
        }

        /// <summary> update the desired distance between particles</summary>
        /// <returns> distance between electrons </returns>
        private void CalcSeperationDistance()
        {
            //Euclidian distance between n points on r radius circle (n = ElectronCount, r = Radius)
            seperationDistance = 2 * Radius * Mathf.Sin(Mathf.PI / ElectronCount);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}
