using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000FB5 RID: 4021
public class AssetMappings
{
	// Token: 0x17000D58 RID: 3416
	// (get) Token: 0x0600801A RID: 32794 RVA: 0x003413E8 File Offset: 0x0033F5E8
	public int Count
	{
		get
		{
			return this.list.Count;
		}
	}

	// Token: 0x0600801B RID: 32795 RVA: 0x003413F5 File Offset: 0x0033F5F5
	public void Add(string name, string address)
	{
		this.list.Add(new AssetMappings.AssetAddress
		{
			name = name,
			address = address
		});
	}

	// Token: 0x0600801C RID: 32796 RVA: 0x00341418 File Offset: 0x0033F618
	public Dictionary<string, string> ToDictionary()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (AssetMappings.AssetAddress assetAddress in this.list)
		{
			dictionary.Add(assetAddress.name, assetAddress.address);
		}
		return dictionary;
	}

	// Token: 0x0400630B RID: 25355
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public List<AssetMappings.AssetAddress> list = new List<AssetMappings.AssetAddress>();

	// Token: 0x02000FB6 RID: 4022
	[Serializable]
	public class AssetAddress
	{
		// Token: 0x0400630C RID: 25356
		public string name;

		// Token: 0x0400630D RID: 25357
		public string address;
	}
}
