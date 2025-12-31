using System;
using GameSparks.Platforms;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class GameSparksUnity : MonoBehaviour
{
	// Token: 0x060000C2 RID: 194 RVA: 0x00009D17 File Offset: 0x00007F17
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		base.gameObject.AddComponent<DefaultPlatform>();
	}

	// Token: 0x060000C3 RID: 195 RVA: 0x00009D28 File Offset: 0x00007F28
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGUI()
	{
		if (GameSparksSettings.PreviewBuild)
		{
			GUILayout.BeginArea(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Space(10f);
			GUILayout.Label("GameSparks Preview mode", new GUILayoutOption[]
			{
				GUILayout.Width(200f),
				GUILayout.Height(25f)
			});
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}

	// Token: 0x040000E8 RID: 232
	public GameSparksSettings settings;
}
