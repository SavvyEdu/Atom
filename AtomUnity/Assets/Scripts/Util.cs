using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Atom.Util
{
    /// <summary>
    /// Reactivity Classification on the periodic table
    /// </summary>
    public enum ElementType
    {
        Alkali_Metal,
        Alkaline_Earth_Metal,
        Lanthanide,
        Actinide,
        Transition_Metal,
        Metalloid,
        Nonmetal,
        Metal,
        Halogen,
        Noble_Gas,
    }

    public struct ElementTypeUtil
    {
        /// <summary>
        /// Color specifications for each element type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Color ColorFromType(ElementType type)
        {
            switch (type)
            {
                //fBlock
                case ElementType.Lanthanide: return new Color(0.0f, 0.7f, 0.7f);
                case ElementType.Actinide: return new Color(0.1f, 0.6f, 0.6f);
                //dBlock
                case ElementType.Transition_Metal: return new Color(0, 0.8f, 0.5f);
                //pBlock
                case ElementType.Metal: return new Color(0.0f, 0.7f, 1.0f);
                case ElementType.Metalloid: return new Color(0.1f, 0.5f, 0.9f);
                case ElementType.Nonmetal: return new Color(0.2f, 0.3f, 0.9f);
                case ElementType.Halogen: return new Color(0.1f, 0.15f, 0.7f);
                case ElementType.Noble_Gas: return new Color(0.0f, 0.0f, 0.5f);
                //sBlock
                case ElementType.Alkali_Metal: return new Color(0.0f, 0.2f, 0.2f);
                case ElementType.Alkaline_Earth_Metal: return new Color(0.1f, 0.35f, 0.35f);
                //default
                default: return Color.black;
            }
        }

        public static Dictionary<string, ElementType> StringToElementType = new Dictionary<string, ElementType>
        {
            { "Alkali Metal", ElementType.Alkali_Metal },
            { "Alkaline Earth Metal", ElementType.Alkaline_Earth_Metal },
            { "Lanthanide", ElementType.Lanthanide },
            { "Actinide", ElementType.Actinide },
            { "Transition Metal", ElementType.Transition_Metal },
            { "Metalloid", ElementType.Metalloid },
            { "Metal", ElementType.Metal },
            { "Nonmetal", ElementType.Nonmetal },
            { "Halogen", ElementType.Halogen },
            { "Noble Gas", ElementType.Noble_Gas }
        };
    }
    public enum BlockType
    {
        sBlock, pBlock, fBlock, dBlock
    }

    public struct BlockTypeUtil
    {
        private static Color sBlockColor = new Color(0, 0.2f, 0.2f);
        private static Color pBlockColor = new Color(0.2f, 0.5f, 0.9f);
        private static Color dBlockColor = new Color(0, 0.8f, 0.5f);
        private static Color fBlockColor = new Color(0, 0.7f, 0.7f);

        public static Color ColorFromBlock(BlockType type)
        {
            switch (type)
            {
                case BlockType.sBlock: return sBlockColor;
                case BlockType.pBlock: return pBlockColor;
                case BlockType.dBlock: return dBlockColor;
                case BlockType.fBlock: return fBlockColor;

                default: return Color.black;
            }
        }

        public static Dictionary<string, BlockType> StringToBlockType = new Dictionary<string, BlockType>
        {
            { "s", BlockType.sBlock },
            { "p", BlockType.pBlock },
            { "d", BlockType.dBlock },
            { "f", BlockType.fBlock }
        };
    }
}