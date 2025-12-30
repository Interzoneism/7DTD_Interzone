using System;
using Platform;

// Token: 0x02000ED9 RID: 3801
public static class UIOptions
{
	// Token: 0x17000C27 RID: 3111
	// (get) Token: 0x060077F9 RID: 30713 RVA: 0x0030D9DF File Offset: 0x0030BBDF
	// (set) Token: 0x060077FA RID: 30714 RVA: 0x0030D9E6 File Offset: 0x0030BBE6
	public static OptionsVideoWindowMode OptionsVideoWindow
	{
		get
		{
			return UIOptions.optionsVideoWindow;
		}
		set
		{
			UIOptions.optionsVideoWindow = value;
			Action<OptionsVideoWindowMode> onOptionsVideoWindowChanged = UIOptions.OnOptionsVideoWindowChanged;
			if (onOptionsVideoWindowChanged == null)
			{
				return;
			}
			onOptionsVideoWindowChanged(value);
		}
	}

	// Token: 0x140000D7 RID: 215
	// (add) Token: 0x060077FB RID: 30715 RVA: 0x0030DA00 File Offset: 0x0030BC00
	// (remove) Token: 0x060077FC RID: 30716 RVA: 0x0030DA34 File Offset: 0x0030BC34
	public static event Action<OptionsVideoWindowMode> OnOptionsVideoWindowChanged;

	// Token: 0x060077FD RID: 30717 RVA: 0x0030DA67 File Offset: 0x0030BC67
	public static void Init()
	{
		UIOptions.optionsVideoWindow = ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent() ? OptionsVideoWindowMode.Simplified : OptionsVideoWindowMode.Detailed);
	}

	// Token: 0x04005B82 RID: 23426
	[PublicizedFrom(EAccessModifier.Private)]
	public static OptionsVideoWindowMode optionsVideoWindow;
}
