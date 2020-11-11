using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum for different types of items
public enum ItemType
{
    Food,
    Equipment,
    Default
}

public enum Attributes      //Determines type of attributes
{
Agility,
Magic,
Stamina,
Attack,
Strength
}

public abstract class ItemObject : ScriptableObject
{
    public int Id;                              //Item ID
    public Sprite uiDisplay;                    //Hold Item Sprite we assign to inventory to display item.
    public ItemType type;                       //Item type
    [TextArea(15, 20)]                          //Easier to read description in Unity editor
    public string description;                  //Item description.
    public ItemBuff[] buffs;                    //Array of itembuffs on said item.

    //public Itemz CreateItem(ItemObject item)             //EASIER TO CREATE ITEMS THIS WAY?
   // {
        //Itemz newItem = new Item(this);                  //Passes in this Itemobject
        //return newItem;
    //}
}


[System.Serializable]
public class Itemz
{
    public string Name;
    public int Id;
    public ItemBuff[] buffs;

    public Itemz(ItemObject item)
    {
        Name = item.name;
        Id = item.Id;
        buffs = new ItemBuff[item.buffs.Length];        //Creates a list of buffs = buffs list above

        for (int i = 0; i < buffs.Length ; i++)
        {
            buffs[i] = new ItemBuff(item.buffs[i].min, item.buffs[i].max);          //every time we create an item we make sure has all the item buffs on that item.
            {
                buffs[i].attributes = item.buffs[i].attributes //Stops us from getting duplicate buffs
                    ;
            }
            



        }
    }
}

[System.Serializable]
public class ItemBuff                   //Item buff class which accesses Attributes enum.
{
    public Attributes attributes;
    public int value;                   //VAL
    public int min;                     //Min attribute value
    public int max;                     //Max attribute value

    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }

    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);         //Generates a value between min and max attribute value
    }
}
