using System;
using System.Threading;

// Token: 0x020011F6 RID: 4598
public static class ReaderWriterLockSlimExtensions
{
	// Token: 0x06008FAD RID: 36781 RVA: 0x00395C9D File Offset: 0x00393E9D
	public static ReaderWriterLockSlimExtensions.ReadScope ReadLockScope(this ReaderWriterLockSlim lockSlim)
	{
		return new ReaderWriterLockSlimExtensions.ReadScope(lockSlim);
	}

	// Token: 0x06008FAE RID: 36782 RVA: 0x00395CA5 File Offset: 0x00393EA5
	public static ReaderWriterLockSlimExtensions.WriteScope WriteLockScope(this ReaderWriterLockSlim lockSlim)
	{
		return new ReaderWriterLockSlimExtensions.WriteScope(lockSlim);
	}

	// Token: 0x06008FAF RID: 36783 RVA: 0x00395CAD File Offset: 0x00393EAD
	public static ReaderWriterLockSlimExtensions.UpgradeableReadScope UpgradableReadLockScope(this ReaderWriterLockSlim lockSlim)
	{
		return new ReaderWriterLockSlimExtensions.UpgradeableReadScope(lockSlim);
	}

	// Token: 0x020011F7 RID: 4599
	public readonly struct ReadScope : IDisposable
	{
		// Token: 0x06008FB0 RID: 36784 RVA: 0x00395CB5 File Offset: 0x00393EB5
		public ReadScope(ReaderWriterLockSlim lockSlim)
		{
			this.m_lockSlim = lockSlim;
			this.m_lockSlim.EnterReadLock();
		}

		// Token: 0x06008FB1 RID: 36785 RVA: 0x00395CC9 File Offset: 0x00393EC9
		public void Dispose()
		{
			this.m_lockSlim.ExitReadLock();
		}

		// Token: 0x04006ECB RID: 28363
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ReaderWriterLockSlim m_lockSlim;
	}

	// Token: 0x020011F8 RID: 4600
	public readonly struct WriteScope : IDisposable
	{
		// Token: 0x06008FB2 RID: 36786 RVA: 0x00395CD6 File Offset: 0x00393ED6
		public WriteScope(ReaderWriterLockSlim lockSlim)
		{
			this.m_lockSlim = lockSlim;
			this.m_lockSlim.EnterWriteLock();
		}

		// Token: 0x06008FB3 RID: 36787 RVA: 0x00395CEA File Offset: 0x00393EEA
		public void Dispose()
		{
			this.m_lockSlim.ExitWriteLock();
		}

		// Token: 0x04006ECC RID: 28364
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ReaderWriterLockSlim m_lockSlim;
	}

	// Token: 0x020011F9 RID: 4601
	public readonly struct UpgradeableReadScope : IDisposable
	{
		// Token: 0x06008FB4 RID: 36788 RVA: 0x00395CF7 File Offset: 0x00393EF7
		public UpgradeableReadScope(ReaderWriterLockSlim lockSlim)
		{
			this.m_lockSlim = lockSlim;
			this.m_lockSlim.EnterUpgradeableReadLock();
		}

		// Token: 0x06008FB5 RID: 36789 RVA: 0x00395D0B File Offset: 0x00393F0B
		public void Dispose()
		{
			this.m_lockSlim.ExitUpgradeableReadLock();
		}

		// Token: 0x04006ECD RID: 28365
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly ReaderWriterLockSlim m_lockSlim;
	}
}
