namespace Elements.Foundations.Disposable
{
	using System;

	/// <summary>
	/// Implements a disposable class to invoke a callback when disposed.
	/// </summary>
	public class CallbackDisposer : IDisposable
	{
		#region Fields

		private bool isDisposed = false;
		private Action callback;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <paramref name="LockDisposer"/> class.
		/// </summary>
		/// <param name="callback">The callback.</param>
		public CallbackDisposer(Action callback)
		{
			this.callback = callback;
		}

		#endregion

		#region IDisposable implementation

		public void Dispose()
		{
			// Assure thread-safety. Simple lock pattern should
			// be enough because concurrent disposal should be
			// pretty unprobable.
			lock (this)
			{
				if (!this.isDisposed)
				{
					this.isDisposed = true;
				}
				else
				{
					return;
				}
			}

			if (this.callback != null)
			{
				this.callback();
			}
		}

		#endregion
	}
}
