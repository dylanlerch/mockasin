using System;
using System.Threading;

namespace Mockasin.Services
{
	public interface IRandomService
	{
		int GetRandomIntegerInclusive(int min, int max);
	}

	/// <summary>
	/// Injectable class for generating thread-safe random values.
	/// 
	/// As with most things, thanks Jon Skeet for the implementation on this one.
	/// Read the post: https://codeblog.jonskeet.uk/2009/11/04/revisiting-randomness/
	/// </summary>
	public class RandomService : IRandomService
	{
		private static readonly Random globalRandom = new Random();
		private static readonly object globalLock = new object();

		private static readonly ThreadLocal<Random> threadRandom = new ThreadLocal<Random>(NewRandom);

		private static Random NewRandom()
		{
			lock (globalLock)
			{
				return new Random(globalRandom.Next());
			}
		}

		private static Random Instance { get { return threadRandom.Value; } }

		/// <summary>
		/// Returns a thread-safe random integer between two given values
		/// (inclusive).
		/// </summary>
		/// <param name="minValue">Minimum (inclusive) value</param>
		/// <param name="maxValue">Maximum (inclusive) value</param>
		/// <returns></returns>
		public int GetRandomIntegerInclusive(int minValue, int maxValue)
		{
			return Instance.Next(minValue, maxValue + 1);
		}
	}
}