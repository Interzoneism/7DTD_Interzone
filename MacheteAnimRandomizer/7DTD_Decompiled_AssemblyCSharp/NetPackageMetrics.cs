using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020006EF RID: 1775
[Preserve]
public class NetPackageMetrics
{
	// Token: 0x1700053E RID: 1342
	// (get) Token: 0x06003450 RID: 13392 RVA: 0x0015FD79 File Offset: 0x0015DF79
	public static NetPackageMetrics Instance
	{
		get
		{
			return NetPackageMetrics._instance;
		}
	}

	// Token: 0x06003451 RID: 13393 RVA: 0x0015FD80 File Offset: 0x0015DF80
	public NetPackageMetrics()
	{
		NetPackageMetrics._instance = this;
		this.lastUpdateTime = Time.time - this.updateLength;
		this.receivedNetPackageCounts = new Dictionary<string, int>();
		this.receivedNetPackageSizes = new Dictionary<string, List<int>>();
		this.sentNetPackageCounts = new Dictionary<string, int>();
		this.sentNetPackageSizes = new Dictionary<string, List<int>>();
		this.packagesSent = new Dictionary<string, List<PackagesSentInfoEntry>>();
	}

	// Token: 0x06003452 RID: 13394 RVA: 0x0015FE0C File Offset: 0x0015E00C
	public void ResetStats()
	{
		SingletonMonoBehaviour<ConnectionManager>.Instance.ResetNetworkStatistics();
		this.receivedNetPackageCounts.Clear();
		this.sentNetPackageCounts.Clear();
		this.sentNetPackageSizes = new Dictionary<string, List<int>>();
		this.receivedNetPackageSizes = new Dictionary<string, List<int>>();
		this.packagesSent = new Dictionary<string, List<PackagesSentInfoEntry>>();
		NetPackageMetrics.tick = 0;
		Log.Out("Network stats reset");
	}

	// Token: 0x06003453 RID: 13395 RVA: 0x0015FE6A File Offset: 0x0015E06A
	public void SetUpdateLength(float length)
	{
		Log.Out("Setting network stat length to " + length.ToString() + " seconds");
		this.updateLength = length;
		this.lastUpdateTime = Time.time;
	}

	// Token: 0x06003454 RID: 13396 RVA: 0x0015FE99 File Offset: 0x0015E099
	public void RestartTimer()
	{
		this.lastUpdateTime = Time.time;
	}

	// Token: 0x06003455 RID: 13397 RVA: 0x0015FEA6 File Offset: 0x0015E0A6
	public void RecordForPeriod(float length)
	{
		this.active = true;
		this.ResetStats();
		this.SetUpdateLength(length);
		this.RestartTimer();
		SingletonMonoBehaviour<ConnectionManager>.Instance.EnableNetworkStatistics();
		this.entityAliveCount = UnityEngine.Object.FindObjectsOfType<EntityAlive>().Length;
		this.playerCount = UnityEngine.Object.FindObjectsOfType<EntityPlayer>().Length;
	}

	// Token: 0x06003456 RID: 13398 RVA: 0x0015FEE6 File Offset: 0x0015E0E6
	public void CopyToClipboard()
	{
		TextEditor textEditor = new TextEditor();
		textEditor.text = this.lastStatsOutput;
		textEditor.SelectAll();
		textEditor.Copy();
	}

	// Token: 0x06003457 RID: 13399 RVA: 0x0015FF04 File Offset: 0x0015E104
	public void CopyToCSV(bool includeDetails = false)
	{
		TextEditor textEditor = new TextEditor();
		textEditor.text = this.ProduceCSV(includeDetails);
		textEditor.SelectAll();
		textEditor.Copy();
	}

	// Token: 0x06003458 RID: 13400 RVA: 0x0015FF24 File Offset: 0x0015E124
	public void RegisterReceivedPackage(string packageType, int length)
	{
		if (!this.active)
		{
			return;
		}
		if (this.receivedNetPackageCounts.ContainsKey(packageType))
		{
			Dictionary<string, int> dictionary = this.receivedNetPackageCounts;
			int num = dictionary[packageType];
			dictionary[packageType] = num + 1;
		}
		else
		{
			this.receivedNetPackageCounts[packageType] = 1;
			this.receivedNetPackageSizes[packageType] = new List<int>();
		}
		this.receivedNetPackageSizes[packageType].Add(length);
	}

