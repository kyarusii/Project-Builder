using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorInternal;
using UnityEngine;

namespace ProjectBuilder
{
	public sealed class BuildWindow : EditorWindow
	{
		private BuildCollection collection = default;
		
		// public List<BuildCollection> collections = new List<BuildCollection>();
		private Editor editor;
		[Range(1f, 20f)]
		public float width = 20f;

		private ECollectionMode _collectionMode = 0;
		private bool inProgress = false;

		private GUIStyle activeProfileStyle;
		private GUIStyle inactiveProfileStyle;

		/// <summary>
		/// 마지막으로 사용한 빌드 컬렉션을 가져옵니다.
		/// </summary>
		/// <returns></returns>
		private BuildCollection GetLastCollection()
		{
			string key = $"{Application.productName}.ProjectBuilder.LastCollectionGUID";
			if (EditorPrefs.HasKey(key))
			{
				string guid = EditorPrefs.GetString(key);
				string path = AssetDatabase.GUIDToAssetPath(guid);

				if (!string.IsNullOrWhiteSpace(path))
				{
					return AssetDatabase.LoadAssetAtPath<BuildCollection>(path);
				}
			}

			return default;
		}

		/// <summary>
		/// 마지막으로 사용한 빌드 컬렉션으로 등록합니다.
		/// </summary>
		/// <param name="buildCollection"></param>
		private void SetLastCollection(BuildCollection buildCollection)
		{
			if (buildCollection == null)
			{
				Debug.LogWarning("Null 컬렉션은 저장되지 않습니다.");
				return;
			}
			
			string path = AssetDatabase.GetAssetPath(buildCollection);
			string guid = AssetDatabase.AssetPathToGUID(path);
			
			string key = $"{Application.productName}.ProjectBuilder.LastCollectionGUID";
			EditorPrefs.SetString(key, guid);
		}

		private void ConfigureStyles()
		{
			if (activeProfileStyle == null)
			{
				activeProfileStyle = new GUIStyle(GUI.skin.button);
				activeProfileStyle.normal.textColor = Color.green;
			}
			
			if (inactiveProfileStyle == null)
			{
				inactiveProfileStyle = new GUIStyle(GUI.skin.button);
				inactiveProfileStyle.normal.textColor = Color.red;
			}
		}
		
		private void OnEnable()
		{
			editor = Editor.CreateEditor(this);
			collection = GetLastCollection();
		}

		private void OnDisable()
		{
			SetLastCollection(collection);
			editor = default;
		}
		
