#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;

namespace TTModdingKit.GizFlow
{
    public class GitAction
    {
        public List<GitBoxProperty> properties = new();

        public void Load(string[] lines, ref int index)
        {
            var actionProps = GetValidProperties(TTUnityProject.Game);
            while(index < lines.Length && !lines[index].Contains('}'))
            {
                string line = lines[index].Trim();
                int endInd = line.IndexOf(' ');
                string propName = endInd == -1 ? line : line[..endInd];

                var baseProp = actionProps.Where(p => p.name.ToLower() == propName.ToLower()).FirstOrDefault();
                if (baseProp == null)
                {
                    Debug.Log("Unknown or Invalid Action: " + line);
                    goto Continue;
                }

                var prop = baseProp.Clone() as GitBoxProperty;
                prop.LoadValue(endInd == -1 ? "" : line[(endInd + 1)..]);
                properties.Add(prop);

                Continue:
                index++;
            }
        }

        public static GitBoxProperty[] GetValidProperties(TTGame game)
        {
            List<GitBoxProperty> props = new()
            {
                new GitStringProperty("SetVisibility"),
                new GitStringProperty("SetGizmoVisibility"),
                new GitStringProperty("ActivateEffect"),
                new GitStringProperty("SetAIMessage"),
                new GitStringProperty("TurnOnFlowBox"),
                new GitStringProperty("CompleteLevel"),
                new GitStringProperty("ActivateGizmo"),
                new GitStringProperty("ActivateChar"),
                new GitStringProperty("PlayCutscene"),
                new GitStringProperty("PlayObstacle"),
                new GitStringProperty("EnableSock"),
                new GitStringProperty("SetPickupVisibility"),
                new GitStringProperty("GoToNewLevel"),
                new GitStringProperty("HitBlowup"),
                new GitStringProperty("GoThroughDoor"),
            };

            if (game == TTGame.TCS) props.AddRange(new GitBoxProperty[] {
                new GitStringProperty("PlayForce"),
                new GitStringProperty("ActivateBelt"),
                new GitStringProperty("PlaySpecial"),
                new GitStringProperty("ChangeObstTriggerType"),
                new GitStringProperty("PlayRadio"),
            });

            if (game == TTGame.LIJ1) props.AddRange(new GitBoxProperty[] {
                new GitStringProperty("SetSuperCounter"),
                new GitStringProperty("StopCutscene"),
                new GitStringProperty("SetRailSpecial"),
                new GitStringProperty("PlayOverlay"),
            });

            if (game == TTGame.LB1) props.AddRange(new GitBoxProperty[] {
                new GitStringProperty("HubSwitchHeroVillian"),
                new GitStringProperty("GetNextSignalSuit"),
                new GitStringProperty("ActivateEpisodeLevelSelect"),
                new GitStringProperty("CanReleaseTakeOver"),
                new GitStringProperty("CanTakeOver"),
                new GitStringProperty("SetRTL"),
                new GitStringProperty("ForceBoxReProcess"),
            });

            if (game == TTGame.LIJ1 || game == TTGame.LB1) props.AddRange(new GitBoxProperty[] {
                new GitStringProperty("ActivatePartEffect"),
                new GitStringProperty("SetObstacleLooping"),
                new GitStringProperty("ActivateGrabber"),
                new GitStringProperty("PlaySfx"),
                new GitStringProperty("ChangeTechnoTarget"),
            });

            return props.ToArray();
        }

        public static DropdownField GetActionDropdown(TTGame game)
        {
            List<string> options = new()
            {
                "SetVisibility",
                "SetGizmoVisibility",
                "ActivateEffect",
                "SetAIMessage",
                "TurnOnFlowBox",
                "CompleteLevel",
                "ActivateGizmo",
                "ActivateChar",
                "PlayCutscene",
                "PlayObstacle",
                "EnableSock",
                "SetPickupVisibility",
                "GoToNewLevel",
                "HitBlowup",
                "GoThroughDoor",
            };

            if (game == TTGame.TCS) options.AddRange(new string[] {
                "PlayForce",
                "ActivateBelt",
                "PlaySpecial",
                "ChangeObstTriggerType",
                "PlayRadio",
            });

            if (game == TTGame.LIJ1) options.AddRange(new string[] {
                "SetSuperCounter",
                "StopCutscene",
                "SetRailSpecial",
                "PlayOverlay",
            });

            if (game == TTGame.LB1) options.AddRange(new string[] {
                "HubSwitchHeroVillian",
                "GetNextSignalSuit",
                "ActivateEpisodeLevelSelect",
                "CanReleaseTakeOver",
                "CanTakeOver",
                "SetRTL",
                "ForceBoxReProcess",
            });

            if (game == TTGame.LIJ1 || game == TTGame.LB1) options.AddRange(new string[] {
                "ActivatePartEffect",
                "SetObstacleLooping",
                "ActivateGrabber",
                "PlaySfx",
                "ChangeTechnoTarget",
            });

            return new("Add Action", options, 0);
        }
    }
}
#endif