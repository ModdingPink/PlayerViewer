using ExampleMod;
using HarmonyLib;
using System;
using TMPro;
using UnityEngine;

namespace PlayerViewer.Patches
{
    internal class ViewUsernamePatch
    {

        static void CreateText(string username, WizardHUD _wizardHUD)
        {
            var NameHUD = _wizardHUD._bottomHUDContainer.transform.Find("NameHUD");
            if (NameHUD == null)
            {
                NameHUD = Singleton<ResourceManager>.instance.InstantiateByName("WIZARD_HUD_RANK", new Vector3(0f, 1f, 0f), _wizardHUD._bottomHUDContainer).transform;
                WBTools.SetLayerRecursively(NameHUD.gameObject, _wizardHUD._bottomHUDContainer.layer, WBTools.LayerType.ANY);
                NameHUD.SetParent(_wizardHUD._bottomHUDContainer.transform, false);
                NameHUD.name = "NameHUD";
                GameObject.Destroy(NameHUD.Find("LabelText").gameObject);
                var text = NameHUD.Find("LabelNumber").GetComponentInChildren<TextMeshPro>();
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 10;
                NameHUD.transform.position += new Vector3(0.45f, -2, 0);

            }
            NameHUD.GetComponentInChildren<TextMeshPro>().text = (username);
        }



        [HarmonyPatch(typeof(WizardHUD))]
        [HarmonyPatch("_SetupButtonViews")]
        internal class WizardHUD_SetupButtonViews
        {
            static void Postfix(WizardHUD __instance)
            {
                var NameHUD = __instance._bottomHUDContainer.transform.Find("NameHUD");
                if (NameHUD == null)
                {
                    CreateText(__instance._wizard._user.username, __instance);
                }
            }
        }



        [HarmonyPatch(typeof(RemoteGamePlayController))]
        [HarmonyPatch("Enable")]
        internal class RemoteGameControllerEnable
        {
            static void Postfix(RemoteGamePlayController __instance)
            {
                CreateText(__instance._netPlayer.username, __instance._wizardHUD);
            }

        }        

    }
}
