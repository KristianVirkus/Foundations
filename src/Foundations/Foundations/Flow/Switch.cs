namespace Elements.Foundations.Flow
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// Implements an extended switch-case with callbacks.
	/// </summary>
	public static class Switch
	{
		/// <summary>
		/// Evaluates the whole switch construct.
		/// </summary>
		/// <typeparam name="T">The type of the value under examination.</typeparam>
		/// <param name="value">The value under examination.</param>
		/// <param name="cases">The cases of <paramref name="value"/> that can be handled.</param>
		/// <returns><c>true</c> if any of the cases had been executed, <c>false</c> otherwise.</returns>
		public static bool Evaluate<T>(T value, IEnumerable<SwitchCase<T>> cases)
		{
			if ((cases == null) || (!cases.Any()))
			{
				return false;
			}

			bool anyInvoked = false;

			foreach (var c in cases)
			{
				if (c.Predicate(value))
				{
					anyInvoked = true;
					if (c.Callback(value))
					{
						break;
					}
				}
			}
			
			return anyInvoked;
		}

		/// <summary>
		/// Creates a new <see cref="Elements.Foundations.Flow.SwitchCase{T}"/>
		/// </summary>
		/// <typeparam name="T">The type of the value under examination.</typeparam>
		/// <param name="predicate">The case's predicate.</param>
		/// <param name="callback">The case's callback to invoke when the predicate is
		///		fulfilled. If null, a default callback returning true will be used.</param>
		/// <returns>A case.</returns>
		public static SwitchCase<T> Case<T>(Predicate<T> predicate, CaseCallback<T> callback)
		{
			return new SwitchCase<T>(predicate, callback);
		}

		#region Nested types

		/// <summary>
		/// The delegate defining a callback when a switch case matches.
		/// </summary>
		/// <typeparam name="T">The type of the value under examination.</typeparam>
		/// <param name="value">The value under examination.</param>
		/// <returns>true if further matching cases must not be executed, false
		///		to "fall through" and evaluate possible further cases.</returns>
		public delegate bool CaseCallback<T>(T value);

		/// <summary>
		/// Represents a case.
		/// </summary>
		/// <typeparam name="T">The type of the value under examination.</typeparam>
		public class SwitchCase<T>
		{
			/// <summary>
			/// Gets the case's predicate.
			/// </summary>
			internal Predicate<T> Predicate { get; private set; }

			/// <summary>
			/// Gets the case's callback to invoke when the predicate is fulfilled.
			/// </summary>
			internal CaseCallback<T> Callback { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="Switch{T}.SwitchCase{T}"/> class.
			/// </summary>
			/// <param name="predicate">The case's predicate.</param>
			/// <param name="callback">The case's callback to invoke when the predicate is
			///		fulfilled. If null, a default callback returning true will be used.</param>
			public SwitchCase(Predicate<T> predicate, CaseCallback<T> callback)
			{
				if (predicate == null)
				{
					throw new ArgumentNullException(nameof(predicate));
				}

				if (callback == null)
				{
					throw new ArgumentNullException(nameof(callback));
				}

				this.Predicate = predicate;
				this.Callback = callback;
			}
		}

		#endregion
	}
}
