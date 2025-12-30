using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200077D RID: 1917
[Preserve]
public class NetPackagePOIAround : NetPackage
{
	// Token: 0x170005AB RID: 1451
	// (get) Token: 0x060037BE RID: 14270 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int Channel
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x170005AC RID: 1452
	// (get) Token: 0x060037BF RID: 14271 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool Compress
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060037C0 RID: 14272 RVA: 0x0016BDC4 File Offset: 0x00169FC4
	public NetPackagePOIAround Setup(Dictionary<int, PrefabInstance> _prefabsAroundFar, Dictionary<int, PrefabInstance> _prefabsAroundNear)
	{
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
		{
			pooledBinaryWriter.SetBaseStream(this.ms);
			pooledBinaryWriter.Write((ushort)_prefabsAroundFar.Count);
			foreach (KeyValuePair<int, PrefabInstance> keyValuePair in _prefabsAroundFar)
			{
				pooledBinaryWriter.Write(keyValuePair.Value.id);
				pooledBinaryWriter.Write(keyValuePair.Value.rotation);
				pooledBinaryWriter.Write((keyValuePair.Value.prefab.distantPOIOverride == null) ? keyValuePair.Value.prefab.PrefabName : keyValuePair.Value.prefab.distantPOIOverride);
				StreamUtils.Write(pooledBinaryWriter, keyValuePair.Value.boundingBoxPosition);
				StreamUtils.Write(pooledBinaryWriter, keyValuePair.Value.boundingBoxSize);
				pooledBinaryWriter.Write(keyValuePair.Value.prefab.distantPOIYOffset);
			}
			pooledBinaryWriter.Write((ushort)_prefabsAroundNear.Count);
			foreach (KeyValuePair<int, PrefabInstance> keyValuePair2 in _prefabsAroundNear)
			{
				pooledBinaryWriter.Write(keyValuePair2.Value.id);
				pooledBinaryWriter.Write(keyValuePair2.Value.rotation);
				pooledBinaryWriter.Write((keyValuePair2.Value.prefab.distantPOIOverride == null) ? keyValuePair2.Value.prefab.PrefabName : keyValuePair2.Value.prefab.distantPOIOverride);
				StreamUtils.Write(pooledBinaryWriter, keyValuePair2.Value.boundingBoxPosition);
				StreamUtils.Write(pooledBinaryWriter, keyValuePair2.Value.boundingBoxSize);
				pooledBinaryWriter.Write(keyValuePair2.Value.prefab.distantPOIYOffset);
			}
		}
		return this;
	}

	// Token: 0x060037C1 RID: 14273 RVA: 0x0016BFF4 File Offset: 0x0016A1F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public ~NetPackagePOIAround()
	{
		MemoryPools.poolMemoryStream.FreeSync(this.ms);
	}

	// Token: 0x060037C2 RID: 14274 RVA: 0x0016C02C File Offset: 0x0016A22C
	public override void read(PooledBinaryReader _reader)
	{
		int length = _reader.ReadInt32();
		StreamUtils.StreamCopy(_reader.BaseStream, this.ms, length, null, true);
	}

	// Token: 0x060037C3 RID: 14275 RVA: 0x0016C054 File Offset: 0x0016A254
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write((int)this.ms.Length);
		this.ms.WriteTo(_writer.BaseStream);
	}

	// Token: 0x060037C4 RID: 14276 RVA: 0x0016C080 File Offset: 0x0016A280
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		Dictionary<int, PrefabInstance> dictionary = new Dictionary<int, PrefabInstance>();
		Dictionary<int, PrefabInstance> dictionary2 = new Dictionary<int, PrefabInstance>();
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
		{
			PooledExpandableMemoryStream obj = this.ms;
			lock (obj)
			{
				pooledBinaryReader.SetBaseStream(this.ms);
				this.ms.Position = 0L;
				int num = (int)pooledBinaryReader.ReadUInt16();
				for (int i = 0; i < num; i++)
				{
					int num2 = pooledBinaryReader.ReadInt32();
					byte rotation = pooledBinaryReader.ReadByte();
					string name = pooledBinaryReader.ReadString();
					PathAbstractions.AbstractedLocation location = PathAbstractions.PrefabsSearchPaths.GetLocation(name, null, null);
					Vector3i position = StreamUtils.ReadVector3i(pooledBinaryReader);
					Vector3i boundingBoxSize = StreamUtils.ReadVector3i(pooledBinaryReader);
					dictionary.Add(num2, new PrefabInstance(num2, location, position, rotation, null, 1)
					{
						boundingBoxSize = boundingBoxSize,
						yOffsetOfPrefab = pooledBinaryReader.ReadSingle()
					});
				}
				num = (int)pooledBinaryReader.ReadUInt16();
				for (int j = 0; j < num; j++)
				{
					int num3 = pooledBinaryReader.ReadInt32();
					byte rotation2 = pooledBinaryReader.ReadByte();
					string name2 = pooledBinaryReader.ReadString();
					PathAbstractions.AbstractedLocation location2 = PathAbstractions.PrefabsSearchPaths.GetLocation(name2, null, null);
					Vector3i position2 = StreamUtils.ReadVector3i(pooledBinaryReader);
					Vector3i boundingBoxSize2 = StreamUtils.ReadVector3i(pooledBinaryReader);
					dictionary2.Add(num3, new PrefabInstance(num3, location2, position2, rotation2, null, 1)
					{
						boundingBoxSize = boundingBoxSize2,
						yOffsetOfPrefab = pooledBinaryReader.ReadSingle()
					});
				}
			}
		}
		GameManager.Instance.prefabLODManager.UpdatePrefabsAround(dictionary, dictionary2);
	}

	// Token: 0x060037C5 RID: 14277 RVA: 0x0016C230 File Offset: 0x0016A430
	public override int GetLength()
	{
		return (int)this.ms.Length;
	}

	// Token: 0x04002D16 RID: 11542
	[PublicizedFrom(EAccessModifier.Private)]
	public PooledExpandableMemoryStream ms = MemoryPools.poolMemoryStream.AllocSync(true);
}
