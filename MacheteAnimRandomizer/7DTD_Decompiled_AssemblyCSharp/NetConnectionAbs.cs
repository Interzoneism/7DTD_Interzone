using System;
using System.Collections.Generic;
using System.IO;
using Noemax.GZip;
using UnityEngine.Profiling;

// Token: 0x020006C8 RID: 1736
public abstract class NetConnectionAbs : INetConnection
{
	// Token: 0x06003328 RID: 13096 RVA: 0x00159E80 File Offset: 0x00158080
	[PublicizedFrom(EAccessModifier.Protected)]
	public NetConnectionAbs(int _channel, ClientInfo _clientInfo, INetworkClient _netClient, string _uniqueId)
	{
		this.channel = _channel;
		this.cInfo = _clientInfo;
		this.netClient = _netClient;
		this.isServer = (_clientInfo != null);
		NetConnectionAbs.encryptedStreamReceived = false;
		this.connectionIdentifier = (this.isServer ? (_uniqueId + "_" + _channel.ToString()) : _channel.ToString());
		NetPackageLogger.BeginLog(this.isServer);
	}

	// Token: 0x06003329 RID: 13097 RVA: 0x00159F25 File Offset: 0x00158125
	public void SetEncryptionModule(IEncryptionModule _module)
	{
		this.encryptionModule = _module;
	}

	// Token: 0x0600332A RID: 13098 RVA: 0x00159F30 File Offset: 0x00158130
	public virtual void Disconnect(bool _kick)
	{
		if (!this.bDisconnected)
		{
			NetPackageLogger.EndLog();
		}
		this.bDisconnected = true;
		if (_kick && this.cInfo != null)
		{
			this.cInfo.disconnecting = true;
			SingletonMonoBehaviour<ConnectionManager>.Instance.DisconnectClient(this.cInfo, false, false);
		}
	}

	// Token: 0x0600332B RID: 13099 RVA: 0x00159F7E File Offset: 0x0015817E
	public virtual bool IsDisconnected()
	{
		return this.bDisconnected;
	}

	// Token: 0x0600332C RID: 13100 RVA: 0x00159F88 File Offset: 0x00158188
	public virtual void GetPackages(List<NetPackage> _dstBuf)
	{
		_dstBuf.Clear();
		if (this.receivedPackages.Count == 0)
		{
			return;
		}
		List<NetPackage> obj = this.receivedPackages;
		lock (obj)
		{
			_dstBuf.AddRange(this.receivedPackages);
			this.receivedPackages.Clear();
		}
	}

	// Token: 0x0600332D RID: 13101 RVA: 0x00159FF0 File Offset: 0x001581F0
	public virtual void AddToSendQueue(List<NetPackage> _packages)
	{
		for (int i = 0; i < _packages.Count; i++)
		{
			this.AddToSendQueue(_packages[i]);
		}
	}

	// Token: 0x0600332E RID: 13102 RVA: 0x0015A01B File Offset: 0x0015821B
	public virtual void UpgradeToFullConnection()
	{
		this.InitStreams(true);
		this.allowCompression = true;
	}

	// Token: 0x0600332F RID: 13103 RVA: 0x0015A02B File Offset: 0x0015822B
	public virtual NetConnectionStatistics GetStats()
	{
		return this.stats;
	}

	// Token: 0x06003330 RID: 13104 RVA: 0x0015A034 File Offset: 0x00158234
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool Compress(bool _compress, MemoryStream _uncompressedSourceStream, DeflateOutputStream _zipTargetStream, MemoryStream _compressedTargetStream, byte[] _copyBuffer, int _packageCount)
	{
		if (_compress)
		{
			_compressedTargetStream.SetLength(0L);
			try
			{
				StreamUtils.StreamCopy(_uncompressedSourceStream, _zipTargetStream, _copyBuffer, true);
			}
			catch (Exception)
			{
				Log.Error(string.Concat(new string[]
				{
					"Compressed buffer size too small: Source stream size (",
					_uncompressedSourceStream.Length.ToString(),
					") > compressed stream capacity (",
					_compressedTargetStream.Capacity.ToString(),
					"), packages: ",
					_packageCount.ToString()
				}));
				throw;
			}
			_zipTargetStream.Restart();
			_compressedTargetStream.Position = 0L;
		}
		return _compress;
	}

	// Token: 0x06003331 RID: 13105 RVA: 0x0015A0D4 File Offset: 0x001582D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool Decompress(bool _compressed, MemoryStream _uncompressedTargetStream, DeflateInputStream _unzipSourceStream, byte[] _copyBuffer)
	{
		if (_compressed)
		{
			_uncompressedTargetStream.SetLength(0L);
			_unzipSourceStream.Restart();
			try
			{
				StreamUtils.StreamCopy(_unzipSourceStream, _uncompressedTargetStream, _copyBuffer, true);
			}
			catch (Exception e)
			{
				Log.Exception(e);
				throw;
			}
			_uncompressedTargetStream.Position = 0L;
		}
		return _compressed;
	}

