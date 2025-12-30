using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000644 RID: 1604
[Preserve]
public class MinEventActionAddProgressionLevel : MinEventActionTargetedBase
{
	// Token: 0x170004B0 RID: 1200
	// (get) Token: 0x060030FA RID: 12538 RVA: 0x0014E6E7 File Offset: 0x0014C8E7
	// (set) Token: 0x060030FB RID: 12539 RVA: 0x0014E6EF File Offset: 0x0014C8EF
	public string progressionName { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x060030FC RID: 12540 RVA: 0x0014E6F8 File Offset: 0x0014C8F8
	// (set) Token: 0x060030FD RID: 12541 RVA: 0x0014E700 File Offset: 0x0014C900
	public int level { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060030FE RID: 12542 RVA: 0x0014E70C File Offset: 0x0014C90C
	public override void Execute(MinEventParams _params)
	{
		if (this.targets == null)
		{
			return;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.targets[i].Progression != null)
			{
				ProgressionValue progressionValue = this.targets[i].Progression.GetProgressionValue(this.progressionName);
				if (progressionValue != null)
				{
					int level = progressionValue.Level;
					progressionValue.Level += this.level;
					if (progressionValue.Level > progressionValue.ProgressionClass.MaxLevel)
					{
						progressionValue.Level = progressionValue.ProgressionClass.MaxLevel;
					}
					if (progressionValue.Level < 0)
					{
						progressionValue.Level = 0;
					}
					if (level != progressionValue.Level && progressionValue.ProgressionClass.IsCrafting && this.targets[i] is EntityPlayerLocal)
					{
						EntityPlayerLocal entityPlayerLocal = this.targets[i] as EntityPlayerLocal;
						entityPlayerLocal.PlayerUI.xui.CollectedItemList.AddCraftingSkillNotification(progressionValue, false);
						if (this.showMessage)
						{
							progressionValue.ProgressionClass.HandleCheckCrafting(entityPlayerLocal, level, progressionValue.Level);
						}
					}
					this.targets[i].Progression.bProgressionStatsChanged = !this.targets[i].isEntityRemote;
					this.targets[i].bPlayerStatsChanged |= !this.targets[i].isEntityRemote;
				}
			}
		}
	}

	// Token: 0x060030FF RID: 12543 RVA: 0x0014E888 File Offset: 0x0014CA88
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && this.progressionName != null && this.level >= -1;
	}

	// Token: 0x06003100 RID: 12544 RVA: 0x0014E8AC File Offset: 0x0014CAAC
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (!(localName == "progression_name"))
			{
				if (!(localName == "level"))
				{
					if (localName == "show_message")
					{
						this.showMessage = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
					}
				}
				else
				{
					this.level = StringParsers.ParseSInt32(_attribute.Value, 0, -1, NumberStyles.Integer);
				}
			}
			else
			{
				this.progressionName = _attribute.Value;
			}
		}
		return flag;
	}

	// Token: 0x0400276B RID: 10091
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showMessage = true;
}
