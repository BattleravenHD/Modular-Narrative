using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;

public class StoryTreeView : GraphView
{
    public Action<PhaseView> OnPhaseSelected;
    public new class UxmlFactory : UxmlFactory<StoryTreeView, GraphView.UxmlTraits> { }
    StoryBase story;

    public StoryTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/StoryTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    internal void PopulateView(StoryBase story)
    {
        this.story = story;


        graphViewChanged -= OnGraphViewChanged;
        graphElements.ForEach(e =>
        {
            RemoveElement(e);
        });
        graphViewChanged += OnGraphViewChanged;

        //Create phases
        story.phases.ForEach(n => createPhaseView(n));

        //create Edges
        story.phases.ForEach(n => {
            var children = n.choices;
            if (n != null)
            {
                children.ForEach(c =>
                {
                    PhaseView parent = FindPhaseView(n);
                    PhaseView child = FindPhaseView(c);

                    Edge edge = parent.output.ConnectTo(child.input);
                    AddElement(edge);
                });
            }
        });
    }

    PhaseView FindPhaseView(StoryPhase phase)
    {
        return GetNodeByGuid(phase.guid) as PhaseView;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        List<Edge> edgesToDelete = new List<Edge>();
        List<PhaseView> phasesToDelete = new List<PhaseView>();

        if (graphViewChange.elementsToRemove != null)
        {
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                PhaseView storyPhase = elem as PhaseView;

                Edge edge = elem as Edge;
                if (edge != null)
                {
                    edgesToDelete.Add(edge);
                }

                if (storyPhase != null)
                {
                    phasesToDelete.Add(storyPhase);
                }              
            });
            StoryPhase lastRoot = story.StartingPhase;
            phasesToDelete.ForEach(phase => { story.DeletePhase(phase.phase); });
            edgesToDelete.ForEach(edge => {
                PhaseView parentView = edge.output.node as PhaseView;
                PhaseView childView = edge.input.node as PhaseView;
                story.RemoveChoice(parentView.phase, childView.phase);
                });
            if (lastRoot != story.StartingPhase)
            {
                PopulateView(story);
            }
        }

        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                PhaseView parentView = edge.output.node as PhaseView;
                PhaseView childView = edge.input.node as PhaseView;
                story.AddChoice(parentView.phase, childView.phase);
            });
        }

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        VisualElement contentViewContainer = ElementAt(1);
        Vector3 screenMousePosition = evt.localMousePosition;
        Vector2 worldMousePosition = screenMousePosition - contentViewContainer.transform.position;
        worldMousePosition *= 1 / contentViewContainer.transform.scale.x;
        //base.BuildContextualMenu(evt);  
        {
            evt.menu.AppendAction("New Phase", (a) =>
            {
                CreatePhase(worldMousePosition);
            });
        }
    }

    void CreatePhase(Vector2 mousePos)
    {
        
        StoryPhase phase = story.CreatePhase(mousePos);
        createPhaseView(phase);
    }

    void createPhaseView(StoryPhase phase)
    {
        PhaseView phaseView = new PhaseView(phase);
        phaseView.OnPhaseSelected = OnPhaseSelected;
        AddElement(phaseView);
    }
}