	// Token: 0x06003459 RID: 13401 RVA: 0x0015FF94 File Offset: 0x0015E194
	public void RegisterSentPackage(string packageType, int length)
	{
		if (!this.active)
		{
			return;
		}
		if (this.sentNetPackageCounts.ContainsKey(packageType))
		{
			Dictionary<string, int> dictionary = this.sentNetPackageCounts;
			int num = dictionary[packageType];
			dictionary[packageType] = num + 1;
		}
		else
		{
			this.sentNetPackageCounts[packageType] = 1;
			this.sentNetPackageSizes[packageType] = new List<int>();
		}
		this.sentNetPackageSizes[packageType].Add(length);
	}

	// Token: 0x0600345A RID: 13402 RVA: 0x00160004 File Offset: 0x0015E204
	public void RegisterPackagesSent(List<NetPackageInfo> packages, int count, long uncompressedSize, long compressedSize, float timeStamp, string client)
	{
		if (!this.active)
		{
			return;
		}
		PackagesSentInfoEntry item = new PackagesSentInfoEntry
		{
			packages = new List<NetPackageInfo>(packages),
			count = count,
			bCompressed = (compressedSize != -1L),
			uncompressedSize = uncompressedSize,
			compressedSize = ((compressedSize == -1L) ? uncompressedSize : compressedSize),
			timestamp = timeStamp,
			client = client
		};
		if (!this.packagesSent.ContainsKey(client))
		{
			this.packagesSent[client] = new List<PackagesSentInfoEntry>();
		}
		this.packagesSent[client].Add(item);
	}

