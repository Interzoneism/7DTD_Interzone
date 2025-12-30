using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Challenges;

// Token: 0x02000B92 RID: 2962
public class ChallengesFromXml
{
	// Token: 0x06005BA8 RID: 23464 RVA: 0x0024C3E8 File Offset: 0x0024A5E8
	public static IEnumerator CreateChallenges(XmlFile xmlFile)
	{
		ChallengeClass.s_Challenges.Clear();
		ChallengeGroup.s_ChallengeGroups.Clear();
		ChallengeCategory.s_ChallengeCategories.Clear();
		XElement root = xmlFile.XmlDoc.Root;
		if (!root.HasElements)
		{
			throw new Exception("No element <challenges> found!");
		}
		ChallengesFromXml.ParseNode(root);
		ChallengesFromXml.LastGroupChallenge.Clear();
		ChallengeClass.InitChallenges();
		yield break;
	}

	// Token: 0x06005BA9 RID: 23465 RVA: 0x0024C3F8 File Offset: 0x0024A5F8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseNode(XElement root)
	{
		if (root.HasAttribute("default_reward"))
		{
			ChallengesFromXml.DefaultRewardEvent = root.GetAttribute("default_reward");
		}
		if (root.HasAttribute("default_reward_text_key"))
		{
			ChallengesFromXml.DefaultRewardText = Localization.Get(root.GetAttribute("default_reward_text_key"), false);
		}
		else if (root.HasAttribute("default_reward_text"))
		{
			ChallengesFromXml.DefaultRewardText = root.GetAttribute("default_reward_text");
		}
		foreach (XElement xelement in root.Elements())
		{
			if (xelement.Name == "challenge")
			{
				ChallengesFromXml.ParseChallenge(xelement);
			}
			else if (xelement.Name == "challenge_group")
			{
				ChallengesFromXml.ParseChallengeGroup(xelement);
			}
			else
			{
				if (!(xelement.Name == "challenge_category"))
				{
					string str = "Unrecognized xml element ";
					XName name = xelement.Name;
					throw new Exception(str + ((name != null) ? name.ToString() : null));
				}
				ChallengesFromXml.ParseChallengeCategory(xelement);
			}
		}
	}

	// Token: 0x06005BAA RID: 23466 RVA: 0x0024C540 File Offset: 0x0024A740
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseChallengeGroup(XElement e)
	{
		DynamicProperties dynamicProperties = null;
		if (!e.HasAttribute("name"))
		{
			throw new Exception("challenge group must have an name attribute");
		}
		string attribute = e.GetAttribute("name");
		ChallengeGroup challengeGroup = ChallengeGroup.NewClass(attribute);
		if (challengeGroup == null)
		{
			throw new Exception("Challenge group with an id of '" + attribute + "' already exists!");
		}
		challengeGroup.ParseElement(e);
		foreach (XElement xelement in e.Elements())
		{
			if (xelement.Name == "property")
			{
				if (dynamicProperties == null)
				{
					dynamicProperties = new DynamicProperties();
				}
				dynamicProperties.Add(xelement, true, false);
			}
			else if (xelement.Name == "challenge_count")
			{
				if (!xelement.HasAttribute("tags"))
				{
					throw new Exception("Challenge count for group '" + attribute + "' does not contain tags!");
				}
				if (!xelement.HasAttribute("count"))
				{
					throw new Exception("Challenge count for group '" + attribute + "' does not contain count!");
				}
				challengeGroup.AddChallengeCount(xelement.GetAttribute("tags"), StringParsers.ParseSInt32(xelement.GetAttribute("count"), 0, -1, NumberStyles.Integer));
			}
		}
		challengeGroup.Effects = MinEffectController.ParseXml(e, null, MinEffectController.SourceParentType.ChallengeGroup, challengeGroup.Name);
	}

	// Token: 0x06005BAB RID: 23467 RVA: 0x0024C6C8 File Offset: 0x0024A8C8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseChallengeCategory(XElement e)
	{
		if (!e.HasAttribute("name"))
		{
			throw new Exception("challenge category must have an name attribute");
		}
		string attribute = e.GetAttribute("name");
		if (ChallengeCategory.s_ChallengeCategories.ContainsKey(attribute))
		{
			throw new Exception("Challenge group with an id of '" + attribute + "' already exists!");
		}
		ChallengeCategory challengeCategory = new ChallengeCategory(attribute);
		challengeCategory.ParseElement(e);
		ChallengeCategory.s_ChallengeCategories.Add(attribute, challengeCategory);
	}

	// Token: 0x06005BAC RID: 23468 RVA: 0x0024C740 File Offset: 0x0024A940
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseChallenge(XElement e)
	{
		DynamicProperties dynamicProperties = null;
		if (!e.HasAttribute("name"))
		{
			throw new Exception("challenge must have an name attribute");
		}
		string attribute = e.GetAttribute("name");
		ChallengeClass challengeClass = ChallengeClass.NewClass(attribute);
		if (challengeClass == null)
		{
			throw new Exception("Challenge with an id of '" + attribute + "' already exists!");
		}
		challengeClass.ParseElement(e);
		ChallengeGroup challengeGroup = challengeClass.ChallengeGroup;
		if (challengeGroup.LinkChallenges)
		{
			if (ChallengesFromXml.LastGroupChallenge.ContainsKey(challengeGroup))
			{
				ChallengesFromXml.LastGroupChallenge[challengeGroup].NextChallenge = challengeClass;
			}
			ChallengesFromXml.LastGroupChallenge[challengeGroup] = challengeClass;
		}
		foreach (XElement xelement in e.Elements())
		{
			if (xelement.Name == "property")
			{
				if (dynamicProperties == null)
				{
					dynamicProperties = new DynamicProperties();
				}
				dynamicProperties.Add(xelement, true, false);
			}
			else if (xelement.Name == "objective")
			{
				ChallengesFromXml.ParseObjective(challengeClass, xelement);
			}
		}
		challengeClass.Effects = MinEffectController.ParseXml(e, null, MinEffectController.SourceParentType.ChallengeClass, challengeClass.Name);
	}

	// Token: 0x06005BAD RID: 23469 RVA: 0x0024C880 File Offset: 0x0024AA80
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseObjective(ChallengeClass Challenge, XElement e)
	{
		if (!e.HasAttribute("type"))
		{
			throw new Exception("Objective must have a type!");
		}
		BaseChallengeObjective baseChallengeObjective = null;
		string attribute = e.GetAttribute("type");
		try
		{
			baseChallengeObjective = (BaseChallengeObjective)Activator.CreateInstance(ReflectionHelpers.GetTypeWithPrefix("Challenges.ChallengeObjective", attribute));
			Challenge.AddObjective(baseChallengeObjective);
		}
		catch (Exception innerException)
		{
			throw new Exception("No objective class '" + attribute + " found!", innerException);
		}
		if (baseChallengeObjective != null)
		{
			baseChallengeObjective.ParseElement(e);
			baseChallengeObjective.Init();
		}
	}

	// Token: 0x04004600 RID: 17920
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<ChallengeGroup, ChallengeClass> LastGroupChallenge = new Dictionary<ChallengeGroup, ChallengeClass>();

	// Token: 0x04004601 RID: 17921
	public static string DefaultRewardEvent;

	// Token: 0x04004602 RID: 17922
	public static string DefaultRewardText;
}
