using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item
{
    public string item;
    public string name;
}

public class Slot
{
    public string item;
    public int count;
}

public class Inventory : MonoBehaviour
{
    static readonly List<Item> items = new List<Item>()
    {
        new Item() { item = "Sword1", name = "Bronze sword" },
        new Item() { item = "Bow1", name = "Simple bow" },
        new Item() { item = "Arrows1", name = "Wooden arrows" },
        new Item() { item = "Berrie", name="Apple berrie" },
    };

    readonly List<Slot> slots = new List<Slot>();

    public ItemObject itemObject;
    public List<SlotUI> inventorySlots;

    private void Start()
    {
        slots.Add(new Slot() { item = "Sword1", count = 1 });
        slots.Add(new Slot() { item = "Bow1", count = 1 });
        slots.Add(new Slot() { item = "Arrows1", count = 10 });
        slots.Add(new Slot() { item = "Shield", count = 1 });

        inventorySlots[2].SetCount(10);

    }

    public int Count(string item)
    {
        foreach (Slot slot in slots)
        {
            if (slot.item == item)
                return slot.count;
        }

        return 0;
    }
    public void Add(string item, int count = 1)
    {
        foreach (Slot slot in slots)
        {
            if (slot.item != item && slot.count > 0)
                continue;

            slot.item = item;
            slot.count += count;
            UpdateCount();
            return;
        }

        if (!itemObject.GetItem(item, out ItemClass itmsClass)) 
        {
            Debug.Log(item + " wrong name!!!!");
            return;
        }

        slots.Add(new Slot() { item = item, count = count });

        inventorySlots[slots.Count - 1].SetItem(itmsClass, count);

    }
    public void Remove(string item, int count = 1)
    {
        foreach (Slot slot in slots)
        {
            if (slot.item != item)
                continue;

            slot.count -= count;
        }
        UpdateCount();
    }

    public int GetSlot(int slotID, out ItemClass item)
    {
        item = null;

        if (slots.Count < slotID)
            return 0;

        if (slots[slotID] == null)
            return 0;

        if (!itemObject.GetItem(slots[slotID].item, out item))
            return 0;
            
        return slots[slotID].count;

    }


    void UpdateCount()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            inventorySlots[i].SetCount(slots[i].count);
        }
    }
}
