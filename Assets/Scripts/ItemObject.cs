using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemClass
{
    public string id;
    public string name;
    public string description;
    public int type; //0 - weapon, 1 - items, 2 - eatable
    public Sprite icon;
    public float health;
    public float stamina;
}

[CreateAssetMenu(menuName = "Viking/Items list", fileName = "ItemsList")]
public class ItemObject : ScriptableObject
{
    public List<ItemClass> items;

    public bool GetItem(string id, out ItemClass item)
    {
        foreach (ItemClass itm in items)
        {
            if (itm.id.ToLower() == id.ToLower())
            {
                item = itm;
                return true;
            }
                
        }

        item = null;
        return false;
    }
}
