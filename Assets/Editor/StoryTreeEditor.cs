using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class StoryTreeEditor : EditorWindow
{
    StoryTreeView storyView;
    InspectorView inspectorView;
    [MenuItem("Window/StoryEditorWindow")]
    public static void OpenWindow()
    {
        StoryTreeEditor wnd = GetWindow<StoryTreeEditor>();
        wnd.titleContent = new GUIContent("StoryTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;


        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/StoryTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/StoryTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        storyView = root.Q<StoryTreeView>();
        inspectorView = root.Q<InspectorView>();
        storyView.OnPhaseSelected = OnPhaseSelectionChanged;
        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        StoryBase story = Selection.activeObject as StoryBase;
        if (story)
        {
            storyView.PopulateView(story);
        }
    }


    private void OnInspectorUpdate()
    {
        if (inspectorView != null)
        {
            inspectorView.Update();
        }
    }

    void OnPhaseSelectionChanged(PhaseView phase)
    {
        inspectorView.UpdateSelection(phase);
    }
}