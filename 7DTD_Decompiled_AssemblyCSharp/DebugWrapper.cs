using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02001160 RID: 4448
public abstract class DebugWrapper
{
	// Token: 0x06008B3C RID: 35644 RVA: 0x00383F8C File Offset: 0x0038218C
	[PublicizedFrom(EAccessModifier.Protected)]
	public DebugWrapper(DebugWrapper parent)
	{
		this.m_readCounter = (((parent != null) ? parent.m_readCounter : null) ?? new AtomicCounter());
		this.m_writeCounter = (((parent != null) ? parent.m_writeCounter : null) ?? new AtomicCounter());
		this.DebugNonMainThreadAccess = (parent != null && parent.DebugNonMainThreadAccess);
		this.DebugConcurrentModifications = (parent != null && parent.DebugConcurrentModifications);
	}

	// Token: 0x17000E80 RID: 3712
	// (get) Token: 0x06008B3D RID: 35645 RVA: 0x00383FF9 File Offset: 0x003821F9
	// (set) Token: 0x06008B3E RID: 35646 RVA: 0x00384001 File Offset: 0x00382201
	public bool DebugNonMainThreadAccess { get; set; }

	// Token: 0x17000E81 RID: 3713
	// (get) Token: 0x06008B3F RID: 35647 RVA: 0x0038400A File Offset: 0x0038220A
	// (set) Token: 0x06008B40 RID: 35648 RVA: 0x00384012 File Offset: 0x00382212
	public bool DebugConcurrentModifications { get; set; }

