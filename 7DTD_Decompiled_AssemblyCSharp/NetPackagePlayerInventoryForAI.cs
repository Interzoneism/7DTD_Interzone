using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x02000773 RID: 1907
[Preserve]
public class NetPackagePlayerInventoryForAI : NetPackage
{
	// Token: 0x170005A3 RID: 1443
	// (get) Token: 0x06003776 RID: 14198 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003777 RID: 14199 RVA: 0x0016AF98 File Offset: 0x00169198
	public NetPackagePlayerInventoryForAI Setup(EntityAlive entity, AIDirectorPlayerInventory inventory)
	{
		this.m_entityId = entity.entityId;
		this.m_inventory = inventory;
		return this;
	}

	// Token: 0x06003778 RID: 14200 RVA: 0x0016AFB0 File Offset: 0x001691B0
	public override int GetLength()
	{
		int num = 8;
		if (this.m_inventory.bag != null)
		{
			num += 4 * this.m_inventory.bag.Count;
		}
		if (this.m_inventory.belt != null)
		{
			num += 4 * this.m_inventory.belt.Count;
		}
		return num;
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x0016B004 File Offset: 0x00169204
	public override void read(PooledBinaryReader _reader)
	{
		this.m_entityId = _reader.ReadInt32();
		this.m_inventory.bag = NetPackagePlayerInventoryForAI.ReadInventorySet(_reader);
		this.m_inventory.belt = NetPackagePlayerInventoryForAI.ReadInventorySet(_reader);
	}

	// Token: 0x0600377A RID: 14202 RVA: 0x0016B034 File Offset: 0x00169234
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.m_entityId);
		NetPackagePlayerInventoryForAI.WriteInventorySet(_writer, this.m_inventory.bag);
		NetPackagePlayerInventoryForAI.WriteInventorySet(_writer, this.m_inventory.belt);
	}

	// Token: 0x0600377B RID: 14203 RVA: 0x0016B06C File Offset: 0x0016926C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null || _world.aiDirector == null)
		{
			return;
		}
		AIDirectorPlayerInventory inventory;
		inventory.bag = this.m_inventory.bag;
		inventory.belt = this.m_inventory.belt;
		_world.aiDirector.UpdatePlayerInventory(this.m_entityId, inventory);
	}

	// Token: 0x0600377C RID: 14204 RVA: 0x0016B0BC File Offset: 0x001692BC
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<AIDirectorPlayerInventory.ItemId> ReadInventorySet(BinaryReader stream)
	{
		List<AIDirectorPlayerInventory.ItemId> list = null;
		int num = (int)stream.ReadInt16();
		for (int i = 0; i < num; i++)
		{
			if (list == null)
			{
				list = new List<AIDirectorPlayerInventory.ItemId>();
			}
			list.Add(AIDirectorPlayerInventory.ItemId.Read(stream));
		}
		return list;
	}

	// Token: 0x0600377D RID: 14205 RVA: 0x0016B0F4 File Offset: 0x001692F4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void WriteInventorySet(BinaryWriter stream, List<AIDirectorPlayerInventory.ItemId> items)
	{
		int num = (items != null) ? items.Count : 0;
		stream.Write((short)num);
		if (num > 0)
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].Write(stream);
			}
		}
	}

	// Token: 0x04002CF3 RID: 11507
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_entityId;

	// Token: 0x04002CF4 RID: 11508
	[PublicizedFrom(EAccessModifier.Private)]
	public AIDirectorPlayerInventory m_inventory;
}
