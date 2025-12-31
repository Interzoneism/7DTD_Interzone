using System;
using Platform;
using UnityEngine;

// Token: 0x02000FCC RID: 4044
public class GUIHUDEntityName : MonoBehaviour
{
	// Token: 0x060080DB RID: 32987 RVA: 0x0034533C File Offset: 0x0034353C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		if (GameManager.IsDedicatedServer)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		this.entity = base.GetComponent<EntityAlive>();
		this.entityPlayer = (this.entity as EntityPlayer);
		if (GUIHUDEntityName.gameManager == null)
		{
			GUIHUDEntityName.gameManager = (GameManager)UnityEngine.Object.FindObjectOfType(typeof(GameManager));
		}
		if (NGuiHUDRoot.go == null)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		GameObject gameObject = Resources.Load("Prefabs/prefabPlayerHUDText", typeof(GameObject)) as GameObject;
		if (gameObject != null)
		{
			GameObject gameObject2 = NGuiHUDRoot.go.AddChild(gameObject);
			gameObject2.name = ((this.entityPlayer != null) ? this.entityPlayer.PlayerDisplayName : this.entity.EntityName);
			this.hudText = gameObject2.GetComponentInChildren<NGuiHUDText>();
			this.hudTextObj = this.hudText.gameObject;
			if (this.hudText.ambigiousFont == null)
			{
				Log.Error("GUIHUDEntityName font null");
			}
			this.followTarget = gameObject2.AddComponent<NGuiUIFollowTarget>();
			this.followTarget.offset = new Vector3(0f, 0.6f, 0f);
			this.followTarget.target = null;
			this.hudText.Add(string.Empty, Color.white, float.MaxValue);
			this.hudText.Add(string.Empty, Color.white, float.MaxValue);
			this.hudText.Add(string.Empty, Color.white, float.MaxValue);
			this.hudTextObj.SetActive(false);
			this.updatePhysicsVisibilityCounter = 9999;
		}
	}

	// Token: 0x060080DC RID: 32988 RVA: 0x003454DC File Offset: 0x003436DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnDestroy()
	{
		if (this.hudText != null)
		{
			UnityEngine.Object.Destroy(this.hudTextObj);
			this.hudText = null;
			this.hudTextObj = null;
		}
	}

	// Token: 0x060080DD RID: 32989 RVA: 0x00345508 File Offset: 0x00343708
	[PublicizedFrom(EAccessModifier.Private)]
	public void findRenderers()
	{
		if (this.entity.emodel)
		{
			Transform modelTransform = this.entity.emodel.GetModelTransform();
			if (modelTransform)
			{
				this.renderers = modelTransform.GetComponentsInChildren<Renderer>();
			}
		}
	}

	// Token: 0x060080DE RID: 32990 RVA: 0x0034554C File Offset: 0x0034374C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnGUI()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (GUIHUDEntityName.mainCamera == null)
		{
			GUIHUDEntityName.mainCamera = Camera.main;
			if (GUIHUDEntityName.mainCamera == null)
			{
				return;
			}
		}
		Vector3 direction = this.entity.getHeadPosition() - Origin.position - GUIHUDEntityName.mainCamera.transform.position;
		float magnitude = direction.magnitude;
		bool flag = this.entityPlayer != null;
		if (magnitude > 8f && flag)
		{
			this.setActiveIfDifferent(false);
			return;
		}
		int num;
		if (this.renderers == null || this.renderers.Length == 0)
		{
			num = this.updatePhysicsVisibilityCounter + 1;
			this.updatePhysicsVisibilityCounter = num;
			if (num > 100)
			{
				this.updatePhysicsVisibilityCounter = 0;
				this.findRenderers();
			}
			return;
		}
		bool flag2 = false;
		for (int i = 0; i < this.renderers.Length; i++)
		{
			Renderer renderer = this.renderers[i];
			if (!renderer)
			{
				this.renderers = null;
				return;
			}
			if (renderer.isVisible)
			{
				flag2 = true;
				break;
			}
		}
		if (!flag2)
		{
			this.setActiveIfDifferent(false);
			return;
		}
		if (this.followTarget.target == null)
		{
			this.followTarget.target = this.entity.ModelTransform;
			this.followTarget.offset = new Vector3(0f, this.entity.GetEyeHeight() + 0.6f, 0f);
		}
		num = this.updatePhysicsVisibilityCounter + 1;
		this.updatePhysicsVisibilityCounter = num;
		if (num > 5)
		{
			this.updatePhysicsVisibilityCounter = 0;
			EntityPlayerLocal primaryPlayer = GUIHUDEntityName.gameManager.World.GetPrimaryPlayer();
			if (primaryPlayer == null || !primaryPlayer.Spawned)
			{
				this.bShowHUDText = false;
				this.setActiveIfDifferent(false);
				return;
			}
			if (!primaryPlayer.PlayerUI.windowManager.IsHUDEnabled())
			{
				this.bShowHUDText = false;
				this.setActiveIfDifferent(false);
				return;
			}
			RaycastHit raycastHit;
			this.bShowHUDText = Physics.Raycast(new Ray(GUIHUDEntityName.mainCamera.transform.position + direction.normalized * 0.15f, direction), out raycastHit, 9.6f, -538480645);
			this.bShowHUDText = (this.bShowHUDText && raycastHit.distance < 8f);
			Transform transform = raycastHit.transform;
			if (this.bShowHUDText && transform.tag.StartsWith("E_BP_"))
			{
				transform = GameUtils.GetHitRootTransform(transform.tag, transform);
			}
			this.bShowHUDText &= (transform == this.entity.transform);
			if (!flag)
			{
				this.bShowHUDText = true;
			}
			if (!this.bShowHUDText && this.bLastShowHUDText && this.hideCountdownTime <= 0f)
			{
				this.hideCountdownTime = 0.4f;
			}
			int num2 = 45;
			string text = (this.entityPlayer != null) ? this.entityPlayer.PlayerDisplayName : this.entity.EntityName;
			if (this.entity.DebugNameInfo.Length > 0 && !this.entity.IsDead())
			{
				text += this.entity.DebugNameInfo;
				num2 = (int)(150f / Utils.FastClamp(magnitude, 3.333f, 8f));
			}
			string text2 = string.Empty;
			PersistentPlayerData persistentPlayerData;
			if (GameManager.Instance.persistentPlayers.EntityToPlayerMap.TryGetValue(this.entity.entityId, out persistentPlayerData))
			{
				GameServerInfo gameServerInfo = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? SingletonMonoBehaviour<ConnectionManager>.Instance.LocalServerInfo : SingletonMonoBehaviour<ConnectionManager>.Instance.LastGameServerInfo;
				if (gameServerInfo != null && gameServerInfo.AllowsCrossplay)
				{
					EPlayGroup playGroup = GameManager.Instance.persistentPlayers.Players[persistentPlayerData.PrimaryId].PlayGroup;
					text2 = PlatformManager.NativePlatform.Utils.GetCrossplayPlayerIcon(playGroup, false, persistentPlayerData.PlatformData.NativeId.PlatformIdentifier);
				}
			}
			UIAtlas atlasByName = primaryPlayer.PlayerUI.xui.GetAtlasByName("SymbolAtlas", text2);
			this.hudText.SetEntry(0, text2, true, atlasByName);
			this.hudText.SetEntrySize(0, num2 - 5);
			this.hudText.SetEntryOffset(0, new Vector3(-0.1f, 0f, 0f));
			this.hudText.SetEntry(1, text, false, null);
			this.hudText.SetEntrySize(1, num2);
			IPartyVoice.EVoiceMemberState playerVoiceState = VoiceHelpers.GetPlayerVoiceState(this.entityPlayer, false);
			if (playerVoiceState != IPartyVoice.EVoiceMemberState.Disabled)
			{
				UIAtlas atlasByName2 = primaryPlayer.PlayerUI.xui.GetAtlasByName("UIAtlas", "ui_game_symbol_talk");
				this.hudText.SetEntry(2, "ui_game_symbol_talk", true, atlasByName2);
				this.hudText.SetEntrySize(2, num2 - 5);
				this.hudText.SetEntryOffset(2, new Vector3(0.1f, 0f, 0f));
				this.hudText.SetEntryColor(2, (playerVoiceState == IPartyVoice.EVoiceMemberState.Muted) ? Color.red : ((playerVoiceState == IPartyVoice.EVoiceMemberState.VoiceActive) ? Color.white : Color.grey));
			}
			else
			{
				this.hudText.SetEntry(2, string.Empty, true, null);
				this.hudText.SetEntrySize(2, num2 - 5);
				this.hudText.SetEntryOffset(2, new Vector3(0.1f, 0f, 0f));
			}
		}
		if (this.hideCountdownTime > 0f)
		{
			this.hideCountdownTime -= Time.deltaTime;
		}
		if (this.hideCountdownTime <= 0f)
		{
			this.setActiveIfDifferent(this.bShowHUDText);
			this.bLastShowHUDText = this.bShowHUDText;
			return;
		}
		if (this.bShowHUDText)
		{
			this.setActiveIfDifferent(this.bShowHUDText);
		}
	}

	// Token: 0x060080DF RID: 32991 RVA: 0x00345ADD File Offset: 0x00343CDD
	[PublicizedFrom(EAccessModifier.Private)]
	public void setActiveIfDifferent(bool _active)
	{
		if (this.hudTextObj.activeSelf != _active)
		{
			this.hudTextObj.SetActive(_active);
		}
	}

	// Token: 0x04006386 RID: 25478
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cRaycastFrameDelay = 5;

	// Token: 0x04006387 RID: 25479
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cVisibleDistance = 8;

	// Token: 0x04006388 RID: 25480
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cHeadOffset = 0.6f;

	// Token: 0x04006389 RID: 25481
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive entity;

	// Token: 0x0400638A RID: 25482
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayer entityPlayer;

	// Token: 0x0400638B RID: 25483
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Renderer[] renderers;

	// Token: 0x0400638C RID: 25484
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int updatePhysicsVisibilityCounter;

	// Token: 0x0400638D RID: 25485
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bShowHUDText;

	// Token: 0x0400638E RID: 25486
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bLastShowHUDText;

	// Token: 0x0400638F RID: 25487
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float hideCountdownTime;

	// Token: 0x04006390 RID: 25488
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameManager gameManager;

	// Token: 0x04006391 RID: 25489
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static Camera mainCamera;

	// Token: 0x04006392 RID: 25490
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NGuiHUDText hudText;

	// Token: 0x04006393 RID: 25491
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject hudTextObj;

	// Token: 0x04006394 RID: 25492
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public NGuiUIFollowTarget followTarget;
}
