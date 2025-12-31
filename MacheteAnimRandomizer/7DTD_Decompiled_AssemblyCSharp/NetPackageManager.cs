using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Profiling;

// Token: 0x020006F4 RID: 1780
public static class NetPackageManager
{
	// Token: 0x06003479 RID: 13433 RVA: 0x00160F2C File Offset: 0x0015F12C
	[PublicizedFrom(EAccessModifier.Private)]
	static NetPackageManager()
	{
		Log.Out("NetPackageManager Init");
		ReflectionHelpers.FindTypesImplementingBase(typeof(NetPackage), delegate(Type _type)
		{
			NetPackageManager.knownPackageTypes.Add(_type.Name, _type);
		}, false);
	}

	// Token: 0x0600347A RID: 13434 RVA: 0x00160F7C File Offset: 0x0015F17C
	public static void ResetMappings()
	{
		NetPackageManager.packageIdToClass = null;
		NetPackageManager.packageIdToPackageInformation = null;
		NetPackageManager.packageClassToPackageId = null;
	}

	// Token: 0x0600347B RID: 13435 RVA: 0x00160F90 File Offset: 0x0015F190
	[PublicizedFrom(EAccessModifier.Private)]
	public static void AddPackageMapping(int _id, Type _type)
	{
		NetPackageManager.packageIdToClass[_id] = _type;
		NetPackageManager.packageClassToPackageId[_type] = _id;
		NetPackageManager.IPackageInformation packageInformation = (NetPackageManager.IPackageInformation)typeof(NetPackageManager.NetPackageInformation<>).MakeGenericType(new Type[]
		{
			_type
		}).GetProperty("Instance").GetValue(null, null);
		NetPackageManager.packageIdToPackageInformation[_id] = packageInformation;
	}

	// Token: 0x0600347C RID: 13436 RVA: 0x00160FE9 File Offset: 0x0015F1E9
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetupBaseMapping()
	{
		NetPackageManager.packageIdToClass = new Type[NetPackageManager.KnownPackageCount];
		NetPackageManager.packageIdToPackageInformation = new NetPackageManager.IPackageInformation[NetPackageManager.KnownPackageCount];
		NetPackageManager.packageClassToPackageId = new Dictionary<Type, int>();
		NetPackageManager.AddPackageMapping(0, NetPackageManager.packageIdsType);
	}

	// Token: 0x0600347D RID: 13437 RVA: 0x00161020 File Offset: 0x0015F220
	public static void StartServer()
	{
		NetPackageManager.ResetMappings();
		NetPackageManager.SetupBaseMapping();
		int num = 1;
		foreach (KeyValuePair<string, Type> keyValuePair in NetPackageManager.knownPackageTypes)
		{
			if (!(keyValuePair.Value == NetPackageManager.packageIdsType))
			{
				NetPackageManager.AddPackageMapping(num, keyValuePair.Value);
				num++;
			}
		}
	}

	// Token: 0x0600347E RID: 13438 RVA: 0x0016109C File Offset: 0x0015F29C
	public static void StartClient()
	{
		NetPackageManager.ResetMappings();
		NetPackageManager.SetupBaseMapping();
	}

	// Token: 0x0600347F RID: 13439 RVA: 0x001610A8 File Offset: 0x0015F2A8
	public static void IdMappingsReceived(string[] _mappings)
	{
		for (int i = 0; i < _mappings.Length; i++)
		{
			Type type;
			if (!NetPackageManager.knownPackageTypes.TryGetValue(_mappings[i], out type))
			{
				Log.Error("[NET] Unknown package type " + _mappings[i] + ", can not proceed connecting to server");
				SingletonMonoBehaviour<ConnectionManager>.Instance.Disconnect();
				GameManager.Instance.ShowMessagePlayerDenied(new GameUtils.KickPlayerData(GameUtils.EKickReason.UnknownNetPackage, 0, default(DateTime), ""));
				return;
			}
			if (!(type == NetPackageManager.packageIdsType))
			{
				NetPackageManager.AddPackageMapping(i, type);
			}
		}
	}

	// Token: 0x06003480 RID: 13440 RVA: 0x0016112A File Offset: 0x0015F32A
	public static NetPackage ParsePackage(PooledBinaryReader _reader, ClientInfo _sender)
	{
		NetPackage rawPackage = NetPackageManager.getPackageInfoByType((int)_reader.ReadByte()).GetRawPackage();
		rawPackage.Sender = _sender;
		rawPackage.read(_reader);
		return rawPackage;
	}

	// Token: 0x06003481 RID: 13441 RVA: 0x0016114A File Offset: 0x0015F34A
	public static void FreePackage(NetPackage _package)
	{
		NetPackageManager.getPackageInfoByType(_package.PackageId).FreePackage(_package);
	}