	// Token: 0x17000E82 RID: 3714
	// (get) Token: 0x06008B41 RID: 35649 RVA: 0x0038401B File Offset: 0x0038221B
	public bool NeedsScope
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.DebugConcurrentModifications;
		}
	}

	// Token: 0x06008B42 RID: 35650 RVA: 0x00384023 File Offset: 0x00382223
	[PublicizedFrom(EAccessModifier.Protected)]
	public IDisposable DebugReadScope()
	{
		if (this.DebugNonMainThreadAccess && !ThreadManager.IsMainThread())
		{
			Log.Exception(new DebugWrapperException("A read is being issued outside of the main thread."));
		}
		if (!this.NeedsScope)
		{
			return DebugWrapper.DummyScope.Instance;
		}
		return new DebugWrapper.ReadScope(this);
	}

	// Token: 0x06008B43 RID: 35651 RVA: 0x00384057 File Offset: 0x00382257
	[PublicizedFrom(EAccessModifier.Protected)]
	public IDisposable DebugReadWriteScope()
	{
		if (this.DebugNonMainThreadAccess && !ThreadManager.IsMainThread())
		{
			Log.Exception(new DebugWrapperException("A read/write is being issued outside of the main thread."));
		}
		if (!this.NeedsScope)
		{
			return DebugWrapper.DummyScope.Instance;
		}
		return new DebugWrapper.ReadWriteScope(this);
	}

	// Token: 0x06008B44 RID: 35652 RVA: 0x0038408B File Offset: 0x0038228B
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator<T> DebugEnumerator<T>(IEnumerator<T> enumerator)
	{
		return new DebugWrapper.Enumerator<T>(this.DebugReadScope(), enumerator);
	}

	// Token: 0x06008B45 RID: 35653 RVA: 0x00384099 File Offset: 0x00382299
	[PublicizedFrom(EAccessModifier.Protected)]
	public IEnumerator DebugEnumerator(IEnumerator enumerator)
	{
		return new DebugWrapper.Enumerator(this.DebugReadScope(), enumerator);
	}

	// Token: 0x04006CE7 RID: 27879
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly AtomicCounter m_readCounter;

	// Token: 0x04006CE8 RID: 27880
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly AtomicCounter m_writeCounter;

	// Token: 0x02001161 RID: 4449
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class DummyScope : IDisposable
	{
		// Token: 0x06008B46 RID: 35654 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Private)]
		public DummyScope()
		{
		}

		// Token: 0x06008B47 RID: 35655 RVA: 0x00002914 File Offset: 0x00000B14
		public void Dispose()
		{
		}

		// Token: 0x04006CEB RID: 27883
		public static readonly DebugWrapper.DummyScope Instance = new DebugWrapper.DummyScope();
	}

	// Token: 0x02001162 RID: 4450
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class ReadScope : IDisposable
	{
		// Token: 0x06008B49 RID: 35657 RVA: 0x003840B4 File Offset: 0x003822B4
		public ReadScope(DebugWrapper wrapper)
		{
			this.m_wrapper = wrapper;
			if (this.m_wrapper.DebugConcurrentModifications && this.m_wrapper.m_writeCounter.Value > 0)
			{
				Log.Exception(new DebugWrapperException("A read is being issued while there are active writers."));
				this.m_concurrentModificationNotified = true;
			}
			this.m_wrapper.m_readCounter.Increment();
		}

		// Token: 0x06008B4A RID: 35658 RVA: 0x00384118 File Offset: 0x00382318
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~ReadScope()
		{
			this.Dispose(false);
		}

		// Token: 0x06008B4B RID: 35659 RVA: 0x00384148 File Offset: 0x00382348
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06008B4C RID: 35660 RVA: 0x00384158 File Offset: 0x00382358
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool disposing)
		{
			if (this.m_disposed)
			{
				return;
			}
			this.m_disposed = true;
			if (!disposing)
			{
				Log.Error("DebugWrapper.ReadScope was not disposed of correctly.");
			}
			this.m_wrapper.m_readCounter.Decrement();
			if (!this.m_concurrentModificationNotified && this.m_wrapper.DebugConcurrentModifications && this.m_wrapper.m_writeCounter.Value > 0)
			{
				Log.Exception(new DebugWrapperException("A read was issued while there were active writers."));
			}
		}

		// Token: 0x04006CEC RID: 27884
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DebugWrapper m_wrapper;

		// Token: 0x04006CED RID: 27885
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool m_concurrentModificationNotified;

		// Token: 0x04006CEE RID: 27886
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_disposed;
	}

	// Token: 0x02001163 RID: 4451
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class ReadWriteScope : IDisposable
	{
		// Token: 0x06008B4D RID: 35661 RVA: 0x003841CC File Offset: 0x003823CC
		public ReadWriteScope(DebugWrapper wrapper)
		{
			this.m_wrapper = wrapper;
			if (this.m_wrapper.DebugConcurrentModifications && this.m_wrapper.m_writeCounter.Value > 0)
			{
				if (this.m_wrapper.m_writeCounter.Value > 0)
				{
					Log.Exception(new DebugWrapperException("A read/write is being issued while there are active writers."));
				}
				else if (this.m_wrapper.m_readCounter.Value > 0)
				{
					Log.Exception(new DebugWrapperException("A read/write is being issued while there are active readers."));
				}
				this.m_concurrentModificationNotified = true;
			}
			this.m_wrapper.m_readCounter.Increment();
			this.m_wrapper.m_writeCounter.Increment();
		}

		// Token: 0x06008B4E RID: 35662 RVA: 0x00384278 File Offset: 0x00382478
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~ReadWriteScope()
		{
			this.Dispose(false);
		}

		// Token: 0x06008B4F RID: 35663 RVA: 0x003842A8 File Offset: 0x003824A8
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x06008B50 RID: 35664 RVA: 0x003842B8 File Offset: 0x003824B8
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool disposing)
		{
			if (this.m_disposed)
			{
				return;
			}
			this.m_disposed = true;
			if (!disposing)
			{
				Log.Error("DebugWrapper.ReadWriteScope was not disposed of correctly.");
			}
			this.m_wrapper.m_writeCounter.Decrement();
			this.m_wrapper.m_readCounter.Decrement();
			if (!this.m_concurrentModificationNotified && this.m_wrapper.DebugConcurrentModifications)
			{
				if (this.m_wrapper.m_writeCounter.Value > 0)
				{
					Log.Exception(new DebugWrapperException("A read/write was issued while there were active writers."));
					return;
				}
				if (this.m_wrapper.m_readCounter.Value > 0)
				{
					Log.Exception(new DebugWrapperException("A read/write was issued while there are active readers."));
				}
			}
		}

		// Token: 0x04006CEF RID: 27887
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly DebugWrapper m_wrapper;

		// Token: 0x04006CF0 RID: 27888
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool m_concurrentModificationNotified;

		// Token: 0x04006CF1 RID: 27889
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_disposed;
	}

	// Token: 0x02001164 RID: 4452
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class Enumerator : IEnumerator, IDisposable
	{
		// Token: 0x06008B51 RID: 35665 RVA: 0x0038435E File Offset: 0x0038255E
		public Enumerator(IDisposable scope, IEnumerator enumerator)
		{
			this.m_scope = scope;
			this.m_enumerator = enumerator;
		}

		// Token: 0x06008B52 RID: 35666 RVA: 0x00384374 File Offset: 0x00382574
		public void Dispose()
		{
			this.m_scope.Dispose();
			IDisposable disposable = this.m_enumerator as IDisposable;
			if (disposable == null)
			{
				return;
			}
			disposable.Dispose();
		}

		// Token: 0x06008B53 RID: 35667 RVA: 0x00384396 File Offset: 0x00382596
		public bool MoveNext()
		{
			return this.m_enumerator.MoveNext();
		}

		// Token: 0x06008B54 RID: 35668 RVA: 0x003843A3 File Offset: 0x003825A3
		public void Reset()
		{
			this.m_enumerator.Reset();
		}

		// Token: 0x17000E83 RID: 3715
		// (get) Token: 0x06008B55 RID: 35669 RVA: 0x003843B0 File Offset: 0x003825B0
		public object Current
		{
			get
			{
				return this.m_enumerator.Current;
			}
		}

		// Token: 0x04006CF2 RID: 27890
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IDisposable m_scope;

		// Token: 0x04006CF3 RID: 27891
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IEnumerator m_enumerator;
	}

	// Token: 0x02001165 RID: 4453
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class Enumerator<T> : IEnumerator<T>, IEnumerator, IDisposable
	{
		// Token: 0x06008B56 RID: 35670 RVA: 0x003843BD File Offset: 0x003825BD
		public Enumerator(IDisposable scope, IEnumerator<T> enumerator)
		{
			this.m_scope = scope;
			this.m_enumerator = enumerator;
		}

		// Token: 0x06008B57 RID: 35671 RVA: 0x003843D3 File Offset: 0x003825D3
		public void Dispose()
		{
			this.m_scope.Dispose();
			this.m_enumerator.Dispose();
		}

		// Token: 0x06008B58 RID: 35672 RVA: 0x003843EB File Offset: 0x003825EB
		public bool MoveNext()
		{
			return this.m_enumerator.MoveNext();
		}

		// Token: 0x06008B59 RID: 35673 RVA: 0x003843F8 File Offset: 0x003825F8
		public void Reset()
		{
			this.m_enumerator.Reset();
		}

		// Token: 0x17000E84 RID: 3716
		// (get) Token: 0x06008B5A RID: 35674 RVA: 0x00384405 File Offset: 0x00382605
		public T Current
		{
			get
			{
				return this.m_enumerator.Current;
			}
		}

		// Token: 0x17000E85 RID: 3717
		// (get) Token: 0x06008B5B RID: 35675 RVA: 0x00384412 File Offset: 0x00382612
		public object Current
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return this.m_enumerator.Current;
			}
		}

		// Token: 0x04006CF4 RID: 27892
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IDisposable m_scope;

		// Token: 0x04006CF5 RID: 27893
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IEnumerator<T> m_enumerator;
	}
}
