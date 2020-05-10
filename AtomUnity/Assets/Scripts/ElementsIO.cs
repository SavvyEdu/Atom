using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Atom {
    public class ElementsIO
    {
        /// <summary>
        /// Loads in the element data from CSV
        /// </summary>
        /// <returns>element dat</returns>
        public static Element[] Load()
        {
            TextAsset textAsset = Resources.Load("Elements") as TextAsset;
            Element[] elements = new Element[118];

            List<Isotope> isotopes = new List<Isotope>(); //temp list of isotopes

            string[] lines = textAsset.text.Split('\n');
            for (int e = 1; e < 119; e++) //loop over rows 1-119
            {
                string[] cells = lines[e].Split(',');


                isotopes.Clear();

                //get isotope abundances
                for (int i = 10; i < 304; i++)
                {
                    float abundance;
                    if (float.TryParse(cells[i], out abundance))
                    {
                        isotopes.Add(new Isotope(i - 9, abundance > 0, abundance));
                    }
                }
                elements[e - 1] = new Element(cells[1], cells[2], ElementTypeUtil.StringToElementType[cells[9]], isotopes.ToArray());
            }
            return elements;
        }
    }
}