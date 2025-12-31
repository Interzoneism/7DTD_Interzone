using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000023 RID: 35
public class MicroSplatKeywords : ScriptableObject
{
	// Token: 0x060000E2 RID: 226 RVA: 0x0000B083 File Offset: 0x00009283
	public bool IsKeywordEnabled(string k)
	{
		return this.keywords.Contains(k);
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x0000B091 File Offset: 0x00009291
	public void EnableKeyword(string k)
	{
		if (!this.IsKeywordEnabled(k))
		{
			this.keywords.Add(k);
		}
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x0000B0A8 File Offset: 0x000092A8
	public void DisableKeyword(string k)
	{
		if (this.IsKeywordEnabled(k))
		{
			this.keywords.Remove(k);
		}
	}

	// Token: 0x04000117 RID: 279
	public List<string> keywords = new List<string>();

	// Token: 0x04000118 RID: 280
	public int drawOrder = 100;
}
