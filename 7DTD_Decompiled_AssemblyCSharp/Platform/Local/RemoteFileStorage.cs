using System;
using System.IO;

namespace Platform.Local
{
	// Token: 0x020018EE RID: 6382
	public class RemoteFileStorage : IRemoteFileStorage
	{
		// Token: 0x0600BC97 RID: 48279 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x170015A3 RID: 5539
		// (get) Token: 0x0600BC98 RID: 48280 RVA: 0x000197A5 File Offset: 0x000179A5
		public bool IsReady
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170015A4 RID: 5540
		// (get) Token: 0x0600BC99 RID: 48281 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool Unavailable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600BC9A RID: 48282 RVA: 0x0047858C File Offset: 0x0047678C
		public void GetFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback)
		{
			if (_callback == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(_filename))
			{
				_callback(IRemoteFileStorage.EFileDownloadResult.EmptyFilename, null, null);
				return;
			}
			string path = GameIO.GetApplicationPath() + "/fakeRemoteFileStorage/" + _filename;
			if (!File.Exists(path))
			{
				_callback(IRemoteFileStorage.EFileDownloadResult.FileNotFound, "", null);
				return;
			}
			try
			{
				byte[] data = File.ReadAllBytes(path);
				_callback(IRemoteFileStorage.EFileDownloadResult.Ok, null, data);
			}
			catch (Exception ex)
			{
				Log.Error("[Local] ReadFile (" + _filename + ") failed:");
				Log.Exception(ex);
				_callback(IRemoteFileStorage.EFileDownloadResult.Other, ex.Message, null);
			}
		}

		// Token: 0x0600BC9B RID: 48283 RVA: 0x000424BD File Offset: 0x000406BD
		public void GetCachedFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback)
		{
			throw new NotImplementedException();
		}
	}
}
