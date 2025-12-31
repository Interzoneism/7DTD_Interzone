using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E4F RID: 3663
[Preserve]
public class XUiC_SpawnMenu : XUiController
{
	// Token: 0x060072FD RID: 29437 RVA: 0x002EE1B4 File Offset: 0x002EC3B4
	public override void Init()
	{
		base.Init();
		XUiC_SpawnMenu.ID = base.WindowGroup.ID;
		this.toggleLookAtYou = base.GetChildById("toggleLookAtYou").GetChildByType<XUiC_ToggleButton>();
		this.toggleLookAtYou.OnValueChanged += this.ToggleLookAtYou_OnValueChanged;
		this.toggleSpawn25 = base.GetChildById("toggleSpawn25").GetChildByType<XUiC_ToggleButton>();
		this.toggleSpawn25.OnValueChanged += this.ToggleSpawn25_OnValueChanged;
		this.toggleFromDynamic = base.GetChildById("toggleFromDynamic").GetChildByType<XUiC_ToggleButton>();
		this.toggleFromDynamic.OnValueChanged += this.ToggleFromDynamic_OnValueChanged;
		this.toggleFromStatic = base.GetChildById("toggleFromStatic").GetChildByType<XUiC_ToggleButton>();
		this.toggleFromStatic.OnValueChanged += this.ToggleFromStatic_OnValueChanged;
		this.toggleFromBiome = base.GetChildById("toggleFromBiome").GetChildByType<XUiC_ToggleButton>();
		this.toggleFromBiome.OnValueChanged += this.ToggleFromBiome_OnValueChanged;
		this.spawnFiltered = base.GetChildById("spawnFiltered").GetChildByType<XUiC_SimpleButton>();
		this.spawnFiltered.OnPressed += this.SpawnFiltered_OnPressed;
		this.entitiesList = (XUiC_SpawnEntitiesList)base.GetChildById("entities");
		this.entitiesList.SelectionChanged += this.EntitiesList_SelectionChanged;
		this.toggleFromDynamic.Value = true;
	}

	// Token: 0x060072FE RID: 29438 RVA: 0x002EE31E File Offset: 0x002EC51E
	public override void OnOpen()
	{
		base.OnOpen();
		XUiC_FocusedBlockHealth.SetData(base.xui.playerUI, null, 0f);
	}

	// Token: 0x060072FF RID: 29439 RVA: 0x002EE33C File Offset: 0x002EC53C
	[PublicizedFrom(EAccessModifier.Private)]
	public void EntitiesList_SelectionChanged(XUiC_ListEntry<XUiC_SpawnEntitiesList.SpawnEntityEntry> _previousEntry, XUiC_ListEntry<XUiC_SpawnEntitiesList.SpawnEntityEntry> _newEntry)
	{
		if (_newEntry != null)
		{
			this.entitiesList.ClearSelection();
			if (_newEntry.GetEntry() != null)
			{
				XUiC_SpawnEntitiesList.SpawnEntityEntry entry = _newEntry.GetEntry();
				this.BtnSpawns_OnPress(entry.key);
			}
		}
	}