	// Token: 0x06003482 RID: 13442 RVA: 0x0016115D File Offset: 0x0015F35D
	[PublicizedFrom(EAccessModifier.Private)]
	public static NetPackageManager.IPackageInformation getPackageInfoByType(int _packageTypeId)
	{
		if (_packageTypeId >= NetPackageManager.packageIdToPackageInformation.Length || NetPackageManager.packageIdToPackageInformation[_packageTypeId] == null)
		{
			throw new NetPackageManager.UnknownNetPackageException(_packageTypeId);
		}
		return NetPackageManager.packageIdToPackageInformation[_packageTypeId];
	}

	// Token: 0x06003483 RID: 13443 RVA: 0x00161180 File Offset: 0x0015F380
	public static TPackage GetPackage<TPackage>() where TPackage : NetPackage
	{
		return NetPackageManager.NetPackageInformation<TPackage>.Instance.GetPackage();
	}

	// Token: 0x06003484 RID: 13444 RVA: 0x0016118C File Offset: 0x0015F38C
	public static int GetPackageId(Type _type)
	{
		return NetPackageManager.packageClassToPackageId[_type];
	}

	// Token: 0x06003485 RID: 13445 RVA: 0x00161199 File Offset: 0x0015F399
	public static string GetPackageName(int _id)
	{
		return NetPackageManager.packageIdToClass[_id].ToString();
	}

	// Token: 0x17000547 RID: 1351
	// (get) Token: 0x06003486 RID: 13446 RVA: 0x001611A7 File Offset: 0x0015F3A7
	public static int KnownPackageCount
	{
		get
		{
			return NetPackageManager.knownPackageTypes.Count;
		}
	}

	// Token: 0x17000548 RID: 1352
	// (get) Token: 0x06003487 RID: 13447 RVA: 0x001611B3 File Offset: 0x0015F3B3
	public static Type[] PackageMappings
	{
		get
		{
			return NetPackageManager.packageIdToClass;
		}
	}

