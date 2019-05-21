using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheForest.Utils;
using UnityEngine;

namespace TheFlash
{
    class Class1 : FirstPersonCharacter
    {
     
        protected override void FixedUpdate()
        {
            maximumVelocity = 100000000;
            maxVelocityChange = 100000000;
            storeMaxVelocity = 100000000;
           
            base.FixedUpdate();
            maximumVelocity = 100000000;
            maxVelocityChange = 100000000;
            storeMaxVelocity = 100000000;

            if (LocalPlayer.AnimControl.doShellRideMode)
            {
                if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
                {
                    rb.AddForce(transform.forward *15, ForceMode.Acceleration);
                    LocalPlayer.Stats.Stamina -= 3*Time.deltaTime;
                }
            }
        }


      
        protected override void HandleFrictionParams()
        {
            if (LocalPlayer.AnimControl.doShellRideMode)
            {
                if (LocalPlayer.Stats.IsInNorthColdArea())
                {
                    playerPhysicMaterial.staticFriction = 0.0f;
                    playerPhysicMaterial.dynamicFriction = 0.0f;
                }
                else
                {
                    playerPhysicMaterial.staticFriction = 0.0f;
                    playerPhysicMaterial.dynamicFriction = 0.0f;
                }
                if (jumpingTimer > 3f)
                {
                    playerPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
                    playerPhysicMaterial.bounciness = 0f;
                }
                else
                {
                    playerPhysicMaterial.bounceCombine = PhysicMaterialCombine.Average;
                    playerPhysicMaterial.bounciness = 0.9f;
                }
                playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            }
            else if (!Grounded)
            {
                playerPhysicMaterial.staticFriction = 0f;
                playerPhysicMaterial.dynamicFriction = 0f;
                playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
                playerPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
                playerPhysicMaterial.bounciness = 0f;
            }
            else if (collFlags.groundAngleVal > extremeAngleGroundedLimit)
            {
                playerPhysicMaterial.staticFriction = 0f;
                playerPhysicMaterial.dynamicFriction = 0f;
                playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
                playerPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
                playerPhysicMaterial.bounciness = 0f;
            }
            else if (inputVelocity.magnitude < 0.05f)
            {
                playerPhysicMaterial.staticFriction = 1f;
                playerPhysicMaterial.dynamicFriction = 1f;
                playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Average;
                playerPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
                playerPhysicMaterial.bounciness = 0f;
            }
            else
            {
                playerPhysicMaterial.staticFriction = 0.01f;
                playerPhysicMaterial.dynamicFriction = 0.01f;
                playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
                playerPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
                playerPhysicMaterial.bounciness = 0f;
            }
        }
    }
}
