using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectBuilder
{
	[Serializable]
	[CreateAssetMenu(menuName ="Build/Build Profile", order = 40)]
	public class BuildProfile : ScriptableObject
	{
		public bool m_exposeToWizard = true;
		public bool m_isActive = true;
		
		public List<SceneAsset> m_scenes;
		public bool m_server;
		public bool m_client;
		public bool m_headless;
		public bool m_development;
		
		public ScriptingImplementation m_scriptingBackend = ScriptingImplementation.Mono2x;
		public ApiCompatibilityLevel m_compatibilityLevel;
		
		public List<string> m_scriptingDefineSymbols;

		[ContextMenu("Print Path")]
		private void PrintData()
		{
			foreach (SceneAsset scene in m_scenes)
			{
				Debug.Log(scene.name);
				Debug.Log(AssetDatabase.GetAssetPath(scene));
			}
		}
	}
}