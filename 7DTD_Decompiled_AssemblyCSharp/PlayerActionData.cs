using System;

// Token: 0x020004D8 RID: 1240
public class PlayerActionData
{
	// Token: 0x04001EEE RID: 7918
	public static readonly PlayerActionData.ActionTab TabMovement = new PlayerActionData.ActionTab("inpTabPlayerControl", 0);

	// Token: 0x04001EEF RID: 7919
	public static readonly PlayerActionData.ActionTab TabToolbelt = new PlayerActionData.ActionTab("inpTabToolbelt", 10);

	// Token: 0x04001EF0 RID: 7920
	public static readonly PlayerActionData.ActionTab TabVehicle = new PlayerActionData.ActionTab("inpTabVehicle", 15);

	// Token: 0x04001EF1 RID: 7921
	public static readonly PlayerActionData.ActionTab TabMenus = new PlayerActionData.ActionTab("inpTabMenus", 20);

	// Token: 0x04001EF2 RID: 7922
	public static readonly PlayerActionData.ActionTab TabUi = new PlayerActionData.ActionTab("inpTabUi", 30);

	// Token: 0x04001EF3 RID: 7923
	public static readonly PlayerActionData.ActionTab TabOther = new PlayerActionData.ActionTab("inpTabOther", 40);

	// Token: 0x04001EF4 RID: 7924
	public static readonly PlayerActionData.ActionTab TabEdit = new PlayerActionData.ActionTab("inpTabEdit", 50);

	// Token: 0x04001EF5 RID: 7925
	public static readonly PlayerActionData.ActionTab TabGlobal = new PlayerActionData.ActionTab("inpTabGlobal", 60);

	// Token: 0x04001EF6 RID: 7926
	public static readonly PlayerActionData.ActionGroup GroupPlayerControl = new PlayerActionData.ActionGroup("inpGrpPlayerControlName", null, 0, PlayerActionData.TabMovement);

	// Token: 0x04001EF7 RID: 7927
	public static readonly PlayerActionData.ActionGroup GroupToolbelt = new PlayerActionData.ActionGroup("inpGrpToolbeltName", null, 10, PlayerActionData.TabToolbelt);

	// Token: 0x04001EF8 RID: 7928
	public static readonly PlayerActionData.ActionGroup GroupVehicle = new PlayerActionData.ActionGroup("inpGrpVehicleName", null, 15, PlayerActionData.TabVehicle);

	// Token: 0x04001EF9 RID: 7929
	public static readonly PlayerActionData.ActionGroup GroupMenu = new PlayerActionData.ActionGroup("inpGrpMenuName", null, 20, PlayerActionData.TabMenus);

	// Token: 0x04001EFA RID: 7930
	public static readonly PlayerActionData.ActionGroup GroupDialogs = new PlayerActionData.ActionGroup("inpGrpDialogsName", null, 30, PlayerActionData.TabMenus);

	// Token: 0x04001EFB RID: 7931
	public static readonly PlayerActionData.ActionGroup GroupUI = new PlayerActionData.ActionGroup("inpGrpUiName", null, 40, PlayerActionData.TabUi);

	// Token: 0x04001EFC RID: 7932
	public static readonly PlayerActionData.ActionGroup GroupMp = new PlayerActionData.ActionGroup("inpGrpMpName", null, 50, PlayerActionData.TabOther);

	// Token: 0x04001EFD RID: 7933
	public static readonly PlayerActionData.ActionGroup GroupAdmin = new PlayerActionData.ActionGroup("inpGrpAdminName", null, 60, PlayerActionData.TabOther);

	// Token: 0x04001EFE RID: 7934
	public static readonly PlayerActionData.ActionGroup GroupGlobalFunctions = new PlayerActionData.ActionGroup("inpGrpGlobalFunctionsName", null, 80, PlayerActionData.TabGlobal);

	// Token: 0x04001EFF RID: 7935
	public static readonly PlayerActionData.ActionGroup GroupDebugFunctions = new PlayerActionData.ActionGroup("inpGrpDebugFunctionsName", null, 100, PlayerActionData.TabGlobal);

	// Token: 0x04001F00 RID: 7936
	public static readonly PlayerActionData.ActionGroup GroupEditCamera = new PlayerActionData.ActionGroup("inpGrpCameraName", null, 20, PlayerActionData.TabEdit);

	// Token: 0x04001F01 RID: 7937
	public static readonly PlayerActionData.ActionGroup GroupEditSelection = new PlayerActionData.ActionGroup("inpGrpSelectionName", null, 40, PlayerActionData.TabEdit);

	// Token: 0x04001F02 RID: 7938
	public static readonly PlayerActionData.ActionGroup GroupEditOther = new PlayerActionData.ActionGroup("inpGrpOtherName", null, 60, PlayerActionData.TabEdit);

	// Token: 0x020004D9 RID: 1241
	public enum EAppliesToInputType
	{
		// Token: 0x04001F04 RID: 7940
		None,
		// Token: 0x04001F05 RID: 7941
		KbdMouseOnly,
		// Token: 0x04001F06 RID: 7942
		ControllerOnly,
		// Token: 0x04001F07 RID: 7943
		Both
	}

	// Token: 0x020004DA RID: 1242
	public class ActionSetUserData
	{
		// Token: 0x06002851 RID: 10321 RVA: 0x0010605A File Offset: 0x0010425A
		public ActionSetUserData(params PlayerActionsBase[] _bindingsConflictWithSet)
		{
			this.bindingsConflictWithSet = _bindingsConflictWithSet;
		}

		// Token: 0x04001F08 RID: 7944
		public readonly PlayerActionsBase[] bindingsConflictWithSet;
	}

