using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ProjectBuilder
{
	[CustomEditor(typeof(BuildProfile))]
	internal sealed class BuildProfileEditor : UnityEditor.Editor
	{
		private SerializedProperty m_Scenes;
		private SerializedProperty m_Server;
		private SerializedProperty m_Client;
		private SerializedProperty m_Headless;
		private SerializedProperty m_Development;
		
		private SerializedProperty m_ScriptingBackend;
		private SerializedProperty m_APICompatibilityLevel;
		private SerializedProperty m_ScriptingDefineSymbols;

		private static Dictionary<ApiCompatibilityLevel, GUIContent> m_NiceApiCompatibilityLevelNames;
		private string serializedScriptingDefines;

		private void OnEnable()
		{
			m_Scenes = serializedObject.FindProperty("m_scenes");
			m_Server = serializedObject.FindProperty("m_server");
			m_Client = serializedObject.FindProperty("m_client");
			m_Headless = serializedObject.FindProperty("m_headless");
			m_Development = serializedObject.FindProperty("m_development");
			
			m_APICompatibilityLevel = serializedObject.FindProperty("m_compatibilityLevel");
			m_ScriptingBackend = serializedObject.FindProperty("m_scriptingBackend");
			m_ScriptingDefineSymbols = serializedObject.FindProperty("m_scriptingDefineSymbols");
		}

		public override void OnInspectorGUI()
		{
			EditorGUI.BeginChangeCheck();
			
			GUILayout.Label(SettingsContent.scriptCompilationTitle, EditorStyles.boldLabel);
			using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
			{
				EditorGUI.indentLevel++;
				
				EditorGUILayout.PropertyField(m_Scenes, new GUIContent("Scenes"));
				EditorGUILayout.Space();
				
				EditorGUILayout.PropertyField(m_Server, new GUIContent("Server"));
				EditorGUILayout.PropertyField(m_Client, new GUIContent("Client"));
				EditorGUILayout.PropertyField(m_Headless, new GUIContent("Headless"));
				EditorGUILayout.PropertyField(m_Development, new GUIContent("Dev Build"));
				
				EditorGUI.indentLevel--;	
			}

			EditorGUILayout.Space();
			
			GUILayout.Label(SettingsContent.configurationTitle, EditorStyles.boldLabel);

			EditorGUI.indentLevel++;

			using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
			{
				using (EditorGUILayout.HorizontalScope horizontalScope =
					new(Array.Empty<GUILayoutOption>()))
				{
					using (new EditorGUI.PropertyScope(horizontalScope.rect, GUIContent.none, m_ScriptingBackend))
					{
						ScriptingImplementation selected = (ScriptingImplementation)m_ScriptingBackend.intValue;
						ScriptingImplementation[] scriptingImplementationArray = new[]
						{
							ScriptingImplementation.Mono2x,
							ScriptingImplementation.IL2CPP
						};

						ScriptingImplementation scriptingImplementation = BuildEnumPopup<ScriptingImplementation>(
							SettingsContent.scriptingBackend, selected, scriptingImplementationArray,
							GetNiceScriptingBackendNames(scriptingImplementationArray));

						m_ScriptingBackend.intValue = (int)scriptingImplementation;
					}
				}

				ApiCompatibilityLevel[] compatibilityLevelArray = {
					ApiCompatibilityLevel.NET_4_6,
					ApiCompatibilityLevel.NET_Standard_2_0
				};

				ApiCompatibilityLevel previous = (ApiCompatibilityLevel)m_APICompatibilityLevel.intValue;
				ApiCompatibilityLevel compatibilityLevel = BuildEnumPopup<ApiCompatibilityLevel>(
					SettingsContent.apiCompatibilityLevel, previous, compatibilityLevelArray,
					GetNiceApiCompatibilityLevelNames(compatibilityLevelArray));

				m_APICompatibilityLevel.intValue = (int)compatibilityLevel;
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.Space();
			
			GUILayout.Label(SettingsContent.scriptCompilationTitle, EditorStyles.boldLabel);

			EditorGUI.indentLevel++;
			using (new EditorGUILayout.VerticalScope(Array.Empty<GUILayoutOption>()))
			{
				EditorGUILayout.PropertyField(m_ScriptingDefineSymbols, new GUIContent("Scripting Define Symbols"));

				using (new EditorGUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Import from PlayerSettings"))
					{
						NamedBuildTarget current =
							NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
						string symbols = PlayerSettings.GetScriptingDefineSymbols(current);
						string[] parsedSymbols = symbols.Split(';');
						m_ScriptingDefineSymbols.arraySize = parsedSymbols.Length;
						for (int i = 0; i < parsedSymbols.Length; i++)
						{
							SerializedProperty property = m_ScriptingDefineSymbols.GetArrayElementAtIndex(i);
							property.stringValue = parsedSymbols[i];
						}
					}
				}
			}

			EditorGUI.indentLevel--;

			serializedObject.ApplyModifiedProperties();
		}

		private static GUIContent[] GetNiceApiCompatibilityLevelNames(
			ApiCompatibilityLevel[] apiCompatibilityLevels)
		{
			m_NiceApiCompatibilityLevelNames ??= new Dictionary<ApiCompatibilityLevel, GUIContent>()
			{
				{
					ApiCompatibilityLevel.NET_2_0,
					SettingsContent.apiCompatibilityLevel_NET_2_0
				},
				{
					ApiCompatibilityLevel.NET_2_0_Subset,
					SettingsContent.apiCompatibilityLevel_NET_2_0_Subset
				},
				{
					ApiCompatibilityLevel.NET_4_6,
					SettingsContent.apiCompatibilityLevel_NET_4_6
				},
				{
					ApiCompatibilityLevel.NET_Standard_2_0,
					SettingsContent.apiCompatibilityLevel_NET_Standard_2_0
				}
			};

			return GetGUIContentsForValues<ApiCompatibilityLevel>(m_NiceApiCompatibilityLevelNames,
				apiCompatibilityLevels);
		}

		private static GUIContent GetNiceScriptingBackendName(
			ScriptingImplementation scriptingBackend)
		{
			switch (scriptingBackend)
			{
				case ScriptingImplementation.Mono2x:
					return SettingsContent.scriptingMono2x;
				case ScriptingImplementation.IL2CPP:
					return SettingsContent.scriptingIL2CPP;
				default:
					throw new ArgumentException(
						$"Scripting backend value {scriptingBackend} is not supported.",
						nameof(scriptingBackend));
			}
		}

		private static GUIContent[] GetNiceScriptingBackendNames(
			ScriptingImplementation[] scriptingBackends)
		{
			return scriptingBackends
				.Select<ScriptingImplementation, GUIContent>(
					GetNiceScriptingBackendName)
				.ToArray<GUIContent>();
		}

		private static GUIContent[] GetGUIContentsForValues<T>(
			Dictionary<T, GUIContent> contents,
			T[] values)
		{
			GUIContent[] guiContentArray = new GUIContent[values.Length];
			for (int index = 0; index < values.Length; ++index)
			{
				if (!contents.ContainsKey(values[index]))
				{
					throw new NotImplementedException($"Missing name for {values[index]}");
				}

				guiContentArray[index] = contents[values[index]];
			}

			return guiContentArray;
		}

		public static T BuildEnumPopup<T>(
			GUIContent uiString,
			T selected,
			T[] options,
			GUIContent[] optionNames)
		{
			int selectedIndex = 0;
			for (int index = 1; index < options.Length; ++index)
			{
				if (selected.Equals(options[index]))
				{
					selectedIndex = index;
					break;
				}
			}

			int index1 = EditorGUILayout.Popup(uiString, selectedIndex, optionNames);
			return options[index1];
		}

		private readonly struct SettingsContent
		{
			public static readonly GUIContent apiCompatibilityLevel =
				EditorGUIUtility.TrTextContent("Api Compatibility Level*");

			public static readonly GUIContent
				apiCompatibilityLevel_NET_2_0 = EditorGUIUtility.TrTextContent(".NET 2.0");

			public static readonly GUIContent apiCompatibilityLevel_NET_2_0_Subset =
				EditorGUIUtility.TrTextContent(".NET 2.0 Subset");

			public static readonly GUIContent
				apiCompatibilityLevel_NET_4_6 = EditorGUIUtility.TrTextContent(".NET 4.x");

			public static readonly GUIContent apiCompatibilityLevel_NET_Standard_2_0 =
				EditorGUIUtility.TrTextContent(".NET Standard 2.0");

			public static readonly GUIContent configurationTitle = EditorGUIUtility.TrTextContent("Configuration");
			public static readonly GUIContent scriptingBackend = EditorGUIUtility.TrTextContent("Scripting Backend");

			public static readonly GUIContent scriptingMono2x = EditorGUIUtility.TrTextContent("Mono");
			public static readonly GUIContent scriptingIL2CPP = EditorGUIUtility.TrTextContent("IL2CPP");
			public static readonly GUIContent scriptingDefault = EditorGUIUtility.TrTextContent("Default");

			public static readonly GUIContent strippingDisabled = EditorGUIUtility.TrTextContent("Disabled");
			public static readonly GUIContent strippingMinimal = EditorGUIUtility.TrTextContent("Minimal");
			public static readonly GUIContent strippingLow = EditorGUIUtility.TrTextContent("Low");
			public static readonly GUIContent strippingMedium = EditorGUIUtility.TrTextContent("Medium");
			public static readonly GUIContent strippingHigh = EditorGUIUtility.TrTextContent("High");

			public static readonly GUIContent scriptingDefineSymbols =
				EditorGUIUtility.TrTextContent("Scripting Define Symbols",
					"Preprocessor defines passed to the C# script compiler.");

			public static readonly GUIContent additionalCompilerArguments =
				EditorGUIUtility.TrTextContent("Additional Compiler Arguments",
					"Additional arguments passed to the C# script compiler.");

			public static readonly GUIContent scriptCompilationTitle =
				EditorGUIUtility.TrTextContent("Script Compilation");
		}
	}
}