﻿using Bolt;
using ChampionsOfForest.Network;
using ChampionsOfForest.Player;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;
using static ChampionsOfForest.Player.BuffDB;
using Random = UnityEngine.Random;

namespace ChampionsOfForest
{
    public class ModdedPlayer : MonoBehaviour
    {
        public static ModdedPlayer instance = null;
        public float MaxHealth
        {
            get
            {
                float x = baseHealth + vitality * HealthPerVitality;
                x += HealthBonus;
                x *= 1 + MaxHealthPercent;
                return x;
            }
        }
        public float MaxEnergy
        {
            get
            {
                float x = baseEnergy + agility * EnergyPerAgility;
                x += EnergyBonus;
                x *= 1 + MaxEnergyPercent;
                return x;
            }
        }
        public bool Critted => CritChance >= 100 * UnityEngine.Random.value;
        public float SpellAMP
        {
            get
            {
                float f = SpellDamageperInt * intelligence;
                return (1 + f) * SpellDamageAmplifier * DamageOutputMult;
            }
        }
        public float MeleeAMP => DamageOutputMult * ((strenght * DamagePerStrenght) + 1) * MeleeDamageAmplifier;
        public float RangedAMP
        {
            get
            {
                float f = agility * RangedDamageperAgi;
                return (1 + f) * RangedDamageAmplifier * DamageOutputMult;

            }
        }
        public float CritDamageBuff
        {
            get
            {
                if (Critted)
                {
                    return (CritDamage / 100) + 1;
                }
                return 1;
            }
        }
        public float StaminaAndEnergyRegenAmp => 1 + intelligence * EnergyRegenPerInt;
        public float ArmorDmgRed => ModReferences.DamageReduction(Armor);
        public int Level = 1;

        public readonly float baseHealth = 30;
        public readonly float baseEnergy = 30;


        public float HealingMultipier = 1;
        public int strenght = 1;    //increases damage
        public int intelligence = 1; //increases spell damage
        public int agility = 1;     //increases energy
        public int vitality = 1;     //increases health
        public float StaminaRecover => (4 + StaminaRegen) * (1 + StaminaRegenPercent);
        public float DamagePerStrenght = 0.00f;
        public float SpellDamageperInt = 0.00f;
        public float RangedDamageperAgi = 0.00f;
        public float EnergyRegenPerInt = 0.00f;
        public float EnergyPerAgility = 0f;
        public float HealthPerVitality = 0f;
        public float HealthRegenPercent = 0;
        public float StaminaRegenPercent = 0;
        public int HealthBonus = 0;
        public int EnergyBonus = 0;
        public float MaxHealthPercent = 0;
        public float MaxEnergyPercent = 0;
        public float CoolDownMultipier = 1;
        public float SpellDamageAmplifier = 1;
        public float MeleeDamageAmplifier = 1;
        public float RangedDamageAmplifier = 1;
        public float SpellDamageBonus = 0;
        public float MeleeDamageBonus = 0;
        public float RangedDamageBonus = 0;
        public float MeleeRange = 1;


        public float DamageReduction = 0;
        public float DamageOutputMult = 1;
        public float CritChance = 0;
        public float CritDamage = 50;
        public float LifeOnHit = 0;
        public float LifeRegen = 0;
        public float StaminaRegen = 0;
        public float DodgeChance = 0;
        public float SlowAmount = 0;
        public bool Silenced = false;
        public bool Stunned = false;
        public int Armor = 0;
        public float MagicResistance = 0;
        public float AttackSpeed = 1;
        public bool StunImmune = false;
        public bool DebuffImmune = false;
        public float MoveSpeed = 1f;
        public float SpellCostToStamina = 0;
        public float SpellCostRatio = 0;
        public float StaminaAttackCostReduction = 0;


        public float BlockFactor = 1;
        public float ExpFactor = 1;
        public long ExpCurrent = 0;
        public long ExpGoal = 1;



        public int PermanentBonusPerkPoints;
        public int MutationPoints = 0;
        public long NewlyGainedExp;

        public int MassacreKills;
        public string MassacreText = "";
        public float MassacreMultipier = 1;
        public float TimeUntillMassacreReset;
        public float MaxMassacreTime = 20;
        public float TimeBonusPerKill = 5;

        public float EnergyOnHit = 0;
        public float EnergyPerSecond = 0;

        public int ARreduction_all = 0;
        public int ARreduction_melee = 0;
        public int ARreduction_ranged = 0;
        public int MeleeArmorReduction => ARreduction_all + ARreduction_melee;
        public int RangedArmorReduction => ARreduction_all + ARreduction_ranged;