	// Token: 0x020004DB RID: 1243
	public class ActionTab : IComparable<PlayerActionData.ActionTab>
	{
		// Token: 0x06002852 RID: 10322 RVA: 0x00106069 File Offset: 0x00104269
		public ActionTab(string _tabNameKey, int _tabPriority)
		{
			this.tabNameKey = _tabNameKey;
			this.tabPriority = _tabPriority;
		}

		// Token: 0x06002853 RID: 10323 RVA: 0x00106080 File Offset: 0x00104280
		public int CompareTo(PlayerActionData.ActionTab _other)
		{
			return this.tabPriority.CompareTo(_other.tabPriority);
		}

		// Token: 0x17000422 RID: 1058
		// (get) Token: 0x06002854 RID: 10324 RVA: 0x001060A1 File Offset: 0x001042A1
		public string LocalizedName
		{
			get
			{
				return Localization.Get(this.tabNameKey, false);
			}
		}

		// Token: 0x04001F09 RID: 7945
		public readonly string tabNameKey;

		// Token: 0x04001F0A RID: 7946
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int tabPriority;
	}

	// Token: 0x020004DC RID: 1244
	public class ActionGroup : IComparable<PlayerActionData.ActionGroup>
	{
		// Token: 0x06002855 RID: 10325 RVA: 0x001060B0 File Offset: 0x001042B0
		public ActionGroup(string _groupNameKey, string _groupDescKey, int _groupPriority, PlayerActionData.ActionTab _actionTab)
		{
			this.groupNameKey = _groupNameKey;
			this.groupDescKey = (_groupDescKey ?? (this.groupNameKey.Replace("Name", "") + "Desc"));
			this.groupPriority = _groupPriority;
			this.actionTab = _actionTab;
		}

		// Token: 0x06002856 RID: 10326 RVA: 0x00106104 File Offset: 0x00104304
		public int CompareTo(PlayerActionData.ActionGroup _other)
		{
			return this.groupPriority.CompareTo(_other.groupPriority);
		}

		// Token: 0x17000423 RID: 1059
		// (get) Token: 0x06002857 RID: 10327 RVA: 0x00106125 File Offset: 0x00104325
		public string LocalizedName
		{
			get
			{
				return Localization.Get(this.groupNameKey, false);
			}
		}

		// Token: 0x17000424 RID: 1060
		// (get) Token: 0x06002858 RID: 10328 RVA: 0x00106134 File Offset: 0x00104334
		public string LocalizedDescription
		{
			get
			{
				string text = Localization.Get(this.groupDescKey, false);
				if (!(text != this.groupDescKey))
				{
					return null;
				}
				return text;
			}
		}

		// Token: 0x04001F0B RID: 7947
		public readonly string groupNameKey;

		// Token: 0x04001F0C RID: 7948
		public readonly string groupDescKey;

		// Token: 0x04001F0D RID: 7949
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int groupPriority;

		// Token: 0x04001F0E RID: 7950
		public readonly PlayerActionData.ActionTab actionTab;
	}

	// Token: 0x020004DD RID: 1245
	public class ActionUserData
	{
		// Token: 0x06002859 RID: 10329 RVA: 0x00106160 File Offset: 0x00104360
		public ActionUserData(string _actionNameKey, string _actionDescKey, PlayerActionData.ActionGroup _actionGroup, PlayerActionData.EAppliesToInputType _appliesToInputType = PlayerActionData.EAppliesToInputType.Both, bool _allowRebind = true, bool _allowMultipleRebindings = false, bool _doNotDisplay = false, bool _defaultOnStartup = true)
		{
			this.actionNameKey = _actionNameKey;
			this.actionDescKey = (_actionDescKey ?? (this.actionNameKey.Replace("Name", "") + "Desc"));
			this.actionGroup = _actionGroup;
			this.appliesToInputType = _appliesToInputType;
			this.allowRebind = _allowRebind;
			this.allowMultipleBindings = _allowMultipleRebindings;
			this.doNotDisplay = _doNotDisplay;
			this.defaultOnStartup = _defaultOnStartup;
			if (this.actionGroup == null)
			{
				throw new ArgumentNullException("_actionGroup");
			}
		}

		// Token: 0x17000425 RID: 1061
		// (get) Token: 0x0600285A RID: 10330 RVA: 0x001061E6 File Offset: 0x001043E6
		public string LocalizedName
		{
			get
			{
				return Localization.Get(this.actionNameKey, false);
			}
		}

		// Token: 0x17000426 RID: 1062
		// (get) Token: 0x0600285B RID: 10331 RVA: 0x001061F4 File Offset: 0x001043F4
		public string LocalizedDescription
		{
			get
			{
				string text = Localization.Get(this.actionDescKey, false);
				if (!(text != this.actionDescKey))
				{
					return null;
				}
				return text;
			}
		}

		// Token: 0x04001F0F RID: 7951
		public readonly string actionNameKey;

		// Token: 0x04001F10 RID: 7952
		public readonly string actionDescKey;

		// Token: 0x04001F11 RID: 7953
		public readonly PlayerActionData.ActionGroup actionGroup;

		// Token: 0x04001F12 RID: 7954
		public readonly PlayerActionData.EAppliesToInputType appliesToInputType;

		// Token: 0x04001F13 RID: 7955
		public readonly bool allowRebind;

		// Token: 0x04001F14 RID: 7956
		public readonly bool allowMultipleBindings;

		// Token: 0x04001F15 RID: 7957
		public readonly bool doNotDisplay;

		// Token: 0x04001F16 RID: 7958
		public readonly bool defaultOnStartup;
	}
}
