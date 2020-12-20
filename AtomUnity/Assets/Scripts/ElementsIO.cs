using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Atom.Util;

namespace Atom {
    public class ElementsIO
    {
        /// <summary>
        /// Loads in the element data from CSV
        /// </summary>
        /// <returns>element dat</returns>
        public static Element[] Load()
        {
            //Load in the file
            TextAsset textAsset = Resources.Load("Elements") as TextAsset;

            //Create the returned array
            Element[] elements = new Element[118];

            //Create a temp isotope list
            List<Isotope> isotopes = new List<Isotope>(); //temp list of isotopes

            string[] lines = textAsset.text.Split('\n');
            for (int e = 1; e < 119; e++) //loop over rows 1-119
            {
                string[] cells = lines[e].Split(',');

                isotopes.Clear();

                //get isotope abundances
                int isotopeStartIndex = 10;
                for (int i = isotopeStartIndex; i < isotopeStartIndex + 294; i++)
                {
                    float abundance;
                    if (float.TryParse(cells[i], out abundance))
                    {
                        isotopes.Add(new Isotope(i - isotopeStartIndex - 1, abundance > 0, abundance));
                    }
                }

                //parse out the type and block
                BlockType block = BlockTypeUtil.StringToBlockType[cells[4]];
                ElementType type = ElementTypeUtil.StringToElementType[cells[9]];

                //add element with name, abbreviation, atomic num, type, block, and isotopes
                elements[e - 1] = new Element(cells[1], cells[2], e, type, block, isotopes.ToArray());
            }
            return elements;
        }
    }
}