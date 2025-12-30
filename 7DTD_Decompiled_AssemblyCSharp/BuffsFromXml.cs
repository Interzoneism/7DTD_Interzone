using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000B90 RID: 2960
public class BuffsFromXml
{
	// Token: 0x06005B9B RID: 23451 RVA: 0x0024B9EC File Offset: 0x00249BEC
	public static IEnumerator CreateBuffs(XmlFile xmlFile)
	{
		BuffManager.Buffs = new CaseInsensitiveStringDictionary<BuffClass>();
		XElement root = xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <buffs> found!");
		}
		MicroStopwatch msw = new MicroStopwatch(true);
		foreach (XElement element in root.Elements("buff"))
		{
			BuffsFromXml.ParseBuff(element);
			if (msw.ElapsedMilliseconds > (long)Constants.cMaxLoadTimePerFrameMillis)
			{
				yield return null;
				msw.ResetAndRestart();
			}
		}
		IEnumerator<XElement> enumerator = null;
		BuffsFromXml.clearBuffValueLinks();
		yield break;
		yield break;
	}

	// Token: 0x06005B9C RID: 23452 RVA: 0x0024B9FC File Offset: 0x00249BFC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void clearBuffValueLinks()
	{
		if (!GameManager.Instance)
		{
			return;
		}
		World world = GameManager.Instance.World;
		DictionaryList<int, Entity> dictionaryList = (world != null) ? world.Entities : null;
		if (dictionaryList == null || dictionaryList.Count == 0)
		{
			return;
		}
		foreach (Entity entity in dictionaryList.list)
		{
			EntityAlive entityAlive = entity as EntityAlive;
			if (entityAlive != null)
			{
				entityAlive.Buffs.ClearBuffClassLinks();
			}
		}
	}

	// Token: 0x06005B9D RID: 23453 RVA: 0x0024BA8C File Offset: 0x00249C8C
	public static void Reload(XmlFile xmlFile)
	{
		ThreadManager.RunCoroutineSync(BuffsFromXml.CreateBuffs(xmlFile));
	}

	// Token: 0x06005B9E RID: 23454 RVA: 0x0024BA9C File Offset: 0x00249C9C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseBuff(XElement _element)
	{
		BuffClass buffClass = new BuffClass("");
		if (_element.HasAttribute("name"))
		{
			buffClass.Name = _element.GetAttribute("name").ToLower();
			buffClass.NameTag = FastTags<TagGroup.Global>.Parse(_element.GetAttribute("name"));
			if (_element.HasAttribute("name_key"))
			{
				buffClass.LocalizedName = Localization.Get(_element.GetAttribute("name_key"), false);
			}
			else
			{
				buffClass.LocalizedName = Localization.Get(buffClass.Name, false);
			}
			if (_element.HasAttribute("description_key"))
			{
				buffClass.DescriptionKey = _element.GetAttribute("description_key");
				buffClass.Description = Localization.Get(buffClass.DescriptionKey, false);
			}
			if (_element.HasAttribute("tooltip_key"))
			{
				buffClass.TooltipKey = _element.GetAttribute("tooltip_key");
				buffClass.Tooltip = Localization.Get(buffClass.TooltipKey, false);
			}
			if (_element.HasAttribute("icon"))
			{
				buffClass.Icon = _element.GetAttribute("icon");
			}
			if (_element.HasAttribute("hidden"))
			{
				buffClass.Hidden = StringParsers.ParseBool(_element.GetAttribute("hidden"), 0, -1, true);
			}
			else
			{
				buffClass.Hidden = false;
			}
			if (_element.HasAttribute("showonhud"))
			{
				buffClass.ShowOnHUD = StringParsers.ParseBool(_element.GetAttribute("showonhud"), 0, -1, true);
			}
			else
			{
				buffClass.ShowOnHUD = true;
			}
			if (_element.HasAttribute("update_rate"))
			{
				buffClass.UpdateRate = StringParsers.ParseFloat(_element.GetAttribute("update_rate"), 0, -1, NumberStyles.Any);
			}
			else
			{
				buffClass.UpdateRate = 1f;
			}
			if (_element.HasAttribute("allow_in_editor"))
			{
				buffClass.AllowInEditor = StringParsers.ParseBool(_element.GetAttribute("allow_in_editor"), 0, -1, true);
			}
			else
			{
				buffClass.AllowInEditor = false;
			}
			if (_element.HasAttribute("required_game_stat"))
			{
				buffClass.RequiredGameStat = Enum.Parse<EnumGameStats>(_element.GetAttribute("required_game_stat"));
			}
			else
			{
				buffClass.RequiredGameStat = EnumGameStats.Last;
			}
			if (_element.HasAttribute("remove_on_death"))
			{
				buffClass.RemoveOnDeath = StringParsers.ParseBool(_element.GetAttribute("remove_on_death"), 0, -1, true);
			}
			if (_element.HasAttribute("display_type"))
			{
				buffClass.DisplayType = EnumUtils.Parse<EnumEntityUINotificationDisplayMode>(_element.GetAttribute("display_type"), false);
			}
			else
			{
				buffClass.DisplayType = EnumEntityUINotificationDisplayMode.IconOnly;
			}
			if (_element.HasAttribute("icon_color"))
			{
				buffClass.IconColor = StringParsers.ParseColor32(_element.GetAttribute("icon_color"));
			}
			else
			{
				buffClass.IconColor = Color.white;
			}
			if (_element.HasAttribute("icon_blink"))
			{
				buffClass.IconBlink = StringParsers.ParseBool(_element.GetAttribute("icon_blink"), 0, -1, true);
			}
			buffClass.DamageSource = EnumDamageSource.Internal;
			buffClass.DamageType = EnumDamageTypes.None;
			buffClass.StackType = BuffEffectStackTypes.Replace;
			buffClass.DurationMax = 0f;
			foreach (XElement xelement in _element.Elements())
			{
				if (xelement.Name == "display_value" && xelement.HasAttribute("value"))
				{
					buffClass.DisplayValueCVar = xelement.GetAttribute("value");
				}
				if (xelement.Name == "display_value_key" && xelement.HasAttribute("value"))
				{
					buffClass.DisplayValueKey = xelement.GetAttribute("value");
				}
				if (xelement.Name == "display_value_format" && xelement.HasAttribute("value") && !Enum.TryParse<BuffClass.CVarDisplayFormat>(xelement.GetAttribute("value"), true, out buffClass.DisplayValueFormat))
				{
					buffClass.DisplayValueFormat = BuffClass.CVarDisplayFormat.None;
				}
				if (xelement.Name == "damage_source" && xelement.HasAttribute("value"))
				{
					buffClass.DamageSource = EnumUtils.Parse<EnumDamageSource>(xelement.GetAttribute("value"), true);
				}
				if (xelement.Name == "damage_type" && xelement.HasAttribute("value"))
				{
					buffClass.DamageType = EnumUtils.Parse<EnumDamageTypes>(xelement.GetAttribute("value"), true);
				}
				if (xelement.Name == "stack_type" && xelement.HasAttribute("value"))
				{
					buffClass.StackType = EnumUtils.Parse<BuffEffectStackTypes>(xelement.GetAttribute("value"), true);
				}
				if (xelement.Name == "tags" && xelement.HasAttribute("value"))
				{
					buffClass.Tags = FastTags<TagGroup.Global>.Parse(xelement.GetAttribute("value"));
				}
				if (xelement.Name == "cures")
				{
					if (xelement.HasAttribute("value"))
					{
						buffClass.Cures = new List<string>(xelement.GetAttribute("value").Split(',', StringSplitOptions.None));
					}
					else
					{
						buffClass.Cures = new List<string>();
					}
				}
				else
				{
					buffClass.Cures = new List<string>();
				}
				if (xelement.Name == "duration" && xelement.HasAttribute("value"))
				{
					buffClass.DurationMax = StringParsers.ParseFloat(xelement.GetAttribute("value"), 0, -1, NumberStyles.Any);
				}
				if (xelement.Name == "update_rate" && xelement.HasAttribute("value"))
				{
					buffClass.UpdateRate = StringParsers.ParseFloat(xelement.GetAttribute("value"), 0, -1, NumberStyles.Any);
				}
				if (xelement.Name == "remove_on_death" && xelement.HasAttribute("value"))
				{
					buffClass.RemoveOnDeath = StringParsers.ParseBool(xelement.GetAttribute("value"), 0, -1, true);
				}
				if (xelement.Name == "requirement")
				{
					IRequirement requirement = RequirementBase.ParseRequirement(_element);
					if (requirement != null)
					{
						buffClass.Requirements.Add(requirement);
					}
				}
				if (xelement.Name == "requirements")
				{
					BuffsFromXml.parseBuffRequirements(buffClass, xelement);
				}
			}
			buffClass.Effects = MinEffectController.ParseXml(_element, null, MinEffectController.SourceParentType.BuffClass, buffClass.Name);
			BuffManager.AddBuff(buffClass);
			return;
		}
		throw new Exception("buff must have an name!");
	}

	// Token: 0x06005B9F RID: 23455 RVA: 0x0024C1D0 File Offset: 0x0024A3D0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void parseBuffRequirements(BuffClass _buff, XElement _element)
	{
		if (_element.HasAttribute("compare_type") && _element.GetAttribute("compare_type").EqualsCaseInsensitive("or"))
		{
			_buff.OrCompare = true;
		}
		foreach (XElement xelement in _element.Elements("requirement"))
		{
			IRequirement requirement = RequirementBase.ParseRequirement(_element);
			if (requirement != null)
			{
				_buff.Requirements.Add(requirement);
			}
		}
	}
}
