namespace Elements.Foundations.UnitTests.Flow
{
	using System;
	using System.Threading;
	using Elements.Foundations.Flow;
	using FluentAssertions;
	using NUnit.Framework;

	[TestFixture]
	public class HourglassTest
	{
		[Test]
		public void NotStarted_Should_SignalNotRunningAndNotTimedOutAndRemainingTimeEqualsInitialTimeAndElapsedTimeIsNull()
		{
			var hg = new Hourglass(TimeSpan.FromSeconds(5));
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromSeconds(5));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			Thread.Sleep(100);

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromSeconds(5));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);
		}

		[Test]
		public void StartedWith100msTimeout_Should_SignalNotRunningAndTimedOutAndRemainingTimeZeroAndElapsedTimeLarger100ms()
		{
			var hg = new Hourglass(TimeSpan.FromMilliseconds(100));
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(100));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Start();

			hg.IsRunning.Should().BeTrue();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(0));

			Thread.Sleep(TimeSpan.FromMilliseconds(100));

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeTrue();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(0));
			hg.ElapsedTime.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(100));
		}

		[Test]
		public void AfterStoppingRunningHourglass_Should_KeepStates()
		{
			var hg = new Hourglass(TimeSpan.FromMinutes(1));
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMinutes(1));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Start();

			hg.IsRunning.Should().BeTrue();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().BeGreaterOrEqualTo(TimeSpan.FromSeconds(59));

			hg.Stop();
			var remaining = hg.RemainingTime;
			var elapsed = hg.ElapsedTime;

			Thread.Sleep(TimeSpan.FromMilliseconds(100));

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(remaining);
			hg.ElapsedTime.Should().Be(elapsed);
		}

		// infinite timeout should never elapse
		[Test]
		public void StartedAndStoppedWithInfiniteTimeout_Should_NeverTimeoutAndKeepRemainingTimeInfiniteButUpdateElapsedTime()
		{
			var hg = new Hourglass(Timeout.InfiniteTimeSpan);
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(Timeout.InfiniteTimeSpan);
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Start();

			hg.IsRunning.Should().BeTrue();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().BeGreaterOrEqualTo(Timeout.InfiniteTimeSpan);

			Thread.Sleep(TimeSpan.FromMilliseconds(100));

			hg.IsRunning.Should().BeTrue();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(Timeout.InfiniteTimeSpan);
			hg.ElapsedTime.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(100));

			hg.Stop();
			Thread.Sleep(TimeSpan.FromMilliseconds(100));

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(Timeout.InfiniteTimeSpan);
			hg.ElapsedTime.Should().BeLessThan(TimeSpan.FromMilliseconds(200));
		}

		[Test]
		public void ResetBeforeStartInvoked_Should_NotChangeStates()
		{
			var hg = new Hourglass(Timeout.InfiniteTimeSpan);
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(Timeout.InfiniteTimeSpan);
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Reset();

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(Timeout.InfiniteTimeSpan);
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);
		}

		// reset to time before ever running
		[Test]
		public void ResetToDifferentTimeBeforeStartInvoked_Should_NotChangeStatesButTimeout()
		{
			var hg = new Hourglass(Timeout.InfiniteTimeSpan);
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(Timeout.InfiniteTimeSpan);
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Reset(TimeSpan.FromSeconds(5));

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromSeconds(5));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);
		}

		[Test]
		public void ResetWhileRunning_Should_StopAndResetElapsedAndRemainingTimes()
		{
			var hg = new Hourglass(TimeSpan.FromMilliseconds(200));
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(200));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Start();
			Thread.Sleep(TimeSpan.FromMilliseconds(50));

			hg.Reset();

			Thread.Sleep(TimeSpan.FromMilliseconds(50));

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(200));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);
		}

		[Test]
		public void ResetToDifferentTimeWhileRunning_Should_StopAndResetElapsedAndRemainingTimes()
		{
			var hg = new Hourglass(TimeSpan.FromMilliseconds(200));
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(200));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Start();
			Thread.Sleep(TimeSpan.FromMilliseconds(50));

			hg.Reset(TimeSpan.FromMilliseconds(500));

			Thread.Sleep(TimeSpan.FromMilliseconds(50));

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(500));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);
		}

		[Test]
		public void ResetWhileStopped_Should_StopAndResetElapsedAndRemainingTimes()
		{
			var hg = new Hourglass(TimeSpan.FromMilliseconds(200));
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(200));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Start();
			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			hg.Stop();

			hg.Reset();

			Thread.Sleep(TimeSpan.FromMilliseconds(50));

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(200));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);
		}

		[Test]
		public void ResetToDifferentTimeWhileStopped_Should_StopAndResetElapsedAndRemainingTimes()
		{
			var hg = new Hourglass(TimeSpan.FromMilliseconds(200));
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(200));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Start();
			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			hg.Stop();

			hg.Reset(TimeSpan.FromMilliseconds(500));

			Thread.Sleep(TimeSpan.FromMilliseconds(50));

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeFalse();
			hg.RemainingTime.Should().Be(TimeSpan.FromMilliseconds(500));
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);
		}

		[Test]
		public void CreateInstanceWithMinus2Milliseconds_ShouldThrow_ArgumentOutOfRangeException()
		{
			Assert.Throws(typeof(ArgumentOutOfRangeException), () => { new Hourglass(TimeSpan.FromMilliseconds(-2)); });
		}

		[Test]
		public void ResetWithMinus2Milliseconds_ShouldThrow_ArgumentOutOfRangeException()
		{
			var hg = new Hourglass(TimeSpan.FromSeconds(1));
			Assert.Throws(typeof(ArgumentOutOfRangeException), () => { hg.Reset(TimeSpan.FromMilliseconds(-2)); });
		}

		[Test]
		public void CreateInstanceWith0Timeout_Should_ImmediatelySignalTimeout()
		{
			var hg = new Hourglass(TimeSpan.Zero);
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeTrue();
			hg.RemainingTime.Should().Be(TimeSpan.Zero);
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);

			hg.Start();
			Thread.Sleep(50);

			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeTrue();
			hg.RemainingTime.Should().Be(TimeSpan.Zero);
			hg.ElapsedTime.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(50));
		}

		[Test]
		public void TestForTimeoutInTimeoutState_ShouldThrow_TimeoutException()
		{
			var hg = new Hourglass(TimeSpan.Zero);
			hg.IsRunning.Should().BeFalse();
			hg.IsTimeout.Should().BeTrue();
			hg.RemainingTime.Should().Be(TimeSpan.Zero);
			hg.ElapsedTime.Should().Be(TimeSpan.Zero);
			Assert.Throws(typeof(TimeoutException), () => { hg.TestForTimeout(); });
		}
	}
}
