using System;
using System.Threading;
using UnityEngine.Scripting;

// Token: 0x02000377 RID: 887
[Preserve]
public class NetPackageDynamicMesh : DynamicMeshServerData, IMemoryPoolableObject
{
	// Token: 0x06001A2B RID: 6699 RVA: 0x000A33D8 File Offset: 0x000A15D8
	public void Setup(DynamicMeshItem item, byte[] byteArray)
	{
		this.Item = item;
		this.bytes = byteArray;
		this.X = item.WorldPosition.x;
		this.Z = item.WorldPosition.z;
		this.UpdateTime = item.UpdateTime;
		DynamicMeshItem item2 = this.Item;
		this.PresumedLength = ((item2 != null) ? item2.PackageLength : 0);
	}

	// Token: 0x06001A2C RID: 6700 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool Prechecks()
	{
		return true;
	}

	// Token: 0x170002EF RID: 751
	// (get) Token: 0x06001A2D RID: 6701 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool FlushQueue
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001A2E RID: 6702 RVA: 0x000A343C File Offset: 0x000A163C
	public override void read(PooledBinaryReader reader)
	{
		this.X = reader.ReadInt32();
		this.Z = reader.ReadInt32();
		this.UpdateTime = reader.ReadInt32();
		int num = reader.ReadInt32();
		NetPackageDynamicMesh.Count++;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			string text = string.Concat(new string[]
			{
				DynamicMeshFile.MeshLocation,
				this.X.ToString(),
				",",
				this.Z.ToString(),
				".mesh"
			});
			if (DynamicMeshManager.DoLog)
			{
				Log.Out(string.Format("Reading {0},{1} len {2}", this.X, this.Z, num));
			}
			if (num == 0)
			{
				if (DynamicMeshManager.Instance != null)
				{
					DynamicMeshManager.Instance.ArrangeChunkRemoval(this.X, this.Z);
				}
				this.bytes = null;
				return;
			}
			int num2 = 0;
			this.bytes = DynamicMeshThread.ChunkDataQueue.GetFromPool(num);
			reader.Read(this.bytes, 0, num);
			if (string.IsNullOrWhiteSpace(DynamicMeshFile.MeshLocation))
			{
				this.IsValidUpdate = false;
				return;
			}
			while (num2++ < 10)
			{
				try
				{
					this.IsValidUpdate = DynamicMeshThread.ChunkDataQueue.SaveNetPackageData(this.X, this.Z, this.bytes, this.UpdateTime);
					break;
				}
				catch (Exception)
				{
					Log.Out(string.Concat(new string[]
					{
						"Failed attempt ",
						num2.ToString(),
						" to write mesh ",
						text,
						". Retrying..."
					}));
					Thread.Sleep(1000);
				}
			}
		}
	}

	// Token: 0x06001A2F RID: 6703 RVA: 0x000A35EC File Offset: 0x000A17EC
	public override void write(PooledBinaryWriter writer)
	{
		base.write(writer);
		writer.Write(this.X);
		writer.Write(this.Z);
		writer.Write(this.UpdateTime);
		int num = this.PresumedLength;
		long position = writer.BaseStream.Position;
		string text = "start";
		try
		{
			text = "len";
			if (DynamicMeshManager.DoLog)
			{
				Log.Out(string.Format("Sending {0},{1} len {2}", this.X, this.Z, num));
			}
			text = "lencheck";
			if (NetPackageDynamicMesh.MaxLength < num)
			{
				NetPackageDynamicMesh.MaxLength = num;
				if (DynamicMeshManager.DoLog)
				{
					Log.Out("New dyMesh maxLen: " + NetPackageDynamicMesh.MaxLength.ToString());
				}
			}
			NetPackageDynamicMesh.LastLength = num;
			NetPackageDynamicMesh.LastX = this.X;
			NetPackageDynamicMesh.LastZ = this.Z;
			text = "writelen";
			writer.Write(num);
			if (this.bytes != null)
			{
				text = "writebytes";
				if (this.bytes.Length < num)
				{
					text = "writebytecheck";
					Log.Warning("Dymesh byte length was lower than expected. Len was " + num.ToString() + " and bytes were " + this.bytes.Length.ToString());
					num = (this.PresumedLength = this.bytes.Length);
				}
				text = "writenow";
				writer.Write(this.bytes, 0, num);
			}
		}
		catch (Exception ex)
		{
			string format = " ERROR MESH EXCEPTION\r\ndyMesh netWrite error: {0}\r\n({1},{2})\r\nLength: {3}\r\nLen: {4}\r\nbytes: {5}\r\nMaxLength: {6}\r\nwriterStartPosition: {7}\r\nwriterLength: {8}\r\nwriterPos: {9}\r\nstep: {10}\r\n";
			object[] array = new object[11];
			array[0] = ex.Message;
			array[1] = this.X;
			array[2] = this.Z;
			array[3] = num;
			array[4] = num;
			int num2 = 5;
			byte[] array2 = this.bytes;
			array[num2] = (((array2 != null) ? array2.Length.ToString() : null) ?? "null");
			array[6] = NetPackageDynamicMesh.MaxLength;
			array[7] = position;
			array[8] = writer.BaseStream.Length;
			array[9] = writer.BaseStream.Position;
			array[10] = text;
			Log.Error(string.Format(format, array));
			throw ex;
		}
	}

	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001A30 RID: 6704 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.Both;
		}
	}

	// Token: 0x06001A31 RID: 6705 RVA: 0x000A381C File Offset: 0x000A1A1C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!DynamicMeshManager.CONTENT_ENABLED)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			DynamicMeshServer.ClientReadyForNextMesh(this);
			return;
		}
		if (this.IsValidUpdate)
		{
			DynamicMeshManager.AddDataFromServer(this.X, this.Z);
		}
		NetPackageDynamicMesh package = NetPackageManager.GetPackage<NetPackageDynamicMesh>();
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
	}

	// Token: 0x06001A32 RID: 6706 RVA: 0x000A386F File Offset: 0x000A1A6F
	public override int GetLength()
	{
		if (this.PresumedLength > 0)
		{
			return Math.Min(this.PresumedLength, NetPackageDynamicMesh.MaxMessageSize);
		}
		if (this.bytes != null)
		{
			return 16 + this.bytes.Length;
		}
		return 16;
	}

	// Token: 0x06001A33 RID: 6707 RVA: 0x000A38A1 File Offset: 0x000A1AA1
	public void Reset()
	{
		DynamicMeshServer.SyncRelease(this.Item);
		this.Item = null;
		this.Attempts = 0;
		this.bytes = null;
		this.PresumedLength = 0;
	}

	// Token: 0x06001A34 RID: 6708 RVA: 0x000A38CA File Offset: 0x000A1ACA
	public void Cleanup()
	{
		this.Reset();
	}

	// Token: 0x170002F1 RID: 753
	// (get) Token: 0x06001A35 RID: 6709 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int Channel
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06001A36 RID: 6710 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool Compress
	{
		get
		{
			return true;
		}
	}

	// Token: 0x040010F9 RID: 4345
	public static byte[] DelayedMessageBytes = new byte[1];

	// Token: 0x040010FA RID: 4346
	public static int MaxMessageSize = 2097152;

	// Token: 0x040010FB RID: 4347
	public static int MaxLength = 0;

	// Token: 0x040010FC RID: 4348
	public static int LastLength;

	// Token: 0x040010FD RID: 4349
	public static int LastZ;

	// Token: 0x040010FE RID: 4350
	public static int LastX;

	// Token: 0x040010FF RID: 4351
	public static int Count = 0;

	// Token: 0x04001100 RID: 4352
	[PublicizedFrom(EAccessModifier.Private)]
	public DynamicMeshItem Item;

	// Token: 0x04001101 RID: 4353
	public int Attempts;

	// Token: 0x04001102 RID: 4354
	public int PresumedLength;

	// Token: 0x04001103 RID: 4355
	public int UpdateTime;

	// Token: 0x04001104 RID: 4356
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] bytes;

	// Token: 0x04001105 RID: 4357
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsValidUpdate;
}
