# Foundations
Helps to handle or eases common tasks.

##Features
###Extensions on ReaderWriterLockSlim class
The ReaderWriterLockSlimExtensions class implements extension methods to the ReaderWriterLockSlim class to allow locking using a using statement instead of more complex and disrupting try...finally blocks.

The class implements the extension methods Read() for read access, UpRead() for upgradeable read access, and Write() for write access which return an IDisposable instance each. Internally these calls invoke EnterReadLock(), EnterUpgradeableReadLock(), or EnterWriteLock() respectively. Disposing the returned object invokes ExitReadLock(), ExitUpgradeableReadLock, or ExitWriteLock respectively. Thus, a lock on the ReaderWriterLockSlim can be acquired using the using statement instead of a more complex and disrupting try...finally block. All functionality of the ReaderWriterLockSlim remain intact and can be combined with the extension methods.
