using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020012FB RID: 4859
public sealed class vp_DecalManager
{
	// Token: 0x17000F76 RID: 3958
	// (get) Token: 0x06009760 RID: 38752 RVA: 0x003C4420 File Offset: 0x003C2620
	// (set) Token: 0x06009761 RID: 38753 RVA: 0x003C4427 File Offset: 0x003C2627
	public static float MaxDecals
	{
		get
		{
			return vp_DecalManager.m_MaxDecals;
		}
		set
		{
			vp_DecalManager.m_MaxDecals = value;
			vp_DecalManager.Refresh();
		}
	}

	// Token: 0x17000F77 RID: 3959
	// (get) Token: 0x06009762 RID: 38754 RVA: 0x003C4434 File Offset: 0x003C2634
	// (set) Token: 0x06009763 RID: 38755 RVA: 0x003C443B File Offset: 0x003C263B
	public static float FadedDecals
	{
		get
		{
			return vp_DecalManager.m_FadedDecals;
		}
		set
		{
			if (value > vp_DecalManager.m_MaxDecals)
			{
				Debug.LogError("FadedDecals can't be larger than MaxDecals");
				return;
			}
			vp_DecalManager.m_FadedDecals = value;
			vp_DecalManager.Refresh();
		}
	}

	// Token: 0x06009764 RID: 38756 RVA: 0x003C445C File Offset: 0x003C265C
	[PublicizedFrom(EAccessModifier.Private)]
	static vp_DecalManager()
	{
		vp_DecalManager.Refresh();
	}

	// Token: 0x06009765 RID: 38757 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Private)]
	public vp_DecalManager()
	{
	}

	// Token: 0x06009766 RID: 38758 RVA: 0x003C44AC File Offset: 0x003C26AC
	public static void Add(GameObject decal)
	{
		if (vp_DecalManager.m_Decals.Contains(decal))
		{
			vp_DecalManager.m_Decals.Remove(decal);
		}
		Color color = decal.GetComponent<Renderer>().material.color;
		color.a = 1f;
		decal.GetComponent<Renderer>().material.color = color;
		vp_DecalManager.m_Decals.Add(decal);
		vp_DecalManager.FadeAndRemove();
	}

	// Token: 0x06009767 RID: 38759 RVA: 0x003C4510 File Offset: 0x003C2710
	[PublicizedFrom(EAccessModifier.Private)]
	public static void FadeAndRemove()
	{
		if ((float)vp_DecalManager.m_Decals.Count > vp_DecalManager.m_NonFadedDecals)
		{
			int num = 0;
			while ((float)num < (float)vp_DecalManager.m_Decals.Count - vp_DecalManager.m_NonFadedDecals)
			{
				if (vp_DecalManager.m_Decals[num] != null)
				{
					Color color = vp_DecalManager.m_Decals[num].GetComponent<Renderer>().material.color;
					color.a -= vp_DecalManager.m_FadeAmount;
					vp_DecalManager.m_Decals[num].GetComponent<Renderer>().material.color = color;
				}
				num++;
			}
		}
		if (vp_DecalManager.m_Decals[0] != null)
		{
			if (vp_DecalManager.m_Decals[0].GetComponent<Renderer>().material.color.a <= 0f)
			{
				vp_Utility.Destroy(vp_DecalManager.m_Decals[0]);
				vp_DecalManager.m_Decals.Remove(vp_DecalManager.m_Decals[0]);
				return;
			}
		}
		else
		{
			vp_DecalManager.m_Decals.RemoveAt(0);
		}
	}

	// Token: 0x06009768 RID: 38760 RVA: 0x003C4610 File Offset: 0x003C2810
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Refresh()
	{
		if (vp_DecalManager.m_MaxDecals < vp_DecalManager.m_FadedDecals)
		{
			vp_DecalManager.m_MaxDecals = vp_DecalManager.m_FadedDecals;
		}
		vp_DecalManager.m_FadeAmount = vp_DecalManager.m_MaxDecals / vp_DecalManager.m_FadedDecals / vp_DecalManager.m_MaxDecals;
		vp_DecalManager.m_NonFadedDecals = vp_DecalManager.m_MaxDecals - vp_DecalManager.m_FadedDecals;
	}

	// Token: 0x06009769 RID: 38761 RVA: 0x003C4650 File Offset: 0x003C2850
	[PublicizedFrom(EAccessModifier.Private)]
	public static void DebugOutput()
	{
		int num = 0;
		int num2 = 0;
		using (List<GameObject>.Enumerator enumerator = vp_DecalManager.m_Decals.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.GetComponent<Renderer>().material.color.a == 1f)
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
		}
		Debug.Log(string.Concat(new string[]
		{
			"Decal count: ",
			vp_DecalManager.m_Decals.Count.ToString(),
			", Full: ",
			num.ToString(),
			", Faded: ",
			num2.ToString()
		}));
	}

	// Token: 0x040073CE RID: 29646
	public static readonly vp_DecalManager instance = new vp_DecalManager();

	// Token: 0x040073CF RID: 29647
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<GameObject> m_Decals = new List<GameObject>();

	// Token: 0x040073D0 RID: 29648
	[PublicizedFrom(EAccessModifier.Private)]
	public static float m_MaxDecals = 100f;

	// Token: 0x040073D1 RID: 29649
	[PublicizedFrom(EAccessModifier.Private)]
	public static float m_FadedDecals = 20f;

	// Token: 0x040073D2 RID: 29650
	[PublicizedFrom(EAccessModifier.Private)]
	public static float m_NonFadedDecals = 0f;

	// Token: 0x040073D3 RID: 29651
	[PublicizedFrom(EAccessModifier.Private)]
	public static float m_FadeAmount = 0f;
}
