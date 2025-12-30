using System;
using System.Collections.Generic;
using System.IO;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000B18 RID: 2840
[Preserve]
public class TurretTracker
{
	// Token: 0x170008DB RID: 2267
	// (get) Token: 0x06005860 RID: 22624 RVA: 0x0023C390 File Offset: 0x0023A590
	public static TurretTracker Instance
	{
		get
		{
			return TurretTracker.instance;
		}
	}

	// Token: 0x06005861 RID: 22625 RVA: 0x0023C397 File Offset: 0x0023A597
	public static void Init()
	{
		TurretTracker.instance = new TurretTracker();
		TurretTracker.instance.Load();
	}

	// Token: 0x06005862 RID: 22626 RVA: 0x0023C3B0 File Offset: 0x0023A5B0
	public void AddTrackedTurret(EntityTurret _turret)
	{
		if (!_turret)
		{
			Log.Error("{0} AddTrackedTurret null", new object[]
			{
				base.GetType()
			});
			return;
		}
		if (this.turretsUnloaded.Contains(_turret.entityId))
		{
			this.turretsUnloaded.Remove(_turret.entityId);
		}
		if (!this.turretsActive.Contains(_turret))
		{
			this.turretsActive.Add(_turret);
			this.TriggerSave();
		}
	}

