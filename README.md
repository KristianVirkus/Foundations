# Foundations
Helps to handle or eases common recurring tasks.

## Features

### `ReaderWriterLockSlimExtensions` class
The `ReaderWriterLockSlimExtensions` class (Namespace `Elements.Foundation.Synchronization`) implements extension methods to the `ReaderWriterLockSlim` class to allow locking using a using statement instead of more complex and disrupting `try...finally` blocks.

The class implements the extension methods `Read` for read access, `UpRead` for upgradeable read access, and `Write` for write access which return an `IDisposable` instance each. Internally these calls invoke `EnterReadLock`, `EnterUpgradeableReadLock`, or `EnterWriteLock` respectively. Disposing the returned object invokes `ExitReadLock`, `ExitUpgradeableReadLock`, or `ExitWriteLock` respectively. Thus, a lock on the `ReaderWriterLockSlim` can be acquired using the using statement instead of a more complex and disrupting `try...finally` block. All functionality of the `ReaderWriterLockSlim` remain intact and can be combined with the extension methods.

### `CallbackDisposer` class
The `CallbackDisposer` class (Namespace `Elements.Foundations.Disposable`) implements the `IDisposable` interface. Upon instantiation a simple parameterless and return-value-less callback action must be specified. This callback action gets invoked when the instance gets disposed through the `Dispose` method. The disposal may only occur once. Any further attempt is ignored and the callback action doesn't get called again.

### `Switch` static class
The static `Switch` class (Namespace `Elements.Foundations.Flow`) implements an extended switch-case functionality by defining cases which don't necessarily rely on constants but on evaluating a set of `Predicate`s. Surely this comes at the cost of efficiency but it could be handy for replacing some if-then-else-trees and when falling through cases is required. Exceptions won't get caught.

In order to use the `Switch` class, the `Switch.Evaluate<T>` method with the generic type parameter `T` needs to be invoked with the possible cases to evaluate as parameters. `T` is the type of the value under examination. The `Evaluate<T>` method returns `true` if any of the cases had been executed, and `false` if none of them applied. The code to be executed if a case applies gets handed in the value under examination as argument and must return `true` if to abort further case evaluation or `false` to fall through to any other applicable case. Cases can be constructed calling the `Case<T>` method of the `Switch` class while handing over a `Predicate<T>` and a `CaseCallback<T>` or by creating instances of the `SwitchCase<T>` class.

Example:
```
int someValue = 42;
if (!Switch.Evaluate<int>(someValue,
    Switch.Case<int>(v => v == 0, (v) =>
      {
        // someValue == 0 code and don't evaluate other cases by returning true
        return true;
      }),
    Switch.Case<int>(v => v == 42, (v) =>
      {
        // someValue == 42 code and allow further cases to be evaluated by returning false
        return false;
      }),
    Switch.Case<int>(v => v == 43, (v) =>
      {
        // someValue == 43 code
        return true;
      })))
{
  // default code in case non of the cases above applied
}
```

The example above would execute both cases 42, and 43 because the callback action of case 42 returns `false` to allow further cases to be evaluated. If `someValue` was neither 0, nor 42, nor 43, the default code would be executed instead.

### `Hourglass` class
The `Hourglass` class (Namespace `Elements.Foundations.Flow`) implements a countdown based on the .NET/Mono Framework's `System.Threading.Stopwatch` class adapted to handle timeouts. Oftentimes calls to `Socket` methods or `WaitHandle`s require the specification of timeouts but whenever an overall timeout is specified for multiple of these calls, calculations for remaining timeouts and expiration disrupts the code.

The `Hourglass` class offers methods to `Start`, `Stop`, and `Reset` the countdown. Multiple properties, e.g. `bool IsTimeout` tell about the current state and the `TestForTimeout` method throws a `TimeoutException` if a timeout has occurred. Calling `Reset` will stop the countdown first and not automatically restart it. Both `Reset` and the constructor accept an argument to specify the intended timeout as `TimeSpan` where -1 milliseconds represent an infinite timeout. `System.Threading.Timeout.InfiniteTimeSpan` can be used for an infinite timeout. Handing in values less than -1 milliseconds will throw `ArgumentOutOfRange` exceptions. Both methods `Start` and `Reset` return the `Hourglass` instance they have been invoked on to allow a single line instantiation and start of the countdown. If an infinite timeout had been specified, the `RemainingTimeout` will always be -1 milliseconds. If a different timeout had been specified, the `RemainingTimeout` will never be less than 0 milliseconds.

There are no events to register handlers for because timeout calculations are only performed when actively checking for timeout. The accurarcy of calculations depends on the accuracy of the framework-provided `Stopwatch` implementation.

Example:

```
var waitHandle1 = new ManualResetEvent(false);
var waitHandle2 = new ManualResetEvent(false);
...
var timeout = new Hourglass(TimeSpan.FromSeconds(15)).Start();

// Use the initial timeout.
if (!waitHandle1.WaitOne(timeout.RemainingTimeout))
{
    ...
    throw new TimeoutException(...);
}

// Use the remaining timeout with the same code on the second WaitHandle.
if (!waitHandle2.WaitOne(timeout.RemainingTimeout))
{
    ...
    throw new TimeoutException(...);
}

// Perform first long-running action.

timeout.TestForTimeout();

// Perform second long-running action.
```
