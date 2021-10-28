using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace ProjectBuilder
{
	[Serializable]
	[CreateAssetMenu(menuName ="Build/Build Profile", order = 40)]
	public class BuildProfile : ScriptableObject
	{
		/// <summary>
		/// 비활성화시 프로젝트 단위 프로필 검색에 노출되지 않습니다.
		/// </summary>
		public bool m_exposeToWizard = true;
		
		public List<SceneAsset> m_scenes;
		
		public bool m_server;
		public bool m_client;
		public bool m_headless;
		public bool m_development;
		
		public BuildTargetGroup m_buildTargetGroup = BuildTargetGroup.Standalone;
		
		public ScriptingImplementation m_scriptingBackend = ScriptingImplementation.Mono2x;
		public ApiCompatibilityLevel m_compatibilityLevel;
		
		public List<string> m_scriptingDefineSymbols;
		public string m_buildPath = "{ProjectRoot}/Build/{Platform}/{ProfileName}/{ProductName}.exe";

		public string GetBuildPath()
		{
			return m_buildPath
				.Replace("{ProjectRoot}", Application.dataPath.Replace("/Assets", ""))
				.Replace("{Platform}",m_buildTargetGroup.ToString())
				.Replace("{ProfileName}",this.name)
				.Replace("{ProductName}", Application.productName)
				;
		}
	}
}