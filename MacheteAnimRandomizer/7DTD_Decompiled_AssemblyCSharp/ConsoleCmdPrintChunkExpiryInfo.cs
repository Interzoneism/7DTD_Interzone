using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200022B RID: 555
[Preserve]
public class ConsoleCmdPrintChunkExpiryInfo : ConsoleCmdAbstract
{
	// Token: 0x06001035 RID: 4149 RVA: 0x00069074 File Offset: 0x00067274
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"expiryinfo"
		};
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x00069084 File Offset: 0x00067284
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Prints location and expiry day/time for the next [x] chunks set to expire.";
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x0006908B File Offset: 0x0006728B
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "expiryinfo [x]\n" + this.GetDescription();
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x000690A0 File Offset: 0x000672A0
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		ChunkProviderGenerateWorld chunkProviderGenerateWorld = GameManager.Instance.World.ChunkCache.ChunkProvider as ChunkProviderGenerateWorld;
		if (chunkProviderGenerateWorld == null)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Failed to retrieve chunk expiry info: ChunkProviderGenerateWorld could not be found for current world instance.");
			return;
		}
		int num;
		if (_params.Count != 1 || !int.TryParse(_params[0], out num) || num < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		try
		{
			List<KeyValuePair<long, ulong>> expiryTimes = new List<KeyValuePair<long, ulong>>();
			chunkProviderGenerateWorld.IterateChunkExpiryTimes(delegate(long chunkKey, ulong expiry)
			{
				expiryTimes.Add(new KeyValuePair<long, ulong>(chunkKey, expiry));
			});
			num = Mathf.Min(num, expiryTimes.Count);
			if (num == 0)
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No chunks are currently set to expire. Ensure max chunk age is enabled.");
			}
			else
			{
				expiryTimes.Sort((KeyValuePair<long, ulong> a, KeyValuePair<long, ulong> b) => a.Value.CompareTo(b.Value));
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Chunk\t\tExpiry");
				for (int i = 0; i < num; i++)
				{
					long key = expiryTimes[i].Key;
					ulong value = expiryTimes[i].Value;
					int num2 = WorldChunkCache.extractX(key);
					int num3 = WorldChunkCache.extractZ(key);
					ValueTuple<int, int, int> valueTuple = GameUtils.WorldTimeToElements(value);
					int item = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					int item3 = valueTuple.Item3;
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("{0}, {1}\t\tDay {2}, {3}", new object[]
					{
						num2,
						num3,
						item,
						string.Format("{0:D2}:{1:D2}", item2, item3)
					}));
				}
			}
		}
		catch (Exception ex)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Failed to retrieve chunk expiry info with exception: " + ex.Message);
		}
	}
}
