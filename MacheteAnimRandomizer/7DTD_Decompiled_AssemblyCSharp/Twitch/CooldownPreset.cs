using System;
using System.Collections.Generic;
using System.Globalization;

namespace Twitch
{
	// Token: 0x02001551 RID: 5457
	public class CooldownPreset
	{
		// Token: 0x0600A7EB RID: 42987 RVA: 0x004224FC File Offset: 0x004206FC
		public void AddCooldownMaxEntry(int start, int end, int cooldownMax, int cooldownTime)
		{
			if (this.CooldownMaxEntries == null)
			{
				this.CooldownMaxEntries = new List<TwitchCooldownEntry>();
			}
			this.CooldownMaxEntries.Add(new TwitchCooldownEntry
			{
				StartGameStage = start,
				EndGameStage = end,
				CooldownMax = cooldownMax,
				CooldownTime = cooldownTime
			});
		}

		// Token: 0x0600A7EC RID: 42988 RVA: 0x0042254C File Offset: 0x0042074C
		public void SetupCooldownInfo(int gameStage, EntityPlayerLocal localPlayer)
		{
			if (localPlayer == null)
			{
				return;
			}
			for (int i = 0; i < this.CooldownMaxEntries.Count; i++)
			{
				if (gameStage >= this.CooldownMaxEntries[i].StartGameStage && (gameStage <= this.CooldownMaxEntries[i].EndGameStage || this.CooldownMaxEntries[i].EndGameStage == -1))
				{
					float num = 1f;
					if (localPlayer.Party != null)
					{
						int num2 = 0;
						for (int j = 0; j < localPlayer.Party.MemberList.Count; j++)
						{
							if (localPlayer.Party.MemberList[j].TwitchActionsEnabled != EntityPlayer.TwitchActionsStates.Disabled)
							{
								num2++;
							}
						}
						num += (float)(num2 - 1) * 0.5f;
					}
					this.CooldownFillMax = (float)this.CooldownMaxEntries[i].CooldownMax * num;
					this.NextCooldownTime = this.CooldownMaxEntries[i].CooldownTime;
					return;
				}
			}
			this.CooldownFillMax = 100f;
			this.NextCooldownTime = 180;
		}

		// Token: 0x0600A7ED RID: 42989 RVA: 0x00422660 File Offset: 0x00420860
		public virtual void ParseProperties(DynamicProperties properties)
		{
			if (properties.Values.ContainsKey(CooldownPreset.PropName))
			{
				this.Name = properties.Values[CooldownPreset.PropName];
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropTitle))
			{
				this.Title = properties.Values[CooldownPreset.PropTitle];
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropTitleKey))
			{
				this.Title = Localization.Get(properties.Values[CooldownPreset.PropTitleKey], false);
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropCooldownType))
			{
				this.CooldownType = (CooldownPreset.CooldownTypes)Enum.Parse(typeof(CooldownPreset.CooldownTypes), properties.Values[CooldownPreset.PropCooldownType], true);
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropIsDefault))
			{
				this.IsDefault = StringParsers.ParseBool(properties.Values[CooldownPreset.PropIsDefault], 0, -1, true);
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropStartCooldown))
			{
				this.StartCooldownTime = StringParsers.ParseSInt32(properties.Values[CooldownPreset.PropStartCooldown], 0, -1, NumberStyles.Integer);
			}
			else
			{
				this.StartCooldownTime = 300;
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropDeathCooldown))
			{
				this.AfterDeathCooldownTime = StringParsers.ParseSInt32(properties.Values[CooldownPreset.PropDeathCooldown], 0, -1, NumberStyles.Integer);
			}
			else
			{
				this.AfterDeathCooldownTime = 180;
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropBMStartOffset))
			{
				this.BMStartOffset = StringParsers.ParseSInt32(properties.Values[CooldownPreset.PropBMStartOffset], 0, -1, NumberStyles.Integer);
			}
			if (properties.Values.ContainsKey(CooldownPreset.PropBMEndOffset))
			{
				this.BMEndOffset = StringParsers.ParseSInt32(properties.Values[CooldownPreset.PropBMEndOffset], 0, -1, NumberStyles.Integer);
			}
		}

		// Token: 0x04008212 RID: 33298
		public static string PropName = "name";

		// Token: 0x04008213 RID: 33299
		public static string PropTitle = "title";

		// Token: 0x04008214 RID: 33300
		public static string PropTitleKey = "title_key";

		// Token: 0x04008215 RID: 33301
		public static string PropCooldownType = "cooldown_type";

		// Token: 0x04008216 RID: 33302
		public static string PropIsDefault = "is_default";

		// Token: 0x04008217 RID: 33303
		public static string PropStartCooldown = "start_cooldown";

		// Token: 0x04008218 RID: 33304
		public static string PropDeathCooldown = "death_cooldown";

		// Token: 0x04008219 RID: 33305
		public static string PropBMStartOffset = "bm_start_offset";

		// Token: 0x0400821A RID: 33306
		public static string PropBMEndOffset = "bm_end_offset";

		// Token: 0x0400821B RID: 33307
		public string Name;

		// Token: 0x0400821C RID: 33308
		public bool IsDefault;

		// Token: 0x0400821D RID: 33309
		public string Title;

		// Token: 0x0400821E RID: 33310
		public CooldownPreset.CooldownTypes CooldownType = CooldownPreset.CooldownTypes.Fill;

		// Token: 0x0400821F RID: 33311
		public float CooldownFillMax;

		// Token: 0x04008220 RID: 33312
		public int NextCooldownTime;

		// Token: 0x04008221 RID: 33313
		public int StartCooldownTime;

		// Token: 0x04008222 RID: 33314
		public int AfterDeathCooldownTime;

		// Token: 0x04008223 RID: 33315
		public int BMStartOffset;

		// Token: 0x04008224 RID: 33316
		public int BMEndOffset;

		// Token: 0x04008225 RID: 33317
		public List<TwitchCooldownEntry> CooldownMaxEntries = new List<TwitchCooldownEntry>();

		// Token: 0x02001552 RID: 5458
		public enum CooldownTypes
		{
			// Token: 0x04008227 RID: 33319
			Always,
			// Token: 0x04008228 RID: 33320
			Fill,
			// Token: 0x04008229 RID: 33321
			None
		}
	}
}
