using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Atom.Util;

namespace Atom
{
    public static class Elements
    {
        /// <summary>
        /// Contains ALL the data about elements
        /// </summary>

        private static Element[] elements = ElementsIO.Load(); //Load in element data

        public static Element GetElement(int protonCount)
        {
            if (protonCount > 0 && protonCount <= elements.Length)
            {
                return elements[protonCount - 1];
            }
            return null;
        }

        public static int NumElements { get { return elements.Length; } }

        public static int[] sblock = { 2, 2, 2, 2, 2, 2, 2 };
        public static int[] pblock = { 0, 6, 6, 6, 6, 6, 6 };
        public static int[] dblock = { 0, 0, 10, 10, 10, 10, 0 };
        public static int[] fblock = { 0, 0, 0, 14, 14, 0, 0 };

        //Amount: 2,  2,  6,  2,  6,  2, 10,  6,  2, 10,  6,  2, 14, 10,  6,  2, 14, 10,  6
        //Order: 1s, 2s, 2p, 3s, 3p, 4s, 3d, 4p, 5s, 4d, 5p, 6s, 4f, 5d, 6p, 7s, 5f, 6d, 7p

        public static int GetMaxElectrons(int shell, int protonCount)
        {
            return sblock[shell] + pblock[shell] + dblock[shell] + fblock[shell];
        }

        public static int GetShells(int protonCount)
        {
            //shells added after Noble gasses
            if (protonCount == 0) return 0;
            if (protonCount <= 2) return 1;
            if (protonCount <= 10) return 2;
            if (protonCount <= 18) return 3;
            if (protonCount <= 36) return 4;
            if (protonCount <= 54) return 5;
            if (protonCount <= 86) return 6;
            return 7;
        }
    }

    public class Element
    {
        public string Name { get; }
        public string Abbreviation { get; }
        public int AtomicNumber { get; }
        public BlockType Block { get; }
        public ElementType Type { get; }
        public Isotope[] Isotopes { get; }
        public Isotope Common { get; }

        public Element(string name, string abbreviation, int atomicNumber, ElementType type, BlockType block, Isotope[] isotopes = null)
        {
            Name = name;
            Abbreviation = abbreviation;
            AtomicNumber = atomicNumber;
            Type = type;
            Block = block;
            Isotopes = isotopes;
            Common = FindCommon();
        }

        public int MaxIsotope { get { return Isotopes[Isotopes.Length - 1].Mass; } }
        public int MinIsotope { get { return Isotopes[0].Mass; } }     

        public Isotope GetIsotope(int mass)
        {
            //make sure there is actually an Isotope
            if (Isotopes != null && mass >= MinIsotope && mass <= MaxIsotope)
            {
                //index = mass - smallest possible mass
                return Isotopes[mass- Isotopes[0].Mass];
            }
            return null;
        }

        /// <summary>
        /// gets the most common isotope of the element
        /// </summary>
        /// <returns></returns>
        private Isotope FindCommon()
        {
            Isotope common = null;

            //find the stable isotope with the greatest abundance
            foreach(Isotope isotope in Isotopes)
            {
                if(isotope.Stable && (common == null || isotope.Abundance > common.Abundance))
                {
                    common = isotope;
                }
            }

            //no stable isotopes, use middle isotope
            if(common == null)
            {
                //round up
                int isotopeIndex = Mathf.CeilToInt((MaxIsotope - MinIsotope) / 2.0f);
                common = Isotopes[isotopeIndex];
            }

            return common;
        }
    }

    public class Isotope
    {
        public int Mass { get; }
        public bool Stable { get; }
        public float Abundance { get; }
        public Isotope(int mass, bool stable = false, float abundance = 0.0f)
        {
            Mass = mass;
            Stable = stable;
            Abundance = abundance;
        }
    }
}

