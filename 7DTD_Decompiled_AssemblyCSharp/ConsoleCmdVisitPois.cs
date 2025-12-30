using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000277 RID: 631
[Preserve]
public class ConsoleCmdVisitPois : ConsoleCmdAbstract
{
	// Token: 0x170001E0 RID: 480
	// (get) Token: 0x060011E6 RID: 4582 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060011E7 RID: 4583 RVA: 0x0006FD9F File Offset: 0x0006DF9F
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"vpois",
			"visitpois"
		};
	}

	// Token: 0x060011E8 RID: 4584 RVA: 0x0002B133 File Offset: 0x00029333
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "";
	}

	// Token: 0x060011E9 RID: 4585 RVA: 0x0006FDB7 File Offset: 0x0006DFB7
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "<s[tart] [pois per auto-pause]|p[ause]|r[eset]>";
	}

	// Token: 0x060011EA RID: 4586 RVA: 0x0006FDBE File Offset: 0x0006DFBE
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!this.ExecuteInternal(_params, _senderInfo))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
		}
	}

	// Token: 0x060011EB RID: 4587 RVA: 0x0006FDDC File Offset: 0x0006DFDC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ExecuteInternal(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (!GameManager.Instance.World.GetPrimaryPlayer())
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("No local player! (Are you in-game?)");
			return true;
		}
		if (_params.Count == 0)
		{
			return false;
		}
		string a = _params[0].ToLowerInvariant();
		if (!(a == "s") && !(a == "start"))
		{
			if (a == "p" || a == "pause")
			{
				this.MacroPause();
				return true;
			}
			if (!(a == "r") && !(a == "reset"))
			{
				return false;
			}
			this.MacroReset();
			return true;
		}
		else
		{
			if (_params.Count > 2)
			{
				return false;
			}
			int poisPerAutoPause = 0;
			if (_params.Count > 1 && !int.TryParse(_params[1], out poisPerAutoPause))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Failed to parse as int " + _params[1]);
				return false;
			}
			this.MacroStart(poisPerAutoPause);
			return true;
		}
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x0006FED4 File Offset: 0x0006E0D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroStart(int _poisPerAutoPause)
	{
		if (this.m_coroutine != null)
		{
			return;
		}
		Coroutine coroutine = ThreadManager.StartCoroutine(this.CoroutineVisit(_poisPerAutoPause));
		if (this.m_isRunning)
		{
			this.m_coroutine = coroutine;
		}
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x0006FF06 File Offset: 0x0006E106
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroPause()
	{
		if (this.m_isRunning)
		{
			this.m_isRunning = false;
			return;
		}
		this.m_isRunning = true;
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x0006FF1F File Offset: 0x0006E11F
	[PublicizedFrom(EAccessModifier.Private)]
	public void MacroReset()
	{
		if (this.m_coroutine == null)
		{
			return;
		}
		this.m_isRunning = false;
		ThreadManager.StopCoroutine(this.m_coroutine);
		this.m_coroutine = null;
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x0006FF43 File Offset: 0x0006E143
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator CoroutineVisit(int _poisPerAutoPause)
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			yield break;
		}
		EntityPlayerLocal player;
		if (!ProfilerGameUtils.TryGetFlyingPlayer(out player))
		{
			yield break;
		}
		this.m_isRunning = true;
		int poisVisited = 0;
		foreach (PrefabInstance prefabInstance in GameManager.Instance.GetDynamicPrefabDecorator().allPrefabs.ToArray())
		{
			while (!this.m_isRunning)
			{
				yield return new WaitForSeconds(1f);
			}
			Vector3 size = prefabInstance.GetAABB().size;
			if (size.x > ConsoleCmdVisitPois.MinPoiSize.x && size.y > ConsoleCmdVisitPois.MinPoiSize.y && size.z > ConsoleCmdVisitPois.MinPoiSize.z)
			{
				if (world != GameManager.Instance.World || player != world.GetPrimaryPlayer())
				{
					this.MacroReset();
					yield break;
				}
				Bounds aabb = prefabInstance.GetAABB();
				Vector3 center = aabb.center;
				Log.Out(string.Format("Visit Pois: {0} ({1}, {2}, {3}) {4}", new object[]
				{
					prefabInstance.name,
					center.x,
					center.y,
					center.z,
					aabb
				}));
				player.SetPosition(center, true);
				yield return ProfilerGameUtils.WaitForChunksAroundObserverToLoad(player.ChunkObserver, ChunkConditions.Displayed);
				yield return null;
				yield return null;
				yield return null;
				if ((_poisPerAutoPause > 0 && poisVisited % _poisPerAutoPause == 0) || poisVisited == 0)
				{
					Log.Out(string.Format("Visit Pois: #{0} (PAUSED)", poisVisited));
					this.MacroPause();
				}
				int num = poisVisited;
				poisVisited = num + 1;
				prefabInstance = null;
			}
		}
		PrefabInstance[] array = null;
		this.MacroReset();
		yield break;
	}

	// Token: 0x04000BD1 RID: 3025
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_isRunning;

	// Token: 0x04000BD2 RID: 3026
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine m_coroutine;

	// Token: 0x04000BD3 RID: 3027
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Vector3 MinPoiSize = new Vector3i(5, 5, 5);
}
