using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000254 RID: 596
[Preserve]
public class ConsoleCmdSleeper : ConsoleCmdAbstract
{
	// Token: 0x06001114 RID: 4372 RVA: 0x0006BBE6 File Offset: 0x00069DE6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"sleeper"
		};
	}

	// Token: 0x06001115 RID: 4373 RVA: 0x0006BBF6 File Offset: 0x00069DF6
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Drawn or list sleeper info";
	}

	// Token: 0x06001116 RID: 4374 RVA: 0x0006BBFD File Offset: 0x00069DFD
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "draw - toggle drawing for current player prefab\nlist - list for current player prefab\nlistall - list all\nr - reset all";
	}

	// Token: 0x06001117 RID: 4375 RVA: 0x0006BC04 File Offset: 0x00069E04
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		string a = _params[0].ToLower();
		if (!(a == "draw"))
		{
			if (a == "listall")
			{
				this.LogInfo(false);
				return;
			}
			if (a == "list")
			{
				this.LogInfo(true);
				return;
			}
			if (!(a == "r"))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Command not recognized. <end/>");
				return;
			}
			this.Reset();
			return;
		}
		else
		{
			if (this.drawVolumesCo != null)
			{
				GameManager.Instance.StopCoroutine(this.drawVolumesCo);
				this.drawVolumesCo = null;
				return;
			}
			this.drawVolumesCo = GameManager.Instance.StartCoroutine(this.DrawVolumes());
			return;
		}
	}

	// Token: 0x06001118 RID: 4376 RVA: 0x0006BCCC File Offset: 0x00069ECC
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogInfo(bool onlyPlayer)
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		EntityPlayerLocal entityPlayerLocal = onlyPlayer ? world.GetPrimaryPlayer() : null;
		int sleeperVolumeCount = world.GetSleeperVolumeCount();
		int num = 0;
		int i = 0;
		while (i < sleeperVolumeCount)
		{
			SleeperVolume sleeperVolume = world.GetSleeperVolume(i);
			if (!entityPlayerLocal)
			{
				goto IL_57;
			}
			if (sleeperVolume.PrefabInstance == entityPlayerLocal.prefab)
			{
				sleeperVolume.Draw(3f);
				goto IL_57;
			}
			IL_80:
			i++;
			continue;
			IL_57:
			num++;
			this.Print("#{0} {1}", new object[]
			{
				i,
				sleeperVolume.GetDescription()
			});
			goto IL_80;
		}
		this.Print("Sleeper volumes {0} of {1}", new object[]
		{
			num,
			sleeperVolumeCount
		});
	}

	// Token: 0x06001119 RID: 4377 RVA: 0x0006BD87 File Offset: 0x00069F87
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator DrawVolumes()
	{
		int num;
		for (int i = 0; i < 99999; i = num)
		{
			World world = GameManager.Instance.World;
			if (world == null)
			{
				break;
			}
			EntityPlayerLocal primaryPlayer = world.GetPrimaryPlayer();
			if (!primaryPlayer)
			{
				break;
			}
			int sleeperVolumeCount = world.GetSleeperVolumeCount();
			for (int j = 0; j < sleeperVolumeCount; j++)
			{
				SleeperVolume sleeperVolume = world.GetSleeperVolume(j);
				if (sleeperVolume.PrefabInstance == primaryPlayer.prefab)
				{
					sleeperVolume.DrawDebugLines(1f);
				}
			}
			int triggerVolumeCount = world.GetTriggerVolumeCount();
			for (int k = 0; k < triggerVolumeCount; k++)
			{
				TriggerVolume triggerVolume = world.GetTriggerVolume(k);
				if (triggerVolume.PrefabInstance == primaryPlayer.prefab)
				{
					triggerVolume.DrawDebugLines(1f);
				}
			}
			yield return new WaitForSeconds(0.5f);
			num = i + 1;
		}
		this.drawVolumesCo = null;
		yield break;
	}

	// Token: 0x0600111A RID: 4378 RVA: 0x0006BD98 File Offset: 0x00069F98
	[PublicizedFrom(EAccessModifier.Private)]
	public void Reset()
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		int sleeperVolumeCount = world.GetSleeperVolumeCount();
		for (int i = 0; i < sleeperVolumeCount; i++)
		{
			SleeperVolume sleeperVolume = world.GetSleeperVolume(i);
			if (sleeperVolume != null)
			{
				sleeperVolume.DespawnAndReset(world);
			}
		}
		this.Print("Reset {0}", new object[]
		{
			sleeperVolumeCount
		});
	}

	// Token: 0x0600111B RID: 4379 RVA: 0x0006BDF4 File Offset: 0x00069FF4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Print(string _s, params object[] _values)
	{
		string line = string.Format(_s, _values);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output(line);
	}

	// Token: 0x04000B9E RID: 2974
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine drawVolumesCo;
}
