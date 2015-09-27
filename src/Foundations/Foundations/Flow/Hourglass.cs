namespace Elements.Foundations.Flow
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Elements.Foundations.Synchronization;

	/// <summary>
	/// Implements a countdown stopwatch which can be used e.g.
	/// to handle timeouts over multiple single actions which
	/// require their own timeout. It uses the
	/// <see cref="System.Diagnostics.Stopwatch"/> class for
	/// time measurement as accurate as possible.
	/// </summary>
	public class Hourglass
	{
		#region Fields

		private ReaderWriterLockSlim syncRoot = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
		private Stopwatch stopWatch = new Stopwatch();
		private bool internalIsRunning = false;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the initial timeout.
		/// </summary>
		public TimeSpan Timeout { get; private set; }

		/// <summary>
		/// Gets the remaining time until a timeout occurs. If the
		/// timeout had been set to infinity, the set up timeout is returned.
		/// </summary>
		public TimeSpan RemainingTime
		{
			get
			{
				using (this.syncRoot.Read())
				{
					if (this.Timeout == System.Threading.Timeout.InfiniteTimeSpan)
					{
						return this.Timeout;
					}
					else
					{
						TimeSpan remaining = this.Timeout - this.stopWatch.Elapsed;
						return TimeSpan.FromTicks(Math.Max(0, remaining.Ticks));
					}
				}
			}
		}

		/// <summary>
		/// Gets whether the hourglass is currently running.
		/// </summary>
		public bool IsRunning
		{
			get
			{
				// The hourglass is said to be running if the Start
				// method had been called (internalIsRunning) and
				// the set up timeout is either infinite or has
				// not elapsed yet.
				return (this.internalIsRunning)
					&& ((this.Timeout == System.Threading.Timeout.InfiniteTimeSpan)
						|| (this.stopWatch.Elapsed < this.Timeout));
			}
		}

		/// <summary>
		/// Gets whether a timeout has occurred. If the timeout had been
		/// specified with -1 milliseconds the value of this property
		/// is always <c>false</c>.
		/// </summary>
		public bool IsTimeout
		{
			get
			{
				return (this.Timeout != System.Threading.Timeout.InfiniteTimeSpan)
					&& (this.stopWatch.Elapsed >= this.Timeout);
			}
		}

		/// <summary>
		/// Gets the time that has elapsed since the Start method was
		/// called. Times while stopped are not included.
		/// </summary>
		public TimeSpan ElapsedTime
		{
			get
			{
				return this.stopWatch.Elapsed;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Starts the hourglass.
		/// </summary>
		/// <returns>Returns this instance to enable a one-liner
		///		instantiation and starting.</returns>
		public Hourglass Start()
		{
			using (this.syncRoot.Write())
			{
				this.stopWatch.Start();
				this.internalIsRunning = true;
				return null;
			}
		}

		/// <summary>
		/// Stops the hourglass.
		/// </summary>
		public void Stop()
		{
			using (this.syncRoot.Write())
			{
				this.stopWatch.Stop();
				this.internalIsRunning = false;
			}
		}

		/// <summary>
		/// Resets the hourglass to the initially set up timeout.
		/// If the hourglass is running upon invocation, it is
		/// stopped first.
		/// </summary>
		/// <returns>Returns this instance to enable a one-liner
		///		instantiation and starting.</returns>
		public Hourglass Reset()
		{
			using (this.syncRoot.Write())
			{
				this.Stop();
				this.stopWatch.Reset();
				return this;
			}
		}

		/// <summary>
		/// Resets the hourglass to an arbitrary timeout.
		/// If the hourglass is running upon invocation, it is
		/// stopped first.
		/// </summary>
		/// <param name="timeout">Time larger or equal 1 millisecond until the
		///		timeout occurs.	Use -1 milliseconds for an infinite timeout.
		///		If the initial timeout is zero, the timeout occurs immediately.</param>
		/// <returns>Returns this instance to enable a one-liner
		///		instantiation and starting.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is less than -1 milliseconds.</exception>
		public Hourglass Reset(TimeSpan timeout)
		{
			if (timeout < System.Threading.Timeout.InfiniteTimeSpan)
			{
				throw new ArgumentOutOfRangeException(nameof(timeout));
			}

			using (this.syncRoot.Write())
			{
				this.Reset();
				this.Timeout = timeout;
				return this;
			}
		}

		/// <summary>
		/// Tests whether the time has elapsed and a timeout has occurred. If
		/// that is true, an <see cref="System.TimeoutException"/> is thrown
		/// otherwise nothing happens.
		/// </summary>
		public void TestForTimeout()
		{
			if (this.IsTimeout)
			{
				throw new TimeoutException();
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Hourglass"/> class.
		/// </summary>
		/// <param name="timeout">Time larger or equal 1 millisecond until the
		///		timeout occurs.	Use -1 milliseconds for an infinite timeout.
		///		If the initial timeout is zero, the timeout occurs immediately.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown if <paramref name="timeout"/> is less than -1 milliseconds.</exception>
		public Hourglass(TimeSpan timeout)
		{
			if (timeout < System.Threading.Timeout.InfiniteTimeSpan)
			{
				throw new ArgumentOutOfRangeException(nameof(timeout));
			}

			this.Timeout = timeout;
		}

		#endregion
	}
}
