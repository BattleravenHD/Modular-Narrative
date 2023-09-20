using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class PhaseView : UnityEditor.Experimental.GraphView.Node
{
    public Action<PhaseView> OnPhaseSelected;
    public StoryPhase phase;
    public Port input;
    public Port output;

    public PhaseView(StoryPhase phase)
    {
        this.phase = phase;
        this.title = phase.phaseName;
        this.viewDataKey = phase.guid;
        style.left = phase.postion.x;
        style.top = phase.postion.y;

        CreateInputPorts();
        CreateOutputPorts();
    }

    private void CreateOutputPorts()
    {
        if (!phase.IsRoot)
        {
            input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));

            if (input != null)
            {
                input.portName = "Last";
                inputContainer.Add(input);
            }
        }
    }

    private void CreateInputPorts()
    {
        output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));

        if (output != null)
        {
            output.portName = "Next";
            inputContainer.Add(output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        phase.postion.x = newPos.xMin;
        phase.postion.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        if (OnPhaseSelected != null)
        {
            OnPhaseSelected.Invoke(this);
        }
    }
}
