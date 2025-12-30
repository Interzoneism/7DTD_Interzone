using System;
using System.Text;

// Token: 0x02000EDF RID: 3807
public class XUiM_InGameService : XUiModel
{
	// Token: 0x06007815 RID: 30741 RVA: 0x0030E3E8 File Offset: 0x0030C5E8
	public static string GetServiceStats(XUi _xui, InGameService service)
	{
		if (service == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (service.ServiceType == InGameService.InGameServiceTypes.VendingRent)
		{
			TileEntityVendingMachine tileEntityVendingMachine = _xui.Trader.TraderTileEntity as TileEntityVendingMachine;
			stringBuilder.Append(XUiM_InGameService.StringFormatHandler(Localization.Get("xuiCost", false), tileEntityVendingMachine.TraderData.TraderInfo.RentCost));
			stringBuilder.Append(XUiM_InGameService.StringFormatHandler(Localization.Get("xuiGameTime", false), tileEntityVendingMachine.TraderData.TraderInfo.RentTimeInDays, Localization.Get("xuiGameDays", false)));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06007816 RID: 30742 RVA: 0x0011007A File Offset: 0x0010E27A
	[PublicizedFrom(EAccessModifier.Private)]
	public static string StringFormatHandler(string title, object value)
	{
		return string.Format("{0}: [REPLACE_COLOR]{1}[-]\n", title, value);
	}

	// Token: 0x06007817 RID: 30743 RVA: 0x0030E486 File Offset: 0x0030C686
	[PublicizedFrom(EAccessModifier.Private)]
	public static string StringFormatHandler(string title, object value, string units)
	{
		return string.Format("{0}: [REPLACE_COLOR]{1} {2}[-]\n", title, value, units);
	}
}
