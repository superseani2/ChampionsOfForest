﻿using FMOD.Studio;
using System;
using TheForest.Items.Inventory;
using TheForest.Tools;
using TheForest.Utils;
using TheForest.Utils.Settings;
using UnityEngine;

namespace ChampionsOfForest
{
    public class PlayerStatsEx : PlayerStats
    {
        protected override void Update()
        {
            if (!(Scene.Atmosphere == null) && !SteamDSConfig.isDedicatedServer)
            {
                float num = Convert.ToSingle(LocalPlayer.Stats.DaySurvived + TheForest.Utils.Scene.Atmosphere.DeltaTimeOfDay);
                if (Mathf.FloorToInt(num) != Mathf.FloorToInt(LocalPlayer.Stats.DaySurvived))
                {
                    LocalPlayer.Stats.DaySurvived = num;
                    EventRegistry.Player.Publish(TfEvent.SurvivedDay, null);
                }
                else
                {
                    LocalPlayer.Stats.DaySurvived = num;
                }
                LocalPlayer.ScriptSetup.targetInfo.isRed = IsRed;
                float num2 = 0f;
                num2 = ((!coldSwitch || LocalPlayer.AnimControl.coldOffsetBool) ? 0f : 1f);
                coldFloatBlend = Mathf.Lerp(coldFloatBlend, num2, Time.deltaTime * 10f);
                if (coldFloatBlend > 0.01f)
                {
                    BoltSetReflectedShim.SetFloatReflected(animator, "coldFloat", coldFloatBlend);
                }
                else
                {
                    BoltSetReflectedShim.SetFloatReflected(animator, "coldFloat", 0f);
                }
                if (Run && HeartRate < 170)
                {
                    HeartRate++;
                }
                else if (!Run && HeartRate > 70)
                {
                    HeartRate--;
                }
                if (Sitted)
                {
                    Energy += 3f * Time.deltaTime;
                }
                if (!Clock.Dark && IsCold && !LocalPlayer.IsInCaves && !IsInNorthColdArea())
                {
                    goto IL_01cb;
                }
                if (LocalPlayer.IsInEndgame)
                {
                    goto IL_01cb;
                }
                goto IL_01e2;
            }
            return;
        IL_01e2:
            if (IsInNorthColdArea() && !Warm)
            {
                SetCold(true);
            }
            if (ShouldDoWetColdRoll && !IsCold && (LocalPlayer.IsInCaves || Clock.Dark))
            {
                if (!LocalPlayer.Buoyancy.InWater)
                {
                    ShouldDoWetColdRoll = false;
                }
                else if (LocalPlayer.IsInCaves)
                {
                    if (LocalPlayer.AnimControl.swimming)
                    {
                        if (Time.time - CaveStartSwimmingTime > 12f)
                        {
                            SetCold(true);
                            ShouldDoWetColdRoll = false;
                        }
                    }
                    else
                    {
                        CaveStartSwimmingTime = Time.time;
                    }
                }
                else
                {
                    Vector3 position = LocalPlayer.Transform.position;
                    if (position.y - LocalPlayer.Buoyancy.WaterLevel < 1f)
                    {
                        if (UnityEngine.Random.Range(0, 100) < 30)
                        {
                            SetCold(true);
                        }
                        ShouldDoWetColdRoll = false;
                    }
                }
            }
            if (ShouldDoGotCleanCheck)
            {
                if (!LocalPlayer.Buoyancy.InWater)
                {
                    ShouldDoGotCleanCheck = false;
                }
                else
                {
                    Vector3 position2 = LocalPlayer.ScriptSetup.hipsJnt.position;
                    if (position2.y - LocalPlayer.Buoyancy.WaterLevel < -0.5f)
                    {
                        ShouldDoGotCleanCheck = false;
                        GotCleanReal();
                    }
                }
            }
            if (Health <= GreyZoneThreshold && AudioListener.volume > 0.2f)
            {
                AudioListener.volume -= 0.1f * Time.deltaTime;
            }
            else if (AudioListener.volume < 1f)
            {
                AudioListener.volume += 0.1f * Time.deltaTime;
            }
            if (IsHealthInGreyZone)
            {
                Tuts.LowHealthTutorial();
            }
            else
            {
                Tuts.CloseLowHealthTutorial();
            }
            if (Energy < 30f)
            {
                Tuts.LowEnergyTutorial();
            }
            else
            {
                Tuts.CloseLowEnergyTutorial();
            }
            if (Stamina <= 10f && !IsTired)
            {
                base.SendMessage("PlayStaminaBreath");
                IsTired = true;
                Run = false;
            }
            if (Stamina > 10f && IsTired)
            {
                IsTired = false;
            }
            fsmStamina.Value = Stamina;
            fsmMaxStamina.Value = Energy;
            HealthResult = Health / ChampionsOfForest.ModdedPlayer.instance.MaxHealth + (ChampionsOfForest.ModdedPlayer.instance.MaxHealth - Health) / ChampionsOfForest.ModdedPlayer.instance.MaxHealth * 0.5f;
            float num3 = HealthTarget / ChampionsOfForest.ModdedPlayer.instance.MaxHealth + (ChampionsOfForest.ModdedPlayer.instance.MaxHealth - HealthTarget) / ChampionsOfForest.ModdedPlayer.instance.MaxHealth * 0.5f;
            if (HealthTargetResult < num3)
            {
                HealthTargetResult = Mathf.MoveTowards(HealthTargetResult, num3, 1f * Time.fixedDeltaTime);
            }
            else
            {
                HealthTargetResult = num3;
            }
            StaminaResult = Stamina / ChampionsOfForest.ModdedPlayer.instance.MaxEnergy + (ChampionsOfForest.ModdedPlayer.instance.MaxEnergy - Stamina) / ChampionsOfForest.ModdedPlayer.instance.MaxEnergy * 0.5f;
            EnergyResult = Energy / ChampionsOfForest.ModdedPlayer.instance.MaxEnergy + (ChampionsOfForest.ModdedPlayer.instance.MaxEnergy - Energy) / ChampionsOfForest.ModdedPlayer.instance.MaxEnergy * 0.5f;
            int num4 = 0;
            int num5 = 0;
            for (int i = 0; i < CurrentArmorTypes.Length; i++)
            {
                switch (CurrentArmorTypes[i])
                {
                    case ArmorTypes.DeerSkin:
                    case ArmorTypes.Warmsuit:
                        num5++;
                        break;
                    case ArmorTypes.LizardSkin:
                    case ArmorTypes.Leaves:
                    case ArmorTypes.Bone:
                        num4++;
                        break;
                    case ArmorTypes.Creepy:
                        num4++;
                        break;
                }
            }
            ColdArmorResult = num5 / 10f / 2f + 0.5f;
            ArmorResult = num4 / 10f / 2f + ColdArmorResult;
            Hud.ColdArmorBar.fillAmount = ColdArmorResult;
            Hud.ArmorBar.fillAmount = ArmorResult;
            Hud.StaminaBar.fillAmount = StaminaResult;
            Hud.HealthBar.fillAmount = HealthResult;
            Hud.HealthBarTarget.fillAmount = HealthTargetResult;
            Hud.EnergyBar.fillAmount = EnergyResult;
            float num6 = (Fullness - 0.2f) / 0.8f;
            TheForest.Utils.Scene.HudGui.Stomach.fillAmount = Mathf.Lerp(0.21f, 0.81f, num6);
            if (num6 < 0.5)
            {
                Hud.StomachOutline.SetActive(true);
                if (!Hud.Tut_Hungry.activeSelf)
                {
                    Tuts.HungryTutorial();
                }
            }
            else
            {
                if (Hud.Tut_Hungry.activeSelf)
                {
                    Tuts.CloseHungryTutorial();
                }
                Hud.StomachOutline.SetActive(false);
            }
            if (!TheForest.Utils.Scene.Atmosphere.Sleeping || Fullness > StarvationSettings.SleepingFullnessThreshold)
            {
                Fullness -= Convert.ToSingle(TheForest.Utils.Scene.Atmosphere.DeltaTimeOfDay * 1.3500000238418579);
            }
            if (!Cheats.NoSurvival)
            {
                if (Fullness < 0.2f)
                {
                    if (Fullness < 0.19f)
                    {
                        Fullness = 0.19f;
                    }
                    if (DaySurvived >= StarvationSettings.StartDay && !Dead && !TheForest.Utils.Scene.Atmosphere.Sleeping && LocalPlayer.Inventory.enabled)
                    {
                        if (!TheForest.Utils.Scene.HudGui.StomachStarvation.gameObject.activeSelf)
                        {
                            if (Starvation == 0f)
                            {
                                StarvationCurrentDuration = StarvationSettings.Duration;
                            }
                            TheForest.Utils.Scene.HudGui.StomachStarvation.gameObject.SetActive(true);
                        }
                        Starvation += Convert.ToSingle(TheForest.Utils.Scene.Atmosphere.DeltaTimeOfDay / StarvationCurrentDuration);
                        if (Starvation >= 1f)
                        {
                            if (!StarvationSettings.TakingDamage)
                            {
                                StarvationSettings.TakingDamage = true;
                                LocalPlayer.Tuts.ShowStarvationTut();
                            }
                            Hit(StarvationSettings.Damage, true, DamageType.Physical);
                            TheForest.Utils.Scene.HudGui.StomachStarvationTween.ResetToBeginning();
                            TheForest.Utils.Scene.HudGui.StomachStarvationTween.PlayForward();
                            Starvation = 0f;
                            StarvationCurrentDuration *= StarvationSettings.DurationDecay;
                        }
                        TheForest.Utils.Scene.HudGui.StomachStarvation.fillAmount = Mathf.Lerp(0.21f, 0.81f, Starvation);
                    }
                }
                else if (Starvation > 0f || TheForest.Utils.Scene.HudGui.StomachStarvation.gameObject.activeSelf)
                {
                    Starvation = 0f;
                    StarvationCurrentDuration = StarvationSettings.Duration;
                    StarvationSettings.TakingDamage = false;
                    LocalPlayer.Tuts.StarvationTutOff();
                    TheForest.Utils.Scene.HudGui.StomachStarvation.gameObject.SetActive(false);
                }
            }
            else
            {
                Fullness = 1f;
                if (Starvation > 0f || TheForest.Utils.Scene.HudGui.StomachStarvation.gameObject.activeSelf)
                {
                    Starvation = 0f;
                    StarvationCurrentDuration = StarvationSettings.Duration;
                    StarvationSettings.TakingDamage = false;
                    TheForest.Utils.Scene.HudGui.StomachStarvation.gameObject.SetActive(false);
                }
            }
            if (Fullness > 1f)
            {
                Fullness = 1f;
            }
            if (!Cheats.NoSurvival)
            {
                if (DaySurvived >= ThirstSettings.StartDay && !Dead && LocalPlayer.Inventory.enabled)
                {
                    if (Thirst >= 1f)
                    {
                        if (!TheForest.Utils.Scene.HudGui.ThirstDamageTimer.gameObject.activeSelf)
                        {
                            TheForest.Utils.Scene.HudGui.ThirstDamageTimer.gameObject.SetActive(true);
                        }
                        if (ThirstCurrentDuration <= 0f)
                        {
                            ThirstCurrentDuration = ThirstSettings.DamageDelay;
                            if (!ThirstSettings.TakingDamage)
                            {
                                ThirstSettings.TakingDamage = true;
                                LocalPlayer.Tuts.ShowThirstTut();
                            }
                            Hit(Mathf.CeilToInt(ThirstSettings.Damage * GameSettings.Survival.ThirstDamageRatio), true, DamageType.Physical);
                            BleedBehavior.BloodAmount += 0.6f;
                            TheForest.Utils.Scene.HudGui.ThirstDamageTimerTween.ResetToBeginning();
                            TheForest.Utils.Scene.HudGui.ThirstDamageTimerTween.PlayForward();
                        }
                        else
                        {
                            ThirstCurrentDuration -= Time.deltaTime;
                            TheForest.Utils.Scene.HudGui.ThirstDamageTimer.fillAmount = 1f - ThirstCurrentDuration / ThirstSettings.DamageDelay;
                        }
                    }
                    else if (Thirst < 0f)
                    {
                        Thirst = 0f;
                    }
                    else
                    {
                        if (!TheForest.Utils.Scene.Atmosphere.Sleeping || Thirst < ThirstSettings.SleepingThirstThreshold)
                        {
                            Thirst += Convert.ToSingle(TheForest.Utils.Scene.Atmosphere.DeltaTimeOfDay / ThirstSettings.Duration * GameSettings.Survival.ThirstRatio);
                        }
                        if (Thirst > ThirstSettings.TutorialThreshold)
                        {
                            LocalPlayer.Tuts.ShowThirstyTut();
                            TheForest.Utils.Scene.HudGui.ThirstOutline.SetActive(true);
                        }
                        else
                        {
                            LocalPlayer.Tuts.HideThirstyTut();
                            TheForest.Utils.Scene.HudGui.ThirstOutline.SetActive(false);
                        }
                        if (ThirstSettings.TakingDamage)
                        {
                            ThirstSettings.TakingDamage = false;
                            LocalPlayer.Tuts.ThirstTutOff();
                        }
                        if (TheForest.Utils.Scene.HudGui.ThirstDamageTimer.gameObject.activeSelf)
                        {
                            TheForest.Utils.Scene.HudGui.ThirstDamageTimer.gameObject.SetActive(false);
                        }
                    }
                    TheForest.Utils.Scene.HudGui.Hydration.fillAmount = 1f - Thirst;
                }
            }
            else if (TheForest.Utils.Scene.HudGui.Hydration.fillAmount != 1f)
            {
                TheForest.Utils.Scene.HudGui.Hydration.fillAmount = 1f;
            }
            bool flag = false;
            bool flag2 = false;
            if (LocalPlayer.WaterViz.ScreenCoverage > AirBreathing.ScreenCoverageThreshold && !Dead)
            {
                if (!TheForest.Utils.Scene.HudGui.AirReserve.gameObject.activeSelf)
                {
                    TheForest.Utils.Scene.HudGui.AirReserve.gameObject.SetActive(true);
                }
                if (!AirBreathing.UseRebreather && AirBreathing.RebreatherIsEquipped && AirBreathing.CurrentRebreatherAir > 0f)
                {
                    AirBreathing.UseRebreather = true;
                }
                if (AirBreathing.UseRebreather)
                {
                    flag = true;
                    AirBreathing.CurrentRebreatherAir -= Time.deltaTime;
                    TheForest.Utils.Scene.HudGui.AirReserve.fillAmount = AirBreathing.CurrentRebreatherAir / AirBreathing.MaxRebreatherAirCapacity;
                    if (AirBreathing.CurrentRebreatherAir < 0f)
                    {
                        AirBreathing.CurrentLungAir = 0f;
                        AirBreathing.UseRebreather = false;
                    }
                    else if (AirBreathing.CurrentRebreatherAir < AirBreathing.OutOfAirWarningThreshold)
                    {
                        if (!TheForest.Utils.Scene.HudGui.AirReserveOutline.activeSelf)
                        {
                            TheForest.Utils.Scene.HudGui.AirReserveOutline.SetActive(true);
                        }
                    }
                    else if (TheForest.Utils.Scene.HudGui.AirReserveOutline.activeSelf)
                    {
                        TheForest.Utils.Scene.HudGui.AirReserveOutline.SetActive(false);
                    }
                }
                else
                {
                    if (Time.timeScale > 0f)
                    {
                        if (!AirBreathing.CurrentLungAirTimer.IsRunning)
                        {
                            AirBreathing.CurrentLungAirTimer.Start();
                        }
                    }
                    else if (AirBreathing.CurrentLungAirTimer.IsRunning)
                    {
                        AirBreathing.CurrentLungAirTimer.Stop();
                    }
                    if (AirBreathing.CurrentLungAir > AirBreathing.MaxLungAirCapacityFinal)
                    {
                        AirBreathing.CurrentLungAir = AirBreathing.MaxLungAirCapacityFinal;
                    }
                    if (AirBreathing.CurrentLungAir > AirBreathing.CurrentLungAirTimer.Elapsed.TotalSeconds * Skills.LungBreathingRatio)
                    {
                        Skills.TotalLungBreathingDuration += Time.deltaTime;
                        TheForest.Utils.Scene.HudGui.AirReserve.fillAmount = Mathf.Lerp(TheForest.Utils.Scene.HudGui.AirReserve.fillAmount, AirBreathing.CurrentAirPercent, Mathf.Clamp01((Time.time - Time.fixedTime) / Time.fixedDeltaTime));
                        if (!TheForest.Utils.Scene.HudGui.AirReserveOutline.activeSelf)
                        {
                            TheForest.Utils.Scene.HudGui.AirReserveOutline.SetActive(true);
                        }
                    }
                    else if (!Cheats.NoSurvival)
                    {
                        flag2 = true;
                        AirBreathing.DamageCounter += AirBreathing.Damage * Time.deltaTime;
                        if (AirBreathing.DamageCounter >= 1f)
                        {
                            Hit((int)AirBreathing.DamageCounter, true, DamageType.Drowning);
                            AirBreathing.DamageCounter -= (int)AirBreathing.DamageCounter;
                        }
                        if (Dead)
                        {
                            AirBreathing.DamageCounter = 0f;
                            DeadTimes++;
                            TheForest.Utils.Scene.HudGui.AirReserve.gameObject.SetActive(false);
                            TheForest.Utils.Scene.HudGui.AirReserveOutline.SetActive(false);
                        }
                        else if (!TheForest.Utils.Scene.HudGui.AirReserveOutline.activeSelf)
                        {
                            TheForest.Utils.Scene.HudGui.AirReserveOutline.SetActive(true);
                        }
                    }
                }
            }
            else if (AirBreathing.CurrentLungAir < AirBreathing.MaxLungAirCapacityFinal || TheForest.Utils.Scene.HudGui.AirReserve.gameObject.activeSelf)
            {
                if (GaspForAirEvent.Length > 0 && FMOD_StudioSystem.instance && !Dead)
                {
                    FMOD_StudioSystem.instance.PlayOneShot(GaspForAirEvent, base.transform.position, delegate (FMOD.Studio.EventInstance instance)
                    {
                        float value = 85f;
                        if (!AirBreathing.UseRebreather)
                        {
                            value = (AirBreathing.CurrentLungAir - (float)AirBreathing.CurrentLungAirTimer.Elapsed.TotalSeconds) / AirBreathing.MaxLungAirCapacity * 100f;
                        }
                        UnityUtil.ERRCHECK(instance.setParameterValue("oxygen", value));
                        return true;
                    });
                }
                AirBreathing.DamageCounter = 0f;
                AirBreathing.CurrentLungAirTimer.Stop();
                AirBreathing.CurrentLungAirTimer.Reset();
                AirBreathing.CurrentLungAir = AirBreathing.MaxLungAirCapacityFinal;
                TheForest.Utils.Scene.HudGui.AirReserve.gameObject.SetActive(false);
                TheForest.Utils.Scene.HudGui.AirReserveOutline.SetActive(false);
            }
            if (flag)
            {
                UpdateRebreatherEvent();
            }
            else
            {
                StopIfPlaying(RebreatherEventInstance);
            }
            if (flag2)
            {
                UpdateDrowningEvent();
            }
            else
            {
                StopIfPlaying(DrowningEventInstance);
            }
            if (Energy > ChampionsOfForest.ModdedPlayer.instance.MaxEnergy)
            {
                Energy = ChampionsOfForest.ModdedPlayer.instance.MaxEnergy;
            }
            if (Energy < 10f)
            {
                Energy = 10f;
            }
            if (Health < 0f)
            {
                Health = 0f;
            }
            if (Health > ChampionsOfForest.ModdedPlayer.instance.MaxHealth)
            {
                Health = ChampionsOfForest.ModdedPlayer.instance.MaxHealth;
            }
            if (Health < HealthTarget)
            {
                Health = Mathf.MoveTowards(Health, HealthTarget, GameSettings.Survival.HealthRegenPerSecond * Time.deltaTime);
                TheForest.Utils.Scene.HudGui.HealthBarTarget.enabled = true;
            }
            else
            {
                TheForest.Utils.Scene.HudGui.HealthBarTarget.enabled = false;
            }
            if (Health < 20f)
            {
                Hud.HealthBarOutline.SetActive(true);
            }
            else
            {
                Hud.HealthBarOutline.SetActive(false);
            }
            if (Energy < 40f || IsCold)
            {
                Hud.EnergyBarOutline.SetActive(true);
            }
            else
            {
                Hud.EnergyBarOutline.SetActive(false);
            }
            if (Stamina < 30f)
            {
                Hud.StaminaBarOutline.SetActive(true);
            }
            else
            {
                Hud.StaminaBarOutline.SetActive(false);
            }
            if (Stamina < 0f)
            {
                Stamina = 0f;
            }
            if (Stamina < Energy)
            {
                if (!LocalPlayer.FpCharacter.running && !(LocalPlayer.FpCharacter.recoveringFromRun > 0f))
                {
                    Stamina += ModdedPlayer.instance.StaminaRecover * Time.deltaTime;
                }
                else if (LocalPlayer.FpCharacter.recoveringFromRun > 0f)
                {
                    LocalPlayer.FpCharacter.recoveringFromRun -= Time.deltaTime;
                }
            }
            else
            {
                Stamina = Energy;
            }
            if (CheckingBlood && TheForest.Utils.Scene.SceneTracker.proxyAttackers.arrayList.Count > 0)
            {
                StopBloodCheck();
            }
            if (IsCold && !Warm && LocalPlayer.Inventory.enabled)
            {
                if (BodyTemp > 14f)
                {
                    BodyTemp -= 1f * (1f - Mathf.Clamp01(ColdArmor));
                }
                if (FrostDamageSettings.DoDeFrost)
                {
                    if (FrostScript.coverage > FrostDamageSettings.DeFrostThreshold)
                    {
                        FrostScript.coverage -= 0.0159999728f * Time.deltaTime / FrostDamageSettings.DeFrostDuration;
                    }
                    else
                    {
                        FrostDamageSettings.DoDeFrost = false;
                    }
                }
                else if (FrostScript.coverage < 0.49f || ColdArmor >= 1f)
                {
                    if (FrostScript.coverage < 0f)
                    {
                        FrostScript.coverage = 0f;
                    }
                    FrostScript.coverage += 0.01f * Time.deltaTime * (1f - Mathf.Clamp01(ColdArmor)) * GameSettings.Survival.FrostSpeedRatio;
                    if (FrostScript.coverage > 0.492f)
                    {
                        FrostScript.coverage = 0.491f;
                    }
                }
                else if (!Cheats.NoSurvival && TheForest.Utils.Scene.Clock.ElapsedGameTime >= FrostDamageSettings.StartDay && LocalPlayer.Inventory.CurrentView != PlayerInventory.PlayerViews.Book && LocalPlayer.Inventory.CurrentView != PlayerInventory.PlayerViews.Inventory && !LocalPlayer.AnimControl.doShellRideMode)
                {
                    if (!LocalPlayer.FpCharacter.jumping && (!LocalPlayer.AnimControl.onRope || !LocalPlayer.AnimControl.VerticalMovement) && !IsLit && LocalPlayer.Rigidbody.velocity.sqrMagnitude < 0.3f && !Dead)
                    {
                        if (FrostDamageSettings.CurrentTimer >= FrostDamageSettings.Duration)
                        {
                            if (FrostDamageSettings.DamageChance == 0)
                            {
                                Hit((int)(FrostDamageSettings.Damage * GameSettings.Survival.FrostDamageRatio), true, DamageType.Frost);
                                FrostScript.coverage = 0.506f;
                                FrostDamageSettings.DoDeFrost = true;
                                FrostDamageSettings.CurrentTimer = 0f;
                            }
                        }
                        else
                        {
                            FrostDamageSettings.CurrentTimer += Time.deltaTime * ((1f - Mathf.Clamp01(ColdArmor)) * 1f);
                        }
                    }
                    else
                    {
                        FrostDamageSettings.CurrentTimer = 0f;
                    }
                }
            }
            if (Warm)
            {
                if (BodyTemp < 37f)
                {
                    BodyTemp += 1f * (1f + Mathf.Clamp01(ColdArmor));
                }
                if (FrostScript.coverage > 0f)
                {
                    FrostScript.coverage -= 0.01f * Time.deltaTime * (1f + Mathf.Clamp01(ColdArmor)) * GameSettings.Survival.DefrostSpeedRatio;
                    if (FrostScript.coverage < 0f)
                    {
                        FrostScript.coverage = 0f;
                    }
                }
                else
                {
                    FrostDamageSettings.TakingDamage = false;
                }
                FrostDamageSettings.CurrentTimer = 0f;
            }
            if (LocalPlayer.IsInCaves)
            {
                Sanity.InCave();
            }
            if (PlayerSfx.MusicPlaying)
            {
                Sanity.ListeningToMusic();
            }
            if (Sitted)
            {
                Sanity.SittingOnBench();
            }
            Calories.Refresh();
            if (DyingEventInstance != null && !flag2 && !Dead)
            {
                UnityUtil.ERRCHECK(DyingEventInstance.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
                UnityUtil.ERRCHECK(DyingHealthParameter.setValue(Health));
            }
            if (FireExtinguishEventInstance != null)
            {
                UnityUtil.ERRCHECK(FireExtinguishEventInstance.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
            }
            if (Cheats.InfiniteEnergy)
            {
                Energy = ChampionsOfForest.ModdedPlayer.instance.MaxEnergy;
                Stamina = ChampionsOfForest.ModdedPlayer.instance.MaxEnergy;
            }
            if (Cheats.GodMode)
            {
                Health = ChampionsOfForest.ModdedPlayer.instance.MaxHealth;
                HealthTarget = ChampionsOfForest.ModdedPlayer.instance.MaxHealth;
            }
            return;
        IL_01cb:
            SetCold(false);
            FrostScript.coverage = 0f;
            goto IL_01e2;
        }
        public override void AteMeds()
        {
            NormalizeHealthTarget();
            HealthTarget += Mathf.Max(60f * ChampionsOfForest.ModdedPlayer.instance.HealingMultipier, ModdedPlayer.instance.MaxHealth * 0.05f * ModdedPlayer.instance.HealingMultipier);
            BleedBehavior.BloodReductionRatio = Health / ChampionsOfForest.ModdedPlayer.instance.MaxHealth * 1.5f;
        }
        public override void AteAloe()
        {
            NormalizeHealthTarget();
            HealthTarget += 6f * ChampionsOfForest.ModdedPlayer.instance.HealingMultipier;
            BleedBehavior.BloodReductionRatio = Health / ChampionsOfForest.ModdedPlayer.instance.MaxHealth;
        }
        public override void HealthChange(float amount)
        {

            NormalizeHealthTarget();
            if (amount < 0f)
            {
                Health += amount;
                HealthTarget += amount;
            }
            else
            {
                HealthTarget = Mathf.Min(HealthTarget + amount * ChampionsOfForest.ModdedPlayer.instance.HealingMultipier, ChampionsOfForest.ModdedPlayer.instance.MaxHealth);
            }

        }
    }
}