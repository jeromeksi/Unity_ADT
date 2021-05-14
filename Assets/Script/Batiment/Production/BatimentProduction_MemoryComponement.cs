using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BatimentProduction_MemoryComponement 
{
    public Memory_ItemComponent memoryItem;
    public BatimentProduction_MemoryComponement()
    {
        memoryItem = new Memory_ItemComponent();
    }
    internal Memory_ItemComponent GetMemoryItemComponent()
    {
        return memoryItem;
    }
}
