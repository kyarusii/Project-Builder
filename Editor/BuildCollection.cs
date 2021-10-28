using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBuilder
{
	[Serializable]
	[CreateAssetMenu(menuName ="Build/Build Collection", order = 42)]
	public class BuildCollection : ScriptableObject
	{
		public List<ProfileConfiguration> configurations = new List<ProfileConfiguration>();
	}
}