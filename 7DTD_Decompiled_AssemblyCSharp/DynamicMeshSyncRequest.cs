using System;

// Token: 0x0200035B RID: 859
public class DynamicMeshSyncRequest
{
	// Token: 0x170002D3 RID: 723
	// (get) Token: 0x0600192B RID: 6443 RVA: 0x0009A1C4 File Offset: 0x000983C4
	public int SecondsAlive
	{
		get
		{
			return (int)(DateTime.Now - this.Created).TotalSeconds;
		}
	}

	// Token: 0x170002D4 RID: 724
	// (get) Token: 0x0600192C RID: 6444 RVA: 0x0009A1EC File Offset: 0x000983EC
	public int SecondsAttempted
	{
		get
		{
			if (this.Initiated != null)
			{
				return (int)(DateTime.Now - this.Initiated.Value).TotalSeconds;
			}
			return 0;
		}
	}

	// Token: 0x0600192D RID: 6445 RVA: 0x0009A226 File Offset: 0x00098426
	public static DynamicMeshSyncRequest Create(DynamicMeshItem item, bool isDelete)
	{
		return new DynamicMeshSyncRequest
		{
			Item = item,
			IsDelete = isDelete
		};
	}

	// Token: 0x0600192E RID: 6446 RVA: 0x0009A23B File Offset: 0x0009843B
	public static DynamicMeshSyncRequest Create(DynamicMeshItem item, bool isDelete, int clientId)
	{
		return new DynamicMeshSyncRequest
		{
			Item = item,
			IsDelete = isDelete,
			ClientId = clientId
		};
	}

	// Token: 0x0600192F RID: 6447 RVA: 0x0009A257 File Offset: 0x00098457
	public bool TryGetData()
	{
		if (DynamicMeshThread.ChunkDataQueue.CollectBytes(this.Item.Key, out this.Data, out this.Length))
		{
			this.HasData = true;
			return true;
		}
		return false;
	}

	// Token: 0x0400101C RID: 4124
	public DynamicMeshItem Item;

	// Token: 0x0400101D RID: 4125
	public bool IsDelete;

	// Token: 0x0400101E RID: 4126
	public bool SyncComplete;

	// Token: 0x0400101F RID: 4127
	public byte[] Data;

	// Token: 0x04001020 RID: 4128
	public bool HasData;

	// Token: 0x04001021 RID: 4129
	public int Length;

	// Token: 0x04001022 RID: 4130
	public int ClientId = -1;

	// Token: 0x04001023 RID: 4131
	public DateTime? Initiated;

	// Token: 0x04001024 RID: 4132
	public DateTime Created;
}
