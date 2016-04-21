using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MaxKitsenko.Voeethos.Console
{
	internal class Program
	{
		private static void Main( string[] args )
		{
			var t = Example1.SomeMethodAsync();
			t.Wait();
			System.Console.WriteLine( "Press any key" );
			System.Console.ReadLine();
		}

		public static class SomeClass
		{
			private static readonly Dictionary< string, string > _locks = new Dictionary< string, string >();

			public static async Task Monitor_SomeMethodAsync( string mark )
			{
				//var mark = Guid.NewGuid().ToString();
				string lockObj;
				if( !_locks.TryGetValue( mark, out lockObj ) )
					lockObj = _locks[ mark ] = mark;
				if( Monitor.TryEnter( lockObj, new TimeSpan( 0, 0, 100 ) ) )
				{
					try
					{
						await SomeMethodAsync( lockObj );
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

			public static async Task SomeMethodAsync( string mark )
			{
				//var mark = Guid.NewGuid().ToString();
				var rnd = new Random();
				System.Console.WriteLine( "		Enter critical section. Mark {0}, Thread Name {1}, Thread Id {2}", mark, Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId );
				await Task.Delay( rnd.Next( 1, 20 ) * 1000 );
				System.Console.WriteLine( "		Exit critical section Mark {0}, Thread Name {1}, Thread Id {2}", mark, Thread.CurrentThread.Name, Thread.CurrentThread.ManagedThreadId );
			}
		}

		public class Example1
		{
			public static async Task SomeMethodAsync()
			{
				var tasksList = new List< Task >();
				for( var i = 0; i < 1000; i++ )
				{
					tasksList.Add( SomeClass.Monitor_SomeMethodAsync( i.ToString() ) );
				}
				await Task.WhenAll( tasksList );
			}
		}
	}
}