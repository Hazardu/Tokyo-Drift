
using System.IO;
using TheForest.Utils;
using UnityEngine;

namespace TokyoDrift
{
    public static class ModSettings
    {
        public static float bounciness = 0.9f, friction = 0, acceleration = 15, bouncetimer = 3, turnRate = 1f;
        public static void SendSyncEvent()
        {
            using (MemoryStream stream = new MemoryStream(sizeof(int) + sizeof(float) * 5))
            {
                using (BinaryWriter w = new BinaryWriter(stream))
                {
                    w.Write(1);
                    w.Write(bounciness);
                    w.Write(acceleration);
                    w.Write(bouncetimer);
                    w.Write(friction);
                    w.Write(turnRate);
                }
                NetworkManager.SendCommand(stream.ToArray(), Bolt.GlobalTargets.Everyone);
            }
        }
    }


    public class ShellModOptionsGUI : MonoBehaviour
    {
        static ShellModOptionsGUI instance;
        [ModAPI.Attributes.ExecuteOnGameStart]

        static void Init()
        {
            instance = new GameObject().AddComponent<ShellModOptionsGUI>();
        }
        void Start()
        {
            Reset();
        }
        public static void Reset()
        {
            instance.textf = new string[5];
            for (int i = 0; i < 5; i++)
            {
                instance.textf[i] = "";
            }
        }
        bool _show = false;
        void Update()
        {
            if (ModAPI.Input.GetButtonDown("Menu"))
            {
                _show = !_show;
                if (LocalPlayer.FpCharacter)
                {

                if (_show)
                    LocalPlayer.FpCharacter.LockView(true);
                else
                    LocalPlayer.FpCharacter.UnLockView();
                }
            }
        }
        string[] textf = new string[5];
        void DrawTF(ref float val, int i)
        {
            if (textf[i] == "")
                textf[i] = "" + val;
            textf[i] = GUILayout.TextField(textf[i]);
            if (float.TryParse(textf[i], out float f))
            {
                val = f;
            }
        }
        void DrawSlider(ref float val, float min, float max, int i)
        {
            float f = GUILayout.HorizontalSlider(val, min, max);
            if (val <= max && val != f)
            {
                val = f;
                textf[i] = "" + f;
            }
        }
        void OnGUI()
        {
            if (!_show) return;
            int i = 0;
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("friction: (default 0.45 or 0.15 for snow area) = " + ModSettings.friction.ToString("N2"));
            DrawTF(ref ModSettings.friction, i);
            DrawSlider(ref ModSettings.friction, 0f, 1f, i);
            i++;
            GUILayout.Space(15f);

            GUILayout.Label("bounciness: (default 0.9)" + ModSettings.bounciness.ToString("N2"));
            DrawTF(ref ModSettings.bounciness, i);
            DrawSlider(ref ModSettings.bounciness, 0f, 1f, i);
            i++;
            GUILayout.Space(15f);

            GUILayout.Label("shift key acceleration: (default 0) " + ModSettings.acceleration.ToString("N2"));
            DrawTF(ref ModSettings.acceleration, i);
            DrawSlider(ref ModSettings.acceleration, 0f, 100f, i);
            i++;
            GUILayout.Space(15f);

            GUILayout.Label("turn rate: (default 1) " + ModSettings.turnRate.ToString("N2"));
            DrawTF(ref ModSettings.turnRate, i);
            DrawSlider(ref ModSettings.turnRate, 0f, 100f, i);
            i++;
            GUILayout.Space(15f);

            GUILayout.Label("bounce dampening timer: (default 3) aka airtime to disable bounciness on landing" + ModSettings.bouncetimer.ToString("N2"));
            DrawTF(ref ModSettings.bouncetimer, i);
            i++;
            GUILayout.Space(55f);

            if (GUILayout.Button("Sync with other players"))
            {
                ModSettings.SendSyncEvent();
            }

            GUILayout.EndVertical();

        }
    }
    public class FirstPersonCharacterTokioDrift : FirstPersonCharacter
    {


        protected override void FixedUpdate()
        {
            maximumVelocity = 100000000;
            maxVelocityChange = 100000000;
            storeMaxVelocity = 100000000;
            if (LocalPlayer.AnimControl.doShellRideMode)
            {
                if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
                {
                    if (LocalPlayer.Stats.Stamina > 4)
                    {
                        var forceAcc = transform.forward * ModSettings.acceleration;
                        rb.AddForce(forceAcc, ForceMode.Acceleration);
                    }
                    LocalPlayer.Stats.Stamina -= 3 * Time.deltaTime;
                }
            }

            base.FixedUpdate();
            maximumVelocity = 100000000;
            maxVelocityChange = 100000000;
            storeMaxVelocity = 100000000;

        }



        protected override void HandleFrictionParams()
        {
            if (LocalPlayer.AnimControl.doShellRideMode)
            {

                playerPhysicMaterial.staticFriction = ModSettings.friction;
                playerPhysicMaterial.dynamicFriction = ModSettings.friction;
                if (jumpingTimer > ModSettings.bouncetimer)
                {
                    playerPhysicMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
                    playerPhysicMaterial.bounciness = 0f;
                }
                else
                {
                    playerPhysicMaterial.bounceCombine = PhysicMaterialCombine.Average;
                    playerPhysicMaterial.bounciness = ModSettings.bounciness;
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
    public class playerAnimatorControlTokioDrift : playerAnimatorControl
    {
        protected override void HandleShellRideControlUpdate()
        {
            this.hVel = Mathf.SmoothDamp(this.hVel, TheForest.Utils.Input.GetAxis("Horizontal") * 75f, ref this.curVel, 0.1f);
            this.shellBlendVal = Mathf.SmoothStep(this.hVel, this.shellBlendVal, Time.deltaTime / 9f);
            this.animator.SetFloatReflected("shellBlend", this.shellBlendVal);
            if (Mathf.Abs(this.hVel) > 0.0001f)
            {
                this.rootTr.RotateAround(base.transform.position, Vector3.up, this.hVel * ModSettings.turnRate * Time.deltaTime);
            }
            else
            {
                this.curVel = 0f;
            }
            Vector3 vector = this.tr.forward * -1f;
        }
    }


}
