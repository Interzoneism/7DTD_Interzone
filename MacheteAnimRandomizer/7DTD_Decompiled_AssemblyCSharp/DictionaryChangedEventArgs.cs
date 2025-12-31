using System;

// Token: 0x020011D6 RID: 4566
public class DictionaryChangedEventArgs<TKey, TValue> : EventArgs
{
	// Token: 0x17000EC5 RID: 3781
	// (get) Token: 0x06008E90 RID: 36496 RVA: 0x003918A7 File Offset: 0x0038FAA7
	public TKey Key { get; }

	// Token: 0x17000EC6 RID: 3782
	// (get) Token: 0x06008E91 RID: 36497 RVA: 0x003918AF File Offset: 0x0038FAAF
	public TValue Value { get; }

	// Token: 0x17000EC7 RID: 3783
	// (get) Token: 0x06008E92 RID: 36498 RVA: 0x003918B7 File Offset: 0x0038FAB7
	public string Action { get; }

	// Token: 0x06008E93 RID: 36499 RVA: 0x003918BF File Offset: 0x0038FABF
	public DictionaryChangedEventArgs(TKey key, TValue value, string action)
	{
		this.Key = key;
		this.Value = value;
		this.Action = action;
	}
}
