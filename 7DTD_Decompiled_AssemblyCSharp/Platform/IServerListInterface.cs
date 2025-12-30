using System;
using System.Collections.Generic;

namespace Platform
{
	// Token: 0x02001821 RID: 6177
	public interface IServerListInterface
	{
		// Token: 0x170014CE RID: 5326
		// (get) Token: 0x0600B7BD RID: 47037
		bool IsPrefiltered { get; }

		// Token: 0x0600B7BE RID: 47038
		void Init(IPlatform _owner);

		// Token: 0x0600B7BF RID: 47039
		void RegisterGameServerFoundCallback(GameServerFoundCallback _serverFound, MaxResultsReachedCallback _maxResultsCallback, ServerSearchErrorCallback _sessionSearchErrorCallback);

		// Token: 0x170014CF RID: 5327
		// (get) Token: 0x0600B7C0 RID: 47040
		bool IsRefreshing { get; }

		// Token: 0x0600B7C1 RID: 47041
		void StartSearch(IList<IServerListInterface.ServerFilter> _activeFilters);

		// Token: 0x0600B7C2 RID: 47042
		void StopSearch();

		// Token: 0x0600B7C3 RID: 47043
		void Disconnect();

		// Token: 0x0600B7C4 RID: 47044
		void GetSingleServerDetails(GameServerInfo _serverInfo, EServerRelationType _relation, GameServerFoundCallback _callback);

		// Token: 0x02001822 RID: 6178
		public class ServerFilter
		{
			// Token: 0x0600B7C5 RID: 47045 RVA: 0x00468874 File Offset: 0x00466A74
			public ServerFilter(string _name, IServerListInterface.ServerFilter.EServerFilterType _type = IServerListInterface.ServerFilter.EServerFilterType.Any, int _intMinValue = 0, int _intMaxValue = 0, bool _boolValue = false, string _stringNeedle = null)
			{
				this.Name = _name;
				this.Type = _type;
				this.IntMinValue = _intMinValue;
				this.IntMaxValue = _intMaxValue;
				this.BoolValue = _boolValue;
				this.StringNeedle = _stringNeedle;
			}

			// Token: 0x04008FF7 RID: 36855
			public readonly string Name;

			// Token: 0x04008FF8 RID: 36856
			public readonly IServerListInterface.ServerFilter.EServerFilterType Type;

			// Token: 0x04008FF9 RID: 36857
			public readonly int IntMinValue;

			// Token: 0x04008FFA RID: 36858
			public readonly int IntMaxValue;

			// Token: 0x04008FFB RID: 36859
			public readonly bool BoolValue;

			// Token: 0x04008FFC RID: 36860
			public readonly string StringNeedle;

			// Token: 0x02001823 RID: 6179
			public enum EServerFilterType
			{
				// Token: 0x04008FFE RID: 36862
				Any,
				// Token: 0x04008FFF RID: 36863
				BoolValue,
				// Token: 0x04009000 RID: 36864
				IntValue,
				// Token: 0x04009001 RID: 36865
				IntNotValue,
				// Token: 0x04009002 RID: 36866
				IntMin,
				// Token: 0x04009003 RID: 36867
				IntMax,
				// Token: 0x04009004 RID: 36868
				IntRange,
				// Token: 0x04009005 RID: 36869
				StringValue,
				// Token: 0x04009006 RID: 36870
				StringContains
			}
		}
	}
}
