using System;

namespace Platform
{
	// Token: 0x02001816 RID: 6166
	public interface IRemotePlayerFileStorage
	{
		// Token: 0x0600B793 RID: 46995
		void Init(IPlatform _owner);

		// Token: 0x0600B794 RID: 46996
		void ReadRemoteData(string _filename, bool _overwriteCache, IRemotePlayerFileStorage.FileReadCompleteCallback _callback);

		// Token: 0x0600B795 RID: 46997
		void WriteRemoteData(string _filename, byte[] _data, bool _overwriteCache, IRemotePlayerFileStorage.FileWriteCompleteCallback _callback);

		// Token: 0x0600B796 RID: 46998 RVA: 0x00468454 File Offset: 0x00466654
		void ReadRemoteObject<T>(string _filename, bool _overwriteCache, IRemotePlayerFileStorage.FileReadObjectCompleteCallback<T> _callback) where T : IRemotePlayerStorageObject, new()
		{
			if (_callback == null)
			{
				Log.Error("[RPFS] Read failed as no callback was supplied");
				return;
			}
			this.ReadRemoteData(_filename, _overwriteCache, delegate(IRemotePlayerFileStorage.CallbackResult result, byte[] data)
			{
				if (data == null)
				{
					_callback(result, default(T));
					return;
				}
				T t = IRemotePlayerFileStorage.BytesToObject<T>(data);
				if (t == null)
				{
					Log.Error(string.Format("[RPFS] Reading data into type {0} yields malformed result.", typeof(T)));
					result = IRemotePlayerFileStorage.CallbackResult.MalformedData;
					_callback(result, default(T));
					return;
				}
				_callback(result, t);
			});
		}

		// Token: 0x0600B797 RID: 46999 RVA: 0x00468495 File Offset: 0x00466695
		void WriteRemoteObject(string _filename, IRemotePlayerStorageObject _object, bool _overwriteCache, IRemotePlayerFileStorage.FileWriteCompleteCallback _callback)
		{
			if (_callback == null)
			{
				Log.Error("[RPFS] Write failed as no callback was supplied");
				return;
			}
			this.WriteRemoteData(_filename, IRemotePlayerFileStorage.ObjectToBytes(_object), _overwriteCache, _callback);
		}

