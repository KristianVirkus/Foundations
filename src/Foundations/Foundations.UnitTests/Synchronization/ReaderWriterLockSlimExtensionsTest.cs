namespace Elements.Foundations.UnitTests
{
	using System.Threading;
	using Synchronization;
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class ReaderWriterLockSlimExtensionsTest
	{
		[Test]
		public void ReadOnUnusedRWLock_Should_BlockRWLockForWritingAndReleaseItAfterDisposal()
		{
			var sync = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			using (sync.Read())
			{
				sync.IsReadLockHeld.Should().BeTrue();
			}

			sync.IsReadLockHeld.Should().BeFalse();
		}

		[Test]
		public void WriteOnUnusedRWLock_Should_BlockRWLockForAnyAccessAndReleaseItAfterDisposal()
		{
			var sync = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			using (sync.Write())
			{
				sync.IsWriteLockHeld.Should().BeTrue();
			}

			sync.IsWriteLockHeld.Should().BeFalse();
		}

		[Test]
		public void ReadUpgradeableRWLock_Should_BlockRWLockForAnyOtherWriteAccessAndReleaseItAfterDisposal()
		{
			var sync = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			using (sync.UpRead())
			{
				sync.IsUpgradeableReadLockHeld.Should().BeTrue();
			}

			sync.IsUpgradeableReadLockHeld.Should().BeFalse();
		}

		[Test]
		public void ReadUpgradeableRWLockThenWrite_Should_AcquireWriteAccessAndReleaseAllAccessAfterDisposal()
		{
			var sync = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
			using (sync.UpRead())
			{
				sync.IsUpgradeableReadLockHeld.Should().BeTrue();

				using (sync.Write())
				{
					sync.IsWriteLockHeld.Should().BeTrue();
				}

				sync.IsWriteLockHeld.Should().BeFalse();
			}

			sync.IsUpgradeableReadLockHeld.Should().BeFalse();
		}
	}
}
