namespace Library.Time
{
	using UnityEngine;

	public class TimeFrame
	{
		public static bool TimeFrameReached(float timeFrame)
		{
			return Time.time >= timeFrame;
		}

		public static float ReturnNextTimeFrame(float timeBetweenTwoTimeFrames)
		{
			return (Time.time + timeBetweenTwoTimeFrames);
		}

		public static void SetNextTimeFrame(ref float nextTimeFrame, float timeBetweenTwoTimeFrames)
		{
			nextTimeFrame = Time.time + timeBetweenTwoTimeFrames;
		}

		public static bool SetNextTimeFrameIfElapsed(ref float nextTimeFrame, float timeBetweenTwoTimeFrames)
		{
			if (TimeFrameReached(nextTimeFrame))
			{
				SetNextTimeFrame(ref nextTimeFrame, timeBetweenTwoTimeFrames);
				return (true);
			}
			return (false);
		}
	}
}