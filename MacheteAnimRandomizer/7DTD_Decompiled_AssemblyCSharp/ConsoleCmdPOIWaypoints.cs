using System;
using System.Collections.Generic;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000224 RID: 548
[Preserve]
public class ConsoleCmdPOIWaypoints : ConsoleCmdAbstract
{
	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06001002 RID: 4098 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06001003 RID: 4099 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypes
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x1700018B RID: 395
	// (get) Token: 0x06001004 RID: 4100 RVA: 0x00058577 File Offset: 0x00056777
	public override DeviceFlag AllowedDeviceTypesClient
	{
		get
		{
			return DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux | DeviceFlag.StandaloneOSX | DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5;
		}
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x000675D2 File Offset: 0x000657D2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"poiwaypoints",
			"pwp"
		};
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x000675EA File Offset: 0x000657EA
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Adds waypoints for specified POIs.";
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x000675F1 File Offset: 0x000657F1
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return this.getDescription() + "\n\npwp * - adds waypoints to all POIs in the world.\npwp <name> - adds waypoints to all POIs that starts with the name.\npwp <distance> - adds waypoints to all POIs with the specified distance.\npwp * <distance> - adds waypoints to all POIs within the specified distance.\npwp <name> <distance> - adds waypoints to all POIs within the specified distance that start with the name.\npwp -clear - removes all POI waypoints.";
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x00067604 File Offset: 0x00065804
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.GetHelp());
			return;
		}
		if (_params.Count != 1)
		{
			if (_params.Count == 2)
			{
				float distance;
				if (float.TryParse(_params[1], out distance))
				{
					if (_params[0] == "*")
					{
						this.CreateWaypoints("", distance);
						return;
					}
					this.CreateWaypoints(_params[0], distance);
					return;
				}
				else
				{
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("\"" + _params[1] + "\" is not a valid distance.");
				}
			}
			return;
		}
		if (_params[0] == "-clear")
		{
			NavObjectManager instance = NavObjectManager.Instance;
			EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			for (int i = primaryPlayer.Waypoints.Collection.list.Count - 1; i >= 0; i--)
			{
				Waypoint waypoint = primaryPlayer.Waypoints.Collection.list[i];
				ConsoleCmdPOIWaypoints.POIWaypoint poiwaypoint = waypoint as ConsoleCmdPOIWaypoints.POIWaypoint;
				if (poiwaypoint != null)
				{
					instance.UnRegisterNavObject(poiwaypoint.navObject);
					primaryPlayer.Waypoints.Collection.Remove(waypoint);
				}
			}
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("POI Waypoints have been cleared.");
			return;
		}
		if (_params[0] == "*")
		{
			this.CreateWaypoints("", 0f);
			return;
		}
		float distance2;
		if (float.TryParse(_params[0], out distance2))
		{
			this.CreateWaypoints("", distance2);
			return;
		}
		this.CreateWaypoints(_params[0], 0f);
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x00067794 File Offset: 0x00065994
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateWaypoints(string filterName, float distance)
	{
		GameManager instance = GameManager.Instance;
		int num = 0;
		if (instance != null && instance.GetDynamicPrefabDecorator() != null)
		{
			NavObjectManager instance2 = NavObjectManager.Instance;
			List<PrefabInstance> dynamicPrefabs = instance.GetDynamicPrefabDecorator().GetDynamicPrefabs();
			if (dynamicPrefabs != null)
			{
				float num2 = distance * distance;
				EntityPlayer primaryPlayer = instance.World.GetPrimaryPlayer();
				foreach (PrefabInstance prefabInstance in dynamicPrefabs)
				{
					if ((distance == 0f || (primaryPlayer.position - prefabInstance.boundingBoxPosition).sqrMagnitude < num2) && prefabInstance.boundingBoxSize.Volume() >= 100 && prefabInstance.name.StartsWith(filterName))
					{
						ConsoleCmdPOIWaypoints.POIWaypoint poiwaypoint = new ConsoleCmdPOIWaypoints.POIWaypoint();
						poiwaypoint.pos = prefabInstance.boundingBoxPosition;
						poiwaypoint.name.Update(prefabInstance.prefab.PrefabName, PlatformManager.MultiPlatform.User.PlatformUserId);
						poiwaypoint.icon = "ui_game_symbol_map_trader";
						poiwaypoint.ownerId = null;
						poiwaypoint.lastKnownPositionEntityId = -1;
						if (!primaryPlayer.Waypoints.ContainsWaypoint(poiwaypoint))
						{
							NavObject navObject = instance2.RegisterNavObject("waypoint", poiwaypoint.pos, poiwaypoint.icon, true, -1, null);
							navObject.UseOverrideColor = true;
							navObject.OverrideColor = Color.cyan;
							navObject.IsActive = true;
							navObject.name = poiwaypoint.name.Text;
							poiwaypoint.navObject = navObject;
							primaryPlayer.Waypoints.Collection.Add(poiwaypoint);
							num++;
						}
					}
				}
			}
		}
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Added {0} POI waypoints.", new object[]
		{
			num
		});
	}

	// Token: 0x04000B54 RID: 2900
	[PublicizedFrom(EAccessModifier.Private)]
	public const int SmallPoiVolumeLimit = 100;

	// Token: 0x02000225 RID: 549
	public class POIWaypoint : Waypoint
	{
		// Token: 0x0600100B RID: 4107 RVA: 0x00067988 File Offset: 0x00065B88
		public POIWaypoint()
		{
			this.IsSaved = false;
		}
	}
}
