using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000349 RID: 841
[Preserve]
public class NetPackageDynamicClientArrive : NetPackage
{
	// Token: 0x06001888 RID: 6280 RVA: 0x00096688 File Offset: 0x00094888
	public void BuildData()
	{
		foreach (DynamicMeshItem i in DynamicMeshManager.Instance.ItemsDictionary.Values)
		{
			this.Items.Add(this.FromPool(i));
		}
		Log.Out("Client package items: " + this.Items.Count.ToString());
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x0009670C File Offset: 0x0009490C
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionItemData FromPool(DynamicMeshItem i)
	{
		return new RegionItemData(i.WorldPosition.x, i.WorldPosition.z, i.UpdateTime);
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x00096730 File Offset: 0x00094930
	public override void read(PooledBinaryReader reader)
	{
		int num = reader.ReadInt32();
		this.Items = new List<RegionItemData>(num);
		for (int i = 0; i < num; i++)
		{
			int x = reader.ReadInt32();
			int z = reader.ReadInt32();
			int updateTime = reader.ReadInt32();
			this.Items.Add(new RegionItemData(x, z, updateTime));
		}
	}

	// Token: 0x170002BC RID: 700
	// (get) Token: 0x0600188B RID: 6283 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x00096788 File Offset: 0x00094988
	public override void write(PooledBinaryWriter writer)
	{
		base.write(writer);
		writer.Write(this.Items.Count);
		for (int i = 0; i < this.Items.Count; i++)
		{
			writer.Write(this.Items[i].X);
			writer.Write(this.Items[i].Z);
			writer.Write(this.Items[i].UpdateTime);
		}
	}

	// Token: 0x0600188D RID: 6285 RVA: 0x00096808 File Offset: 0x00094A08
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (DynamicMeshManager.CONTENT_ENABLED)
		{
			DynamicMeshServer.ClientMessageRecieved(this);
		}
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x00096817 File Offset: 0x00094A17
	public override int GetLength()
	{
		return 4 + 12 * this.Items.Count;
	}

	// Token: 0x170002BD RID: 701
	// (get) Token: 0x0600188F RID: 6287 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int Channel
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170002BE RID: 702
	// (get) Token: 0x06001890 RID: 6288 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool Compress
	{
		get
		{
			return true;
		}
	}

	// Token: 0x04000FB2 RID: 4018
	public List<RegionItemData> Items = new List<RegionItemData>();
}
