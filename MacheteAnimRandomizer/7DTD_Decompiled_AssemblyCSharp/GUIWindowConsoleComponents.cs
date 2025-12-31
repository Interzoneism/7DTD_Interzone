using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000FBF RID: 4031
public class GUIWindowConsoleComponents : MonoBehaviour
{
	// Token: 0x06008073 RID: 32883 RVA: 0x003429BB File Offset: 0x00340BBB
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.buttonPrompts = new List<GUIButtonPrompt>(base.GetComponentsInChildren<GUIButtonPrompt>());
	}

	// Token: 0x06008074 RID: 32884 RVA: 0x003429D0 File Offset: 0x00340BD0
	public void RefreshButtonPrompts()
	{
		foreach (GUIButtonPrompt guibuttonPrompt in this.buttonPrompts)
		{
			guibuttonPrompt.RefreshIcon();
		}
	}

	// Token: 0x04006341 RID: 25409
	public ScrollRect scrollRect;

	// Token: 0x04006342 RID: 25410
	public Transform contentRect;

	// Token: 0x04006343 RID: 25411
	public InputField commandField;

	// Token: 0x04006344 RID: 25412
	public Button closeButton;

	// Token: 0x04006345 RID: 25413
	public Button openLogsButton;

	// Token: 0x04006346 RID: 25414
	public GameObject controllerPrompts;

	// Token: 0x04006347 RID: 25415
	public GameObject consoleLinePrefab;

	// Token: 0x04006348 RID: 25416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GUIButtonPrompt> buttonPrompts;
}