	// Token: 0x06003488 RID: 13448 RVA: 0x001611BC File Offset: 0x0015F3BC
	public static void LogStats()
	{
		Log.Out("NetPackage pool stats:");
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < NetPackageManager.packageIdToPackageInformation.Length; i++)
		{
			NetPackageManager.IPackageInformation packageInformation = NetPackageManager.packageIdToPackageInformation[i];
			if (packageInformation != null)
			{
				int num3;
				int num4;
				packageInformation.GetStats(out num3, out num4);
				Log.Out("    {0}: {1} packages, {2} Bytes", new object[]
				{
					NetPackageManager.GetPackageName(i),
					num3,
					num4
				});
				num += num3;
				num2 += num4;
			}
		}
		Log.Out("  Total: {0} packages, {1} Bytes", new object[]
		{
			num,
			num2
		});
	}

	// Token: 0x04002AFB RID: 11003
	[PublicizedFrom(EAccessModifier.Private)]
	public static Type[] packageIdToClass;

	// Token: 0x04002AFC RID: 11004
	[PublicizedFrom(EAccessModifier.Private)]
	public static NetPackageManager.IPackageInformation[] packageIdToPackageInformation;

	// Token: 0x04002AFD RID: 11005
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<Type, int> packageClassToPackageId;

	// Token: 0x04002AFE RID: 11006
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, Type> knownPackageTypes = new CaseInsensitiveStringDictionary<Type>();

	// Token: 0x04002AFF RID: 11007
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Type packageIdsType = typeof(NetPackagePackageIds);

	// Token: 0x020006F5 RID: 1781
	[PublicizedFrom(EAccessModifier.Private)]
	public interface IPackageInformation
	{
		// Token: 0x06003489 RID: 13449
		NetPackage GetRawPackage();

		// Token: 0x0600348A RID: 13450
		void FreePackage(NetPackage _package);

		// Token: 0x0600348B RID: 13451
		void GetStats(out int _packages, out int _totalSize);
	}

	// Token: 0x020006F6 RID: 1782
	[PublicizedFrom(EAccessModifier.Private)]
	public class NetPackageInformation<TPackage> : NetPackageManager.IPackageInformation where TPackage : NetPackage
	{
		// Token: 0x17000549 RID: 1353
		// (get) Token: 0x0600348C RID: 13452 RVA: 0x00161257 File Offset: 0x0015F457
		public static NetPackageManager.NetPackageInformation<TPackage> Instance
		{
			get
			{
				if (NetPackageManager.NetPackageInformation<TPackage>.instance == null)
				{
					NetPackageManager.NetPackageInformation<TPackage>.instance = new NetPackageManager.NetPackageInformation<TPackage>();
				}
				return NetPackageManager.NetPackageInformation<TPackage>.instance;
			}
		}

		// Token: 0x0600348D RID: 13453 RVA: 0x00161270 File Offset: 0x0015F470
		[PublicizedFrom(EAccessModifier.Private)]
		public NetPackageInformation()
		{
			Type typeFromHandle = typeof(TPackage);
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			this.ctor = typeFromHandle.GetConstructor(bindingAttr, null, CallingConventions.Any, Type.EmptyTypes, null);
			if (typeof(IMemoryPoolableObject).IsAssignableFrom(typeFromHandle))
			{
				this.capacity = 10;
				MethodInfo method = typeFromHandle.GetMethod("GetPoolSize", BindingFlags.Static | BindingFlags.Public, null, Type.EmptyTypes, null);
				if (method != null)
				{
					if (method.ReturnType == typeof(int))
					{
						this.capacity = (int)method.Invoke(null, null);
					}
					else
					{
						Log.Warning("Poolable NetPackage has GetPoolSize method with wrong return type");
					}
				}
				this.pool = new TPackage[this.capacity];
			}
		}

		// Token: 0x0600348E RID: 13454 RVA: 0x001613A8 File Offset: 0x0015F5A8
		public TPackage GetPackage()
		{
			if (this.pool != null)
			{
				TPackage[] obj = this.pool;
				lock (obj)
				{
					if (this.poolSize > 0)
					{
						this.poolSize--;
						TPackage result = this.pool[this.poolSize];
						this.pool[this.poolSize] = default(TPackage);
						return result;
					}
				}
			}
			return (TPackage)((object)this.ctor.Invoke(null));
		}

		// Token: 0x0600348F RID: 13455 RVA: 0x00161444 File Offset: 0x0015F644
		public NetPackage GetRawPackage()
		{
			return this.GetPackage();
		}

		// Token: 0x06003490 RID: 13456 RVA: 0x00161454 File Offset: 0x0015F654
		public void FreePackage(NetPackage _package)
		{
			if (this.pool == null)
			{
				return;
			}
			IMemoryPoolableObject memoryPoolableObject = (IMemoryPoolableObject)_package;
			TPackage[] obj = this.pool;
			lock (obj)
			{
				if (this.poolSize < this.capacity)
				{
					memoryPoolableObject.Reset();
					this.pool[this.poolSize] = (TPackage)((object)_package);
					this.poolSize++;
				}
				else
				{
					memoryPoolableObject.Cleanup();
				}
			}
		}

		// Token: 0x06003491 RID: 13457 RVA: 0x001614E0 File Offset: 0x0015F6E0
		public void GetStats(out int _packages, out int _totalSize)
		{
			_packages = 0;
			_totalSize = 0;
			if (this.pool == null)
			{
				return;
			}
			TPackage[] obj = this.pool;
			lock (obj)
			{
				_packages = this.poolSize;
			}
		}

		// Token: 0x06003492 RID: 13458 RVA: 0x00161534 File Offset: 0x0015F734
		public void Cleanup()
		{
			if (this.pool == null)
			{
				return;
			}
			TPackage[] obj = this.pool;
			lock (obj)
			{
				for (int i = 0; i < this.poolSize; i++)
				{
					((IMemoryPoolableObject)((object)this.pool[i])).Cleanup();
				}
			}
		}

		// Token: 0x04002B00 RID: 11008
		[PublicizedFrom(EAccessModifier.Private)]
		public static NetPackageManager.NetPackageInformation<TPackage> instance;

		// Token: 0x04002B01 RID: 11009
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly TPackage[] pool;

		// Token: 0x04002B02 RID: 11010
		[PublicizedFrom(EAccessModifier.Private)]
		public int poolSize;

		// Token: 0x04002B03 RID: 11011
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int capacity;

		// Token: 0x04002B04 RID: 11012
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ConstructorInfo ctor;

		// Token: 0x04002B05 RID: 11013
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler getSampler = CustomSampler.Create("NPM.PI.GetPackage", false);

		// Token: 0x04002B06 RID: 11014
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler getSamplerPool = CustomSampler.Create("Pooled", false);

		// Token: 0x04002B07 RID: 11015
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler getSamplerNew = CustomSampler.Create("New", false);

		// Token: 0x04002B08 RID: 11016
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler getSamplerType = CustomSampler.Create(typeof(TPackage).Name, false);

		// Token: 0x04002B09 RID: 11017
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler freeSampler = CustomSampler.Create("NPM.PI.FreePackage", false);

		// Token: 0x04002B0A RID: 11018
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler freeSamplerPool = CustomSampler.Create("ToPool", false);

		// Token: 0x04002B0B RID: 11019
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly CustomSampler freeSamplerCleanup = CustomSampler.Create("Cleanup", false);
	}

	// Token: 0x020006F7 RID: 1783
	public class UnknownNetPackageException : Exception
	{
		// Token: 0x06003493 RID: 13459 RVA: 0x001615A4 File Offset: 0x0015F7A4
		public UnknownNetPackageException(int _packageId) : base("Unknown NetPackage ID: " + _packageId.ToString())
		{
		}
	}
}
