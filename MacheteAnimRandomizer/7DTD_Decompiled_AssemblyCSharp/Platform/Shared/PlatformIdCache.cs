using System;
using System.IO;

namespace Platform.Shared
{
	// Token: 0x020018DC RID: 6364
	public static class PlatformIdCache
	{
		// Token: 0x17001572 RID: 5490
		// (get) Token: 0x0600BBF5 RID: 48117 RVA: 0x00476FE9 File Offset: 0x004751E9
		public static string IdFilePath
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return Path.Combine(GameIO.GetUserGameDataDir(), "PlatformIdCache.txt");
			}
		}

		// Token: 0x0600BBF6 RID: 48118 RVA: 0x00476FFC File Offset: 0x004751FC
		public static bool TryGetCachedId<T>(out T _platformUserIdentifier) where T : PlatformUserIdentifierAbs
		{
			string idFilePath = PlatformIdCache.IdFilePath;
			if (SdFile.Exists(idFilePath))
			{
				using (Stream stream = SdFile.OpenRead(idFilePath))
				{
					using (StreamReader streamReader = new StreamReader(stream))
					{
						string text = streamReader.ReadLine();
						if (text == null)
						{
							Log.Out("[PlatformIdCache] no cached user id");
							_platformUserIdentifier = default(T);
							return false;
						}
						PlatformUserIdentifierAbs platformUserIdentifierAbs = PlatformUserIdentifierAbs.FromCombinedString(text, true);
						if (!(platformUserIdentifierAbs is T))
						{
							Log.Error(string.Format("[PlatformIdCache] cannot retrieved cached id {0} as {1}", text, typeof(T)));
							_platformUserIdentifier = default(T);
							return false;
						}
						_platformUserIdentifier = (T)((object)platformUserIdentifierAbs);
						return true;
					}
				}
			}
			Log.Out("[PlatformIdCache] no id cache file at " + idFilePath);
			_platformUserIdentifier = default(T);
			return false;
		}

		// Token: 0x0600BBF7 RID: 48119 RVA: 0x004770E0 File Offset: 0x004752E0
		public static void SetCachedId(PlatformUserIdentifierAbs _platformUserIdentifier)
		{
			using (Stream stream = SdFile.OpenWrite(PlatformIdCache.IdFilePath))
			{
				using (StreamWriter streamWriter = new StreamWriter(stream))
				{
					streamWriter.WriteLine(_platformUserIdentifier.CombinedString);
				}
			}
		}

		// Token: 0x040092DB RID: 37595
		[PublicizedFrom(EAccessModifier.Private)]
		public const string idCacheFile = "PlatformIdCache.txt";
	}
}