        public float DamageAbsorbAmount = 0;

        public float StealthDamage = 1; //to do


        public float ThirstRate = 1;
        public float HungerRate = 1;
        public float FireDamageTakenMult = 1;
        public float AreaDamageProcChance = 0.15f;
        public float AreaDamage = 0;
        public float AreaDamageRadius = 4;
        public float ProjectileSpeedRatio = 1f;
        public float ProjectileSizeRatio = 1;
        public Dictionary<int, int> GeneratedResources = new Dictionary<int, int>();


        public static bool IsSacredArrow = false;
        public static bool IsArrowHuge = false;


        public int LastDayOfGeneration = 0;
        public float StunDuration = 0;

        public Dictionary<int, ExtraItemCapacity> ExtraCarryingCapactity = new Dictionary<int, ExtraItemCapacity>();
        public struct ExtraItemCapacity
        {
            public int ID;
            public int Amount;
            public bool applied;
            public ExtraItemCapacity(int id, int amount)
            {
                ID = id;
                Amount = amount;
                applied = false;
            }
            public void NewApply()
            {
                applied = true;
                LocalPlayer.Inventory.AddMaxAmountBonus(ID, Amount);
            }
            public void ExistingApply(int NewAmount)
            {
                if (applied)
                {
                    Remove();
                }

                Amount = NewAmount;
                NewApply();
            }
            public void Remove()
            {
                applied = false;
                LocalPlayer.Inventory.AddMaxAmountBonus(ID, -Amount);

            }
        }
        public void AddExtraItemCapacity(int ID, int Amount)
        {
            if (ExtraCarryingCapactity.ContainsKey(ID))
            {
                ExtraCarryingCapactity[ID].ExistingApply(Amount);
            }
            else
            {
                ExtraItemCapacity EIC = new ExtraItemCapacity(ID, Amount);
                EIC.NewApply();
                ExtraCarryingCapactity.Add(ID, EIC);
            }
            if (ExtraCarryingCapactity[ID].Amount == 0)
            {
                ExtraCarryingCapactity.Remove(ID);
            }
        }

        private void Start()
        {
            instance = this;
            MoveSpeed = 1f;
            MutationPoints = 1;
            ExpGoal = GetGoalExp();
            if (!GameSetup.IsNewGame)
            {
                Serializer.Load();
            }
        }


