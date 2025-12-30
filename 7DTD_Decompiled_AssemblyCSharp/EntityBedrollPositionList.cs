using System;

// Token: 0x0200048B RID: 1163
public class EntityBedrollPositionList
{
	// Token: 0x060025E9 RID: 9705 RVA: 0x000F3F21 File Offset: 0x000F2121
	public EntityBedrollPositionList(EntityAlive _e)
	{
		this.theEntity = _e;
	}

	// Token: 0x060025EA RID: 9706 RVA: 0x000F3F30 File Offset: 0x000F2130
	public Vector3i GetPos()
	{
		PersistentPlayerData data = this.GetData();
		if (data != null)
		{
			return data.BedrollPos;
		}
		return new Vector3i(0, int.MaxValue, 0);
	}

	// Token: 0x060025EB RID: 9707 RVA: 0x000F3F5C File Offset: 0x000F215C
	public void Set(Vector3i _pos)
	{
		PersistentPlayerData data = this.GetData();
		if (data != null)
		{
			data.BedrollPos = _pos;
			data.ShowBedrollOnMap();
		}
	}

	// Token: 0x060025EC RID: 9708 RVA: 0x000F3F80 File Offset: 0x000F2180
	public void Clear()
	{
		PersistentPlayerData data = this.GetData();
		if (data != null)
		{
			data.ClearBedroll();
		}
	}

	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x060025ED RID: 9709 RVA: 0x000F3F9D File Offset: 0x000F219D
	public int Count
	{
		get
		{
			if (this.GetPos().y == 2147483647)
			{
				return 0;
			}
			return 1;
		}
	}

	// Token: 0x17000400 RID: 1024
	public Vector3i this[int _idx]
	{
		get
		{
			return this.GetPos();
		}
	}

	// Token: 0x060025EF RID: 9711 RVA: 0x000F3FBC File Offset: 0x000F21BC
	[PublicizedFrom(EAccessModifier.Private)]
	public PersistentPlayerData GetData()
	{
		return GameManager.Instance.GetPersistentPlayerList().GetPlayerDataFromEntityID(this.theEntity.entityId);
	}

	// Token: 0x04001CB7 RID: 7351
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive theEntity;
}
