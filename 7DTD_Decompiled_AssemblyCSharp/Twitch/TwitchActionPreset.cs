using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x02001527 RID: 5415
	public class TwitchActionPreset
	{
		// Token: 0x0600A700 RID: 42752 RVA: 0x0041F23A File Offset: 0x0041D43A
		public void AddCooldownModifier(TwitchActionCooldownModifier modifier)
		{
			if (this.ActionCooldownModifiers == null)
			{
				this.ActionCooldownModifiers = new List<TwitchActionCooldownModifier>();
			}
			this.ActionCooldownModifiers.Add(modifier);
		}

		// Token: 0x0600A701 RID: 42753 RVA: 0x0041F25C File Offset: 0x0041D45C
		public void HandleCooldowns()
		{
			foreach (TwitchAction twitchAction in TwitchActionManager.TwitchActions.Values)
			{
				twitchAction.Cooldown = twitchAction.OriginalCooldown;
				if (twitchAction.IsInPreset(this) && this.ActionCooldownModifiers != null)
				{
					for (int i = 0; i < this.ActionCooldownModifiers.Count; i++)
					{
						TwitchActionCooldownModifier twitchActionCooldownModifier = this.ActionCooldownModifiers[i];
						if (twitchActionCooldownModifier.ActionName == twitchAction.Name || twitchActionCooldownModifier.CategoryName == twitchAction.MainCategory.Name)
						{
							switch (twitchActionCooldownModifier.Modifier)
							{
							case PassiveEffect.ValueModifierTypes.base_set:
								twitchAction.Cooldown = twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.base_add:
								twitchAction.Cooldown += twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.base_subtract:
								twitchAction.Cooldown -= twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.perc_set:
								twitchAction.Cooldown *= twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.perc_add:
								twitchAction.Cooldown += twitchAction.Cooldown * twitchActionCooldownModifier.Value;
								break;
							case PassiveEffect.ValueModifierTypes.perc_subtract:
								twitchAction.Cooldown -= twitchAction.Cooldown * twitchActionCooldownModifier.Value;
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x040081C5 RID: 33221
		public string Name;

		// Token: 0x040081C6 RID: 33222
		public bool IsEnabled = true;

		// Token: 0x040081C7 RID: 33223
		public bool IsDefault;

		// Token: 0x040081C8 RID: 33224
		public bool IsEmpty;

		// Token: 0x040081C9 RID: 33225
		public string Title;

		// Token: 0x040081CA RID: 33226
		public string Description;

		// Token: 0x040081CB RID: 33227
		public bool AllowPointGeneration = true;

		// Token: 0x040081CC RID: 33228
		public bool UseHelperReward = true;

		// Token: 0x040081CD RID: 33229
		public bool ShowNewCommands = true;

		// Token: 0x040081CE RID: 33230
		public List<TwitchActionCooldownModifier> ActionCooldownModifiers;

		// Token: 0x040081CF RID: 33231
		public List<string> AddedActions = new List<string>();

		// Token: 0x040081D0 RID: 33232
		public List<string> RemovedActions = new List<string>();
	}
}