        private void Update()
        {

            try
            {
                if (ModAPI.Input.GetButtonDown("EquipWeapon"))
                {
                    if (Inventory.Instance.ItemList[-12] != null)
                    {
                        if (Inventory.Instance.ItemList[-12].Equipped)
                        {
                            PlayerInventoryMod.ToEquipWeaponType = Inventory.Instance.ItemList[-12].weaponModel;
                            LocalPlayer.Inventory.StashEquipedWeapon(false);
                            LocalPlayer.Inventory.Equip(80, false);

                            PlayerInventoryMod.ToEquipWeaponType = BaseItem.WeaponModelType.None;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ModAPI.Log.Write("Custom Weapon error" + e.ToString());

            }
            try
            {
                float dmgPerSecond = 0;
                int poisonCount = 0;
                int[] keys = new List<int>(BuffDB.activeBuffs.Keys).ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    Buff buff = BuffDB.activeBuffs[keys[i]];
                    if (DebuffImmune && buff.isNegative && buff.DispellAmount <= 2)
                    {
                        BuffDB.activeBuffs[keys[i]].ForceEndBuff(keys[i]);
                    }
                    else if (StunImmune && buff.isNegative && buff.DispellAmount <= 1)
                    {
                        BuffDB.activeBuffs[keys[i]].ForceEndBuff(keys[i]);
                    }
                    else
                    {
                        BuffDB.activeBuffs[keys[i]].UpdateBuff(keys[i]);
                        if (buff._ID == 3)
                        {
                            poisonCount++;
                            dmgPerSecond += buff.amount;
                        }
                    }
                }
                if (dmgPerSecond != 0)
                {
                    LocalPlayer.Stats.Health -= dmgPerSecond * Time.deltaTime;
                    LocalPlayer.Stats.HealthTarget -= dmgPerSecond * Time.deltaTime;

                    if (poisonCount > 1)
                    {
                        BuffDB.AddBuff(1, 33, 0.7f, 1);
                    }


                }
                if (LocalPlayer.Stats.Health <= 0)
                {
                    LocalPlayer.Stats.Hit(1, true);
                }

            }
            catch (Exception e)
            {
                ModAPI.Log.Write("Poisoning player error" + e.ToString());
            }
            try
            {


                if (LocalPlayer.Stats != null)
                {
                    ////LocalPlayer.Stats.Skills.BreathingSkillLevelBonus = 0.05f;
                    //LocalPlayer.Stats.Skills.BreathingSkillLevelDuration = 1500000;
                    //LocalPlayer.Stats.Skills.RunSkillLevelBonus = 0.05f;
                    //LocalPlayer.Stats.Skills.RunSkillLevelDuration = 6000000;
                    //LocalPlayer.Stats.Skills.TotalRunDuration = 0;
                    //LocalPlayer.Stats.PhysicalStrength.CurrentStrength = 10;


                    if (LocalPlayer.Stats.Health < MaxHealth)
                    {
                        if (LocalPlayer.Stats.Health < LocalPlayer.Stats.HealthTarget)
                        {
                            LocalPlayer.Stats.Health += LifeRegen * (HealthRegenPercent + 1) * HealingMultipier;
                        }
                        else
                        {
                            LocalPlayer.Stats.Health += LifeRegen * (HealthRegenPercent + 1) * HealingMultipier / 10;
                        }
                    }

                    if (Clock.Day > LastDayOfGeneration)
                    {
                        for (int i = 0; i < Clock.Day - LastDayOfGeneration; i++)
                        {
                            foreach (KeyValuePair<int, int> pair in GeneratedResources)
                            {
                                LocalPlayer.Inventory.AddItem(pair.Key, pair.Value);
                            }
                        }
                        LastDayOfGeneration = Clock.Day;
                    }

                }
            }
            catch (Exception e)
            {

                ModAPI.Log.Write("Stats error" + e.ToString());

            }
            try
            {


                if (TimeUntillMassacreReset > 0)
                {
                    TimeUntillMassacreReset -= Time.unscaledDeltaTime;
                    if (TimeUntillMassacreReset <= 0)
                    {
                        AddFinalExperience(Convert.ToInt64((double)NewlyGainedExp * MassacreMultipier));
                        NewlyGainedExp = 0;
                        TimeUntillMassacreReset = 0;
                        MassacreKills = 0;
                        CountMassacre();
                    }


                }
            }
            catch (Exception e)
            {

                ModAPI.Log.Write("Massacre error" + e.ToString());

            }
            if (Stunned)
            {
                if (StunImmune)
                {
                    Stunned = false;
                }

                StunDuration -= Time.deltaTime;
                if (StunDuration < 0)
                {
                    Stunned = false;
                }
            }
        }
        public void Stun(float duration)
        {
            if (StunImmune)
            {
                return;
            }

            Stunned = true;
            if (StunDuration < duration)
            {
                StunDuration = duration;
            }
        }
        public void AddKillExperience(long Amount)
        {
            MassacreKills++;
            CountMassacre();
            if (TimeUntillMassacreReset == 0)
            {
                TimeUntillMassacreReset = MaxMassacreTime;
            }
            else
            {
                TimeUntillMassacreReset = Mathf.Clamp(TimeUntillMassacreReset + TimeBonusPerKill, 20, MaxMassacreTime);
            }
            NewlyGainedExp += Amount;
        }
        public void AddFinalExperience(long Amount)
        {
            ExpCurrent += (long)(Amount * ExpFactor);
            int i = 0;
            while (ModdedPlayer.instance.ExpCurrent >= ModdedPlayer.instance.ExpGoal)
            {
                ModdedPlayer.instance.ExpCurrent -= ModdedPlayer.instance.ExpGoal;
                ModdedPlayer.instance.LevelUp();
                MainMenu.Instance.LevelsToGain++;
                i++;
            }

            if (i > 0)
            {
                if (GameSetup.IsMultiplayer)
                {
                    NetworkManager.SendLine("AL" + ModReferences.ThisPlayerPacked + ";" + ModdedPlayer.instance.Level + ";", NetworkManager.Target.Everyone);
                }
            }
        }
        public void DoOnHit()
        {
            try
            {
                LocalPlayer.Stats.HealthTarget += LifeOnHit * HealingMultipier;
                LocalPlayer.Stats.Health += LifeOnHit * HealingMultipier;
                LocalPlayer.Stats.Energy += EnergyOnHit * StaminaAndEnergyRegenAmp;
            }
            catch (Exception exc)
            {

                ModAPI.Log.Write("Area dmg exception " + exc.ToString());
            }
        }
        public bool DoAreaDamage(Transform rootTR, float damage)
        {
            try
            {
                if (Random.value < AreaDamageProcChance && AreaDamage > 0)
                {
                    DoGuaranteedAreaDamage(rootTR, damage);
                    return true;
                }
            }
            catch (Exception exc)
            {

                ModAPI.Log.Write("Area dmg exception " + exc.ToString());
            }
            return false;
        }
        public void DoGuaranteedAreaDamage(Transform rootTR, float damage)
        {
            RaycastHit[] hits = Physics.SphereCastAll(rootTR.position, AreaDamageRadius, Vector3.one);
            int d = Mathf.FloorToInt(damage * AreaDamage);
            if (d > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].transform.root != rootTR.root)
                    {
                        if (hits[i].transform.tag == "enemyCollide")
                        {
                            if (GameSetup.IsMpClient)
                            {
                                BoltEntity entity = hits[i].transform.GetComponent<BoltEntity>();
                                if (entity == null)
                                {
                                    entity = hits[i].transform.GetComponentInParent<BoltEntity>();
                                }
                                if (entity != null)
                                {
                                    PlayerHitEnemy playerHitEnemy = PlayerHitEnemy.Create(GlobalTargets.OnlyServer);
                                    playerHitEnemy.Hit = d;
                                    playerHitEnemy.Target = entity;
                                    playerHitEnemy.Send();
                                }
                            }
                            else
                            {
                                hits[i].transform.root.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver);
                            }
                        }
                        else if (hits[i].transform.tag == "BreakableWood" || hits[i].transform.tag == "animalCollide")
                        {
                            hits[i].transform.root.SendMessage("Hit", d, SendMessageOptions.DontRequireReceiver);

                        }
                    }
                }
            }
        }

        public void LevelUp()
        {
            MutationPoints++;
            Level++;
            ExpGoal = GetGoalExp();
        }
        public long GetGoalExp()
        {
            return GetGoalExp(Level);
        }
        public long GetGoalExp(int lvl)
        {
            int x = lvl;
            float a = 4f;
            float b = 17.5f;
            float c = 675;
            double y = System.Math.Pow(x, a) * b + c * x;
            return Convert.ToInt64(y);
        }
        public void CountMassacre()
        {
            if (KCInRange(0, 3))
            {
                MassacreMultipier = 1;
                MassacreText = "";
            }
            else if (KCInRange(3, 4))
            {
                MassacreMultipier = 1.1f;
                MassacreText = "TRIPLE KILL";
            }
            else if (KCInRange(4, 5))
            {
                MassacreMultipier = 1.3f;
                MassacreText = "QUADRA KILL";
            }
            else if (KCInRange(5, 6))
            {
                MassacreMultipier = 1.5f;
                MassacreText = "PENTA KILL";
            }
            else if (KCInRange(6, 10))
            {
                MassacreMultipier = 1.75f;
                MassacreText = "MASSACRE";
            }
            else if (KCInRange(10, 12))
            {
                MassacreMultipier = 2.3f;
                MassacreText = "BIG MASSACRE";
            }
            else if (KCInRange(12, 16))
            {
                MassacreMultipier = 3f;
                MassacreText = "BIGGER MASSACRE";
            }
            else if (KCInRange(16, 20))
            {
                MassacreMultipier = 5f;
                MassacreText = "HUGE MASSACRE";
            }
            else if (KCInRange(20, 25))
            {
                MassacreMultipier = 8.5F;
                MassacreText = "BLOODY MASSACRE";
            }
            else if (KCInRange(25, 30))
            {
                MassacreMultipier = 15F;
                MassacreText = "WICKED SICK";
            }
            else if (KCInRange(30, 40))
            {
                MassacreMultipier = 22.5f;
                MassacreText = "UNSTOPPABLE";
            }
            else if (KCInRange(40, 50))
            {
                MassacreMultipier = 35;
                MassacreText = "GODLIKE MASSACRE";
            }
            else if (KCInRange(50, 65))
            {
                MassacreMultipier = 50;
                MassacreText = "BEYOND GODLIKE";
            }
            else if (KCInRange(65, 75))
            {
                MassacreMultipier = 250;
                MassacreText = "SLAUGHTER";
            }
            else if (KCInRange(75, 100))
            {
                MassacreMultipier = 1000;
                MassacreText = "BLOODBATH";
            }
            else if (MassacreKills >= 100)
            {
                MassacreMultipier = 10000;
                MassacreText = "R A M P A G E";
            }
        }

        private bool KCInRange(int min, int max)
        {
            if (MassacreKills >= min && MassacreKills < max)
            {
                return true;
            }
            return false;
        }

        private void FinishMassacre()
        {

            TimeUntillMassacreReset = 0;
            long Amount = Convert.ToInt64((double)NewlyGainedExp * MassacreMultipier);
            NewlyGainedExp = 0;
            MassacreMultipier = 1;
            AddFinalExperience(Amount);
        }
    }
}
