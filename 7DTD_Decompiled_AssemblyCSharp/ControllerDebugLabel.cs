using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using InControl;
using UnityEngine;

// Token: 0x02000F36 RID: 3894
public class ControllerDebugLabel : MonoBehaviour
{
	// Token: 0x06007BFA RID: 31738 RVA: 0x00322F94 File Offset: 0x00321194
	public ControllerDebugLabel()
	{
		this.m_debugStringProviders = new SortedDictionary<string, Action<StringBuilder>>();
		this.m_debugStringProviderNames = new List<string>();
		this.m_debugStringBuilderMain = new StringBuilder(4096);
		this.m_debugStringBuilderForProvider = new StringBuilder(4096);
		this.m_debugStringBuilderForControlsString = new StringBuilder(512);
	}

	// Token: 0x06007BFB RID: 31739 RVA: 0x00323021 File Offset: 0x00321221
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.m_label = base.GetComponent<UILabel>();
		this.AddDebugProvider("Key Codes", new Action<StringBuilder>(this.BuildKeyCodeString));
		this.AddDebugProvider("Devices", new Action<StringBuilder>(this.BuildDevicesString));
	}

	// Token: 0x06007BFC RID: 31740 RVA: 0x0032305D File Offset: 0x0032125D
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.RemoveDebugProvider("Key Codes");
		this.RemoveDebugProvider("Devices");
	}

	// Token: 0x06007BFD RID: 31741 RVA: 0x00323078 File Offset: 0x00321278
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.m_label == null)
		{
			return;
		}
		this.m_debugStringBuilderMain.Clear();
		for (int i = 0; i < this.m_debugStringProviderNames.Count; i++)
		{
			string text = this.m_debugStringProviderNames[i];
			Action<StringBuilder> action = this.m_debugStringProviders[text];
			this.m_debugStringBuilderForProvider.Clear();
			try
			{
				action(this.m_debugStringBuilderForProvider);
			}
			catch (Exception ex)
			{
				this.m_debugStringBuilderForProvider.Clear();
				this.m_debugStringBuilderForProvider.Append(ex.Message);
			}
			if (this.m_debugStringBuilderForProvider.Length != 0)
			{
				if (this.m_debugStringBuilderMain.Length != 0)
				{
					this.m_debugStringBuilderMain.Append('\n');
				}
				this.m_debugStringBuilderMain.Append(text);
				this.m_debugStringBuilderMain.Append(": ");
				for (int j = 0; j < this.m_debugStringBuilderForProvider.Length; j++)
				{
					char c = this.m_debugStringBuilderForProvider[j];
					this.m_debugStringBuilderMain.Append(c);
					if (c == '\n')
					{
						for (int k = 0; k < text.Length + 2; k++)
						{
							this.m_debugStringBuilderMain.Append(' ');
						}
					}
				}
			}
		}
		if (!this.m_debugStringBuilderMain.Equals(this.m_label.text))
		{
			this.m_label.text = this.m_debugStringBuilderMain.ToString();
		}
	}

	// Token: 0x06007BFE RID: 31742 RVA: 0x00323200 File Offset: 0x00321400
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateProviderNames()
	{
		this.m_debugStringProviderNames.Clear();
		this.m_debugStringProviderNames.AddRange(this.m_debugStringProviders.Keys);
	}

	// Token: 0x06007BFF RID: 31743 RVA: 0x00323223 File Offset: 0x00321423
	public void AddDebugProvider(string providerName, Action<StringBuilder> provider)
	{
		bool flag = !this.m_debugStringProviders.ContainsKey(providerName);
		this.m_debugStringProviders[providerName] = provider;
		if (flag)
		{
			this.UpdateProviderNames();
		}
	}

	// Token: 0x06007C00 RID: 31744 RVA: 0x00323249 File Offset: 0x00321449
	public void RemoveDebugProvider(string providerName)
	{
		if (this.m_debugStringProviders.Remove(providerName))
		{
			this.UpdateProviderNames();
		}
	}

	// Token: 0x06007C01 RID: 31745 RVA: 0x00323260 File Offset: 0x00321460
	[PublicizedFrom(EAccessModifier.Private)]
	public void BuildKeyCodeString(StringBuilder builder)
	{
		foreach (KeyCode keyCode in this.m_allKeyCodes)
		{
			if (Input.GetKey(keyCode))
			{
				if (builder.Length != 0)
				{
					builder.Append(" + ");
				}
				builder.Append(keyCode.ToStringCached<KeyCode>());
			}
		}
	}

	// Token: 0x06007C02 RID: 31746 RVA: 0x003232B0 File Offset: 0x003214B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BuildDevicesString(StringBuilder builder)
	{
		ReadOnlyCollection<InputDevice> devices = InputManager.Devices;
		for (int i = 0; i < devices.Count; i++)
		{
			InputDevice inputDevice = devices[i];
			if (inputDevice.IsActive)
			{
				this.m_debugStringBuilderForControlsString.Clear();
				this.BuildControlsString(this.m_debugStringBuilderForControlsString, inputDevice);
				if (this.m_debugStringBuilderForControlsString.Length != 0)
				{
					if (builder.Length != 0)
					{
						builder.Append('\n');
					}
					builder.Append(this.m_debugStringBuilderForControlsString);
				}
			}
		}
	}

	// Token: 0x06007C03 RID: 31747 RVA: 0x00323328 File Offset: 0x00321528
	[PublicizedFrom(EAccessModifier.Private)]
	public void BuildControlsString(StringBuilder builder, InputDevice device)
	{
		bool flag = false;
		builder.Append(device.Name);
		builder.Append(" (");
		builder.Append(device.Meta);
		builder.Append("): ");
		foreach (InputControlType inputControlType in this.m_allControls)
		{
			if (inputControlType != InputControlType.None && inputControlType != InputControlType.Count)
			{
				InputControl control = device.GetControl(inputControlType);
				if (control.IsPressed || control.RawValue != 0f)
				{
					flag = true;
					if (builder.Length != 0)
					{
						builder.Append(" + ");
					}
					builder.Append(inputControlType.ToStringCached<InputControlType>());
					if (control.IsAnalog)
					{
						builder.AppendFormat("={0:F4}", control.RawValue);
					}
				}
			}
		}
		if (!flag)
		{
			builder.Clear();
		}
	}

	// Token: 0x04005ED1 RID: 24273
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string KEY_DEVICES = "Devices";

	// Token: 0x04005ED2 RID: 24274
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const string KEY_KEY_CODES = "Key Codes";

	// Token: 0x04005ED3 RID: 24275
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly SortedDictionary<string, Action<StringBuilder>> m_debugStringProviders;

	// Token: 0x04005ED4 RID: 24276
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly List<string> m_debugStringProviderNames;

	// Token: 0x04005ED5 RID: 24277
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly StringBuilder m_debugStringBuilderMain;

	// Token: 0x04005ED6 RID: 24278
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly StringBuilder m_debugStringBuilderForProvider;

	// Token: 0x04005ED7 RID: 24279
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly StringBuilder m_debugStringBuilderForControlsString;

	// Token: 0x04005ED8 RID: 24280
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UILabel m_label;

	// Token: 0x04005ED9 RID: 24281
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly KeyCode[] m_allKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

	// Token: 0x04005EDA RID: 24282
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly InputControlType[] m_allControls = (InputControlType[])Enum.GetValues(typeof(InputControlType));
}
