using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class MatUpdater : EditorWindow
{
    [MenuItem("Tools/Material Updater")]
    public static void ShowUpdater()
    {
        MatUpdater wnd = GetWindow<MatUpdater>();
        wnd.titleContent = new GUIContent("Material Updater");
    }

    private Shader selectedShader;

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        Button button = new Button();
        button.name = "update";
        button.text = "Update Materials";
        root.Add(button);

        button.clicked += UpdateMats;

    }
    void OnGUI()
    {
        
        GUILayout.Label("Select a Shader", EditorStyles.boldLabel);

        selectedShader = (Shader)EditorGUILayout.ObjectField("Shader", selectedShader, typeof(Shader), false);

        if (selectedShader != null)
        {
            EditorGUILayout.LabelField("Selected Shader:", selectedShader.name);
        }
    }

    public void UpdateMats()
    {
        Object[] selections = Selection.GetFiltered(typeof(Material), SelectionMode.Assets);

        for (int i = 0; i < selections.Length; i++)
        {
            Material mat = (Material)selections[i];
            Color color = mat.color;

            if (selectedShader != null)
            {
                mat.shader = selectedShader;
                mat.color = color;
            } else
            {
                Debug.LogError("No Shader with that name!");
            }
        }

    }
}