	// Token: 0x06005863 RID: 22627 RVA: 0x0023C424 File Offset: 0x0023A624
	public void RemoveTrackedTurret(EntityTurret _turret, EnumRemoveEntityReason _reason)
	{
		this.turretsActive.Remove(_turret);
		if (_reason == EnumRemoveEntityReason.Unloaded)
		{
			this.turretsUnloaded.Add(_turret.entityId);
		}
		this.TriggerSave();
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageVehicleCount>().Setup(), false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x06005864 RID: 22628 RVA: 0x0023C480 File Offset: 0x0023A680
	public void TriggerSave()
	{
		this.saveTime = Mathf.Min(this.saveTime, 10f);
	}

	// Token: 0x06005865 RID: 22629 RVA: 0x0023C498 File Offset: 0x0023A698
	public void Update()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		World world = GameManager.Instance.World;
		if (world == null || world.Players == null || world.Players.Count == 0)
		{
			return;
		}
		if (!GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		this.saveTime -= Time.deltaTime;
		if (this.saveTime <= 0f && (this.saveThread == null || this.saveThread.HasTerminated()))
		{
			this.saveTime = 120f;
			this.Save();
		}
	}

	// Token: 0x06005866 RID: 22630 RVA: 0x0023C52D File Offset: 0x0023A72D
	public static void Cleanup()
	{
		if (TurretTracker.instance != null)
		{
			TurretTracker.instance.SaveAndClear();
		}
	}

	// Token: 0x06005867 RID: 22631 RVA: 0x0023C540 File Offset: 0x0023A740
	[PublicizedFrom(EAccessModifier.Private)]
	public void SaveAndClear()
	{
		this.WaitOnSave();
		this.Save();
		this.WaitOnSave();
		this.turretsActive.Clear();
		this.turretsUnloaded.Clear();
		TurretTracker.instance = null;
	}

	// Token: 0x06005868 RID: 22632 RVA: 0x0023C570 File Offset: 0x0023A770
	[PublicizedFrom(EAccessModifier.Private)]
	public void WaitOnSave()
	{
		if (this.saveThread != null)
		{
			this.saveThread.WaitForEnd(30);
			this.saveThread = null;
		}
	}

	// Token: 0x06005869 RID: 22633 RVA: 0x0023C590 File Offset: 0x0023A790
	public void Load()
	{
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "turrets.dat");
		if (SdFile.Exists(text))
		{
			try
			{
				using (Stream stream = SdFile.OpenRead(text))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						this.read(pooledBinaryReader);
					}
				}
			}
			catch (Exception)
			{
				text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "turrets.dat.bak");
				if (SdFile.Exists(text))
				{
					using (Stream stream2 = SdFile.OpenRead(text))
					{
						using (PooledBinaryReader pooledBinaryReader2 = MemoryPools.poolBinaryReader.AllocSync(false))
						{
							pooledBinaryReader2.SetBaseStream(stream2);
							this.read(pooledBinaryReader2);
						}
					}
				}
			}
			Log.Out("{0} {1}, loaded {2}", new object[]
			{
				base.GetType(),
				text,
				this.turretsUnloaded.Count
			});
		}
	}

	// Token: 0x0600586A RID: 22634 RVA: 0x0023C6C8 File Offset: 0x0023A8C8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Save()
	{
		if (this.saveThread == null || !ThreadManager.ActiveThreads.ContainsKey("turretDataSave"))
		{
			Log.Out("{0} saving {1} ({2} + {3})", new object[]
			{
				base.GetType(),
				this.turretsActive.Count + this.turretsUnloaded.Count,
				this.turretsActive.Count,
				this.turretsUnloaded.Count
			});
			PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true);
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.write(pooledBinaryWriter);
			}
			this.saveThread = ThreadManager.StartThread("turretDataSave", null, new ThreadManager.ThreadFunctionLoopDelegate(this.SaveThread), null, pooledExpandableMemoryStream, null, false, true);
		}
	}

	// Token: 0x0600586B RID: 22635 RVA: 0x0023C7B4 File Offset: 0x0023A9B4
	[PublicizedFrom(EAccessModifier.Private)]
	public int SaveThread(ThreadManager.ThreadInfo _threadInfo)
	{
		PooledExpandableMemoryStream pooledExpandableMemoryStream = (PooledExpandableMemoryStream)_threadInfo.parameter;
		string text = string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "turrets.dat");
		if (SdFile.Exists(text))
		{
			SdFile.Copy(text, string.Format("{0}/{1}", GameIO.GetSaveGameDir(), "turrets.dat.bak"), true);
		}
		pooledExpandableMemoryStream.Position = 0L;
		StreamUtils.WriteStreamToFile(pooledExpandableMemoryStream, text);
		Log.Out("{0} saved {1} bytes", new object[]
		{
			base.GetType(),
			pooledExpandableMemoryStream.Length
		});
		MemoryPools.poolMemoryStream.FreeSync(pooledExpandableMemoryStream);
		return -1;
	}

	// Token: 0x0600586C RID: 22636 RVA: 0x0023C848 File Offset: 0x0023AA48
	[PublicizedFrom(EAccessModifier.Private)]
	public void read(PooledBinaryReader _br)
	{
		if (_br.ReadChar() != 'v' || _br.ReadChar() != 'd' || _br.ReadChar() != 'a' || _br.ReadChar() != '\0')
		{
			Log.Error("{0} file bad signature", new object[]
			{
				base.GetType()
			});
			return;
		}
		if (_br.ReadByte() != 1)
		{
			Log.Error("{0} file bad version", new object[]
			{
				base.GetType()
			});
			return;
		}
		this.turretsUnloaded.Clear();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			this.turretsUnloaded.Add(_br.ReadInt32());
		}
	}

	// Token: 0x0600586D RID: 22637 RVA: 0x0023C8E8 File Offset: 0x0023AAE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void write(PooledBinaryWriter _bw)
	{
		_bw.Write('v');
		_bw.Write('d');
		_bw.Write('a');
		_bw.Write(0);
		_bw.Write(1);
		List<int> list = new List<int>();
		this.GetTurrets(list);
		_bw.Write(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			_bw.Write(list[i]);
		}
	}

	// Token: 0x0600586E RID: 22638 RVA: 0x0023C954 File Offset: 0x0023AB54
	public List<int> GetTurretsList()
	{
		List<int> list = new List<int>();
		this.GetTurrets(list);
		return list;
	}

	// Token: 0x0600586F RID: 22639 RVA: 0x0023C970 File Offset: 0x0023AB70
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetTurrets(List<int> _list)
	{
		for (int i = 0; i < this.turretsActive.Count; i++)
		{
			_list.Add(this.turretsActive[i].entityId);
		}
		for (int j = 0; j < this.turretsUnloaded.Count; j++)
		{
			_list.Add(this.turretsUnloaded[j]);
		}
	}

	// Token: 0x06005870 RID: 22640 RVA: 0x0023C9D2 File Offset: 0x0023ABD2
	public static int GetServerTurretCount()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return TurretTracker.Instance.turretsActive.Count + TurretTracker.Instance.turretsUnloaded.Count;
		}
		return TurretTracker.serverTurretCount;
	}

	// Token: 0x06005871 RID: 22641 RVA: 0x0023CA05 File Offset: 0x0023AC05
	public static void SetServerTurretCount(int count)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		TurretTracker.serverTurretCount = count;
	}

	// Token: 0x06005872 RID: 22642 RVA: 0x0023CA1A File Offset: 0x0023AC1A
	public static bool CanAddMoreTurrets()
	{
		return !(DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() || TurretTracker.GetServerTurretCount() < 500;
	}

	// Token: 0x040043A0 RID: 17312
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cVersion = 1;

	// Token: 0x040043A1 RID: 17313
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSaveTime = 120f;

	// Token: 0x040043A2 RID: 17314
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cChangeSaveDelay = 10f;

	// Token: 0x040043A3 RID: 17315
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cMaxTurrets = 500;

	// Token: 0x040043A4 RID: 17316
	[PublicizedFrom(EAccessModifier.Private)]
	public static int serverTurretCount;

	// Token: 0x040043A5 RID: 17317
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<EntityTurret> turretsActive = new List<EntityTurret>();

	// Token: 0x040043A6 RID: 17318
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<int> turretsUnloaded = new List<int>();

	// Token: 0x040043A7 RID: 17319
	[PublicizedFrom(EAccessModifier.Private)]
	public float saveTime = 120f;

	// Token: 0x040043A8 RID: 17320
	[PublicizedFrom(EAccessModifier.Private)]
	public static TurretTracker instance;

	// Token: 0x040043A9 RID: 17321
	[PublicizedFrom(EAccessModifier.Private)]
	public ThreadManager.ThreadInfo saveThread;

	// Token: 0x040043AA RID: 17322
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cNameKey = "turrets";

	// Token: 0x040043AB RID: 17323
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cThreadKey = "turretDataSave";
}
