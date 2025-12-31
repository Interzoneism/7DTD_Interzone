using System;

namespace Platform
{
	// Token: 0x02001812 RID: 6162
	public interface IRemoteFileStorage
	{
		// Token: 0x0600B788 RID: 46984
		void Init(IPlatform _owner);

		// Token: 0x170014CC RID: 5324
		// (get) Token: 0x0600B789 RID: 46985
		bool IsReady { get; }

		// Token: 0x170014CD RID: 5325
		// (get) Token: 0x0600B78A RID: 46986
		bool Unavailable { get; }

		// Token: 0x0600B78B RID: 46987
		void GetFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback);

		// Token: 0x0600B78C RID: 46988
		void GetCachedFile(string _filename, IRemoteFileStorage.FileDownloadCompleteCallback _callback);

		// Token: 0x02001813 RID: 6163
		public enum EFileDownloadResult
		{
			// Token: 0x04008FE6 RID: 36838
			Ok,
			// Token: 0x04008FE7 RID: 36839
			EmptyFilename,
			// Token: 0x04008FE8 RID: 36840
			FileNotFound,
			// Token: 0x04008FE9 RID: 36841
			Other
		}

		// Token: 0x02001814 RID: 6164
		// (Invoke) Token: 0x0600B78E RID: 46990
		public delegate void FileDownloadCompleteCallback(IRemoteFileStorage.EFileDownloadResult _result, string _errorName, byte[] _data);
	}
}
