using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200026A RID: 618
[Preserve]
public abstract class ConsoleCmdTeleportsAbs : ConsoleCmdAbstract
{
	// Token: 0x0600118D RID: 4493 RVA: 0x0006E438 File Offset: 0x0006C638
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool TryParseV3i(List<string> _params, int _startIndex, out Vector3i _result)
	{
		_result = default(Vector3i);
		if (!int.TryParse(_params[_startIndex], out _result.x))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("x argument is not a valid integer");
			return false;
		}
		if (!int.TryParse(_params[_startIndex + 1], out _result.y))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("y argument is not a valid integer");
			return false;
		}
		if (!int.TryParse(_params[_startIndex + 2], out _result.z))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("z argument is not a valid integer");
			return false;
		}
		return true;
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x0006E4C0 File Offset: 0x0006C6C0
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector3? TryParseViewDirection(string _viewDirectionString)
	{
		if (_viewDirectionString.EqualsCaseInsensitive("n") || _viewDirectionString.EqualsCaseInsensitive("north"))
		{
			return new Vector3?(new Vector3(0f, 0f, 0f));
		}
		if (_viewDirectionString.EqualsCaseInsensitive("ne") || _viewDirectionString.EqualsCaseInsensitive("northeast"))
		{
			return new Vector3?(new Vector3(0f, 45f, 0f));
		}
		if (_viewDirectionString.EqualsCaseInsensitive("e") || _viewDirectionString.EqualsCaseInsensitive("east"))
		{
			return new Vector3?(new Vector3(0f, 90f, 0f));
		}
		if (_viewDirectionString.EqualsCaseInsensitive("se") || _viewDirectionString.EqualsCaseInsensitive("southeast"))
		{
			return new Vector3?(new Vector3(0f, 135f, 0f));
		}
		if (_viewDirectionString.EqualsCaseInsensitive("s") || _viewDirectionString.EqualsCaseInsensitive("south"))
		{
			return new Vector3?(new Vector3(0f, 180f, 0f));
		}
		if (_viewDirectionString.EqualsCaseInsensitive("sw") || _viewDirectionString.EqualsCaseInsensitive("southwest"))
		{
			return new Vector3?(new Vector3(0f, 225f, 0f));
		}
		if (_viewDirectionString.EqualsCaseInsensitive("w") || _viewDirectionString.EqualsCaseInsensitive("west"))
		{
			return new Vector3?(new Vector3(0f, 270f, 0f));
		}
		if (_viewDirectionString.EqualsCaseInsensitive("nw") || _viewDirectionString.EqualsCaseInsensitive("northwest"))
		{
			return new Vector3?(new Vector3(0f, 315f, 0f));
		}
		return null;
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x0006E678 File Offset: 0x0006C878
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityPlayer GetExecutingEntityPlayer(CommandSenderInfo _senderInfo)
	{
		if (_senderInfo.IsLocalGame)
		{
			return GameManager.Instance.World.GetPrimaryPlayer();
		}
		if (_senderInfo.RemoteClientInfo != null)
		{
			return GameManager.Instance.World.Players.dict[_senderInfo.RemoteClientInfo.entityId];
		}
		return null;
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0006E6CC File Offset: 0x0006C8CC
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool TryGetDestinationFromPlayer(string _targetPlayerString, out Vector3 _destination)
	{
		_destination = default(Vector3);
		ClientInfo clientInfo = ConsoleHelper.ParseParamIdOrName(_targetPlayerString, true, false);
		EntityPlayer entityPlayer;
		if (clientInfo == null)
		{
			if (GameManager.IsDedicatedServer || !ConsoleHelper.ParamIsLocalPlayer(_targetPlayerString, true, false))
			{
				SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Target playername or entity/userid id not found.");
				return false;
			}
			entityPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		}
		else
		{
			entityPlayer = GameManager.Instance.World.Players.dict[clientInfo.entityId];
		}
		_destination = entityPlayer.GetPosition();
		_destination.y += 1f;
		_destination.z += 1f;
		return true;
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x0006E76B File Offset: 0x0006C96B
	[PublicizedFrom(EAccessModifier.Protected)]
	public void ExecuteTeleport(ClientInfo _player, Vector3 _destPos, Vector3? _viewDirection)
	{
		ThreadManager.StartCoroutine(this.ExecuteTeleportCo(_player, _destPos, _viewDirection));
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x0006E77C File Offset: 0x0006C97C
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ExecuteTeleportCo(ClientInfo _player, Vector3 _destPos, Vector3? _viewDirection)
	{
		int playerId = (_player != null) ? _player.entityId : ((GameManager.Instance.GetPersistentLocalPlayer() != null) ? GameManager.Instance.GetPersistentLocalPlayer().EntityId : -1);
		yield return GameManager.Instance.ResetWindowsAndLocksByPlayer(playerId);
		NetPackageTeleportPlayer netPackageTeleportPlayer = NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(_destPos, _viewDirection, false);
		if (_player == null)
		{
			netPackageTeleportPlayer.ProcessPackage(GameManager.Instance.World, GameManager.Instance);
		}
		else
		{
			_player.SendPackage(netPackageTeleportPlayer);
		}
		yield break;
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x0005855F File Offset: 0x0005675F
	[PublicizedFrom(EAccessModifier.Protected)]
	public ConsoleCmdTeleportsAbs()
	{
	}
}
