using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ProjectBuilder
{
	public sealed class BuildWindow : EditorWindow
	{
		public int value;
		public List<BuildCollection> collections = new List<BuildCollection>();

		[MenuItem("Build/Project Builder Window")]
		private static void OpenWindow()
		{
			BuildWindow window = GetWindow<BuildWindow>();
			window.titleContent = new GUIContent("Project Builder");
			window.Show();
			
			window.LoadData();
		}

		private const string key = "ProjectBuilder.LastCollections";
		
		private void LoadData()
		{
			string json = EditorPrefs.GetString(key);
			if (string.IsNullOrWhiteSpace(json)) return;

			var guids = JsonUtility.FromJson<List<string>>(json);
			foreach (string guid in guids)
			{
				try
				{
					string path = AssetDatabase.GUIDToAssetPath(guid);
					BuildCollection collection = AssetDatabase.LoadAssetAtPath<BuildCollection>(path);
					if (collection != null)
					{
						collections.Add(collection);
					}
				}
				catch (Exception e)
				{
					Debug.LogWarning(e);
				}
			}
		}

		private void SaveData()
		{
			if (collections == null || collections.Count < 1) return;
			
			string json = JsonUtility.ToJson(collections);
			EditorPrefs.SetString(key, json);
		}

		private Editor editor;

		private void OnEnable()
		{
			editor = Editor.CreateEditor(this);
		}

		public void OnGUI()
		{
			editor.OnInspectorGUI();
			
			EditorGUILayout.Space();
			using (new EditorGUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Build", new []{GUILayout.Height(24)}))
				{
					Build();
				}
			}
		}

		private void Build()
		{
			var currentTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			var currentSubTarget = EditorUserBuildSettings.standaloneBuildSubtarget;
			var backend = PlayerSettings.GetScriptingDefineSymbols(currentTarget);
			
			foreach (BuildCollection collection in collections)
			{
				foreach (BuildProfile profile in collection.profiles)
				{
					try
					{
						var current_backend = backend + ";";
						foreach (string symbol in profile.m_scriptingDefineSymbols)
						{
							current_backend += $"{symbol};";
						}

						if (profile.m_client)
						{
							current_backend += "GAME_CLIENT;";
						}
					
						if (profile.m_server)
						{
							current_backend += "GAME_SERVER;";
						}
					
						PlayerSettings.SetScriptingDefineSymbols(currentTarget, current_backend);
						BuildOptions options = profile.m_development ? BuildOptions.Development : BuildOptions.None;

						StandaloneBuildSubtarget target = profile.m_headless
							? StandaloneBuildSubtarget.Server
							: StandaloneBuildSubtarget.Player;

						var scenePaths = profile.m_scenes.Select(AssetDatabase.GetAssetPath).ToArray();
					
						string subPath = (profile.m_scriptingBackend == ScriptingImplementation.IL2CPP ? "IL2CPP" : "MONO") + "_" +
						                 (!profile.m_development ? "RELEASE" : "DEVELOPMENT") + "_" +
						                 (profile.m_headless ? "HEADLESS" : "CLIENT");
					
						string targetPath = Application.dataPath.Replace("/Assets", $"/Build/Windows64/{subPath}/{PlayerSettings.productName}.exe");

						string dir = Path.GetDirectoryName(targetPath);
						Directory.CreateDirectory(dir);

						Debug.Log(dir);

						EditorUserBuildSettings.standaloneBuildSubtarget = target;
						BuildPipeline.BuildPlayer(scenePaths, targetPath, BuildTarget.StandaloneWindows64, options);
					}
					catch (Exception e)
					{
						Debug.LogException(e);
					}
					
				}
			}

			PlayerSettings.SetScriptingDefineSymbols(currentTarget, backend);
			EditorUserBuildSettings.standaloneBuildSubtarget = currentSubTarget;
		}
	}
}