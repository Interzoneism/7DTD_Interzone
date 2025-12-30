using System;
using System.Collections.Generic;
using Epic.OnlineServices;
using Epic.OnlineServices.Reports;

namespace Platform.EOS
{
	// Token: 0x02001923 RID: 6435
	public class PlayerReporting : IPlayerReporting
	{
		// Token: 0x170015C2 RID: 5570
		// (get) Token: 0x0600BDF8 RID: 48632 RVA: 0x0047FE4C File Offset: 0x0047E04C
		public ReportsInterface reportsInterface
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return ((Api)this.owner.Api).PlatformInterface.GetReportsInterface();
			}
		}

		// Token: 0x170015C3 RID: 5571
		// (get) Token: 0x0600BDF9 RID: 48633 RVA: 0x0047FE68 File Offset: 0x0047E068
		public ProductUserId localProductUserId
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return ((UserIdentifierEos)this.owner.User.PlatformUserId).ProductUserId;
			}
		}

		// Token: 0x0600BDFA RID: 48634 RVA: 0x0047FE84 File Offset: 0x0047E084
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x0600BDFB RID: 48635 RVA: 0x0047FE90 File Offset: 0x0047E090
		public IList<IPlayerReporting.PlayerReportCategory> ReportCategories()
		{
			if (this.reportCategories != null)
			{
				return this.reportCategories.list;
			}
			this.reportCategories = new DictionaryList<PlayerReportsCategory, IPlayerReporting.PlayerReportCategory>();
			foreach (PlayerReportsCategory playerReportsCategory in EnumUtils.Values<PlayerReportsCategory>())
			{
				if (playerReportsCategory != PlayerReportsCategory.Invalid)
				{
					this.reportCategories.Add(playerReportsCategory, new PlayerReporting.PlayerReportCategoryEos(playerReportsCategory, Localization.Get("xuiCategoryPlayerReport" + playerReportsCategory.ToStringCached<PlayerReportsCategory>(), false)));
				}
			}
			return this.reportCategories.list;
		}

		// Token: 0x0600BDFC RID: 48636 RVA: 0x0047FF2C File Offset: 0x0047E12C
		public void ReportPlayer(PlatformUserIdentifierAbs _reportedUserCross, IPlayerReporting.PlayerReportCategory _reportCategory, string _message, Action<bool> _reportCompleteCallback)
		{
			if (_message != null && _message.Length > 256)
			{
				Log.Out("[EOS-Report] Long message, might get truncated");
			}
			EosHelpers.AssertMainThread("PRep.Send");
			SendPlayerBehaviorReportOptions sendPlayerBehaviorReportOptions = new SendPlayerBehaviorReportOptions
			{
				ReporterUserId = this.localProductUserId,
				ReportedUserId = ((UserIdentifierEos)_reportedUserCross).ProductUserId,
				Category = ((PlayerReporting.PlayerReportCategoryEos)_reportCategory).Category,
				Message = _message
			};
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.reportsInterface.SendPlayerBehaviorReport(ref sendPlayerBehaviorReportOptions, null, delegate(ref SendPlayerBehaviorReportCompleteCallbackInfo _callbackData)
				{
					if (_callbackData.ResultCode != Result.Success)
					{
						Log.Error("[EOS-Report] Reporting player failed: " + _callbackData.ResultCode.ToStringCached<Result>());
						_reportCompleteCallback(false);
						return;
					}
					Log.Out("[EOS-Report] Sent player report");
					_reportCompleteCallback(true);
				});
			}
		}

		// Token: 0x0600BDFD RID: 48637 RVA: 0x0047FFFC File Offset: 0x0047E1FC
		public IPlayerReporting.PlayerReportCategory GetPlayerReportCategoryMapping(EnumReportCategory _reportCategory)
		{
			PlayerReportsCategory playerReportsCategory;
			if (_reportCategory != EnumReportCategory.Cheating)
			{
				if (_reportCategory != EnumReportCategory.VerbalAbuse)
				{
					playerReportsCategory = PlayerReportsCategory.Other;
				}
				else
				{
					playerReportsCategory = PlayerReportsCategory.VerbalAbuse;
				}
			}
			else
			{
				playerReportsCategory = PlayerReportsCategory.Cheating;
			}
			PlayerReportsCategory key = playerReportsCategory;
			IPlayerReporting.PlayerReportCategory result;
			if (!this.reportCategories.dict.TryGetValue(key, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x040093F3 RID: 37875
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x040093F4 RID: 37876
		[PublicizedFrom(EAccessModifier.Private)]
		public DictionaryList<PlayerReportsCategory, IPlayerReporting.PlayerReportCategory> reportCategories;

		// Token: 0x02001924 RID: 6436
		[PublicizedFrom(EAccessModifier.Private)]
		public class PlayerReportCategoryEos : IPlayerReporting.PlayerReportCategory
		{
			// Token: 0x0600BDFF RID: 48639 RVA: 0x00480037 File Offset: 0x0047E237
			public PlayerReportCategoryEos(PlayerReportsCategory _category, string _displayString)
			{
				this.Category = _category;
				this.displayString = _displayString;
			}

			// Token: 0x0600BE00 RID: 48640 RVA: 0x0048004D File Offset: 0x0047E24D
			public override string ToString()
			{
				return this.displayString;
			}

			// Token: 0x040093F5 RID: 37877
			public readonly PlayerReportsCategory Category;

			// Token: 0x040093F6 RID: 37878
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly string displayString;
		}
	}
}
