using System;
using Elements.Foundations.Flow;
using FluentAssertions;
using NUnit.Framework;

namespace Elements.Foundations.UnitTests.Flow
{
	[TestFixture]
	public class SwitchTest
	{
		[Test]
		public void IntegerNullCases_Should_ReturnFalse()
		{
			Switch.Evaluate(7, null).Should().BeFalse();
		}

		[Test]
		public void IntegerEmptyCases_Should_ReturnFalse()
		{
			Switch.Evaluate(7, new Switch.SwitchCase<int>[0]).Should().BeFalse();
		}

		[Test]
		public void IntegerOneCaseFirstMatching_Should_ReturnTrue()
		{
			Switch.Evaluate(7, new[] { Switch.Case<int>(v => v == 7, (value) => { return false; }) }).Should().BeTrue();
		}

		[Test]
		public void IntegerOneCaseNoneMatching_Should_NotInvokeCallbackReturnFalse()
		{
			var invoked = false;
			Switch.Evaluate(7, new[] { Switch.Case<int>(v => v == 8, (value) => { invoked = true; return true; }) }).Should().BeFalse();
			invoked.Should().BeFalse();
		}

		[Test]
		public void IntegerOneCaseFirstMatching_Should_InvokeCallbackAndReturnTrue()
		{
			var invoked = false;
			Switch.Evaluate(7, new[] { Switch.Case<int>(v => v == 7, (value) => { invoked = true; return true; }) }).Should().BeTrue();
			invoked.Should().BeTrue();
		}

		[Test]
		public void IntegerTwoCasesFirstMatching_Should_InvokeOnlyOneCallbackAndReturnTrue()
		{
			var invoked1 = false;
			var invoked2 = false;
			Switch.Evaluate(7, new[]
			{
				Switch.Case<int>(v => v == 7, (value) => { invoked1 = true; return true; }),
				Switch.Case<int>(v => v == 8, (value) => { invoked2 = true; return true; }),
			}).Should().BeTrue();
			invoked1.Should().BeTrue();
			invoked2.Should().BeFalse();
		}

		[Test]
		public void IntegerTwoCasesSecondMatching_Should_InvokeOnlyOneCallbackAndReturnTrue()
		{
			var invoked1 = false;
			var invoked2 = false;
			Switch.Evaluate(7, new[]
			{
				Switch.Case<int>(v => v == 6, (value) => { invoked1 = true; return true; }),
				Switch.Case<int>(v => v == 7, (value) => { invoked2 = true; return true; }),
			}).Should().BeTrue();
			invoked1.Should().BeFalse();
			invoked2.Should().BeTrue();
		}

		[Test]
		public void IntegerTwoCasesFirstAndSecondMatchingAndReturningFalse_Should_InvokeBothCallbacksAndReturnTrue()
		{
			var invoked1 = false;
			var invoked2 = false;
			Switch.Evaluate(7, new[]
			{
				Switch.Case<int>(v => v == 7, (value) => { invoked1 = true; return false; }),
				Switch.Case<int>(v => v == 7, (value) => { invoked2 = true; return false; }),
			}).Should().BeTrue();
			invoked1.Should().BeTrue();
			invoked2.Should().BeTrue();
		}

		[Test]
		public void IntegerTwoCasesFirstAndSecondMatchingAndReturningTrue_Should_InvokeFirstCallbackOnlyAndReturnTrue()
		{
			var invoked1 = false;
			var invoked2 = false;
			Switch.Evaluate(7, new[]
			{
				Switch.Case<int>(v => v == 7, (value) => { invoked1 = true; return true; }),
				Switch.Case<int>(v => v == 7, (value) => { invoked2 = true; return true; }),
			}).Should().BeTrue();
			invoked1.Should().BeTrue();
			invoked2.Should().BeFalse();
		}

		[Test]
		public void SwitchCaseWithNullPredicate_ShouldThrow_ArgumentNullException()
		{
			Assert.Throws(typeof(ArgumentNullException), () => { Switch.Case<int>(null, (v) => { return true; }); });
		}

		[Test]
		public void SwitchCaseWithNullCallback_ShouldThrow_ArgumentNullException()
		{
			Assert.Throws(typeof(ArgumentNullException), () => { Switch.Case<int>(v => v == 7, null); });
		}
	}
}
