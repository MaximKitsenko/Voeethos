using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaxKitsenko.Voeethos.Threading
{
	public class AsyncLock
	{
		private readonly Task< Releaser > _releaser;
		internal readonly AsyncSemaphore _semaphore;

		public AsyncLock()
		{
			this._semaphore = new AsyncSemaphore( 1 );
			this._releaser = Task.FromResult( new Releaser( this ) );
		}

		public Task< Releaser > LockAsync()
		{
			var wait = this._semaphore.WaitAsync();
			return wait.IsCompleted ? this._releaser :
				wait.ContinueWith( ( _, state ) => new Releaser( ( AsyncLock )state ),
					this, CancellationToken.None,
					TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default );
		}

	}

	public struct Releaser: IDisposable
	{
		private readonly AsyncLock _toRelease;

		internal Releaser( AsyncLock toRelease )
		{
			this._toRelease = toRelease;
		}

		public void Dispose()
		{
			if( this._toRelease != null )
				this._toRelease._semaphore.Release();
		}
	}
}