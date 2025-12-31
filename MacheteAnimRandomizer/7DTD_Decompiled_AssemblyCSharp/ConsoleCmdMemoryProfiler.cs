using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200020D RID: 525
[Preserve]
public class ConsoleCmdMemoryProfiler : ConsoleCmdAbstract
{
	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000F71 RID: 3953 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000F72 RID: 3954 RVA: 0x00064EE0 File Offset: 0x000630E0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"memprofile",
			"mprof"
		};
	}

	// Token: 0x06000F73 RID: 3955 RVA: 0x00064EF8 File Offset: 0x000630F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Toggles screen Memory Profiler UI";
	}

	// Token: 0x06000F74 RID: 3956 RVA: 0x00064F00 File Offset: 0x00063100
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		UnityMemoryProfilerLabel[] array2;
		if (!this.enabled)
		{
			this.enabled = true;
			UnityMemoryProfilerLabel[] array = UnityEngine.Object.FindObjectsOfType<UnityMemoryProfilerLabel>();
			if (array == null || array.Length == 0)
			{
				UnityEngine.Object original = Resources.Load("GUI/Prefabs/Debug_ProfilerLabel");
				using (List<UIRoot>.Enumerator enumerator = UIRoot.list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						UIRoot uiroot = enumerator.Current;
						Transform transform = uiroot.gameObject.transform;
						if (uiroot.gameObject.GetComponentInChildren<UIAnchor>() != null)
						{
							transform = uiroot.gameObject.GetComponentInChildren<UIAnchor>().transform;
						}
						UnityEngine.Object.Instantiate(original, transform);
					}
					return;
				}
			}
			array2 = UnityEngine.Object.FindObjectsOfType<UnityMemoryProfilerLabel>();
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].gameObject.SetActive(true);
			}
			return;
		}
		this.enabled = false;
		array2 = UnityEngine.Object.FindObjectsOfType<UnityMemoryProfilerLabel>();
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].gameObject.SetActive(false);
		}
	}

	// Token: 0x04000B2A RID: 2858
	[PublicizedFrom(EAccessModifier.Private)]
	public bool enabled;
}
