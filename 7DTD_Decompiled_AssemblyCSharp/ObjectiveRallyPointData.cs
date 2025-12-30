using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008D0 RID: 2256
public class ObjectiveRallyPointData : MonoBehaviour
{
	// Token: 0x06004259 RID: 16985 RVA: 0x001ACA34 File Offset: 0x001AAC34
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		for (int i = 0; i < this.FlagParents.Length; i++)
		{
			GameObject gameObject = this.FlagParents[i];
			List<GameObject> list = new List<GameObject>();
			List<GameObject> list2 = new List<GameObject>();
			foreach (Transform transform in gameObject.GetComponentsInChildren<Transform>(true))
			{
				if (gameObject.transform != transform)
				{
					GameObject gameObject2 = transform.gameObject;
					if (gameObject2.name.EndsWith("Highlight"))
					{
						list2.Add(gameObject2);
					}
					else
					{
						list.Add(gameObject2);
					}
				}
			}
			list.Sort((GameObject x, GameObject y) => x.name.CompareTo(y.name));
			this.flagNodes.Add(gameObject, list);
			list2.Sort((GameObject x, GameObject y) => x.name.CompareTo(y.name));
			this.highlightNodes.Add(gameObject, list2);
			this.flagCounts = new int[this.Flags.Length];
		}
		GameObject[] flags = this.Flags;
		for (int j = 0; j < flags.Length; j++)
		{
			flags[j].SetActive(false);
		}
		this.HelperFlag.SetActive(true);
		this.Highlight.SetActive(false);
		this.Root.SetActive(GameManager.Instance.World.IsEditor());
	}

	// Token: 0x0600425A RID: 16986 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
	}

	// Token: 0x0600425B RID: 16987 RVA: 0x001ACBA0 File Offset: 0x001AADA0
	public void ClearAllFlags()
	{
		for (int i = 0; i < this.Flags.Length; i++)
		{
			this.flagCounts[i] = 0;
		}
		this.highlightFlag = -1;
	}

	// Token: 0x0600425C RID: 16988 RVA: 0x001ACBD0 File Offset: 0x001AADD0
	public void AddFlag(string flagName, bool highLight)
	{
		for (int i = 0; i < this.Flags.Length; i++)
		{
			if (this.Flags[i].name.Equals(flagName))
			{
				if (highLight)
				{
					this.highlightFlag = i;
				}
				this.flagCounts[i]++;
			}
		}
	}

	// Token: 0x0600425D RID: 16989 RVA: 0x001ACC20 File Offset: 0x001AAE20
	public void RemoveFlag(string flagName)
	{
		for (int i = 0; i < this.Flags.Length; i++)
		{
			if (this.Flags[i].name.Equals(flagName) && this.flagCounts[i] > 0)
			{
				this.flagCounts[i]--;
			}
		}
	}

	// Token: 0x0600425E RID: 16990 RVA: 0x001ACC74 File Offset: 0x001AAE74
	public void UpdateAllFlags()
	{
		int i = 0;
		while (i < this.flagCounts.Length && this.flagCounts[i] <= 0)
		{
			i++;
		}
		bool flag = i < this.flagCounts.Length;
		this.Root.SetActive(flag);
		this.Highlight.SetActive(this.highlightFlag >= 0);
		if (flag)
		{
			this.Highlight.SetActive(false);
			this.HelperFlag.SetActive(false);
			List<GameObject> list = new List<GameObject>();
			int num = -1;
			for (i = 0; i < this.flagCounts.Length; i++)
			{
				bool flag2 = this.flagCounts[i] > 0;
				GameObject gameObject = this.Flags[i];
				if (flag2)
				{
					if (i == this.highlightFlag)
					{
						num = list.Count;
					}
					list.Add(gameObject);
				}
				gameObject.SetActive(flag2);
			}
			int count = list.Count;
			for (i = 0; i < count; i++)
			{
				Transform transform = this.flagNodes[this.FlagParents[count - 1]][i].transform;
				list[i].transform.SetParent(transform, false);
				if (i == num)
				{
					this.Highlight.transform.SetParent(this.highlightNodes[this.FlagParents[count - 1]][i].transform, false);
					this.Highlight.SetActive(true);
				}
			}
		}
	}

	// Token: 0x0600425F RID: 16991 RVA: 0x001ACDD4 File Offset: 0x001AAFD4
	public GameObject GetFlagParentNode(int flagCount, int flagIndex)
	{
		if (flagCount >= 0 && flagCount < this.FlagParents.Length)
		{
			List<GameObject> list = this.flagNodes[this.FlagParents[flagCount]];
			if (flagIndex >= 0 && flagIndex < list.Count)
			{
				return list[flagIndex];
			}
		}
		return null;
	}

	// Token: 0x040034A8 RID: 13480
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject[] Flags;

	// Token: 0x040034A9 RID: 13481
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject[] FlagParents;

	// Token: 0x040034AA RID: 13482
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject Root;

	// Token: 0x040034AB RID: 13483
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject HelperFlag;

	// Token: 0x040034AC RID: 13484
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject Highlight;

	// Token: 0x040034AD RID: 13485
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<GameObject, List<GameObject>> flagNodes = new Dictionary<GameObject, List<GameObject>>();

	// Token: 0x040034AE RID: 13486
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<GameObject, List<GameObject>> highlightNodes = new Dictionary<GameObject, List<GameObject>>();

	// Token: 0x040034AF RID: 13487
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] flagCounts;

	// Token: 0x040034B0 RID: 13488
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int highlightFlag = -1;
}
