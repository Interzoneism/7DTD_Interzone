using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000643 RID: 1603
[Preserve]
public class MinEventActionSetProgressionLevel : MinEventActionTargetedBase
{
	// Token: 0x170004AE RID: 1198
	// (get) Token: 0x060030F2 RID: 12530 RVA: 0x0014E4FD File Offset: 0x0014C6FD
	// (set) Token: 0x060030F3 RID: 12531 RVA: 0x0014E505 File Offset: 0x0014C705
	public string progressionName { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004AF RID: 1199
	// (get) Token: 0x060030F4 RID: 12532 RVA: 0x0014E50E File Offset: 0x0014C70E
	// (set) Token: 0x060030F5 RID: 12533 RVA: 0x0014E516 File Offset: 0x0014C716
	public int level { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060030F6 RID: 12534 RVA: 0x0014E520 File Offset: 0x0014C720
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
					if (this.level != -1)
					{
						progressionValue.Level = this.level;
						this.targets[i].Progression.bProgressionStatsChanged = !this.targets[i].isEntityRemote;
						this.targets[i].bPlayerStatsChanged |= !this.targets[i].isEntityRemote;
					}
					else
					{
						progressionValue.Level = progressionValue.ProgressionClass.MaxLevel;
						this.targets[i].Progression.bProgressionStatsChanged = !this.targets[i].isEntityRemote;
						this.targets[i].bPlayerStatsChanged |= !this.targets[i].isEntityRemote;
					}
				}
			}
		}
	}

	// Token: 0x060030F7 RID: 12535 RVA: 0x0014E65F File Offset: 0x0014C85F
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && this.progressionName != null && this.level >= -1;
	}

	// Token: 0x060030F8 RID: 12536 RVA: 0x0014E684 File Offset: 0x0014C884
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (!(localName == "progression_name"))
			{
				if (localName == "level")
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
}
