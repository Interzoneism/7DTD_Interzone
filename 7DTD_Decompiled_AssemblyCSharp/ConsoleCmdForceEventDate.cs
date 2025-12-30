using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020001E2 RID: 482
[Preserve]
public class ConsoleCmdForceEventDate : ConsoleCmdAbstract
{
	// Token: 0x06000E61 RID: 3681 RVA: 0x0005E168 File Offset: 0x0005C368
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"ForceEventDate"
		};
	}

	// Token: 0x17000137 RID: 311
	// (get) Token: 0x06000E62 RID: 3682 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x06000E63 RID: 3683 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x0005E178 File Offset: 0x0005C378
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Specify date for testing event dates";
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x0005E180 File Offset: 0x0005C380
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count < 1)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Current forced date: " + ((EventsFromXml.ForceTestDateTime == DateTime.MinValue) ? "-none-" : EventsFromXml.ForceTestDateTime.ToShortDateString()));
			return;
		}
		string text = _params[0];
		DateTime minValue;
		if (text == "now")
		{
			minValue = DateTime.MinValue;
		}
		else if (!EventsFromXml.TryParseDate(text, out minValue))
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Failed parsing date argument, must be in the form 'mm/dd'");
			return;
		}
		EventsFromXml.ForceTestDateTime = minValue;
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Forced date: " + minValue.ToShortDateString());
		foreach (KeyValuePair<string, EventsFromXml.EventDefinition> keyValuePair in EventsFromXml.Events)
		{
			string text2;
			EventsFromXml.EventDefinition eventDefinition;
			keyValuePair.Deconstruct(out text2, out eventDefinition);
			EventsFromXml.EventDefinition eventDefinition2 = eventDefinition;
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(string.Format("Name={0}, Start={1}, End={2}, Active={3}", new object[]
			{
				eventDefinition2.Name,
				eventDefinition2.Start,
				eventDefinition2.End,
				eventDefinition2.Active
			}));
		}
	}
}