		public void OnGUI()
		{
			ConfigureStyles();

			EditorGUI.BeginDisabledGroup(inProgress);
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space(10);

			EditorGUI.indentLevel++;
			
			using (new EditorGUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
			{
				EditorGUILayout.Space(16);
				_collectionMode = (ECollectionMode)GUILayout.Toolbar((int)_collectionMode, new[] { "Single", "Multi" },
					GUILayout.Height(28));
				EditorGUILayout.Space(16);
			}
			
			EditorGUILayout.Space(16);

			switch (_collectionMode)
			{
				case ECollectionMode.Single:
					OnGUI_Single();
					EditorGUILayout.Space(2);
					OnGUI_Build();
					break;
				case ECollectionMode.Multi:
					OnGUI_Multi();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			
			EditorGUI.indentLevel--;
			
			EditorGUILayout.Space(10);
			
			if (EditorGUI.EndChangeCheck())
			{
				AssetDatabase.SaveAssets();
				SetLastCollection(collection);
			}
			EditorGUI.EndDisabledGroup();
		}

		private void OnGUI_Single()
		{
			using (new EditorGUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
			{
				collection = EditorGUILayout.ObjectField(new GUIContent("Collection"), collection,
						typeof(BuildCollection), false) as
					BuildCollection;

				if (GUILayout.Button("New", GUILayout.Width(40)))
				{
					string path = EditorUtility.SaveFilePanelInProject("Create BuildCollection",
						"MyBuildCollection.asset", "asset", "Select path to save");

					if (!string.IsNullOrWhiteSpace(path))
					{
						BuildCollection newCollection = CreateInstance<BuildCollection>();
						AssetDatabase.CreateAsset(newCollection, path);

						collection = newCollection;
					}
				}
			}
			
			EditorGUILayout.Space(20);

			if (collection == null)
			{
				EditorGUILayout.HelpBox("Select or create a collection", MessageType.Info);
			}
			else
			{
				EditorGUILayout.Space(5);
				EditorGUILayout.BeginVertical("HelpBox");
				EditorGUILayout.Space(10);
				
				using (new EditorGUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
				{
					EditorGUILayout.LabelField("Active", GUILayout.Width(80));
					EditorGUILayout.LabelField("Build Profile");
				}
				
				for (int i = 0; i < collection.profiles.Count; i++)
				{
					BuildProfile profile = collection.profiles[i];
					using (new EditorGUILayout.HorizontalScope(GUILayout.Height(20)))
					{
						if (profile == null)
						{
							using (new EditorGUI.DisabledScope(true))
							{
								GUILayout.Button("Enabled", activeProfileStyle, GUILayout.Width(80));
							}
							
							collection.profiles[i] =
								EditorGUILayout.ObjectField(profile, typeof(BuildProfile), false) as BuildProfile;

							if (collection.profiles[i] != null)
							{
								EditorUtility.SetDirty(collection);
							}
							
							if (GUILayout.Button("X", GUILayout.Width(20)))
							{
								collection.profiles.RemoveAt(i);
								EditorUtility.SetDirty(collection);
							}
							
							continue;
						}
						
						string label = profile.m_isActive ? "Enabled" : "Disabled";
						var style = profile.m_isActive ? activeProfileStyle : inactiveProfileStyle;

						bool pushed = GUILayout.Button(label, style, GUILayout.Width(80));
						if (pushed)
						{
							profile.m_isActive = !profile.m_isActive;
							EditorUtility.SetDirty(profile);
						}

						BuildProfile changedProfile =
							EditorGUILayout.ObjectField(profile, typeof(BuildProfile), false) as BuildProfile;
						
						if (!ReferenceEquals(profile, changedProfile))
						{
							collection.profiles[i] = changedProfile;
							EditorUtility.SetDirty(collection);
						}
						
						if (GUILayout.Button("X", GUILayout.Width(20)))
						{
							collection.profiles.RemoveAt(i);
							EditorUtility.SetDirty(collection);
						}
					}
				}

				EditorGUILayout.Space(10);
				using (new EditorGUILayout.HorizontalScope(GUILayout.Height(20)))
				{
					GUILayout.FlexibleSpace();
					
					if (GUILayout.Button("Add", GUILayout.Height(20), GUILayout.Width(100)))
					{
						collection.profiles.Add(null);
						EditorUtility.SetDirty(collection);
					}
					
					if (GUILayout.Button("Create", GUILayout.Height(20), GUILayout.Width(100)))
					{
						string path = EditorUtility.SaveFilePanelInProject($"Create {nameof(BuildProfile)}",
							$"My{nameof(BuildProfile)}.asset", "asset", "Select path to save");
						
						if (!string.IsNullOrWhiteSpace(path))
						{
							BuildProfile newProfile = CreateInstance<BuildProfile>();
							AssetDatabase.CreateAsset(newProfile, path);

							collection.profiles.Add(newProfile);
							EditorUtility.SetDirty(collection);
						}
					}
				}
				
				EditorGUILayout.Space(10);
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space(5);
			}
		}

		private void OnGUI_Multi()
		{
			EditorGUILayout.HelpBox("Not Implemented Yet", MessageType.Warning);
		}

		private void OnGUI_Build()
		{
			try
			{
				EditorGUILayout.LabelField($"Status : Ready to build {collection.profiles.Count(e => e != null && e.m_isActive)} profiles.");
				
				using (new EditorGUILayout.HorizontalScope())
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Build", GUILayout.Width(120), GUILayout.Height(30)))
					{
						inProgress = true;
						
						var title = "Build Wizard";
						var message = "This process cannot be canceled. Proceed?";
						var yes = "Proceed";
						var no = "Abort";

						bool res = EditorUtility.DisplayDialog(title, message, yes, no);
						if (res)
						{
							Build();
							inProgress = false;
						}
					}	
					GUILayout.FlexibleSpace();
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				inProgress = false;
			}
			finally
			{
				
			}
		}

		private async void BuildCheck()
		{
			await Task.Delay(2000);
			inProgress = false;
		}

		[MenuItem("Window/Project Builder Wizard")]
		private static void OpenWindow()
		{
			BuildWindow window = GetWindow<BuildWindow>();
			window.titleContent = new GUIContent("Project Builder");
			window.Show();
		}

		private void Build()
		{
#if UNITY_2021_2_OR_NEWER
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
#else

			var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;
			string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget);
			
			foreach (var profile in collection.profiles)
			{
				if (profile == null) continue;

				try
				{
					string current_backend = symbols + ";";
					
					// 커스텀 심볼 추가
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

					PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, current_backend);
					BuildOptions options = profile.m_development ? BuildOptions.Development : BuildOptions.None;
					if (profile.m_headless)
					{
						options |= BuildOptions.EnableHeadlessMode;
					}

					var scenePaths = profile.m_scenes.Select(AssetDatabase.GetAssetPath).ToArray();

					string subPath =
						(profile.m_scriptingBackend == ScriptingImplementation.IL2CPP ? "IL2CPP" : "MONO") + "_" +
						(!profile.m_development ? "RELEASE" : "DEVELOPMENT") + "_" +
						(profile.m_headless ? "HEADLESS" : "CLIENT");

					string targetPath = Application.dataPath.Replace("/Assets",
						$"/Build/Windows64/{subPath}/{PlayerSettings.productName}.exe");

					string dir = Path.GetDirectoryName(targetPath);
					Directory.CreateDirectory(dir);

					Debug.Log(dir);

					BuildPipeline.BuildPlayer(scenePaths, targetPath, BuildTarget.StandaloneWindows64, options);
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
			}

			PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, symbols);
#endif
		}
	}
}