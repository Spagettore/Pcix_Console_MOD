using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UI;
using System.Collections.Generic;

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
                AppendLine(__instance.inputField.text);
                __instance.GetType().GetMethod("ExtractComand", AccessTools.all).Invoke(__instance, new object[]
                {
                    __instance.inputField.text
                });
            }
        }

        public static bool Pcix_Console_MOD_OnUpdate()
        {
            return !SingletonBehavior<DebugConsoleScript>.Instance.ActiveRoot.active;
        }

        public static void Pcix_Console_MOD_Init(DebugConsoleScript __instance)
        {
            DebugConsoleScript.ParamType[] nullparam = new DebugConsoleScript.ParamType[0];
            __instance.dic.Add("help", new DebugConsoleScript.DebugCmdParam(nullparam));
            __instance.dic.Add("battleprepare", new DebugConsoleScript.DebugCmdParam(nullparam).OnlyBattle());
            DebugConsoleScript.ParamType[] param = new DebugConsoleScript.ParamType[]
            {
                DebugConsoleScript.ParamType.BOOL
            };
            __instance.dic.Add("showids", new DebugConsoleScript.DebugCmdParam(param).OnlyBattle());
            DebugConsoleScript.ParamType[] param1 = new DebugConsoleScript.ParamType[]
            {
                DebugConsoleScript.ParamType.STRING,
                DebugConsoleScript.ParamType.INT
            };
            __instance.dic.Add("removeunit", new DebugConsoleScript.DebugCmdParam(param1).OnlyBattle());
            DebugConsoleScript.ParamType[] param2 = new DebugConsoleScript.ParamType[]
            {
                DebugConsoleScript.ParamType.INT,
                DebugConsoleScript.ParamType.INT
            };
            __instance.dic.Add("addunit", new DebugConsoleScript.DebugCmdParam(param2).OnlyBattle());
            __instance.dic.Add("restorelight", new DebugConsoleScript.DebugCmdParam(param2).OnlyBattle());
            __instance.dic.Add("drawcards", new DebugConsoleScript.DebugCmdParam(param2).OnlyBattle());
            DebugConsoleScript.ParamType[] param3 = new DebugConsoleScript.ParamType[]
            {
                DebugConsoleScript.ParamType.INT
            };
            __instance.dic.Add("fullhealth", new DebugConsoleScript.DebugCmdParam(param3).OnlyBattle());
            __instance.dic.Add("revive", new DebugConsoleScript.DebugCmdParam(param3).OnlyBattle());
        }
        public static void Pcix_Console_MOD_ExtractComand(DebugConsoleScript __instance, ref string text)
        {
            string[] array = text.Split(new char[]
            {
            ' '
            });
            DebugConsoleScript.DebugCmdParam debugCmdParam;
            if (__instance.dic.TryGetValue(array[0], out debugCmdParam))
            {
                if (debugCmdParam.CheckParameterValidate(array))
                {
                    Harmony_Patch.ExecuteModdedCommand(array[0], debugCmdParam);
                }
            }
        }
        public static void ExecuteModdedCommand(string cmd, DebugConsoleScript.DebugCmdParam cmdParam)
        {
            if (cmdParam.onlyBattle && Singleton<StageController>.Instance.State != StageController.StageState.Battle)
            {
                return;
            }
            switch (cmd)
            {
                case "help":
                Harmony_Patch.HelpCommand();
                    break;
                case "showids":
                    bool state = DebugConsoleScript.ParamParser.GetBool(cmdParam.paramValue[0]);
                    Harmony_Patch.ShowIds(state);
                    break;
                case "removeunit":
                    string team = DebugConsoleScript.ParamParser.GetString(cmdParam.paramValue[0]);
                    int index = DebugConsoleScript.ParamParser.GetInteger(cmdParam.paramValue[1]);
                    if (team == "player")
                    {
                        RemoveUnit(Faction.Player, index);
                        break;
                    }
                    RemoveUnit(Faction.Enemy, index);
                    break;
                case "addunit":
                    index = DebugConsoleScript.ParamParser.GetInteger(cmdParam.paramValue[0]);
                    int id = DebugConsoleScript.ParamParser.GetInteger(cmdParam.paramValue[1]);
                    AddUnit(id, index);
                    break;
                case "battleprepare":
                    OpenBattlePrepare();
                    break;
                case "restorelight":
                    index = DebugConsoleScript.ParamParser.GetInteger(cmdParam.paramValue[0]);
                    int count = DebugConsoleScript.ParamParser.GetInteger(cmdParam.paramValue[1]);
                    RestoreLight(index, count);
                    break;
                case "fullhealth":
                    index = DebugConsoleScript.ParamParser.GetInteger(cmdParam.paramValue[0]);
                    FullHealth(index);
                    break;
                case "drawcards":
                    index = DebugConsoleScript.ParamParser.GetInteger(cmdParam.paramValue[0]);
                    count = DebugConsoleScript.ParamParser.GetInteger(cmdParam.paramValue[1]);
                    DrawCards(index, count);
                    break;
                case "revive":
                    index = DebugConsoleScript.ParamParser.GetInteger(cmdParam.paramValue[0]);
                    Revive(index);
                    break;
            }
            cmdParam.Clear();
        }
        public static void HelpCommand()
        {
            DebugConsoleScript instance = SingletonBehavior<DebugConsoleScript>.Instance;
            foreach (string text in instance.dic.Keys.ToArray<string>())
            {
                DebugConsoleScript.DebugCmdParam debugCmdParam;
                instance.dic.TryGetValue(text, out debugCmdParam);
                AppendLine(text + " " + string.Join<DebugConsoleScript.ParamType>(" ", debugCmdParam.parameters.ToArray()));
            }
        }
        public static void AppendLine(string text)
        {
            DebugConsoleScript instance = SingletonBehavior<DebugConsoleScript>.Instance;
            instance.GetType().GetMethod("AppendLine", AccessTools.all).Invoke(instance, new object[]
            {
                    text
            });
        }
        public static void ShowIds(bool state)
        {
            foreach (BattleUnitModel battleUnitModel in BattleObjectManager.instance.GetList())
            {
                battleUnitModel.UnitData.unitData.ResetTempName();
                if (state)
                {
                    battleUnitModel.UnitData.unitData.SetTempName(String.Format(battleUnitModel.UnitData.unitData.name + " [{0}]", battleUnitModel.id - 1));
                }
            }
            BattleObjectManager.instance.InitUI();
        }
        public static void AddUnit(int id, int index)
        {
            RemoveUnit(Faction.Enemy, index);
            BattleUnitModel battleUnitModel = Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, id, index);
            UpdateUI();
            battleUnitModel.OnRoundStartOnlyUI();
            battleUnitModel.RollSpeedDice();
        }
        public static void RemoveUnit(Faction team, int index)
        {
            Singleton<StageController>.Instance.RemoveUnit(team, index);
            UpdateUI();
        }
        public static void OpenBattlePrepare()
        {
            GameSceneManager.Instance.OpenBattleSettingUI();
            StageModel currentstage = Singleton<StageController>.Instance.GetStageModel();
            List<SephirahType> usedSephiras = (List<SephirahType>)currentstage.GetType().GetField("_usedFloorList", AccessTools.all).GetValue(currentstage);
            usedSephiras.Remove(Singleton<StageController>.Instance.CurrentFloor);
        }
        public static void UpdateUI()
        {
            int num = 0;
            foreach (BattleUnitModel battleUnitModel2 in BattleObjectManager.instance.GetList())
            {
                SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnitModel2.UnitData.unitData, num++, false);
            }
            BattleObjectManager.instance.InitUI();
        }
        public static void RestoreLight(int index, int count)
        {
            BattleUnitModel battleUnitModel = BattleObjectManager.instance.GetTargetByIndex(index);
            battleUnitModel.cardSlotDetail.RecoverPlayPoint(count);
        }
        public static void FullHealth(int index)
        {
            BattleUnitModel battleUnitModel = BattleObjectManager.instance.GetTargetByIndex(index);
            battleUnitModel.RecoverHP(battleUnitModel.MaxHp);
            battleUnitModel.breakDetail.RecoverBreak(battleUnitModel.breakDetail.GetDefaultBreakGauge());
        }
        public static void DrawCards(int index, int count)
        {
            BattleUnitModel battleUnitModel = BattleObjectManager.instance.GetTargetByIndex(index);
            battleUnitModel.allyCardDetail.DrawCards(count);
        }
        public static void Revive(int index)
        {
            BattleUnitModel battleUnitModel = BattleObjectManager.instance.GetTargetByIndex(index);
            battleUnitModel.Revive(battleUnitModel.MaxHp);
            battleUnitModel.breakDetail.RecoverBreak(battleUnitModel.breakDetail.GetDefaultBreakGauge());
            battleUnitModel.view.EnableView(true);
            battleUnitModel.OnRoundStartOnlyUI();
            battleUnitModel.RollSpeedDice();
        }
    }
}
