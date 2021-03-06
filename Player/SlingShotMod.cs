﻿using TheForest.Items.World;
using TheForest.Utils;
using UnityEngine;
namespace ChampionsOfForest.Player
{
    public class SlingShotMod : slingShotController
    {
        public override void fireProjectile()
        {
            if (LocalPlayer.Inventory.RemoveItem(_ammoItemId, 1, false, true))
            {
                Vector3 position = _ammoSpawnPos.transform.position;
                Quaternion rotation = _ammoSpawnPos.transform.rotation;
                if (ForestVR.Enabled)
                {
                    position = _ammoSpawnPosVR.transform.position;
                    rotation = _ammoSpawnPosVR.transform.rotation;
                }
                GameObject gameObject = Object.Instantiate(_Ammo, position, rotation);
                gameObject.transform.localScale *= ModdedPlayer.instance.ProjectileSizeRatio;
                Rigidbody component = gameObject.GetComponent<Rigidbody>();
                rockSound component2 = gameObject.GetComponent<rockSound>();
                if ((bool)component2)
                {
                    component2.slingShot = true;
                }
                if (BoltNetwork.isRunning)
                {
                    BoltEntity component3 = gameObject.GetComponent<BoltEntity>();
                    if ((bool)component3)
                    {
                        BoltNetwork.Attach(gameObject);
                    }
                }
                PickUp componentInChildren = gameObject.GetComponentInChildren<PickUp>();
                if ((bool)componentInChildren)
                {
                    SheenBillboard[] componentsInChildren = gameObject.GetComponentsInChildren<SheenBillboard>();
                    SheenBillboard[] array = componentsInChildren;
                    foreach (SheenBillboard sheenBillboard in array)
                    {
                        sheenBillboard.gameObject.SetActive(false);
                    }
                    componentInChildren.gameObject.SetActive(false);
                    if (base.gameObject.activeInHierarchy)
                    {
                        base.StartCoroutine(enablePickupTrigger(componentInChildren.gameObject));
                    }
                }
                Vector3 forward = _ammoSpawnPos.transform.forward;
                if (ForestVR.Enabled)
                {
                    forward = _ammoSpawnPosVR.transform.forward;
                }
                component.AddForce(4000f * ModdedPlayer.instance.ProjectileSpeedRatio * (0.016666f / Time.fixedDeltaTime) * forward);
            }

        }
    }
}
