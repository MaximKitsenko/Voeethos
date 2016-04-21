using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaxKitsenko.Voeethos.Console.Threading;
using MaxKitsenko.Voeethos.Threading;

namespace MaxKitsenko.Voeethos.Console
{
	internal class Program
	{
		private static void Main( string[] args )
		{
			AsyncLockExample1.StartAsyncLockExample();
		}
	}
}