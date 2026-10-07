#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NuGizWrap.GizFlow
{
    public class GitBoxGraphNode : Node
    {
        public readonly List<Port> inputPorts = new();
        public readonly List<Port> outputPorts = new();

        public GitBox box;

        public GitBoxGraphNode(GitBox box)
        {
            this.box = box;
            title = box.name;
            
            var color = box.boxColor;
            //var color = UnityEngine.Random.ColorHSV(); //TEMP
            color.a = 1;
            style.backgroundColor = color;

            if(box is CustomGitBox custom)
            {
                foreach (var inp in custom.inputs)
                {
                    var inpPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null);
                    inpPort.name = inp.name;
                    inpPort.portName = inp.name;
                    inputPorts.Add(inpPort);
                }

                foreach(var outpt in custom.outputs)
                {
                    var outPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, null);
                    outPort.name = outpt.name;
                    outPort.portName = outpt.name;
                    outputPorts.Add(outPort);
                }
            }
            else
            {
                var inpPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, null);
                inpPort.name = "Input";
                inpPort.portName = "Input";
                inputPorts.Add(inpPort);

                if(box is FlowBox flowbox)
                {
                    foreach(var outpt in flowbox.GetOutputs())
                    {
                        var outPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, null);
                        outPort.name = outpt;
                        outPort.portName = outpt;
                        outputPorts.Add(outPort);
                    }
                }
                else
                {
                    var outPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, null);
                    outPort.name = "Output";
                    outPort.portName = "Output";
                    outputPorts.Add(outPort);
                }
            }

            RefreshPortContainers();
        }

        public void RefreshPortContainers()
        {
            inputContainer.Clear();
            foreach(var inp in inputPorts) inputContainer.Add(inp);

            outputContainer.Clear();
            foreach (var outpt in outputPorts) outputContainer.Add(outpt);
        }
    }
}
#endif