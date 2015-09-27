namespace Elements.Foundations.UnitTests.Disposable
{
	using System;
	using System.Threading;
	using Foundations.Disposable;
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class CallbackDisposerTest
	{
		[Test]
		public void DisposeTwice_Should_InvokeTheCallbackOnce()
		{
			int invokations = 0;
			IDisposable disposable = new CallbackDisposer(() => { Interlocked.Increment(ref invokations); });
			invokations.Should().Be(0);
			disposable.Dispose();
			invokations.Should().Be(1);
			disposable.Dispose();
			invokations.Should().Be(1);
		}
	}
}
