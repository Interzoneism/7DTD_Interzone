using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020010D5 RID: 4309
public class GameLightManager
{
	// Token: 0x06008795 RID: 34709 RVA: 0x0036D9E8 File Offset: 0x0036BBE8
	public static GameLightManager Create(EntityPlayerLocal player)
	{
		GameLightManager gameLightManager = new GameLightManager();
		gameLightManager.player = player;
		gameLightManager.Init();
		return gameLightManager;
	}

	// Token: 0x06008796 RID: 34710 RVA: 0x0036D9FC File Offset: 0x0036BBFC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init()
	{
		GameLightManager.Instance = this;
		this.UpdateLightInit();
	}

	// Token: 0x06008797 RID: 34711 RVA: 0x0036DA0A File Offset: 0x0036BC0A
	public void Destroy()
	{
		this.lights.Clear();
		this.priorityLights.Clear();
		this.removeLights.Clear();
		this.UpdateLightCleanup();
		GameLightManager.Instance = null;
	}

	// Token: 0x06008798 RID: 34712 RVA: 0x0036DA3C File Offset: 0x0036BC3C
	public void FrameUpdate()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (GameManager.Instance.World == null)
		{
			return;
		}
		if (MeshDescription.bDebugStability || LightViewer.IsAllOff)
		{
			return;
		}
		this.isUpdating = true;
		Vector3 position = this.player.cameraTransform.position;
		int count = this.lights.Count;
		int num = (count + 19) / 20;
		if (this.lightUpdateIndex >= count)
		{
			this.lightUpdateIndex = 0;
		}
		for (int i = 0; i < num; i++)
		{
			LightLOD lightLOD = this.lights[this.lightUpdateIndex];
			if (lightLOD.priority <= 0f)
			{
				lightLOD.FrameUpdate(position);
				if (lightLOD.priority > 0f)
				{
					this.priorityLights.Add(lightLOD);
				}
			}
			int num2 = this.lightUpdateIndex + 1;
			this.lightUpdateIndex = num2;
			if (num2 >= count)
			{
				this.lightUpdateIndex = 0;
			}
		}
		int j = 0;
		while (j < this.priorityLights.Count)
		{
			LightLOD lightLOD2 = this.priorityLights[j];
			lightLOD2.FrameUpdate(position);
			if (lightLOD2.priority <= 0f)
			{
				this.priorityLights.RemoveAt(j);
			}
			else
			{
				j++;
			}
		}
		int count2 = this.removeLights.Count;
		if (count2 > 0)
		{
			for (int k = count2 - 1; k >= 0; k--)
			{
				LightLOD lightLOD3 = this.removeLights[k];
				this.RemoveLightFromLists(lightLOD3);
			}
			this.removeLights.Clear();
		}
		if (this.isWaterLevelChanged)
		{
			this.isWaterLevelChanged = false;
			foreach (LightLOD lightLOD4 in this.lights)
			{
				if (!lightLOD4.bWorksUnderwater)
				{
					lightLOD4.WaterLevelDirty = true;
				}
			}
		}
		this.isUpdating = false;
		this.UpdateLightFrameUpdate();
	}

	// Token: 0x06008799 RID: 34713 RVA: 0x0036DC1C File Offset: 0x0036BE1C
	public void AddLight(LightLOD lightLOD)
	{
		this.lights.Add(lightLOD);
		lightLOD.priority = 1f;
		this.priorityLights.Add(lightLOD);
		if (OcclusionManager.Instance.cullLights)
		{
			OcclusionManager.AddLight(lightLOD);
		}
	}

	// Token: 0x0600879A RID: 34714 RVA: 0x0036DC53 File Offset: 0x0036BE53
	public void RemoveLight(LightLOD lightLOD)
	{
		if (this.isUpdating)
		{
			this.removeLights.Add(lightLOD);
		}
		else
		{
			this.RemoveLightFromLists(lightLOD);
		}
		if (OcclusionManager.Instance.cullLights)
		{
			OcclusionManager.RemoveLight(lightLOD);
		}
	}

	// Token: 0x0600879B RID: 34715 RVA: 0x0036DC84 File Offset: 0x0036BE84
	[PublicizedFrom(EAccessModifier.Private)]
	public void RemoveLightFromLists(LightLOD lightLOD)
	{
		int num = this.lights.IndexOf(lightLOD);
		if (num < 0)
		{
			Log.Warning("RemoveLightFromLists none");
			return;
		}
		this.lights.RemoveAt(num);
		if (num < this.lightUpdateIndex)
		{
			this.lightUpdateIndex--;
		}
		this.priorityLights.Remove(lightLOD);
	}

	// Token: 0x0600879C RID: 34716 RVA: 0x0036DCDD File Offset: 0x0036BEDD
	public void MakeLightAPriority(LightLOD lightLOD)
	{
		if (lightLOD.priority <= 0f)
		{
			lightLOD.priority = 1f;
			this.priorityLights.Add(lightLOD);
		}
	}

	// Token: 0x0600879D RID: 34717 RVA: 0x0036DD03 File Offset: 0x0036BF03
	public Vector3 CameraPos()
	{
		return this.player.cameraTransform.position;
	}

	// Token: 0x0600879E RID: 34718 RVA: 0x0036DD15 File Offset: 0x0036BF15
	public void HandleWaterLevelChanged()
	{
		this.isWaterLevelChanged = true;
	}

	// Token: 0x0600879F RID: 34719 RVA: 0x0036DD20 File Offset: 0x0036BF20
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLightInit()
	{
		for (int i = 0; i < this.fastULs.Length; i++)
		{
			this.fastULs[i] = new List<UpdateLight>(64);
		}
		for (int j = 0; j < this.slowULs.Length; j++)
		{
			this.slowULs[j] = new List<UpdateLight>(256);
		}
	}

	// Token: 0x060087A0 RID: 34720 RVA: 0x0036DD74 File Offset: 0x0036BF74
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLightCleanup()
	{
		this.newULs.Clear();
		for (int i = 0; i < this.fastULs.Length; i++)
		{
			this.fastULs[i] = null;
		}
		for (int j = 0; j < this.slowULs.Length; j++)
		{
			this.slowULs[j] = null;
		}
	}

	// Token: 0x060087A1 RID: 34721 RVA: 0x0036DDC4 File Offset: 0x0036BFC4
	public void AddUpdateLight(UpdateLight _ul)
	{
		this.newULs.Add(_ul);
	}

	// Token: 0x060087A2 RID: 34722 RVA: 0x0036DDD4 File Offset: 0x0036BFD4
	public void RemoveUpdateLight(UpdateLight _ul)
	{
		bool flag;
		if (_ul.IsDynamicObject)
		{
			int num = _ul.GetHashCode() >> 2 & 3;
			flag = this.fastULs[num].Remove(_ul);
		}
		else
		{
			int num2 = _ul.GetHashCode() >> 2 & 63;
			flag = this.slowULs[num2].Remove(_ul);
		}
		if (!flag && !this.newULs.Remove(_ul))
		{
			Log.Warning("RemoveUpdateLight {0} dy{1} missing!", new object[]
			{
				_ul.transform.name,
				_ul.IsDynamicObject
			});
		}
	}

	// Token: 0x060087A3 RID: 34723 RVA: 0x0036DE60 File Offset: 0x0036C060
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLightFrameUpdate()
	{
		if (GameManager.Instance == null || GameManager.Instance.World == null || !GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		float step = Time.deltaTime * 4f;
		this.fastULUpdateIndex = (this.fastULUpdateIndex + 1 & 3);
		List<UpdateLight> list = this.fastULs[this.fastULUpdateIndex];
		for (int i = list.Count - 1; i >= 0; i--)
		{
			UpdateLight updateLight = list[i];
			if (updateLight)
			{
				updateLight.UpdateLighting(step);
			}
			else
			{
				list.RemoveAt(i);
			}
		}
		this.slowULUpdateIndex = (this.slowULUpdateIndex + 1 & 63);
		List<UpdateLight> list2 = this.slowULs[this.slowULUpdateIndex];
		for (int j = list2.Count - 1; j >= 0; j--)
		{
			UpdateLight updateLight2 = list2[j];
			if (updateLight2)
			{
				if (updateLight2.appliedLit < 0f)
				{
					updateLight2.UpdateLighting(1f);
				}
			}
			else
			{
				list2.RemoveAt(j);
			}
		}
		int num = Utils.FastMin(160, this.newULs.Count);
		for (int k = 0; k < num; k++)
		{
			UpdateLight updateLight3 = this.newULs[k];
			if (updateLight3)
			{
				updateLight3.ManagerFirstUpdate();
				int num2 = updateLight3.GetHashCode() >> 2;
				if (updateLight3.IsDynamicObject)
				{
					this.fastULs[num2 & 3].Add(updateLight3);
				}
				else
				{
					this.slowULs[num2 & 63].Add(updateLight3);
				}
			}
		}
		this.newULs.RemoveRange(0, num);
	}

	// Token: 0x04006956 RID: 26966
	public static GameLightManager Instance;

	// Token: 0x04006957 RID: 26967
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x04006958 RID: 26968
	[PublicizedFrom(EAccessModifier.Private)]
	public List<LightLOD> lights = new List<LightLOD>();

	// Token: 0x04006959 RID: 26969
	[PublicizedFrom(EAccessModifier.Private)]
	public List<LightLOD> priorityLights = new List<LightLOD>();

	// Token: 0x0400695A RID: 26970
	[PublicizedFrom(EAccessModifier.Private)]
	public List<LightLOD> removeLights = new List<LightLOD>();

	// Token: 0x0400695B RID: 26971
	[PublicizedFrom(EAccessModifier.Private)]
	public int lightUpdateIndex;

	// Token: 0x0400695C RID: 26972
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isUpdating;

	// Token: 0x0400695D RID: 26973
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile bool isWaterLevelChanged;

	// Token: 0x0400695E RID: 26974
	[PublicizedFrom(EAccessModifier.Private)]
	public List<UpdateLight> newULs = new List<UpdateLight>(512);

	// Token: 0x0400695F RID: 26975
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFastULGroups = 4;

	// Token: 0x04006960 RID: 26976
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFastULGroupMask = 3;

	// Token: 0x04006961 RID: 26977
	[PublicizedFrom(EAccessModifier.Private)]
	public List<UpdateLight>[] fastULs = new List<UpdateLight>[4];

	// Token: 0x04006962 RID: 26978
	[PublicizedFrom(EAccessModifier.Private)]
	public int fastULUpdateIndex;

	// Token: 0x04006963 RID: 26979
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cULGroups = 64;

	// Token: 0x04006964 RID: 26980
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSlowULGroupMask = 63;

	// Token: 0x04006965 RID: 26981
	[PublicizedFrom(EAccessModifier.Private)]
	public List<UpdateLight>[] slowULs = new List<UpdateLight>[64];

	// Token: 0x04006966 RID: 26982
	[PublicizedFrom(EAccessModifier.Private)]
	public int slowULUpdateIndex;
}