	// Token: 0x06007300 RID: 29440 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleLookAtYou_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
	}

	// Token: 0x06007301 RID: 29441 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleSpawn25_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
	}

	// Token: 0x06007302 RID: 29442 RVA: 0x002EE374 File Offset: 0x002EC574
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnFiltered_OnPressed(XUiController _sender, int _mouseButton)
	{
		int num = 0;
		Vector3 hitPoint = this.GetHitPoint();
		float y = GameManager.Instance.World.GetPrimaryPlayer().finalCamera.transform.eulerAngles.y;
		int entryCount = this.entitiesList.EntryCount;
		float num2 = 0f;
		float num3 = 45f;
		float num4 = (float)((entryCount <= 1) ? 0 : 2);
		for (int i = 0; i < entryCount; i++)
		{
			XUiC_SpawnEntitiesList.SpawnEntityEntry entry = this.entitiesList.GetEntry(i);
			Vector3 spawnPos = hitPoint;
			float f = (num2 + y) * 0.017453292f;
			spawnPos.x += Mathf.Sin(f) * num4;
			spawnPos.z += Mathf.Cos(f) * num4;
			num += this.Spawn(entry.key, spawnPos);
			if (num > 200)
			{
				break;
			}
			num2 += num3;
			if (num2 > 359f)
			{
				num2 = 0f;
				num3 /= 2f;
				num4 += 2f;
			}
		}
		Log.Out("Spawned {0}", new object[]
		{
			num
		});
	}

	// Token: 0x06007303 RID: 29443 RVA: 0x002EE48F File Offset: 0x002EC68F
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleFromDynamic_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		if (_newValue)
		{
			this.toggleFromStatic.Value = false;
			this.toggleFromBiome.Value = false;
			return;
		}
		this.toggleFromDynamic.Value = true;
	}

	// Token: 0x06007304 RID: 29444 RVA: 0x002EE4B9 File Offset: 0x002EC6B9
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleFromStatic_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		if (_newValue)
		{
			this.toggleFromDynamic.Value = false;
			this.toggleFromBiome.Value = false;
			return;
		}
		this.toggleFromStatic.Value = true;
	}

	// Token: 0x06007305 RID: 29445 RVA: 0x002EE4E3 File Offset: 0x002EC6E3
	[PublicizedFrom(EAccessModifier.Private)]
	public void ToggleFromBiome_OnValueChanged(XUiC_ToggleButton _sender, bool _newValue)
	{
		if (_newValue)
		{
			this.toggleFromDynamic.Value = false;
			this.toggleFromStatic.Value = false;
			return;
		}
		this.toggleFromBiome.Value = true;
	}

	// Token: 0x06007306 RID: 29446 RVA: 0x002EE510 File Offset: 0x002EC710
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnSpawns_OnPress(int _key)
	{
		Vector3 hitPoint = this.GetHitPoint();
		this.Spawn(_key, hitPoint);
	}

	// Token: 0x06007307 RID: 29447 RVA: 0x002EE530 File Offset: 0x002EC730
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 GetHitPoint()
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		float offsetUp = (float)(primaryPlayer.AttachedToEntity ? 2 : 0);
		Vector3 result = XUiC_LevelTools3Window.getRaycastHitPoint(100f, offsetUp);
		if (result.Equals(Vector3.zero))
		{
			Ray ray = primaryPlayer.finalCamera.ScreenPointToRay(new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f, 0f));
			result = ray.origin + ray.direction * 10f + Origin.position;
		}
		result.y += 0.25f;
		return result;
	}

	// Token: 0x06007308 RID: 29448 RVA: 0x002EE5E4 File Offset: 0x002EC7E4
	[PublicizedFrom(EAccessModifier.Private)]
	public int Spawn(int _key, Vector3 _spawnPos)
	{
		Camera finalCamera = GameManager.Instance.World.GetPrimaryPlayer().finalCamera;
		Vector3 vector = new Vector3(0f, this.toggleLookAtYou.Value ? (finalCamera.transform.eulerAngles.y + 180f) : finalCamera.transform.eulerAngles.y, 0f);
		EnumSpawnerSource enumSpawnerSource = EnumSpawnerSource.Unknown;
		if (this.toggleFromDynamic.Value)
		{
			enumSpawnerSource = EnumSpawnerSource.Dynamic;
		}
		if (this.toggleFromStatic.Value)
		{
			enumSpawnerSource = EnumSpawnerSource.StaticSpawner;
		}
		if (this.toggleFromBiome.Value)
		{
			enumSpawnerSource = EnumSpawnerSource.Biome;
		}
		int num = this.toggleSpawn25.Value ? 25 : 1;
		if (InputUtils.ShiftKeyPressed)
		{
			num = 5;
		}
		Vector3 vector2 = finalCamera.transform.right;
		if (!InputUtils.AltKeyPressed)
		{
			vector2 *= 0.01f;
		}
		if (EntityClass.list[_key].entityClassName == "entityJunkDrone")
		{
			if (!EntityDrone.IsValidForLocalPlayer())
			{
				return 0;
			}
			GameManager.Instance.World.EntityLoadedDelegates += EntityDrone.OnClientSpawnRemote;
			num = 1;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			_spawnPos -= vector2 * ((float)(num - 1) * 0.5f);
			for (int i = 0; i < num; i++)
			{
				Entity entity = EntityFactory.CreateEntity(_key, _spawnPos, vector);
				entity.SetSpawnerSource(enumSpawnerSource);
				GameManager.Instance.World.SpawnEntityInWorld(entity);
				_spawnPos += vector2;
			}
		}
		else
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageConsoleCmdServer>().Setup(string.Concat(new string[]
			{
				"spawnentityat \"",
				EntityClass.list[_key].entityClassName,
				"\" ",
				_spawnPos.x.ToCultureInvariantString(),
				" ",
				_spawnPos.y.ToCultureInvariantString(),
				" ",
				_spawnPos.z.ToCultureInvariantString(),
				" ",
				num.ToString(),
				" ",
				vector.x.ToCultureInvariantString(),
				" ",
				vector.y.ToCultureInvariantString(),
				" ",
				vector.z.ToCultureInvariantString(),
				" ",
				vector2.x.ToCultureInvariantString(),
				" ",
				vector2.y.ToCultureInvariantString(),
				" ",
				vector2.z.ToCultureInvariantString(),
				" ",
				enumSpawnerSource.ToStringCached<EnumSpawnerSource>()
			})), false);
		}
		return num;
	}

	// Token: 0x04005792 RID: 22418
	public static string ID = "";

	// Token: 0x04005793 RID: 22419
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SpawnEntitiesList entitiesList;

	// Token: 0x04005794 RID: 22420
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleLookAtYou;

	// Token: 0x04005795 RID: 22421
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleSpawn25;

	// Token: 0x04005796 RID: 22422
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleFromDynamic;

	// Token: 0x04005797 RID: 22423
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleFromStatic;

	// Token: 0x04005798 RID: 22424
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ToggleButton toggleFromBiome;

	// Token: 0x04005799 RID: 22425
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton spawnFiltered;
}
