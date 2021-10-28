using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ProjectBuilder
{
	public static class BuilderData
	{
		public static List<BuildProfile> Profiles {
			get
			{
				if (profiles == null)
				{
					UpdateProfiles();
				}
				
				return profiles;
			}
		}

		private static List<BuildProfile> profiles;

		public static void UpdateProfiles()
		{
			profiles = AssetDatabase.FindAssets($"t:{nameof(BuildProfile)}")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<BuildProfile>)
				.Where(e => e.m_exposeToWizard)
				.ToList();
		}
	}
}