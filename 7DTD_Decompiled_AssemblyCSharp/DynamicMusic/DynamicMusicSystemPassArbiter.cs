using System;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001770 RID: 6000
	[Preserve]
	public struct DynamicMusicSystemPassArbiter : IPassArbiter, IGamePrefsChangedListener, IPauseable
	{
		// Token: 0x17001418 RID: 5144
		// (get) Token: 0x0600B3DF RID: 46047 RVA: 0x00459F63 File Offset: 0x00458163
		public bool WillAllowPass
		{
			get
			{
				return this.BoolContainer.Equals(224);
			}
		}

		// Token: 0x17001419 RID: 5145
		// (set) Token: 0x0600B3E0 RID: 46048 RVA: 0x00459F75 File Offset: 0x00458175
		public bool DoesPlayerExist
		{
			set
			{
				this.SetBoolContainer(value, 64);
			}
		}

		// Token: 0x1700141A RID: 5146
		// (set) Token: 0x0600B3E1 RID: 46049 RVA: 0x00459F80 File Offset: 0x00458180
		public bool IsGameUnPaused
		{
			[PublicizedFrom(EAccessModifier.Private)]
			set
			{
				this.SetBoolContainer(value, 32);
			}
		}

		// Token: 0x1700141B RID: 5147
		// (set) Token: 0x0600B3E2 RID: 46050 RVA: 0x00459F8B File Offset: 0x0045818B
		public bool IsDynamicMusicEnabled
		{
			set
			{
				this.SetBoolContainer(value, 128);
			}
		}

		// Token: 0x0600B3E3 RID: 46051 RVA: 0x00459F99 File Offset: 0x00458199
		public DynamicMusicSystemPassArbiter(bool _enabled)
		{
			this.BoolContainer = 0;
			this.IsDynamicMusicEnabled = _enabled;
			GamePrefs.AddChangeListener(this);
		}

		// Token: 0x0600B3E4 RID: 46052 RVA: 0x00459FB9 File Offset: 0x004581B9
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetBoolContainer(bool _value, byte _place)
		{
			if (_value)
			{
				this.BoolContainer |= _place;
				return;
			}
			this.BoolContainer &= ~_place;
		}

		// Token: 0x0600B3E5 RID: 46053 RVA: 0x00459FDF File Offset: 0x004581DF
		public void OnGamePrefChanged(EnumGamePrefs _enum)
		{
			if (_enum == EnumGamePrefs.OptionsDynamicMusicEnabled)
			{
				this.IsDynamicMusicEnabled = GamePrefs.GetBool(_enum);
			}
		}

		// Token: 0x0600B3E6 RID: 46054 RVA: 0x00459FF5 File Offset: 0x004581F5
		public void OnPause()
		{
			this.IsGameUnPaused = false;
		}

		// Token: 0x0600B3E7 RID: 46055 RVA: 0x00459FFE File Offset: 0x004581FE
		public void OnUnPause()
		{
			this.IsGameUnPaused = true;
		}

		// Token: 0x04008C81 RID: 35969
		[PublicizedFrom(EAccessModifier.Private)]
		public byte BoolContainer;
	}
}
