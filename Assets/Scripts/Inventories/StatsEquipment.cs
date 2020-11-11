using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)            //Gets all additive modifiers in equipment slots
        {
            foreach (var slot in GetAllPopulatedSlots())       //Loops through all populated equipment slots
            {
                var item = GetItemInSlot(slot) as IModifierProvider;        //Gets all items and cast as ImodifierpROVIDER to make sure it modifies stats
                if (item == null) continue; //Null check

                foreach (float modifier in item.GetAdditiveModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var slot in GetAllPopulatedSlots())       //Loops through all populated equipment slots
            {
                var item = GetItemInSlot(slot) as IModifierProvider;        //Gets all items and cast as ImodifierpROVIDER to make sure it modifies stats
                if (item == null) continue; //Null check

                foreach (float modifier in item.GetPercentageModifiers(stat))
                {
                    yield return modifier;
                }
            }
        }
    }
}
