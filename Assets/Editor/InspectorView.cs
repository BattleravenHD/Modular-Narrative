using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    Editor editor;

    PhaseView currentlyViewedPhase;

    public InspectorView()
    {


    }

    internal void UpdateSelection(PhaseView phase)
    {
        if (currentlyViewedPhase != null)
        {
            currentlyViewedPhase.title = currentlyViewedPhase.phase.phaseName;
        }
        currentlyViewedPhase = null;
        Clear();
        currentlyViewedPhase = phase;
        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(phase.phase);
        
        IMGUIContainer container = new IMGUIContainer(() => { editor.OnInspectorGUI(); });

        Add(container);
    }

    internal void Update()
    {
        if (currentlyViewedPhase != null)
        {
            currentlyViewedPhase.title = currentlyViewedPhase.phase.phaseName;
        }
    }
}
