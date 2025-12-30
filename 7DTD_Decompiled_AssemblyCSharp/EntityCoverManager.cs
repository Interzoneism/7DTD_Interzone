using System;
using System.Collections.Generic;
using ExtUtilsForEnt;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003F9 RID: 1017
[Preserve]
public class EntityCoverManager
{
	// Token: 0x17000366 RID: 870
	// (get) Token: 0x06001EB0 RID: 7856 RVA: 0x000BFA94 File Offset: 0x000BDC94
	public static EntityCoverManager Instance
	{
		get
		{
			return EntityCoverManager.instance;
		}
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000BFA9B File Offset: 0x000BDC9B
	public static void Init()
	{
		EntityCoverManager.instance = new EntityCoverManager();
		EntityCoverManager.instance.Load();
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x000BFAB1 File Offset: 0x000BDCB1
	public void Clear()
	{
		this.CoverDic.Clear();
		this.CoverPoints.Clear();
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x000BFACC File Offset: 0x000BDCCC
	public void Clear(EntityAlive entity, float dist)
	{
		foreach (KeyValuePair<int, EntityCoverManager.CoverPos> keyValuePair in this.CoverDic)
		{
			if (Vector3.Distance(keyValuePair.Value.BlockPos, entity.position) > dist)
			{
				this.CoverDic.Remove(keyValuePair.Key);
			}
		}
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Load()
	{
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x000BFB48 File Offset: 0x000BDD48
	public void Update()
	{
		this.DrawCoverPoints();
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x000BFB50 File Offset: 0x000BDD50
	public bool HasCover(int entityId)
	{
		EntityCoverManager.CoverPos coverPos = null;
		return this.CoverDic.TryGetValue(entityId, out coverPos) && coverPos.InUse;
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x000BFB7C File Offset: 0x000BDD7C
	public bool HasCoverReserved(int entityId)
	{
		EntityCoverManager.CoverPos coverPos = null;
		return this.CoverDic.TryGetValue(entityId, out coverPos) && (coverPos.Reserved || coverPos.InUse);
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x000BFBB0 File Offset: 0x000BDDB0
	public bool IsFree(Vector3 coverPos)
	{
		foreach (KeyValuePair<int, EntityCoverManager.CoverPos> keyValuePair in this.CoverDic)
		{
			if (keyValuePair.Value.BlockPos == coverPos)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x000BFC18 File Offset: 0x000BDE18
	public EntityCoverManager.CoverPos AddCover(Vector3 pos, Vector3 dir)
	{
		if (this.CoverPoints.Find((EntityCoverManager.CoverPos c) => c.BlockPos == pos) == null)
		{
			EntityCoverManager.CoverPos coverPos = new EntityCoverManager.CoverPos(pos, dir, Time.time);
			this.CoverPoints.Add(coverPos);
			return coverPos;
		}
		return null;
	}

	// Token: 0x06001EBA RID: 7866 RVA: 0x000BFC6C File Offset: 0x000BDE6C
	public EntityCoverManager.CoverPos GetCoverPos(int entityId)
	{
		EntityCoverManager.CoverPos result = null;
		this.CoverDic.TryGetValue(entityId, out result);
		return result;
	}

	// Token: 0x06001EBB RID: 7867 RVA: 0x000BFC8C File Offset: 0x000BDE8C
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityCoverManager.CoverPos GetCover(Vector3 pos)
	{
		return this.CoverPoints.Find((EntityCoverManager.CoverPos c) => c.BlockPos == pos);
	}

	// Token: 0x06001EBC RID: 7868 RVA: 0x000BFCC0 File Offset: 0x000BDEC0
	public bool MarkReserved(int entityId, Vector3 pos)
	{
		if (!this.CoverDic.ContainsKey(entityId))
		{
			EntityCoverManager.CoverPos cover = this.GetCover(pos);
			if (cover != null)
			{
				cover.Reserved = true;
				this.CoverDic.Add(entityId, cover);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001EBD RID: 7869 RVA: 0x000BFD00 File Offset: 0x000BDF00
	public bool UseCover(int entityId, Vector3 pos)
	{
		EntityCoverManager.CoverPos cover = this.GetCover(pos);
		if (!this.CoverDic.ContainsKey(entityId))
		{
			if (cover != null)
			{
				cover.InUse = true;
				this.CoverDic.Add(entityId, cover);
				return true;
			}
		}
		else if (this.CoverDic.TryGetValue(entityId, out cover))
		{
			cover.InUse = true;
			return true;
		}
		return false;
	}

	// Token: 0x06001EBE RID: 7870 RVA: 0x000BFD58 File Offset: 0x000BDF58
	public void FreeCover(int entityId)
	{
		EntityCoverManager.CoverPos coverPos = null;
		if (this.CoverDic.TryGetValue(entityId, out coverPos))
		{
			this.CoverDic.Remove(entityId);
		}
	}

	// Token: 0x06001EBF RID: 7871 RVA: 0x000BFD84 File Offset: 0x000BDF84
	public void DrawCoverPoints()
	{
		for (int i = 0; i < this.CoverPoints.Count; i++)
		{
			EntityCoverManager.CoverPos coverPos = this.CoverPoints[i];
			EUtils.DrawBounds(new Vector3i(coverPos.BlockPos), Color.yellow, 1f, 1f);
			EUtils.DrawLine(coverPos.BlockPos, coverPos.BlockPos + coverPos.CoverDir, Color.blue, 1f);
		}
	}

	// Token: 0x0400153C RID: 5436
	public static bool DebugModeEnabled;

	// Token: 0x0400153D RID: 5437
	[PublicizedFrom(EAccessModifier.Private)]
	public static EntityCoverManager instance;

	// Token: 0x0400153E RID: 5438
	public Dictionary<int, EntityCoverManager.CoverPos> CoverDic = new Dictionary<int, EntityCoverManager.CoverPos>();

	// Token: 0x0400153F RID: 5439
	public List<EntityCoverManager.CoverPos> CoverPoints = new List<EntityCoverManager.CoverPos>();

	// Token: 0x020003FA RID: 1018
	[Preserve]
	public class CoverPos
	{
		// Token: 0x06001EC1 RID: 7873 RVA: 0x000BFE17 File Offset: 0x000BE017
		public CoverPos(Vector3 _pos, Vector3 _coverDir, float _timeCreated)
		{
			this.BlockPos = _pos;
			this.CoverDir = _coverDir;
			this.TimeCreated = _timeCreated;
		}

		// Token: 0x04001540 RID: 5440
		public Vector3 BlockPos;

		// Token: 0x04001541 RID: 5441
		public Vector3 CoverDir;

		// Token: 0x04001542 RID: 5442
		public float TimeCreated;

		// Token: 0x04001543 RID: 5443
		public bool Reserved;

		// Token: 0x04001544 RID: 5444
		public bool InUse;
	}
}