	// Token: 0x06003332 RID: 13106 RVA: 0x0015A120 File Offset: 0x00158320
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool EnableEncryptData()
	{
		return this.encryptionModule != null && (this.isServer || NetConnectionAbs.encryptedStreamReceived);
	}

	// Token: 0x06003333 RID: 13107 RVA: 0x0015A13B File Offset: 0x0015833B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool ExpectEncryptedData()
	{
		return this.encryptionModule != null && this.isServer && this.cInfo.loginDone;
	}

	// Token: 0x06003334 RID: 13108 RVA: 0x0015A15C File Offset: 0x0015835C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool Encrypt(MemoryStream _stream)
	{
		if (this.EnableEncryptData())
		{
			bool result = this.encryptionModule.EncryptStream(this.cInfo, _stream);
			_stream.Position = 0L;
			return result;
		}
		return false;
	}

	// Token: 0x06003335 RID: 13109 RVA: 0x0015A184 File Offset: 0x00158384
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool Decrypt(bool _bEncrypted, MemoryStream _stream)
	{
		if (_bEncrypted)
		{
			NetConnectionAbs.encryptedStreamReceived = true;
			bool flag = this.encryptionModule.DecryptStream(this.cInfo, _stream);
			_stream.Position = 0L;
			if (!flag)
			{
				if (this.isServer)
				{
					GameUtils.KickPlayerForClientInfo(this.cInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.EncryptionFailure, 0, default(DateTime), ""));
					return flag;
				}
				GameUtils.ForceDisconnect(new GameUtils.KickPlayerData(GameUtils.EKickReason.EncryptionFailure, 0, default(DateTime), ""));
			}
			return flag;
		}
		if (!this.ExpectEncryptedData())
		{
			return true;
		}
		Log.Error(string.Format("[NET] Client logged in but sent unencrypted message, dropping! {0}", this.cInfo));
		this.cInfo.loginDone = false;
		GameUtils.KickPlayerForClientInfo(this.cInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.EncryptionFailure, 0, default(DateTime), ""));
		return false;
	}

	// Token: 0x06003336 RID: 13110 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void FlushSendQueue()
	{
	}

	// Token: 0x06003337 RID: 13111
	public abstract void AddToSendQueue(NetPackage _package);

	// Token: 0x06003338 RID: 13112
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void InitStreams(bool _full);

	// Token: 0x06003339 RID: 13113
	public abstract void AppendToReaderStream(byte[] _data, int _size);

	// Token: 0x040029DD RID: 10717
	public const int PROCESSING_BUFFER_SIZE = 2097152;

	// Token: 0x040029DE RID: 10718
	public const int COMPRESSED_BUFFER_SIZE = 2097152;

	// Token: 0x040029DF RID: 10719
	[PublicizedFrom(EAccessModifier.Protected)]
	public const int PREAUTH_BUFFER_SIZE = 32768;

	// Token: 0x040029E0 RID: 10720
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly int channel;

	// Token: 0x040029E1 RID: 10721
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly ClientInfo cInfo;

	// Token: 0x040029E2 RID: 10722
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly INetworkClient netClient;

	// Token: 0x040029E3 RID: 10723
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly bool isServer;

	// Token: 0x040029E4 RID: 10724
	[PublicizedFrom(EAccessModifier.Private)]
	public IEncryptionModule encryptionModule;

	// Token: 0x040029E5 RID: 10725
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly string connectionIdentifier;

	// Token: 0x040029E6 RID: 10726
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool fullConnection;

	// Token: 0x040029E7 RID: 10727
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool allowCompression;

	// Token: 0x040029E8 RID: 10728
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool encryptedStreamReceived;

	// Token: 0x040029E9 RID: 10729
	[PublicizedFrom(EAccessModifier.Protected)]
	public volatile bool bDisconnected;

	// Token: 0x040029EA RID: 10730
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly NetConnectionStatistics stats = new NetConnectionStatistics();

	// Token: 0x040029EB RID: 10731
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly List<NetPackage> receivedPackages = new List<NetPackage>();

	// Token: 0x040029EC RID: 10732
	[PublicizedFrom(EAccessModifier.Private)]
	public CustomSampler threadSamplerEncrypt = CustomSampler.Create("Encrypt", false);

	// Token: 0x040029ED RID: 10733
	[PublicizedFrom(EAccessModifier.Private)]
	public CustomSampler threadSamplerDecrypt = CustomSampler.Create("Decrypt", false);
}
