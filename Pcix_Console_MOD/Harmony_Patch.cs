using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Pcix_Console_MOD
{
    public class Harmony_Patch
    {
        public Harmony_Patch()
        {
            try
            {
                Harmony harmony = new Harmony("Pcix_Console_MOD");
                harmony.Patch(typeof(DebugConsoleScript).GetMethod("Update", AccessTools.all), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("Pcix_Console_MOD_Update")), null, null);
                harmony.Patch(typeof(StageController).GetMethod("OnUpdate", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("Pcix_Console_MOD_OnUpdate")), null, null, null);
                harmony.Patch(typeof(DebugConsoleScript).GetMethod("Init", AccessTools.all), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("Pcix_Console_MOD_Init")), null, null, null);
                harmony.Patch(typeof(DebugConsoleScript).GetMethod("ExtractComand", AccessTools.all), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("Pcix_Console_MOD_ExtractComand")), null, null);
            }
            catch (Exception ex)
            {
                File.WriteAllText(Application.dataPath + "/BaseMods/Pcix_Console_MOD_ERROR.txt", ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public static void Pcix_Console_MOD_Update(DebugConsoleScript __instance)
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                __instance.GetType().GetMethod("SetActive", BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null).Invoke(__instance, null);
            }
            if (Input.GetKeyDown(KeyCode.Return) && __instance.ActiveRoot.active)
            {
                __instance.GetType().GetMethod("ExtractComand", AccessTools.all).Invoke(__instance, new object[]
                {
                    __instance.inputField.text
                });
                __instance.inputField.text = null;
            }
        }

        public static bool Pcix_Console_MOD_OnUpdate()
        {
            return !SingletonBehavior<DebugConsoleScript>.Instance.ActiveRoot.active;
        }

        public static void Pcix_Console_MOD_Init(DebugConsoleScript __instance)
        {
            __instance.dic.Add("help", new DebugConsoleScript.DebugCmdParam(new DebugConsoleScript.ParamType[0]));
        }

        public static void HelpCommand(DebugConsoleScript instance)
        {
            foreach (string text in instance.dic.Keys.ToArray<string>())
            {
                DebugConsoleScript.DebugCmdParam debugCmdParam;
                instance.dic.TryGetValue(text, out debugCmdParam);
                instance.GetType().GetMethod("AppendLine", AccessTools.all).Invoke(instance, new object[]
                {
                    text + " " + string.Join<DebugConsoleScript.ParamType>(" ", debugCmdParam.parameters.ToArray())
                });
            }
        }

        public static void Pcix_Console_MOD_ExtractComand(DebugConsoleScript __instance, ref string text)
        {
            if (text == "help")
            {
                Harmony_Patch.HelpCommand(__instance);
            }
        }
    }
}