	// Token: 0x0600345B RID: 13403 RVA: 0x001600A0 File Offset: 0x0015E2A0
	public string ProduceCSV(bool includeDetails = false)
	{
		string text = "";
		int num = 0;
		int num2 = 0;
		text += "NET METRICS";
		text = text + "\nPrioritization Enabled: " + GameManager.enableNetworkdPrioritization.ToString();
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer)
		{
			text = text + "\nPlayer name: " + primaryPlayer.EntityName;
		}
		text += "\n\nSent NetPackages:";
		text += "\npackage type,count,total length, average length";
		foreach (KeyValuePair<string, int> keyValuePair in this.sentNetPackageCounts)
		{
			num2 += keyValuePair.Value;
			float num3 = 0f;
			foreach (int num4 in this.sentNetPackageSizes[keyValuePair.Key])
			{
				num3 += (float)num4;
			}
			float num5 = num3 / (float)keyValuePair.Value;
			text = string.Concat(new string[]
			{
				text,
				"\n",
				keyValuePair.Key,
				",",
				keyValuePair.Value.ToString(),
				",",
				num3.ToString(),
				",",
				num5.ToString()
			});
		}
		text += "\n\nReceived NetPackages:";
		text += "\npackage type,count,total length, average length";
		foreach (KeyValuePair<string, int> keyValuePair2 in this.receivedNetPackageCounts)
		{
			num += keyValuePair2.Value;
			float num3 = 0f;
			foreach (int num6 in this.receivedNetPackageSizes[keyValuePair2.Key])
			{
				num3 += (float)num6;
			}
			float num5 = num3 / (float)keyValuePair2.Value;
			text = string.Concat(new string[]
			{
				text,
				"\n",
				keyValuePair2.Key,
				",",
				keyValuePair2.Value.ToString(),
				",",
				num3.ToString(),
				",",
				num5.ToString()
			});
		}
		text += "\n\n\nTotals";
		text = text + "\nPackages Sent: " + num2.ToString();
		text = text + "\nPackages Received: " + num.ToString();
		text = text + "\n\nPlayers: " + this.playerCount.ToString();
		text = text + "\n\nEntityAlive Count: " + this.entityAliveCount.ToString();
		foreach (KeyValuePair<string, List<PackagesSentInfoEntry>> keyValuePair3 in this.packagesSent)
		{
			new List<PackagesSentInfoEntry>();
			text = text + "\n Packages sent for client " + keyValuePair3.Key + "\n\n";
			text += "\nmilliseconds,count,uncompressed size, compressed size";
			foreach (PackagesSentInfoEntry packagesSentInfoEntry in keyValuePair3.Value)
			{
				if (this.includeRelPosRot)
				{
					text = string.Concat(new string[]
					{
						text,
						"\n",
						packagesSentInfoEntry.timestamp.ToString(),
						",",
						packagesSentInfoEntry.count.ToString(),
						",",
						packagesSentInfoEntry.uncompressedSize.ToString(),
						",",
						packagesSentInfoEntry.compressedSize.ToString()
					});
				}
				else
				{
					int num7 = 0;
					int num8 = 0;
					foreach (NetPackageInfo netPackageInfo in packagesSentInfoEntry.packages)
					{
						if (!(netPackageInfo.netPackageType == "NetPackageEntityRelPosAndRot"))
						{
							num7++;
							num8 += netPackageInfo.length;
						}
					}
					text = string.Concat(new string[]
					{
						text,
						"\n",
						packagesSentInfoEntry.timestamp.ToString(),
						",",
						num7.ToString(),
						",",
						num8.ToString()
					});
				}
				if (includeDetails)
				{
					text += "\n,package type,package length";
					foreach (NetPackageInfo netPackageInfo2 in packagesSentInfoEntry.packages)
					{
						if (this.includeRelPosRot || !(netPackageInfo2.netPackageType == "NetPackageEntityRelPosAndRot"))
						{
							text = string.Concat(new string[]
							{
								text,
								"\n,",
								netPackageInfo2.netPackageType,
								",",
								netPackageInfo2.length.ToString()
							});
						}
					}
				}
			}
		}
		return text;
	}

	// Token: 0x0600345C RID: 13404 RVA: 0x001606A0 File Offset: 0x0015E8A0
	public void Update()
	{
		if (!this.active)
		{
			return;
		}
		if (Time.time - this.lastUpdateTime >= this.updateLength)
		{
			this.lastStatsOutput = SingletonMonoBehaviour<ConnectionManager>.Instance.PrintNetworkStatistics();
			int num = 0;
			int num2 = 0;
			this.lastStatsOutput += "NET METRICS";
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			if (primaryPlayer)
			{
				this.lastStatsOutput = this.lastStatsOutput + "\nPlayer name: " + primaryPlayer.EntityName;
			}
			this.lastStatsOutput += "\n\nSent NetPackages:";
			foreach (KeyValuePair<string, int> keyValuePair in this.sentNetPackageCounts)
			{
				num2 += keyValuePair.Value;
				float num3 = 0f;
				foreach (int num4 in this.sentNetPackageSizes[keyValuePair.Key])
				{
					num3 += (float)num4;
				}
				float num5 = num3 / (float)keyValuePair.Value;
				this.lastStatsOutput = string.Concat(new string[]
				{
					this.lastStatsOutput,
					"\n",
					keyValuePair.Key,
					": Count:",
					keyValuePair.Value.ToString(),
					" Total Size: ",
					num3.ToString(),
					" Avg Size: ",
					num5.ToString()
				});
			}
			this.lastStatsOutput += "\n\nReceived NetPackages:";
			foreach (KeyValuePair<string, int> keyValuePair2 in this.receivedNetPackageCounts)
			{
				num += keyValuePair2.Value;
				float num3 = 0f;
				foreach (int num6 in this.receivedNetPackageSizes[keyValuePair2.Key])
				{
					num3 += (float)num6;
				}
				float num5 = num3 / (float)keyValuePair2.Value;
				this.lastStatsOutput = string.Concat(new string[]
				{
					this.lastStatsOutput,
					"\n",
					keyValuePair2.Key,
					": Count:",
					keyValuePair2.Value.ToString(),
					" Total Size: ",
					num3.ToString(),
					" Avg Size: ",
					num5.ToString()
				});
			}
			this.lastStatsOutput += "\n\n\nTotals";
			this.lastStatsOutput = this.lastStatsOutput + "\nPackages Sent: " + num2.ToString();
			this.lastStatsOutput = this.lastStatsOutput + "\nPackages Received: " + num.ToString();
			this.lastStatsOutput = this.lastStatsOutput + "\n\nPlayers: " + this.playerCount.ToString();
			this.lastStatsOutput = this.lastStatsOutput + "\n\nEntityAlive Count: " + this.entityAliveCount.ToString();
			Log.Out(this.lastStatsOutput);
			string csv = this.ProduceCSV(false);
			this.lastUpdateTime = Time.time;
			this.active = false;
			if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				NetPackageNetMetrics package = NetPackageNetMetrics.SetupClient(this.lastStatsOutput, csv);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
			}
		}
	}

	// Token: 0x0600345D RID: 13405 RVA: 0x00160A70 File Offset: 0x0015EC70
	public void OutputPackageSentDetails(string client, float timestamp)
	{
		List<PackagesSentInfoEntry> list;
		if (this.packagesSent.TryGetValue(client, out list))
		{
			PackagesSentInfoEntry packagesSentInfoEntry = list.Find((PackagesSentInfoEntry x) => x.timestamp == timestamp);
			if (packagesSentInfoEntry != null)
			{
				string text = string.Concat(new string[]
				{
					"Packages for ",
					client,
					" at timestamp ",
					timestamp.ToString(),
					"\n\n"
				});
				text += this.GetOutputPacakageSentDetails(packagesSentInfoEntry);
				Log.Out(text);
				TextEditor textEditor = new TextEditor();
				textEditor.text = text;
				textEditor.SelectAll();
				textEditor.Copy();
				Log.Out("Copied to clipboard");
			}
		}
	}

	// Token: 0x0600345E RID: 13406 RVA: 0x00160B20 File Offset: 0x0015ED20
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetOutputPacakageSentDetails(PackagesSentInfoEntry entry)
	{
		string text = "\nPackage Type, Length";
		foreach (NetPackageInfo netPackageInfo in entry.packages)
		{
			text = string.Concat(new string[]
			{
				text,
				"\n",
				netPackageInfo.netPackageType,
				",",
				netPackageInfo.length.ToString()
			});
		}
		return text;
	}

	// Token: 0x0600345F RID: 13407 RVA: 0x00160BAC File Offset: 0x0015EDAC
	public void AppendClientCSV(string csv)
	{
		this.clientCSV = this.clientCSV + "\n\n" + csv;
	}

	// Token: 0x04002AE0 RID: 10976
	[PublicizedFrom(EAccessModifier.Private)]
	public static NetPackageMetrics _instance;

	// Token: 0x04002AE1 RID: 10977
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateLength = 5f;

	// Token: 0x04002AE2 RID: 10978
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastUpdateTime;

	// Token: 0x04002AE3 RID: 10979
	[PublicizedFrom(EAccessModifier.Private)]
	public string lastStatsOutput = "";

	// Token: 0x04002AE4 RID: 10980
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, int> receivedNetPackageCounts;

	// Token: 0x04002AE5 RID: 10981
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, List<int>> receivedNetPackageSizes;

	// Token: 0x04002AE6 RID: 10982
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, int> sentNetPackageCounts;

	// Token: 0x04002AE7 RID: 10983
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, List<int>> sentNetPackageSizes;

	// Token: 0x04002AE8 RID: 10984
	[PublicizedFrom(EAccessModifier.Private)]
	public int playerCount;

	// Token: 0x04002AE9 RID: 10985
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityAliveCount;

	// Token: 0x04002AEA RID: 10986
	[PublicizedFrom(EAccessModifier.Private)]
	public bool active;

	// Token: 0x04002AEB RID: 10987
	public string clientCSV = "";

	// Token: 0x04002AEC RID: 10988
	[PublicizedFrom(EAccessModifier.Private)]
	public static int tick;

	// Token: 0x04002AED RID: 10989
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, List<PackagesSentInfoEntry>> packagesSent;

	// Token: 0x04002AEE RID: 10990
	public bool includeRelPosRot = true;
}
