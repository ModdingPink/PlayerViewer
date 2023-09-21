using HarmonyLib;
using PlayerViewer.Handlers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PlayerViewer.Patches
{
    internal class ViewUsernamePatch
    {

        static PlayerHUD CreateText(string username, WizardHUD _wizardHUD)
        {
            float lowestY = _wizardHUD._bottomHUDContainer.transform.position.y + 1.2f;
            if (_wizardHUD._bottomHUDContainer != null)
            {
                foreach (var T in _wizardHUD._bottomHUDContainer.GetComponentsInChildren<Transform>())
                {
                    if (T.transform == _wizardHUD._bottomHUDContainer.transform)
                        continue;

                    Debug.Log(T.name);
                    if (T.position.y < lowestY)
                    {
                        Debug.Log("Lower: " + T.name);
                        lowestY = T.position.y;
                    }
                }
            }

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
                lowestY -= 0.2f;
                NameHUD.transform.position = new Vector3(_wizardHUD._bottomHUDContainer.transform.position.x + 0.35f, lowestY, 0);
            }

            var playerHUD = NameHUD.gameObject.AddComponent<PlayerHUD>();

            playerHUD.Init(NameHUD.GetComponentInChildren<TextMeshPro>(), username);
            WBTools.SetLayerRecursively(NameHUD.gameObject, _wizardHUD._bottomHUDContainer.layer, WBTools.LayerType.ANY);

            return playerHUD;

        }



        static List<PlayerHUD> PlayerUIItems = new List<PlayerHUD>();


        [HarmonyPatch(typeof(AbstractGameTypeController))]
        [HarmonyPatch("_FinishGame")]
        internal class AbstractGameTypeController_GamesFinished
        {
            static void Postfix(AbstractGameTypeController __instance)
            {
                HideUI();
            }
        }


        static void HideUI()
        {
            foreach(var HUD in PlayerUIItems)
            {
                if(HUD != null)
                {
                    if(HUD.isActiveAndEnabled)
                        HUD.Finish();
                }
            }
            PlayerUIItems.Clear();
        }


        [HarmonyPatch(typeof(LocalGamePlayController))]
        [HarmonyPatch("_Init")]
        internal class WizardHUD_SetupButtonViews
        {
            static void Postfix(LocalGamePlayController __instance)
            {
                var HUD = CreateText(__instance._wizard._user.username, __instance._wizardHUD);
                PlayerUIItems.Add(HUD);
            }
        }

        [HarmonyPatch(typeof(RemoteGamePlayController))]
        [HarmonyPatch("Enable")]
        internal class AbstractHUD_SetBrickPack
        {
            static void Postfix(RemoteGamePlayController __instance)
            {
                var HUD = CreateText(__instance._netPlayer.username, __instance._wizardHUD);
                PlayerUIItems.Add(HUD);
            }

        }        

    }
}
