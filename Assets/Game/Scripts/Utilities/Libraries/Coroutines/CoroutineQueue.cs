namespace Library.Coroutines
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	/* 
	 * Imposes a limit on the maximum number of coroutines that can be running at any given time. Runs
	 * coroutines until the limit is reached and then begins queueing coroutines instead. When
	 * coroutines finish, queued coroutines are run.
	*/
	public class CoroutineQueue
	{
		private readonly uint _maxActiveCoroutinesQuantity;
		private readonly Func<IEnumerator, Coroutine> _coroutineStarter;
		private readonly Queue<IEnumerator> _queue;
		private uint currentlyActiveCoroutinesQuantity;

		public CoroutineQueue(uint maxActiveCoroutinesQuantity, Func<IEnumerator, Coroutine> coroutineStarter)
		{
			if (maxActiveCoroutinesQuantity == 0)
				throw new ArgumentException("Must be at least one", nameof(maxActiveCoroutinesQuantity));
			_maxActiveCoroutinesQuantity = maxActiveCoroutinesQuantity;
			_coroutineStarter = coroutineStarter;
			_queue = new Queue<IEnumerator>();
		}

		private void RunCoroutine(IEnumerator coroutineToRun)
		{
			if (currentlyActiveCoroutinesQuantity < _maxActiveCoroutinesQuantity)
			{
				var runner = CoroutineRunner(coroutineToRun);
				_coroutineStarter(runner);
			}
			else
				_queue.Enqueue(coroutineToRun);
		}

		private IEnumerator CoroutineRunner(IEnumerator coroutine)
		{
			currentlyActiveCoroutinesQuantity++;
			while (coroutine.MoveNext())
			{
				yield return coroutine.Current;
			}
			currentlyActiveCoroutinesQuantity--;
			if (_queue.Count > 0)
			{
				var next = _queue.Dequeue();
				RunCoroutine(next);
			}
		}
	}
}