using System;
using System.Xml.Linq;

// Token: 0x02000668 RID: 1640
public class MinEventActionSoundBase : MinEventActionTargetedBase
{
	// Token: 0x06003178 RID: 12664 RVA: 0x00151548 File Offset: 0x0014F748
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
			if (num <= 1866706639U)
			{
				if (num != 235771284U)
				{
					if (num != 681419758U)
					{
						if (num != 1866706639U)
						{
							return flag;
						}
						if (!(localName == "silent_on_equip"))
						{
							return flag;
						}
						this.silentOnEquip = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
						return true;
					}
					else
					{
						if (!(localName == "toggle_dms"))
						{
							return flag;
						}
						this.toggleDMS = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
						return true;
					}
				}
				else if (!(localName == "sound"))
				{
					return flag;
				}
			}
			else if (num <= 2660347774U)
			{
				if (num != 1956785724U)
				{
					if (num != 2660347774U)
					{
						return flag;
					}
					if (!(localName == "play_in_head"))
					{
						return flag;
					}
					this.localPlayerOnly = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
					return true;
				}
				else
				{
					if (!(localName == "play_at_self"))
					{
						return flag;
					}
					this.playAtSelf = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
					return true;
				}
			}
			else if (num != 2992025991U)
			{
				if (num != 3723446379U)
				{
					return flag;
				}
				if (!(localName == "loop"))
				{
					return flag;
				}
				this.loop = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				return true;
			}
			else if (!(localName == "soundGroup"))
			{
				return flag;
			}
			this.soundGroup = _attribute.Value.Trim();
			return true;
		}
		return flag;
	}

	// Token: 0x06003179 RID: 12665 RVA: 0x001516D4 File Offset: 0x0014F8D4
	[PublicizedFrom(EAccessModifier.Protected)]
	public string GetSoundGroupForTarget()
	{
		int num = this.soundGroup.IndexOfAny(MinEventActionSoundBase.convertChars);
		if (num < 0)
		{
			return this.soundGroup;
		}
		if (this.soundGroup[num] == '#')
		{
			return this.soundGroup.Replace("#", this.targets[0].IsMale ? "1" : "2");
		}
		return this.soundGroup.Replace("$", this.targets[0].IsMale ? "Male" : "Female");
	}

	// Token: 0x040027DD RID: 10205
	[PublicizedFrom(EAccessModifier.Protected)]
	public string soundGroup;

	// Token: 0x040027DE RID: 10206
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool localPlayerOnly;

	// Token: 0x040027DF RID: 10207
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool loop;

	// Token: 0x040027E0 RID: 10208
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool toggleDMS;

	// Token: 0x040027E1 RID: 10209
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool playAtSelf;

	// Token: 0x040027E2 RID: 10210
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool silentOnEquip;

	// Token: 0x040027E3 RID: 10211
	[PublicizedFrom(EAccessModifier.Private)]
	public static char[] convertChars = new char[]
	{
		'#',
		'$'
	};
}
