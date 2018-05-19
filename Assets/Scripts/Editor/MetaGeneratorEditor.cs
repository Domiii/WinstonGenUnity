using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MetaGenerator))]
public class MetaGeneratorEditor : Editor {
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		var t = (MetaGenerator)target;
		if (GUILayout.Button ("Gen!")) {
			t.Randomize();
			t.GenerateAll ();
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}
		if (GUILayout.Button ("Gen the same")) {
			t.GenerateAll ();
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}
		if (GUILayout.Button ("Clear")) {
			t.ClearAll ();
			UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
		}
	}
}
