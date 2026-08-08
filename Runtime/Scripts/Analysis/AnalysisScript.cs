#if UNITY_EDITOR
using System;
using UnityEngine;

namespace NuGizWrap.Analysis
{
    [CreateAssetMenu(fileName = "AnalysisScript", menuName = "TT Modding/Analysis Script")]
    public class AnalysisScript : ScriptableObject
    {
        public enum Command
        {
            Comment, SetVarWithR1, SetVarWithR2, SetVarWithR3,
            GetVarInR1, GetVarInR2, GetVarInR3,
            StartLoop, EndLoop, Break, Continue, Return, Log, LogVar, LogR1, LogR2, LogR3,
            IncR1, IncR2, IncR3, AddR1R2ToR3, SubR1R2toR3, MultR1R2toR3, DivR1R2toR3,
            StartIfR1, StartIfR2, StartIfR3, Else, EndIf,

            ByteReadToR1, ByteReadToR2, ByteReadToR3,
            RunSubScript,
            
            EnumerateAllGIZ, PopNextFileInQueue, GetFileQueueCountR1, PropertyReadToR1, LogCurrFile
        }

        [Serializable]
        public struct Line
        {
            public Command command;
            public string parameter;
        }

        public bool alertFinish;

        public AnalysisScript[] subScripts;

        //Properties
        public Line[] lines;
    }
}
#endif