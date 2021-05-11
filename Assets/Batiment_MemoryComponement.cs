using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Batiment_MemoryComponement 
{
    public Memory_ItemComponent memoryItem;
    public Batiment_MemoryComponement()
    {
        memoryItem = new Memory_ItemComponent();
    }
    internal Memory_ItemComponent GetMemoryItemComponent()
    {
        return memoryItem;
    }
}
