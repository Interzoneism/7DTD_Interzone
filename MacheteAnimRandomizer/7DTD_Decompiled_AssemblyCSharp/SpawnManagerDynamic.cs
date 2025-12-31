using System;
using System.IO;
using System.Xml;
using UnityEngine;

// Token: 0x0200098C RID: 2444
public class SpawnManagerDynamic : SpawnManagerAbstract
{
	// Token: 0x06004992 RID: 18834 RVA: 0x001D1586 File Offset: 0x001CF786
	public SpawnManagerDynamic(World _world, XmlDocument _spawnXml) : base(_world)
	{
		this.lastDaySpawned = -1;
	}

	// Token: 0x06004993 RID: 18835 RVA: 0x001D1596 File Offset: 0x001CF796
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(1);
		_bw.Write(this.currentSpawner != null);
		if (this.currentSpawner != null)
		{
			_bw.Write(this.lastDaySpawned);
			this.currentSpawner.Write(_bw);
		}
	}

	// Token: 0x06004994 RID: 18836 RVA: 0x001D15CE File Offset: 0x001CF7CE
	public void Read(BinaryReader _br)
	{
		_br.ReadByte();
		if (_br.ReadBoolean())
		{
			this.currentSpawner = new EntitySpawner();
			this.lastDaySpawned = _br.ReadInt32();
			this.currentSpawner.Read(_br);
		}
	}

	// Token: 0x06004995 RID: 18837 RVA: 0x001D1604 File Offset: 0x001CF804
	public override void Update(string _spawnerName, bool _bSpawnEnemyEntities, object _userData)
	{
		if (this.world.IsDaytime())
		{
			return;
		}
		if (this.world.Players.list.Count == 0)
		{
			return;
		}
		int num = GameUtils.WorldTimeToDays(this.world.worldTime);
		if (num != this.lastDaySpawned || this.currentSpawner == null)
		{
			this.lastDaySpawned = num;
			EntitySpawner entitySpawner = this.currentSpawner;
			Log.Out("New ES '" + _spawnerName + "' for day: " + num.ToString());
			this.currentSpawner = new EntitySpawner(_spawnerName, Vector3i.zero, Vector3i.zero, 0, (entitySpawner != null) ? entitySpawner.GetEntityIdsSpaned() : null);
		}
		if (this.currentSpawner != null)
		{
			this.currentSpawner.SpawnManually(this.world, num, _bSpawnEnemyEntities, delegate(EntitySpawner _es, out EntityPlayer _outPlayerToAttack)
			{
				_outPlayerToAttack = null;
				return true;
			}, delegate(EntitySpawner _es, EntityPlayer _inPlayerToAttack, out EntityPlayer _outPlayerToAttack, out Vector3 _pos)
			{
				return this.world.GetRandomSpawnPositionMinMaxToRandomPlayer(64, 96, true, out _outPlayerToAttack, out _pos);
			}, null, null);
		}
	}

	// Token: 0x040038D4 RID: 14548
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte CurrentFileVersion = 1;

	// Token: 0x040038D5 RID: 14549
	public const int cMinRange = 64;

	// Token: 0x040038D6 RID: 14550
	public const int cMaxRange = 96;

	// Token: 0x040038D7 RID: 14551
	[PublicizedFrom(EAccessModifier.Private)]
	public EntitySpawner currentSpawner;

	// Token: 0x040038D8 RID: 14552
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastDaySpawned;
}
