using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Platform.XBL
{
	// Token: 0x02001878 RID: 6264
	public class TextCensor : ITextCensor
	{
		// Token: 0x0600B959 RID: 47449 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600B95A RID: 47450 RVA: 0x0046D398 File Offset: 0x0046B598
		public void Update()
		{
			if (this.fetchStarted)
			{
				return;
			}
			this.fetchStarted = true;
			if (PlatformManager.MultiPlatform.RemoteFileStorage != null)
			{
				ThreadManager.StartCoroutine(this.RetrieveBannedWords());
			}
		}

		// Token: 0x0600B95B RID: 47451 RVA: 0x0046D3C2 File Offset: 0x0046B5C2
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator RetrieveBannedWords()
		{
			IRemoteFileStorage storage = PlatformManager.MultiPlatform.RemoteFileStorage;
			if (storage == null)
			{
				yield break;
			}
			while (!storage.IsReady)
			{
				yield return null;
			}
			storage.GetFile("BannedWordsXBL.txt", new IRemoteFileStorage.FileDownloadCompleteCallback(this.StorageProviderCallback));
			yield break;
		}

		// Token: 0x0600B95C RID: 47452 RVA: 0x0046D3D4 File Offset: 0x0046B5D4
		[PublicizedFrom(EAccessModifier.Private)]
		public void StorageProviderCallback(IRemoteFileStorage.EFileDownloadResult _result, string _errorDetails, byte[] _data)
		{
			if (_result != IRemoteFileStorage.EFileDownloadResult.Ok)
			{
				Log.Warning(string.Concat(new string[]
				{
					"Retrieving banned words list failed: ",
					_result.ToStringCached<IRemoteFileStorage.EFileDownloadResult>(),
					" (",
					_errorDetails,
					")"
				}));
				return;
			}
			using (MemoryStream memoryStream = new MemoryStream(_data))
			{
				using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8))
				{
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						if (Regex.IsMatch(text, "^[a-zA-Z0-9]+$"))
						{
							this.bannedPatterns.Add("(?<![a-zA-Z])" + Regex.Escape(text) + "(?![a-zA-Z])");
						}
						else
						{
							this.bannedPatterns.Add(Regex.Escape(text));
						}
					}
				}
			}
			this.fetchComplete = true;
		}

		// Token: 0x0600B95D RID: 47453 RVA: 0x0046D4B4 File Offset: 0x0046B6B4
		public void CensorProfanity(string _input, Action<CensoredTextResult> _callback)
		{
			if (string.IsNullOrEmpty(_input) || _input.Length == 0 || !this.fetchComplete)
			{
				_callback(new CensoredTextResult(true, _input, _input));
				return;
			}
			Task.Run(delegate()
			{
				char[] array = _input.ToCharArray();
				foreach (string pattern in this.bannedPatterns)
				{
					foreach (object obj in Regex.Matches(_input, pattern, RegexOptions.IgnoreCase))
					{
						Match match = (Match)obj;
						for (int i = match.Index; i < match.Index + match.Length; i++)
						{
							array[i] = '*';
						}
					}
				}
				_callback(new CensoredTextResult(true, _input, new string(array)));
			});
		}

		// Token: 0x040090FC RID: 37116
		[PublicizedFrom(EAccessModifier.Private)]
		public const string isAlpNum = "^[a-zA-Z0-9]+$";

		// Token: 0x040090FD RID: 37117
		[PublicizedFrom(EAccessModifier.Private)]
		public const string nonAlphabeticBefore = "(?<![a-zA-Z])";

		// Token: 0x040090FE RID: 37118
		[PublicizedFrom(EAccessModifier.Private)]
		public const string nonAlphabeticAfter = "(?![a-zA-Z])";

		// Token: 0x040090FF RID: 37119
		[PublicizedFrom(EAccessModifier.Private)]
		public bool fetchStarted;

		// Token: 0x04009100 RID: 37120
		[PublicizedFrom(EAccessModifier.Private)]
		public bool fetchComplete;

		// Token: 0x04009101 RID: 37121
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSet<string> bannedPatterns = new HashSet<string>();
	}
}
