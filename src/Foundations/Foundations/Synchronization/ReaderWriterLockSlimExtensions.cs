namespace Elements.Foundations.Synchronization
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Threading;
	using Disposable;

	/// <summary>
	/// Implements extensions to the <see cref="System.Threading.ReaderWriterLockSlim"/> class.
	/// Eases handling of <see cref="System.Threading.ReaderWriterLockSlim"/>s by making them
	/// accessible in any way through an <see cref="System.IDisposable"/> instance in a using block.
	/// This provides code that is simpler to read and less error-prone due to never missed
	/// lock releasing.
	/// </summary>
	public static class ReaderWriterLockSlimExtensions
	{
		#region Static texts

		private static readonly string TextPerformativeNotSupported = "The performative {0} is not supported.";

		#endregion

		#region Constructors

		/// <summary>
		/// The static contructor of the <see cref="ReaderWriterLockSlimExtensions"/> class.
		/// </summary>
		static ReaderWriterLockSlimExtensions()
		{
			
		}

		#endregion

		#region Extension methods

		/// <summary>
		/// Acquires read access to <paramref name="theLock"/>. Disposing the
		/// returned object releases the lock again. This way using
		/// <paramref name="theLock"/> can be used in conjunction with directly
		/// accessing <paramref name="theLock"/> and works with either
		/// <c>NoRecursion</c> and <c>SupportsRecursion</c> locks.
		/// </summary>
		/// <param name="theLock">The lock to access.</param>
		/// <returns>Disposable instance to dispose in order to release the lock again.</returns>
		public static IDisposable Read(this ReaderWriterLockSlim theLock)
		{
			return processLock(theLock, LockPerformative.Read);
		}

		/// <summary>
		/// Acquires upgradeable read access to <paramref name="theLock"/>. Disposing the
		/// returned object releases the lock again. This way using
		/// <paramref name="theLock"/> can be used in conjunction with directly
		/// accessing <paramref name="theLock"/> and works with either
		/// <c>NoRecursion</c> and <c>SupportsRecursion</c> locks.
		/// </summary>
		/// <param name="theLock">The lock to access.</param>
		/// <returns>Disposable instance to dispose in order to release the lock again.</returns>
		public static IDisposable UpRead(this ReaderWriterLockSlim theLock)
		{
			return processLock(theLock, LockPerformative.ReadUpgradeable);
		}

		/// <summary>
		/// Acquires write access to <paramref name="theLock"/>. Disposing the
		/// returned object releases the lock again. This way using
		/// <paramref name="theLock"/> can be used in conjunction with directly
		/// accessing <paramref name="theLock"/> and works with either
		/// <c>NoRecursion</c> and <c>SupportsRecursion</c> locks.
		/// </summary>
		/// <param name="theLock">The lock to access.</param>
		/// <returns>Disposable instance to dispose in order to release the lock again.</returns>
		public static IDisposable Write(this ReaderWriterLockSlim theLock)
		{
			return processLock(theLock, LockPerformative.Write);
		}

		/// <summary>
		/// Processes a locking request.
		/// </summary>
		/// <param name="theLock">The lock to use for the request.</param>
		/// <param name="performative">The performative/action to take.</param>
		/// <returns>An <see cref="System.IDisposable"/> object to release the lock upon disposal.</returns>
		/// <exception cref="System.NotSupportedException">Thrown if <paramref name="performative"/> is not supported.</exception>
		// Inline method to improve performance while maintaining readability.
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static IDisposable processLock(ReaderWriterLockSlim theLock, LockPerformative performative)
		{
			if (theLock == null)
			{
				throw new ArgumentNullException(nameof(theLock));
			}

			switch (performative)
			{
				case LockPerformative.Read:
					theLock.EnterReadLock();
					return new CallbackDisposer(() => { theLock.ExitReadLock(); });
				case LockPerformative.ReadUpgradeable:
					theLock.EnterUpgradeableReadLock();
					return new CallbackDisposer(() => { theLock.ExitUpgradeableReadLock(); });
				case LockPerformative.Write:
					theLock.EnterWriteLock();
					return new CallbackDisposer(() => { theLock.ExitWriteLock(); });
				default:
					throw new NotSupportedException(string.Format(TextPerformativeNotSupported, performative.ToString()));
			}
		}

		#endregion

		#region Nested types

		/// <summary>
		/// Enumeration of lock performative alternatives.
		/// </summary>
		private enum LockPerformative
		{
			/// <summary>
			/// Read access.
			/// </summary>
			Read,

			/// <summary>
			/// Upgradeable read access.
			/// </summary>
			ReadUpgradeable,

			/// <summary>
			/// Write access.
			/// </summary>
			Write,
		}

		#endregion
	}
}
