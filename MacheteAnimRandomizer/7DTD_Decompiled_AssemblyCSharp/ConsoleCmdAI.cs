using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020001AF RID: 431
[Preserve]
public class ConsoleCmdAI : ConsoleCmdAbstract
{
	// Token: 0x06000D36 RID: 3382 RVA: 0x00058BC4 File Offset: 0x00056DC4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"ai"
		};
	}

	// Token: 0x06000D37 RID: 3383 RVA: 0x00058BD4 File Offset: 0x00056DD4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "AI commands";
	}

	// Token: 0x06000D38 RID: 3384 RVA: 0x00058BDB File Offset: 0x00056DDB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "AI commands:\nactivityclear - remove all activity areas (heat)\nanim <name> - trigger an animation (attack, attack2)\nanimmove <forward> <strafe> - set animation forward and strafe motion\nfreezepos - toggles movement\nlatency - toggles drawing\npathlines - toggles drawing editor path lines\npathgrid - force grid update\nragdoll <force> <time>\nrage <speed> <time> - make all zombies rage (0 - 2, 0 stops) (seconds)\nsendnames - toggles admin clients receiving debug name info";
	}

	// Token: 0x06000D39 RID: 3385 RVA: 0x00058BE4 File Offset: 0x00056DE4
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		World world = GameManager.Instance.World;
		string text = _params[0].ToLower();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 2125639436U)
		{
			if (num <= 1141294589U)
			{
				if (num != 511673672U)
				{
					if (num != 1098165981U)
					{
						if (num != 1141294589U)
						{
							goto IL_550;
						}
						if (!(text == "animmove"))
						{
							goto IL_550;
						}
						goto IL_2D6;
					}
					else
					{
						if (!(text == "pathlines"))
						{
							goto IL_550;
						}
						GameManager.Instance.DebugAILines = !GameManager.Instance.DebugAILines;
						return;
					}
				}
				else
				{
					if (!(text == "ragdoll"))
					{
						goto IL_550;
					}
					float d = 1f;
					float stunTime = 1f;
					if (_params.Count >= 2)
					{
						float.TryParse(_params[1], out d);
					}
					if (_params.Count >= 3)
					{
						float.TryParse(_params[2], out stunTime);
					}
					using (List<Entity>.Enumerator enumerator = world.Entities.list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Entity entity = enumerator.Current;
							EntityAlive entityAlive = entity as EntityAlive;
							if (entityAlive && !(entityAlive is EntityPlayer) && !(entityAlive is EntityTrader))
							{
								entityAlive.emodel.DoRagdoll(stunTime, EnumBodyPartHit.None, -entityAlive.GetForwardVector() * d, Vector3.zero, false);
							}
						}
						return;
					}
				}
			}
			else if (num != 1311049565U)
			{
				if (num != 1428345803U)
				{
					if (num != 2125639436U)
					{
						goto IL_550;
					}
					if (!(text == "rage"))
					{
						goto IL_550;
					}
				}
				else
				{
					if (!(text == "fp"))
					{
						goto IL_550;
					}
					goto IL_373;
				}
			}
			else
			{
				if (!(text == "ac"))
				{
					goto IL_550;
				}
				goto IL_21D;
			}
			float num2 = 1f;
			float time = 5f;
			if (_params.Count >= 2)
			{
				float.TryParse(_params[1], out num2);
			}
			if (_params.Count >= 3)
			{
				float.TryParse(_params[2], out time);
			}
			using (List<Entity>.Enumerator enumerator = world.Entities.list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Entity entity2 = enumerator.Current;
					EntityHuman entityHuman = entity2 as EntityHuman;
					if (entityHuman)
					{
						if (num2 <= 0f)
						{
							entityHuman.StopRage();
						}
						else
						{
							entityHuman.StartRage(num2, time);
						}
					}
				}
				return;
			}
			goto IL_537;
		}
		if (num <= 2689840001U)
		{
			if (num != 2540159512U)
			{
				if (num != 2670967086U)
				{
					if (num != 2689840001U)
					{
						goto IL_550;
					}
					if (!(text == "activityclear"))
					{
						goto IL_550;
					}
				}
				else
				{
					if (!(text == "pathgrid"))
					{
						goto IL_550;
					}
					if (AstarManager.Instance)
					{
						AstarManager.Instance.OriginChanged();
						return;
					}
					return;
				}
			}
			else
			{
				if (!(text == "freezepos"))
				{
					goto IL_550;
				}
				goto IL_373;
			}
		}
		else if (num <= 3319670096U)
		{
			if (num != 2708253965U)
			{
				if (num != 3319670096U)
				{
					goto IL_550;
				}
				if (!(text == "anim"))
				{
					goto IL_550;
				}
				if (_params.Count >= 2)
				{
					using (List<Entity>.Enumerator enumerator = world.Entities.list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Entity entity3 = enumerator.Current;
							EntityAlive entityAlive2 = entity3 as EntityAlive;
							if (entityAlive2 && entityAlive2.entityType != EntityType.Player)
							{
								string a = _params[1].ToLower();
								if (!(a == "attack"))
								{
									if (a == "attack2")
									{
										entityAlive2.StartAnimAction(3000);
									}
								}
								else
								{
									entityAlive2.StartAnimAction(0);
								}
							}
						}
						return;
					}
					goto IL_2D6;
				}
				return;
			}
			else
			{
				if (!(text == "sendnames"))
				{
					goto IL_550;
				}
				goto IL_537;
			}
		}
		else
		{
			if (num != 3909890315U)
			{
				if (num != 4076569433U)
				{
					goto IL_550;
				}
				if (!(text == "latency"))
				{
					goto IL_550;
				}
			}
			else if (!(text == "l"))
			{
				goto IL_550;
			}
			int num3 = world.GetPrimaryPlayerId();
			if (_senderInfo.RemoteClientInfo != null)
			{
				num3 = _senderInfo.RemoteClientInfo.entityId;
			}
			if (num3 != -1)
			{
				AIDirector.DebugToggleSendLatency(num3);
				return;
			}
			return;
		}
		IL_21D:
		GameManager.Instance.World.aiDirector.GetComponent<AIDirectorChunkEventComponent>().Clear();
		return;
		IL_2D6:
		float speedForward = 0f;
		float speedStrafe = 0f;
		if (_params.Count >= 2)
		{
			float.TryParse(_params[1], out speedForward);
		}
		if (_params.Count >= 3)
		{
			float.TryParse(_params[2], out speedStrafe);
		}
		using (List<Entity>.Enumerator enumerator = world.Entities.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Entity entity4 = enumerator.Current;
				EntityAlive entityAlive3 = entity4 as EntityAlive;
				if (entityAlive3 && entityAlive3.entityType != EntityType.Player)
				{
					entityAlive3.speedForward = speedForward;
					entityAlive3.speedStrafe = speedStrafe;
				}
			}
			return;
		}
		IL_373:
		AIDirector.DebugToggleFreezePos();
		return;
		IL_537:
		if (_senderInfo.RemoteClientInfo != null)
		{
			AIDirector.DebugToggleSendNameInfo(_senderInfo.RemoteClientInfo.entityId);
			return;
		}
		return;
		IL_550:
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Unknown command " + text + ".");
	}
}
