using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x020017D2 RID: 6098
	public static class EPlayGroupExtensions
	{
		// Token: 0x0600B5FD RID: 46589 RVA: 0x00466E49 File Offset: 0x00465049
		public static bool IsCurrent(this EPlayGroup group)
		{
			return group == EPlayGroupExtensions.Current;
		}

		// Token: 0x0600B5FE RID: 46590 RVA: 0x00466E53 File Offset: 0x00465053
		[PublicizedFrom(EAccessModifier.Private)]
		public static EPlayGroup GetCurrentPlayGroup()
		{
			if (DeviceFlag.PS5.IsCurrent())
			{
				return EPlayGroup.PS5;
			}
			if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX).IsCurrent())
			{
				return EPlayGroup.XBS;
			}
			return EPlayGroup.Standalone;
		}

		// Token: 0x0600B5FF RID: 46591 RVA: 0x00466E6C File Offset: 0x0046506C
		public static EPlayGroup ToPlayGroup(this DeviceFlag device)
		{
			if (device <= DeviceFlag.XBoxSeriesS)
			{
				switch (device)
				{
				case DeviceFlag.StandaloneWindows:
					return EPlayGroup.Standalone;
				case DeviceFlag.StandaloneLinux:
					return EPlayGroup.Standalone;
				case DeviceFlag.StandaloneWindows | DeviceFlag.StandaloneLinux:
					break;
				case DeviceFlag.StandaloneOSX:
					return EPlayGroup.Standalone;
				default:
					if (device == DeviceFlag.XBoxSeriesS)
					{
						return EPlayGroup.XBS;
					}
					break;
				}
			}
			else
			{
				if (device == DeviceFlag.XBoxSeriesX)
				{
					return EPlayGroup.XBS;
				}
				if (device == DeviceFlag.PS5)
				{
					return EPlayGroup.PS5;
				}
			}
			throw new ArgumentOutOfRangeException("device", device, string.Format("Missing play group mapping for {0}.", device));
		}

		// Token: 0x0600B600 RID: 46592 RVA: 0x00466EE4 File Offset: 0x004650E4
		public static EPlayGroup ToPlayGroup(this ClientInfo.EDeviceType deviceType)
		{
			EPlayGroup result;
			switch (deviceType)
			{
			case ClientInfo.EDeviceType.Linux:
				result = EPlayGroup.Standalone;
				break;
			case ClientInfo.EDeviceType.Mac:
				result = EPlayGroup.Standalone;
				break;
			case ClientInfo.EDeviceType.Windows:
				result = EPlayGroup.Standalone;
				break;
			case ClientInfo.EDeviceType.PlayStation:
				result = EPlayGroup.PS5;
				break;
			case ClientInfo.EDeviceType.Xbox:
				result = EPlayGroup.XBS;
				break;
			case ClientInfo.EDeviceType.Unknown:
				result = EPlayGroup.Standalone;
				break;
			default:
				throw new ArgumentOutOfRangeException("deviceType", deviceType, string.Format("Missing play group mapping for {0}.", deviceType));
			}
			return result;
		}

		// Token: 0x0600B601 RID: 46593 RVA: 0x00466F4B File Offset: 0x0046514B
		public static uint[] GetCurrentlyAllowedPlatformIds()
		{
			if (PermissionsManager.IsCrossplayAllowed())
			{
				return null;
			}
			return EPlayGroupExtensions.s_playGroupToAllowedPlatformIds[EPlayGroupExtensions.Current];
		}

		// Token: 0x04008F69 RID: 36713
		public static readonly EPlayGroup Current = EPlayGroupExtensions.GetCurrentPlayGroup();

		// Token: 0x04008F6A RID: 36714
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<EPlayGroup, uint[]> s_playGroupToAllowedPlatformIds = new Dictionary<EPlayGroup, uint[]>
		{
			{
				EPlayGroup.Standalone,
				null
			},
			{
				EPlayGroup.XBS,
				null
			},
			{
				EPlayGroup.PS5,
				null
			}
		};
	}
}
