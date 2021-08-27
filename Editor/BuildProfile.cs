﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProjectBuilder
{
	[Serializable]
	[CreateAssetMenu(menuName ="Build/Build Profile", order = 40)]
	public class BuildProfile : ScriptableObject
	{
		public List<SceneAsset> m_scenes;
		public bool m_server;
		public bool m_client;
		public bool m_headless;
		public bool m_development;
		
		public ScriptingImplementation m_scriptingBackend = ScriptingImplementation.Mono2x;
		public ApiCompatibilityLevel m_compatibilityLevel;
		
		public List<string> m_scriptingDefineSymbols;
	}
}