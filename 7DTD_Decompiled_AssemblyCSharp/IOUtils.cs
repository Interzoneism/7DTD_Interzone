using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using Force.Crc32;

// Token: 0x020011B1 RID: 4529
public static class IOUtils
{
	// Token: 0x06008DA4 RID: 36260 RVA: 0x0038E1B8 File Offset: 0x0038C3B8
	public static byte[] CalcHashSync(string _filename, string _algorithmName = "MD5")
	{
		byte[] result;
		using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(_algorithmName))
		{
			using (Stream stream = SdFile.OpenRead(_filename))
			{
				result = hashAlgorithm.ComputeHash(stream);
			}
		}
		return result;
	}

	// Token: 0x06008DA5 RID: 36261 RVA: 0x0038E210 File Offset: 0x0038C410
	public static IEnumerator CalcHashCoroutine(string _filename, Action<byte[]> _resultCallback, int _maxTimePerFrameMs = 5, byte[] _buffer = null, string _algorithmName = "MD5")
	{
		MicroStopwatch msw = new MicroStopwatch(true);
		using (HashAlgorithm hashAlgo = HashAlgorithm.Create(_algorithmName))
		{
			using (Stream stream = SdFile.OpenRead(_filename))
			{
				if (_buffer == null || _buffer.Length < 8192)
				{
					_buffer = new byte[32768];
				}
				int bufferSize = _buffer.Length;
				int bytesRead;
				do
				{
					bytesRead = stream.Read(_buffer, 0, bufferSize);
					if (bytesRead > 0)
					{
						hashAlgo.TransformBlock(_buffer, 0, bytesRead, null, 0);
					}
					if (msw.ElapsedMilliseconds >= (long)_maxTimePerFrameMs)
					{
						yield return null;
						msw.ResetAndRestart();
					}
				}
				while (bytesRead > 0);
				hashAlgo.TransformFinalBlock(_buffer, 0, 0);
				_resultCallback(hashAlgo.Hash);
			}
			Stream stream = null;
		}
		HashAlgorithm hashAlgo = null;
		yield break;
		yield break;
	}

	// Token: 0x06008DA6 RID: 36262 RVA: 0x0038E23C File Offset: 0x0038C43C
	public static IEnumerator CalcCrcCoroutine(string _filename, Action<uint> _resultCallback, int _maxTimePerFrameMs = 5, byte[] _buffer = null)
	{
		MicroStopwatch msw = new MicroStopwatch(true);
		using (Stream stream = SdFile.OpenRead(_filename))
		{
			if (_buffer == null || _buffer.Length < 8192)
			{
				_buffer = new byte[32768];
			}
			int bufferSize = _buffer.Length;
			uint crc = 0U;
			int bytesRead;
			do
			{
				bytesRead = stream.Read(_buffer, 0, bufferSize);
				if (bytesRead > 0)
				{
					crc = Crc32Algorithm.Append(crc, _buffer, 0, bytesRead);
				}
				if (msw.ElapsedMilliseconds >= (long)_maxTimePerFrameMs)
				{
					yield return null;
					msw.ResetAndRestart();
				}
			}
			while (bytesRead > 0);
			_resultCallback(crc);
		}
		Stream stream = null;
		yield break;
		yield break;
	}

	// Token: 0x06008DA7 RID: 36263 RVA: 0x0038E260 File Offset: 0x0038C460
	public static uint HashUint(this Crc32Algorithm _crcInstance)
	{
		if (_crcInstance == null)
		{
			throw new ArgumentNullException("_crcInstance");
		}
		byte[] hash = _crcInstance.Hash;
		return (uint)((int)hash[0] << 24 | (int)hash[1] << 16 | (int)hash[2] << 8 | (int)hash[3]);
	}

	// Token: 0x04006DD6 RID: 28118
	[PublicizedFrom(EAccessModifier.Private)]
	public const int minBufferSize = 8192;

	// Token: 0x04006DD7 RID: 28119
	public const int DefaultBufferSize = 32768;
}
