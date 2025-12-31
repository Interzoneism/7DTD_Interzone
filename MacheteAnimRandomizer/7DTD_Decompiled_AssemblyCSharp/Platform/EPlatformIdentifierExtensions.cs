using System;

namespace Platform
{
	// Token: 0x020017D0 RID: 6096
	public static class EPlatformIdentifierExtensions
	{
		// Token: 0x0600B5F9 RID: 46585 RVA: 0x00466CA0 File Offset: 0x00464EA0
		public static bool IsNative(this EPlatformIdentifier platformIdentifier)
		{
			bool result;
			switch (platformIdentifier)
			{
			case EPlatformIdentifier.None:
				result = false;
				break;
			case EPlatformIdentifier.Local:
				result = true;
				break;
			case EPlatformIdentifier.EOS:
				result = false;
				break;
			case EPlatformIdentifier.Steam:
				result = true;
				break;
			case EPlatformIdentifier.XBL:
				result = true;
				break;
			case EPlatformIdentifier.PSN:
				result = true;
				break;
			case EPlatformIdentifier.EGS:
				result = true;
				break;
			case EPlatformIdentifier.LAN:
				result = false;
				break;
			case EPlatformIdentifier.Count:
				result = false;
				break;
			default:
				throw new ArgumentOutOfRangeException("platformIdentifier", platformIdentifier, null);
			}
			return result;
		}

		// Token: 0x0600B5FA RID: 46586 RVA: 0x00466D10 File Offset: 0x00464F10
		public static bool IsCross(this EPlatformIdentifier platformIdentifier)
		{
			bool result;
			switch (platformIdentifier)
			{
			case EPlatformIdentifier.None:
				result = true;
				break;
			case EPlatformIdentifier.Local:
				result = false;
				break;
			case EPlatformIdentifier.EOS:
				result = true;
				break;
			case EPlatformIdentifier.Steam:
				result = false;
				break;
			case EPlatformIdentifier.XBL:
				result = false;
				break;
			case EPlatformIdentifier.PSN:
				result = false;
				break;
			case EPlatformIdentifier.EGS:
				result = false;
				break;
			case EPlatformIdentifier.LAN:
				result = false;
				break;
			case EPlatformIdentifier.Count:
				result = false;
				break;
			default:
				throw new ArgumentOutOfRangeException("platformIdentifier", platformIdentifier, null);
			}
			return result;
		}

		// Token: 0x0600B5FB RID: 46587 RVA: 0x00466D80 File Offset: 0x00464F80
		public static bool IsServer(this EPlatformIdentifier platformIdentifier)
		{
			bool result;
			switch (platformIdentifier)
			{
			case EPlatformIdentifier.None:
				result = false;
				break;
			case EPlatformIdentifier.Local:
				result = false;
				break;
			case EPlatformIdentifier.EOS:
				result = false;
				break;
			case EPlatformIdentifier.Steam:
				result = true;
				break;
			case EPlatformIdentifier.XBL:
				result = true;
				break;
			case EPlatformIdentifier.PSN:
				result = true;
				break;
			case EPlatformIdentifier.EGS:
				result = true;
				break;
			case EPlatformIdentifier.LAN:
				result = true;
				break;
			case EPlatformIdentifier.Count:
				result = false;
				break;
			default:
				throw new ArgumentOutOfRangeException("platformIdentifier", platformIdentifier, null);
			}
			return result;
		}

		// Token: 0x0600B5FC RID: 46588 RVA: 0x00466DF0 File Offset: 0x00464FF0
		public static bool IsServerValid(this EPlatformIdentifier serverPlatform, EPlatformIdentifier nativePlatform, EPlatformIdentifier crossPlatform)
		{
			bool flag = serverPlatform.IsServer();
			if (flag)
			{
				bool flag2;
				switch (serverPlatform)
				{
				case EPlatformIdentifier.Steam:
					flag2 = true;
					break;
				case EPlatformIdentifier.XBL:
				case EPlatformIdentifier.PSN:
				case EPlatformIdentifier.EGS:
					flag2 = (crossPlatform == EPlatformIdentifier.EOS);
					break;
				case EPlatformIdentifier.LAN:
					flag2 = true;
					break;
				default:
					throw new ArgumentOutOfRangeException("serverPlatform", serverPlatform, null);
				}
				flag = flag2;
			}
			return flag;
		}
	}
}
