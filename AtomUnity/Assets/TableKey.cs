using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Atom.Util;

public class TableKey : MonoBehaviour
{
    private void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>();
        //Text[] texts = GetComponentsInChildren<Text>();

        images[0].color = BlockTypeUtil.ColorFromBlock(BlockType.sBlock);
        images[1].color = BlockTypeUtil.ColorFromBlock(BlockType.pBlock);
        images[2].color = BlockTypeUtil.ColorFromBlock(BlockType.dBlock);
        images[3].color = BlockTypeUtil.ColorFromBlock(BlockType.fBlock);
    }
}