		// Token: 0x0600B798 RID: 47000 RVA: 0x004684B8 File Offset: 0x004666B8
		public static T BytesToObject<T>(byte[] _data) where T : IRemotePlayerStorageObject, new()
		{
			T result;
			if (_data == null || _data.Length == 0)
			{
				Log.Warning("[RPFS] Byte data was empty or null.");
				result = default(T);
				return result;
			}
			try
			{
				using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
				{
					pooledExpandableMemoryStream.Write(_data, 0, _data.Length);
					pooledExpandableMemoryStream.Position = 0L;
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(true))
					{
						pooledBinaryReader.SetBaseStream(pooledExpandableMemoryStream);
						T t = Activator.CreateInstance<T>();
						t.ReadInto(pooledBinaryReader);
						result = t;
					}
				}
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("[RPFS] Error while reading object from byte data. Error: {0}.", arg));
				result = default(T);
			}
			return result;
		}

		// Token: 0x0600B799 RID: 47001 RVA: 0x00468588 File Offset: 0x00466788
		public static byte[] ObjectToBytes(IRemotePlayerStorageObject _obj)
		{
			if (_obj == null)
			{
				Log.Warning("[RPFS] Object was null.");
				return null;
			}
			byte[] result;
			try
			{
				using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
				{
					using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(true))
					{
						pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
						_obj.WriteFrom(pooledBinaryWriter);
						pooledBinaryWriter.Flush();
					}
					result = pooledExpandableMemoryStream.ToArray();
				}
			}
			catch (Exception arg)
			{
				Log.Error(string.Format("[RPFS] Error while writing object into byte data. Error: {0}.", arg));
				result = null;
			}
			return result;
		}

		// Token: 0x0600B79A RID: 47002 RVA: 0x00468630 File Offset: 0x00466830
		public static string GetCacheFolder(IUserClient _user)
		{
			if (_user == null || _user.UserStatus > EUserStatus.LoggedIn)
			{
				return null;
			}
			return GameIO.GetUserGameDataDir() + "/RfsPlayerCache/" + _user.PlatformUserId.ReadablePlatformUserIdentifier;
		}

		// Token: 0x0600B79B RID: 47003 RVA: 0x0046865F File Offset: 0x0046685F
		public static T ReadCachedObject<T>(IUserClient _user, string _filename) where T : IRemotePlayerStorageObject, new()
		{
			return IRemotePlayerFileStorage.BytesToObject<T>(IRemotePlayerFileStorage.ReadCachedData(_user, _filename));
		}

		// Token: 0x0600B79C RID: 47004 RVA: 0x00468670 File Offset: 0x00466870
		public static byte[] ReadCachedData(IUserClient _user, string _filename)
		{
			object localCacheLock = IRemotePlayerFileStorage.LocalCacheLock;
			byte[] result;
			lock (localCacheLock)
			{
				string path = IRemotePlayerFileStorage.GetCacheFolder(_user) + "/" + _filename;
				if (string.IsNullOrEmpty(_filename) || !SdFile.Exists(path))
				{
					Log.Warning("[RPFS] File path was not found.");
					result = null;
				}
				else
				{
					result = SdFile.ReadAllBytes(path);
				}
			}
			return result;
		}

		// Token: 0x0600B79D RID: 47005 RVA: 0x004686E4 File Offset: 0x004668E4
		public static bool WriteCachedObject(IUserClient _user, string _filename, IRemotePlayerStorageObject _object)
		{
			return IRemotePlayerFileStorage.WriteCachedObject(_user, _filename, IRemotePlayerFileStorage.ObjectToBytes(_object));
		}

		// Token: 0x0600B79E RID: 47006 RVA: 0x004686F4 File Offset: 0x004668F4
		public static bool WriteCachedObject(IUserClient _user, string _filename, byte[] _data)
		{
			object localCacheLock = IRemotePlayerFileStorage.LocalCacheLock;
			bool result;
			lock (localCacheLock)
			{
				try
				{
					if (_data == null)
					{
						Log.Warning("[RPFS] Error while converting object to bytes.");
						result = false;
					}
					else
					{
						SdDirectory.CreateDirectory(IRemotePlayerFileStorage.GetCacheFolder(_user));
						SdFile.WriteAllBytes(IRemotePlayerFileStorage.GetCacheFolder(_user) + "/" + _filename, _data);
						result = true;
					}
				}
				catch (Exception arg)
				{
					Log.Warning(string.Format("[RPFS] Error while writing object to cache. Error: {0}.", arg));
					result = false;
				}
			}
			return result;
		}

		// Token: 0x0600B79F RID: 47007 RVA: 0x00468788 File Offset: 0x00466988
		public static void ClearCache(IUserClient _user)
		{
			object localCacheLock = IRemotePlayerFileStorage.LocalCacheLock;
			lock (localCacheLock)
			{
				if (SdDirectory.Exists(IRemotePlayerFileStorage.GetCacheFolder(_user)))
				{
					string[] files = SdDirectory.GetFiles(IRemotePlayerFileStorage.GetCacheFolder(_user));
					for (int i = 0; i < files.Length; i++)
					{
						SdFile.Delete(files[i]);
					}
				}
			}
		}

		// Token: 0x04008FEA RID: 36842
		public static object LocalCacheLock = new object();

		// Token: 0x02001817 RID: 6167
		public enum CallbackResult
		{
			// Token: 0x04008FEC RID: 36844
			Success,
			// Token: 0x04008FED RID: 36845
			FileNotFound,
			// Token: 0x04008FEE RID: 36846
			NoConnection,
			// Token: 0x04008FEF RID: 36847
			MalformedData,
			// Token: 0x04008FF0 RID: 36848
			Other
		}

		// Token: 0x02001818 RID: 6168
		// (Invoke) Token: 0x0600B7A2 RID: 47010
		public delegate void FileReadCompleteCallback(IRemotePlayerFileStorage.CallbackResult _result, byte[] _data);

		// Token: 0x02001819 RID: 6169
		// (Invoke) Token: 0x0600B7A6 RID: 47014
		public delegate void FileReadObjectCompleteCallback<T>(IRemotePlayerFileStorage.CallbackResult _result, T _object) where T : IRemotePlayerStorageObject;

		// Token: 0x0200181A RID: 6170
		// (Invoke) Token: 0x0600B7AA RID: 47018
		public delegate void FileWriteCompleteCallback(IRemotePlayerFileStorage.CallbackResult _result);
	}
}
