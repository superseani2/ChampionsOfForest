﻿using ChampionsOfForest.Items;
using System.Collections.Generic;
using UnityEngine;

namespace ChampionsOfForest
{
    public class ItemDataBase
    {
        public static ItemDataBase Instance
        {
            get;
            private set;
        }

        public List<BaseItem> _Item_Bases = new List<BaseItem>();
        public Dictionary<int, BaseItem> ItemBases = new Dictionary<int, BaseItem>();
        public List<ItemStat> statList = new List<ItemStat>();
        public Dictionary<int, ItemStat> Stats = new Dictionary<int, ItemStat>();

        //Called from Initializer
        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new ItemDataBase();
            }
            else
            {
                return;
            }

            Instance.FillStats();

            Instance.Stats.Clear();
            for (int i = 0; i < Instance.statList.Count; i++)
            {
                try
                {
                    Instance.Stats.Add(Instance.statList[i].StatID, Instance.statList[i]);
                }
                catch (System.Exception ex)
                {
                    ModAPI.Log.Write("Error with adding a stat " + ex.ToString());
                }
            }
            try
            {
                Instance.FillItems();
            }
            catch (System.Exception ex)
            {

                ModAPI.Log.Write("Error with item " + ex.ToString());

            }
            Instance.ItemBases.Clear();
            for (int i = 0; i < Instance._Item_Bases.Count; i++)
            {
                try
                {
                    Instance.ItemBases.Add(Instance._Item_Bases[i].ID, Instance._Item_Bases[i]);
                }
                catch (System.Exception ex)
                {

                    ModAPI.Log.Write("Error with adding an item " + ex.ToString());
                }

            }
            ModAPI.Log.Write("SETUP: ITEM DATABASE");

        }

        public static void AddItem(BaseItem item)
        {
            Instance._Item_Bases.Add(item);
        }
        public static void AddStat(ItemStat item)
        {
            Instance.statList.Add(item);
        }

        public static ItemStat StatByID(int id)
        {
            return ItemDataBase.Instance.Stats[id];
        }


        public void FillStats()
        {
            int i = 1;
            new ItemStat(i, 1, 1.4f, 1.6f, "Strenght", 2, StatActions.AddStrenght, StatActions.RemoveStrenght, StatActions.AddStrenght); i++;
            new ItemStat(i, 1, 1.4f, 1.6f, "Agility", 2, StatActions.AddAgility, StatActions.RemoveAgility, StatActions.AddAgility); i++;
            new ItemStat(i, 1, 1.4f, 1.6f, "Vitality", 2, StatActions.AddVitality, StatActions.RemoveVitality, StatActions.AddVitality); i++;
            new ItemStat(i, 1, 1.4f, 1.6f, "Intelligence", 2, StatActions.AddIntelligence, StatActions.RemoveIntelligence, StatActions.AddIntelligence); i++;
            new ItemStat(i, 3, 6, 1.7f, "Maximum Life", 3, StatActions.AddHealth, StatActions.RemoveHealth, StatActions.AddHealth); i++;
            new ItemStat(i, 3, 6, 1.7f, "Maximum Energy", 3, StatActions.AddHealth, StatActions.RemoveHealth, StatActions.AddHealth); i++;
            new ItemStat(i, 0.1f, 0.25f, 1.7f, "Life Per Second", 4, StatActions.AddHPRegen, StatActions.RemoveHPRegen, StatActions.AddHPRegen); i++;
            new ItemStat(i, 0.1f, 0.25f, 1.7f, "Energy Per Second", 4, StatActions.AddERegen, StatActions.RemoveERegen, StatActions.AddERegen); i++;
            new ItemStat(i, 0.01f, 0.025f, 1.6f, "Energy Regen %", 5, StatActions.AddEnergyRegenPercent, StatActions.RemoveEnergyRegenPercent, StatActions.AddEnergyRegenPercent); i++;
            new ItemStat(i, 0.01f, 0.025f, 1.6f, "Life Regen %", 5, StatActions.AddHealthRegenPercent, StatActions.RemoveHealthRegenPercent, StatActions.AddHealthRegenPercent); i++;
            new ItemStat(i, 0.005f, 0.02f, 1.5f, "Damage Reduction %", 7, StatActions.AddDamageReduction, StatActions.RemoveDamageReduction, StatActions.AddDamageReduction); i++;
            new ItemStat(i, 0.01f, 0.03f, 1.5f, "Critical Hit Chance", 6, StatActions.AddCritChance, StatActions.RemoveCritChance, StatActions.AddCritChance); i++;
            new ItemStat(i, 0.01f, 0.03f, 1.7f, "Critical Hit Damage", 6, StatActions.AddCritDamage, StatActions.RemoveCritDamage, StatActions.AddCritDamage); i++;
            new ItemStat(i, 0.1f, 0.35f, 1.6f, "Life on hit", 6, StatActions.AddLifeOnHit, StatActions.RemoveLifeOnHit, StatActions.AddLifeOnHit); i++;
            new ItemStat(i, 0.005f, 0.02f, 1.3f, "Dodge chance", 7, StatActions.AddDodgeChance, StatActions.RemoveDodgeChance, StatActions.AddDodgeChance); i++;
            new ItemStat(i, 1f, 2f, 1.6f, "Armor", 2, StatActions.AddArmor, StatActions.RemoveArmor, StatActions.AddArmor); i++;
            new ItemStat(i, 0.5f, 1f, 1.4f, "Resistance to magic", 4, StatActions.AddMagicResistance, StatActions.RemoveMagicResistance, StatActions.AddMagicResistance); i++;
            new ItemStat(i, 0.005f, 0.01f, 1.4f, "Attack speed", 6, StatActions.AddAttackSpeed, StatActions.RemoveAttackSpeed, StatActions.AddAttackSpeed); i++;
            new ItemStat(i, 0.02f, 0.06f, 1.1f, "Exp %", 6, StatActions.AddExpFactor, StatActions.RemoveExpFactor, StatActions.AddExpFactor); i++;
            new ItemStat(i, 1f, 2f, 1.1f, "Massacre Duration", 5, StatActions.AddMaxMassacreTime, StatActions.RemoveMaxMassacreTime, StatActions.AddMaxMassacreTime); i++;
            new ItemStat(i, 0.01f, 0.02f, 1.7f, "Spell Damage %", 7, StatActions.AddSpellDamageAmplifier, StatActions.RemoveSpellDamageAmplifier, StatActions.AddSpellDamageAmplifier); i++;
            new ItemStat(i, 0.01f, 0.02f, 1.7f, "Meele Damage %", 7, StatActions.AddMeeleDamageAmplifier, StatActions.RemoveMeeleDamageAmplifier, StatActions.AddMeeleDamageAmplifier); i++;
            new ItemStat(i, 0.01f, 0.02f, 1.7f, "Ranged Damage %", 7, StatActions.AddRangedDamageAmplifier, StatActions.RemoveRangedDamageAmplifier, StatActions.AddRangedDamageAmplifier); i++;
            new ItemStat(i, 1f, 2f, 1.5f, "Bonus Spell Damage", 5, StatActions.AddSpellDamageBonus, StatActions.RemoveSpellDamageBonus, StatActions.AddSpellDamageBonus); i++;
            new ItemStat(i, 1f, 2f, 1.5f, "Bonus Meele Damage", 5, StatActions.AddMeeleDamageBonus, StatActions.RemoveMeeleDamageBonus, StatActions.AddMeeleDamageBonus); i++;
            new ItemStat(i, 1f, 2f, 1.5f, "Bonus Ranged Damage", 5, StatActions.AddRangedDamageBonus, StatActions.RemoveRangedDamageBonus, StatActions.AddRangedDamageBonus); i++;
            new ItemStat(i, 0.005f, 0.01f, 1.2f, "Energy Per Agility", 7, StatActions.AddEnergyPerAgility, StatActions.RemoveEnergyPerAgility, StatActions.AddEnergyPerAgility); i++;
            new ItemStat(i, 0.05f, 0.1f, 1.2f, "Health Per Vitality", 7, StatActions.AddHealthPerVitality, StatActions.RemoveHealthPerVitality, StatActions.AddHealthPerVitality); i++;
            new ItemStat(i, 0.0001f, 0.0005f, 1.2f, "Spell Damage Per Intelligence", 7, StatActions.AddSpellDamageperInt, StatActions.RemoveSpellDamageperInt, StatActions.AddSpellDamageperInt); i++;
            new ItemStat(i, 0.0001f, 0.0005f, 1.2f, "Damage Per Strenght", 7, StatActions.AddDamagePerStrenght, StatActions.RemoveDamagePerStrenght, StatActions.AddDamagePerStrenght); i++;
            new ItemStat(i, 0.001f, 0.005f, 1.6f, "All Healing %", 7, StatActions.AddHealingMultipier, StatActions.RemoveHealingMultipier, StatActions.AddHealingMultipier); i++;
            new ItemStat(i, 1f, 1f, 0f, "PERMANENT PERK POINTS", 7, null, null, StatActions.PERMANENT_perkPointIncrease); i++;
            new ItemStat(i, 4f, 7f, 1.5f, "EXPERIENCE", 3, null, null, StatActions.PERMANENT_expIncrease); i++;
            new ItemStat(i, 0.05f, 0.15f, 1.1f, "Movement Speed", 5, StatActions.AddMoveSpeed, StatActions.RemoveMoveSpeed, StatActions.AddMoveSpeed); i++;



        }
        public void FillItems()
        {
            new BaseItem(new List<List<ItemStat>> {
                new List<ItemStat>() {statList[0], statList[1] },
                new List<ItemStat>() {statList[0], statList[1] ,statList[2]},
                new List<ItemStat>() {statList[0], statList[1] ,statList[2]},
                new List<ItemStat>() { statList[2]}
            }, 5, 1, 1, BaseItem.ItemType.Weapon, "My dick", "this is my dig bick", "its ancient, and kinda immortal", "it simply oneshots everything", 1, Texture2D.whiteTexture);

            
        }




    }
}