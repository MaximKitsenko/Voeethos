using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaxKitsenko.Voeethos.Threading;

namespace MaxKitsenko.Voeethos.Console.Threading
{
	public class AsyncLockExample1
	{
		private static readonly ConcurrentDictionary< string, AsyncLock > concurentLocks = new ConcurrentDictionary< string, AsyncLock >();

		public static void StartAsyncLockExample()
		{
			var t = SomeClassWithAsyncMethods.BulkTasksMethodAsyn();
			t.Wait();
			System.Console.WriteLine( "Press any key" );
			System.Console.ReadLine();
		}

		public static class SomeClassWithAsyncMethods
		{
			private static readonly Dictionary< string, string > _locks = new Dictionary< string, string >();

			public static async Task BulkTasksMethodAsyn()
			{
				var tasksList = new List< Task >();
				for( var i = 0; i < 1000; i++ )
				{
					tasksList.Add( SomeMethodAsyncLockSyncronisedAsync( "i" ) );
				}
				await Task.WhenAll( tasksList );
			}

			public static async Task SomeMethodAsyncLockSyncronisedAsync( string mark )
			{
				AsyncLock lockObj;
				if( !concurentLocks.TryGetValue( mark, out lockObj ) )
					lockObj = concurentLocks[ mark ] = new AsyncLock();
				using( var releaser = await lockObj.LockAsync() )
				{
					try
					{
						await SomeMethodWithoutSyncronisationAsync( mark );
					}
					catch( Exception e )
					{
						System.Console.WriteLine( "[CX]		Can't Exit critical section Mark {0}, Thread Name {1}, Thread Id {2}", lockObj, Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId );
						throw e;
					}
				}
			}

			public static async Task SomeMethodMonitorSyncronisedAsync( string mark )
			{
				string lockObj;
				if( !_locks.TryGetValue( mark, out lockObj ) )
					lockObj = _locks[ mark ] = mark;
				if( Monitor.TryEnter( lockObj, new TimeSpan( 0, 0, 100 ) ) )
				{
					try
					{
						await SomeMethodWithoutSyncronisationAsync( lockObj );
					}
					finally
					{
						try
						{
							Monitor.Exit( lockObj );
							System.Console.WriteLine( "[X ]		Exit critical section Mark {0}, Thread Name {1}, Thread Id {2}", lockObj, Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId );
						}
						catch( Exception e )
						{
							System.Console.WriteLine( "[CX]		Can't Exit critical section Mark {0}, Thread Name {1}, Thread Id {2}", lockObj, Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId );
						}
					}
				}
				else
					System.Console.WriteLine( "[CE] Can't Enter critical section Mark {0}, Thread Name {1}, Thread Id {2}", lockObj, Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId );
			}

			public static async Task SomeMethodWithoutSyncronisationAsync( string mark )
			{
				var rnd = new Random();
				System.Console.WriteLine( "		Enter critical section. Mark {0}, Thread Name {1}, Thread Id {2}", mark, Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId );
				await Task.Delay( rnd.Next( 1, 20 ) * 1000 );
				System.Console.WriteLine( "		Exit critical section Mark {0}, Thread Name {1}, Thread Id {2}", mark, Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId );
			}
		}
	}
}