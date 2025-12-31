using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020006A6 RID: 1702
public class NavObject
{
	// Token: 0x170004CF RID: 1231
	// (get) Token: 0x0600323C RID: 12860 RVA: 0x00155208 File Offset: 0x00153408
	public string DisplayName
	{
		get
		{
			if (this.usingLocalizationId)
			{
				if (string.IsNullOrEmpty(this.localizedName))
				{
					this.localizedName = Localization.Get(this.name, false);
				}
				return this.localizedName;
			}
			return this.name;
		}
	}

	// Token: 0x170004D0 RID: 1232
	// (get) Token: 0x0600323D RID: 12861 RVA: 0x0015523E File Offset: 0x0015343E
	// (set) Token: 0x0600323E RID: 12862 RVA: 0x00155246 File Offset: 0x00153446
	public Transform TrackedTransform
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.trackedTransform;
		}
		set
		{
			this.trackedTransform = value;
			this.TrackType = NavObject.TrackTypes.Transform;
		}
	}

	// Token: 0x170004D1 RID: 1233
	// (get) Token: 0x0600323F RID: 12863 RVA: 0x00155256 File Offset: 0x00153456
	// (set) Token: 0x06003240 RID: 12864 RVA: 0x0015525E File Offset: 0x0015345E
	public Vector3 TrackedPosition
	{
		get
		{
			return this.trackedPosition;
		}
		set
		{
			this.trackedPosition = value;
			this.TrackType = NavObject.TrackTypes.Position;
		}
	}

	// Token: 0x170004D2 RID: 1234
	// (get) Token: 0x06003241 RID: 12865 RVA: 0x0015526E File Offset: 0x0015346E
	// (set) Token: 0x06003242 RID: 12866 RVA: 0x00155276 File Offset: 0x00153476
	public Entity TrackedEntity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return this.trackedEntity;
		}
		set
		{
			this.trackedEntity = value;
			this.EntityID = (this.trackedEntity ? this.trackedEntity.entityId : -1);
			this.TrackType = NavObject.TrackTypes.Entity;
			this.SetupEntityOptions();
		}
	}

	// Token: 0x170004D3 RID: 1235
	// (get) Token: 0x06003243 RID: 12867 RVA: 0x001552AD File Offset: 0x001534AD
	public NavObjectMapSettings CurrentMapSettings
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			NavObjectClass navObjectClass = this.NavObjectClass;
			if (navObjectClass == null)
			{
				return null;
			}
			return navObjectClass.GetMapSettings(this.IsActive);
		}
	}

	// Token: 0x170004D4 RID: 1236
	// (get) Token: 0x06003244 RID: 12868 RVA: 0x001552C6 File Offset: 0x001534C6
	public NavObjectCompassSettings CurrentCompassSettings
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			NavObjectClass navObjectClass = this.NavObjectClass;
			if (navObjectClass == null)
			{
				return null;
			}
			return navObjectClass.GetCompassSettings(this.IsActive);
		}
	}

	// Token: 0x170004D5 RID: 1237
	// (get) Token: 0x06003245 RID: 12869 RVA: 0x001552DF File Offset: 0x001534DF
	public NavObjectScreenSettings CurrentScreenSettings
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			NavObjectClass navObjectClass = this.NavObjectClass;
			if (navObjectClass == null)
			{
				return null;
			}
			return navObjectClass.GetOnScreenSettings(this.IsActive);
		}
	}

	// Token: 0x170004D6 RID: 1238
	// (get) Token: 0x06003246 RID: 12870 RVA: 0x001552F8 File Offset: 0x001534F8
	public bool HasOnScreen
	{
		get
		{
			return this.hasOnScreen;
		}
	}

	// Token: 0x170004D7 RID: 1239
	// (get) Token: 0x06003247 RID: 12871 RVA: 0x00155300 File Offset: 0x00153500
	public Vector3 Rotation
	{
		get
		{
			if (!this.CurrentMapSettings.UseRotation || this.TrackType != NavObject.TrackTypes.Entity)
			{
				return Vector3.zero;
			}
			if (this.trackedEntity.AttachedToEntity != null)
			{
				return this.trackedEntity.AttachedToEntity.rotation;
			}
			return this.trackedEntity.rotation;
		}
	}

	// Token: 0x06003248 RID: 12872 RVA: 0x00155358 File Offset: 0x00153558
	public bool IsTrackedTransform(Transform transform)
	{
		return this.TrackType == NavObject.TrackTypes.Transform && this.trackedTransform == transform;
	}

	// Token: 0x06003249 RID: 12873 RVA: 0x00155371 File Offset: 0x00153571
	public bool IsTrackedPosition(Vector3 position)
	{
		return this.TrackType == NavObject.TrackTypes.Position && this.trackedPosition == position;
	}

	// Token: 0x0600324A RID: 12874 RVA: 0x0015538A File Offset: 0x0015358A
	public bool IsTrackedEntity(Entity entity)
	{
		return this.TrackType == NavObject.TrackTypes.Entity && this.trackedEntity == entity;
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x001553A4 File Offset: 0x001535A4
	public bool IsValidPlayer(EntityPlayerLocal player, NavObjectClass navObjectClass)
	{
		if (this.ForceDisabled)
		{
			return false;
		}
		bool flag = true;
		if (this.TrackType == NavObject.TrackTypes.Entity)
		{
			flag = this.IsValidEntity(player, this.TrackedEntity, navObjectClass);
		}
		switch (navObjectClass.RequirementType)
		{
		case NavObjectClass.RequirementTypes.CVar:
			return player.GetCVar(navObjectClass.RequirementName) > 0f && flag;
		case NavObjectClass.RequirementTypes.QuestBounds:
		case NavObjectClass.RequirementTypes.Tracking:
			return flag;
		case NavObjectClass.RequirementTypes.NoTag:
			return !NavObjectManager.Instance.HasNavObjectTag(navObjectClass.RequirementName) && flag;
		case NavObjectClass.RequirementTypes.IsOwner:
			return this.OwnerEntity == player && flag;
		case NavObjectClass.RequirementTypes.MinimumTreasureRadius:
		{
			float num = this.ExtraData;
			num = EffectManager.GetValue(PassiveEffects.TreasureRadius, null, num, player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			num = Mathf.Clamp(num, 0f, num);
			return num == 0f;
		}
		}
		return flag;
	}

	// Token: 0x0600324C RID: 12876 RVA: 0x00155490 File Offset: 0x00153690
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsValidEntity(EntityPlayerLocal player, Entity entity, NavObjectClass navObjectClass)
	{
		if (entity == null)
		{
			return true;
		}
		if (player == null)
		{
			return true;
		}
		EntityAlive entityAlive = entity as EntityAlive;
		if (entityAlive)
		{
			if (navObjectClass.RequirementType == NavObjectClass.RequirementTypes.None)
			{
				return entityAlive.IsAlive() && !entityAlive.IsSleeperPassive;
			}
			if (!entityAlive.IsAlive() || entityAlive.IsSleeperPassive)
			{
				return false;
			}
			switch (navObjectClass.RequirementType)
			{
			case NavObjectClass.RequirementTypes.CVar:
				return entityAlive.GetCVar(navObjectClass.RequirementName) > 0f;
			case NavObjectClass.RequirementTypes.QuestBounds:
				if (player.QuestJournal.ActiveQuest != null && entityAlive.IsSleeper)
				{
					Vector3 position = entity.position;
					position.y = position.z;
					if (player.ZombieCompassBounds.Contains(position))
					{
						return true;
					}
				}
				return false;
			case NavObjectClass.RequirementTypes.Tracking:
				return EffectManager.GetValue(PassiveEffects.Tracking, null, 0f, player, null, entity.EntityTags, true, true, true, true, true, 1, true, false) > 0f;
			case NavObjectClass.RequirementTypes.InParty:
				return player.Party != null && player.Party.MemberList.Contains(entity as EntityPlayer) && entity != player && !(entity as EntityPlayer).IsSpectator && (player.AttachedToEntity == null || player.AttachedToEntity != entity.AttachedToEntity);
			case NavObjectClass.RequirementTypes.IsAlly:
				return entity as EntityPlayer != null && (entity as EntityPlayer).IsFriendOfLocalPlayer && entity != player && !(entity as EntityPlayer).IsSpectator;
			case NavObjectClass.RequirementTypes.IsPlayer:
				return entity == player;
			case NavObjectClass.RequirementTypes.IsVehicleOwner:
				return (entity as EntityVehicle != null && (entity as EntityVehicle).HasOwnedEntity(player.entityId)) || (entity as EntityTurret != null && (entity as EntityTurret).belongsPlayerId == player.entityId);
			case NavObjectClass.RequirementTypes.NoActiveQuests:
				return entity as EntityNPC == null || player.QuestJournal.FindReadyForTurnInQuestByGiver(entity.entityId) == null;
			case NavObjectClass.RequirementTypes.IsTwitchSpawnedSelf:
				return entity.spawnById == player.entityId && !string.IsNullOrEmpty(entity.spawnByName);
			case NavObjectClass.RequirementTypes.IsTwitchSpawnedOther:
				return entity.spawnById > 0 && entity.spawnById != player.entityId && !string.IsNullOrEmpty(entity.spawnByName);
			}
		}
		else
		{
			NavObjectClass.RequirementTypes requirementType = navObjectClass.RequirementType;
			if (requirementType == NavObjectClass.RequirementTypes.IsTwitchSpawnedSelf)
			{
				return entity.spawnById == player.entityId;
			}
			if (requirementType == NavObjectClass.RequirementTypes.IsTwitchSpawnedOther)
			{
				return entity.spawnById > 0 && entity.spawnById != player.entityId;
			}
		}
		return true;
	}

	// Token: 0x0600324D RID: 12877 RVA: 0x0015573F File Offset: 0x0015393F
	public void AddNavObjectClass(NavObjectClass navClass)
	{
		if (!this.NavObjectClassList.Contains(navClass))
		{
			this.NavObjectClassList.Insert(0, navClass);
		}
	}

	// Token: 0x0600324E RID: 12878 RVA: 0x0015575C File Offset: 0x0015395C
	public bool RemoveNavObjectClass(NavObjectClass navClass)
	{
		this.NavObjectClassList.Remove(navClass);
		if (this.NavObjectClassList.Count == 0)
		{
			NavObjectManager.Instance.UnRegisterNavObject(this);
			return true;
		}
		return false;
	}

	// Token: 0x0600324F RID: 12879 RVA: 0x00155788 File Offset: 0x00153988
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupEntityOptions()
	{
		if (this.TrackType == NavObject.TrackTypes.Entity && this.NavObjectClass != null && this.NavObjectClass.RequirementType == NavObjectClass.RequirementTypes.Tracking)
		{
			this.OverrideSpriteName = ((this.TrackedEntity.GetTrackerIcon() == null) ? "" : this.TrackedEntity.GetTrackerIcon());
			return;
		}
		this.OverrideSpriteName = "";
	}

	// Token: 0x06003250 RID: 12880 RVA: 0x001557E8 File Offset: 0x001539E8
	public bool IsValid()
	{
		if (this.TrackType == NavObject.TrackTypes.Transform && this.TrackedTransform == null)
		{
			this.TrackType = NavObject.TrackTypes.None;
		}
		else if (this.TrackType == NavObject.TrackTypes.Entity && this.TrackedEntity == null)
		{
			this.TrackType = NavObject.TrackTypes.None;
		}
		return this.TrackType > NavObject.TrackTypes.None;
	}

	// Token: 0x06003251 RID: 12881 RVA: 0x0015583C File Offset: 0x00153A3C
	public Vector3 GetPosition()
	{
		switch (this.TrackType)
		{
		case NavObject.TrackTypes.Transform:
			return this.trackedTransform.position;
		case NavObject.TrackTypes.Position:
			return this.trackedPosition - Origin.position;
		case NavObject.TrackTypes.Entity:
			return this.trackedEntity.position - Origin.position;
		default:
			return NavObject.InvalidPos;
		}
	}

	// Token: 0x06003252 RID: 12882 RVA: 0x001558A0 File Offset: 0x00153AA0
	public float GetMaxDistance(NavObjectSettings settings, EntityPlayer player)
	{
		if (this.TrackType == NavObject.TrackTypes.Entity && this.NavObjectClass.RequirementType == NavObjectClass.RequirementTypes.Tracking && settings.MaxDistance == -1f)
		{
			return EffectManager.GetValue(PassiveEffects.TrackDistance, null, 0f, player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		return settings.MaxDistance;
	}

	// Token: 0x06003253 RID: 12883 RVA: 0x001558FB File Offset: 0x00153AFB
	public string GetSpriteName(NavObjectSettings settings)
	{
		if (!this.NavObjectClass.UseOverrideIcon)
		{
			return settings.SpriteName;
		}
		return this.OverrideSpriteName;
	}

	// Token: 0x06003254 RID: 12884 RVA: 0x00155918 File Offset: 0x00153B18
	public NavObject(string className)
	{
		this.Key = NavObject.nextKey++;
		this.SetupNavObjectClass(className);
	}

	// Token: 0x06003255 RID: 12885 RVA: 0x0015596C File Offset: 0x00153B6C
	public void Reset(string className)
	{
		this.UseOverrideColor = false;
		this.OverrideColor = Color.white;
		this.SetupNavObjectClass(className);
		this.trackedPosition = NavObject.InvalidPos;
		this.trackedTransform = null;
		this.trackedEntity = null;
		this.OwnerEntity = null;
		this.name = "";
		this.ForceDisabled = false;
		this.usingLocalizationId = false;
		this.TrackType = NavObject.TrackTypes.None;
	}

	// Token: 0x06003256 RID: 12886 RVA: 0x001559D4 File Offset: 0x00153BD4
	public void SetupNavObjectClass(string className)
	{
		this.NavObjectClassList.Clear();
		this.hasOnScreen = false;
		if (className.Contains(","))
		{
			string[] array = className.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				NavObjectClass navObjectClass = NavObjectClass.GetNavObjectClass(array[i]);
				if (navObjectClass != null)
				{
					if (navObjectClass.OnScreenSettings != null || navObjectClass.InactiveOnScreenSettings != null)
					{
						this.hasOnScreen = true;
					}
					this.NavObjectClassList.Add(navObjectClass);
				}
			}
		}
		else
		{
			NavObjectClass navObjectClass2 = NavObjectClass.GetNavObjectClass(className);
			if (navObjectClass2.OnScreenSettings != null || navObjectClass2.InactiveOnScreenSettings != null)
			{
				this.hasOnScreen = true;
			}
			this.NavObjectClassList.Add(navObjectClass2);
		}
		this.NavObjectClass = this.NavObjectClassList[0];
	}

	// Token: 0x06003257 RID: 12887 RVA: 0x00155A88 File Offset: 0x00153C88
	public void HandleActiveNavClass(EntityPlayerLocal localPlayer)
	{
		if (this.NavObjectClassList != null && this.NavObjectClassList.Count > 0)
		{
			for (int i = 0; i < this.NavObjectClassList.Count; i++)
			{
				if (this.IsValidPlayer(localPlayer, this.NavObjectClassList[i]))
				{
					if (this.NavObjectClass != this.NavObjectClassList[i])
					{
						this.NavObjectClass = this.NavObjectClassList[i];
						this.SetupEntityOptions();
					}
					return;
				}
			}
			this.NavObjectClass = null;
		}
	}

	// Token: 0x06003258 RID: 12888 RVA: 0x00155B0C File Offset: 0x00153D0C
	public virtual float GetCompassIconScale(float _distance)
	{
		float t = 1f - _distance / this.CurrentCompassSettings.MaxScaleDistance;
		return Mathf.Lerp(this.CurrentCompassSettings.MinCompassIconScale, this.CurrentCompassSettings.MaxCompassIconScale, t);
	}

	// Token: 0x06003259 RID: 12889 RVA: 0x00155B4C File Offset: 0x00153D4C
	public override string ToString()
	{
		string text = "";
		if (this.TrackType == NavObject.TrackTypes.Transform)
		{
			text = ((this.TrackedTransform != null) ? this.TrackedTransform.name : "none");
		}
		else if (this.TrackType == NavObject.TrackTypes.Entity)
		{
			text = ((this.TrackedEntity != null) ? this.TrackedEntity.GetDebugName() : "none");
		}
		string format = "{0} #{1}, {2}, {3}, {4}, {5}, {6}";
		object[] array = new object[7];
		array[0] = this.name;
		array[1] = this.NavObjectClassList.Count;
		array[2] = ((this.NavObjectClass != null) ? this.NavObjectClass.NavObjectClassName : "null");
		int num = 3;
		NavObjectClass navObjectClass = this.NavObjectClass;
		array[num] = ((navObjectClass != null) ? new NavObjectClass.RequirementTypes?(navObjectClass.RequirementType) : null);
		array[4] = this.TrackType;
		array[5] = text;
		array[6] = this.GetPosition();
		return string.Format(format, array);
	}

	// Token: 0x04002914 RID: 10516
	public static Vector3 InvalidPos = new Vector3(-99999f, -99999f, -99999f);

	// Token: 0x04002915 RID: 10517
	public List<NavObjectClass> NavObjectClassList = new List<NavObjectClass>();

	// Token: 0x04002916 RID: 10518
	public NavObjectClass NavObjectClass;

	// Token: 0x04002917 RID: 10519
	public NavObject.TrackTypes TrackType;

	// Token: 0x04002918 RID: 10520
	public bool IsActive = true;

	// Token: 0x04002919 RID: 10521
	public bool ForceDisabled;

	// Token: 0x0400291A RID: 10522
	public int EntityID;

	// Token: 0x0400291B RID: 10523
	public Entity OwnerEntity;

	// Token: 0x0400291C RID: 10524
	public string name;

	// Token: 0x0400291D RID: 10525
	public bool usingLocalizationId;

	// Token: 0x0400291E RID: 10526
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName;

	// Token: 0x0400291F RID: 10527
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform trackedTransform;

	// Token: 0x04002920 RID: 10528
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 trackedPosition;

	// Token: 0x04002921 RID: 10529
	[PublicizedFrom(EAccessModifier.Private)]
	public Entity trackedEntity;

	// Token: 0x04002922 RID: 10530
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasOnScreen;

	// Token: 0x04002923 RID: 10531
	[PublicizedFrom(EAccessModifier.Private)]
	public static int nextKey;

	// Token: 0x04002924 RID: 10532
	public int Key;

	// Token: 0x04002925 RID: 10533
	public float ExtraData;

	// Token: 0x04002926 RID: 10534
	public bool IsTracked;

	// Token: 0x04002927 RID: 10535
	public bool hiddenOnCompass = true;

	// Token: 0x04002928 RID: 10536
	public string HiddenDisplayName;

	// Token: 0x04002929 RID: 10537
	public bool hiddenOnMap;

	// Token: 0x0400292A RID: 10538
	public string OverrideSpriteName = "";

	// Token: 0x0400292B RID: 10539
	public bool UseOverrideColor;

	// Token: 0x0400292C RID: 10540
	public bool UseOverrideFontColor;

	// Token: 0x0400292D RID: 10541
	public Color OverrideColor;

	// Token: 0x020006A7 RID: 1703
	public enum TrackTypes
	{
		// Token: 0x0400292F RID: 10543
		None,
		// Token: 0x04002930 RID: 10544
		Transform,
		// Token: 0x04002931 RID: 10545
		Position,
		// Token: 0x04002932 RID: 10546
		Entity
	}
}
