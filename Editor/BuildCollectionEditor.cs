using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ProjectBuilder
{
	[CustomEditor(typeof(BuildCollection))]
	internal sealed class BuildCollectionEditor : UnityEditor.Editor
	{
		private void OnEnable()
		{
			BuilderData.UpdateProfiles();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			BuildCollection collection = (BuildCollection)target;
			
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Linked Profiles", EditorStyles.boldLabel);
			EditorGUILayout.Space(4);

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField("Active", GUILayout.Width(60));
				EditorGUILayout.LabelField("Profile");
			}

			for (int i = 0; i < collection.configurations.Count; i++)
			{
				ProfileConfiguration configuration = collection.configurations[i];
				using (new EditorGUILayout.HorizontalScope())
				{
					bool changedActive = EditorGUILayout.Toggle(configuration.isActive,
						GUILayout.Width(60));
					
					BuildProfile changedProfile = EditorGUILayout.ObjectField(configuration.profile,
						typeof(BuildProfile), false) as BuildProfile;

					if (changedActive != configuration.isActive
					    || !ReferenceEquals(changedProfile, configuration.profile))
					{
						configuration.isActive = changedActive;
						configuration.profile = changedProfile;
						
						EditorUtility.SetDirty(collection);
					}

					if (GUILayout.Button("X", GUILayout.Width(20)))
					{
						collection.configurations.RemoveAt(i);
						EditorUtility.SetDirty(this);
					}
				}
			}

			EditorGUILayout.Space(10);
			
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.FlexibleSpace();
				
				if (GUILayout.Button("Add", GUILayout.Width(40)))
				{
					collection.configurations.Add(new ProfileConfiguration());
				}
			}

			EditorGUILayout.Space();
			
			using (new EditorGUILayout.VerticalScope("HelpBox"))
			{
				EditorGUILayout.Space(1);
			}
			
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Profiles In Project", EditorStyles.boldLabel);
			EditorGUILayout.Space(4);

			foreach (BuildProfile profile in BuilderData.Profiles)
			{
				bool exist = collection.configurations.Select(e => e.profile).Contains(profile);

				using (new EditorGUILayout.HorizontalScope())
				{
					using (new EditorGUI.DisabledScope(true))
					{
						EditorGUILayout.ObjectField(profile, typeof(BuildProfile), false);
					}

#if UNITY_2021_1_OR_NEWER
					if (GUILayout.Button("Edit", GUILayout.Width(40)))
					{
						EditorUtility.OpenPropertyEditor(profile);
					}
#endif

					if (GUILayout.Button("Select", GUILayout.Width(60)))
					{
						Selection.objects = new UnityEngine.Object[] { profile };
					}

					using (new EditorGUI.DisabledScope(exist))
					{
						if (GUILayout.Button("Add", GUILayout.Width(40)))
						{
							Assert.IsNotNull(collection);
							collection.configurations.Add(new ProfileConfiguration()
							{
								isActive = true,
								profile = profile,
							});
						}
					}
				}
			}

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}