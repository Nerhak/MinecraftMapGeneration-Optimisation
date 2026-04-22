using Unity.Mathematics;
using UnityEngine;

namespace Library.Legacy
{
	#region PerlinNoise
	public static class PerlinNoise
	{
		private const float _perlinNoiseMinReturnValue = 0;
		private const float _perlinNoiseMaxReturnValue = 1;

		public static float FractalBrownianMotion2D(float x, float z, float frequency, float amplitude, int octaves,
			float persistence)
		{
			float total = 0;
			float maxValue = 0;

			for (int i = 0; i < octaves; i++)
			{
				var perlinNoise = Mathf.PerlinNoise(x * frequency * amplitude, z * frequency * amplitude);
				total += perlinNoise;
				maxValue += amplitude;
				amplitude *= persistence;
				frequency *= 2;
			}
			return total / maxValue;
		}

		public static float FractalBrownianMotion3D(float x, float y, float z, float frequency, float amplitude,
			int octaves, float persistence)
		{
			float XY = FractalBrownianMotion2D(x, y, frequency, amplitude, octaves, persistence);
			float YZ = FractalBrownianMotion2D(y, z, frequency, amplitude, octaves, persistence);
			float XZ = FractalBrownianMotion2D(x, z, frequency, amplitude, octaves, persistence);

			float YX = FractalBrownianMotion2D(y, x, frequency, amplitude, octaves, persistence);
			float ZY = FractalBrownianMotion2D(z, y, frequency, amplitude, octaves, persistence);
			float ZX = FractalBrownianMotion2D(z, x, frequency, amplitude, octaves, persistence);

			return ((XY + YZ + XZ + YX + ZY + ZX) / 6);
		}

		private static float MapToNewRange(float newRangeMin, float newRangeMax, float fractalBrownianMotion)
		{
			return (Mathf.Lerp(newRangeMin, newRangeMax, Mathf.InverseLerp(
				_perlinNoiseMinReturnValue,
				_perlinNoiseMaxReturnValue,
				fractalBrownianMotion)));
		}

		public static uint GenerateBiomeWorldSurfaceHeightAtWorldPositionXZ(float x, float z, float surfaceHeightMin,
			float surfaceHeightMax, float surfaceFrequency, float surfaceAmplitude, int surfaceOctaves,
			float surfacePersistence)
		{
			float height = MapToNewRange(surfaceHeightMin, surfaceHeightMax,
				FractalBrownianMotion2D(
					x,
					z,
					surfaceFrequency,
					surfaceAmplitude,
					surfaceOctaves,
					surfacePersistence));
			return (uint)height;
		}
	}
	#endregion
	public static class PerlinNoiseMultiThread
	{
		public static int GenerateBiomeWorldSurfaceHeightAtWorldPositionXZ(float bWPX, float bWPZz, float sHMin, float sHMax,
			float sFreq, float sAmp, float sOct, float sPers)
		{
			return (int)MapToNewRange(sHMin, sHMax, FractalBrownianMotion2D(bWPX, bWPZz, sFreq, sAmp, sOct, sPers));
		}

		public static float MapToNewRange(float newRangeMin, float newRangeMax, float fractalBrownianMotion)
		{
			return (math.lerp(newRangeMin, newRangeMax, math.unlerp(-1, 1, fractalBrownianMotion)));
		}

		public static float FractalBrownianMotion2D(float bWPX, float bWPZ, float freq, float amp, float oct, float pers)
		{
			float total = 0;
			float maxValue = 0;

			for (int i = 0; i < oct; i++)
			{
				var perlinNoise = noise.cnoise(new float2(bWPX * freq * amp, bWPZ * freq * amp));
				total += perlinNoise;
				maxValue += amp;
				amp *= pers;
				freq *= 2;
			}
			return total / maxValue;
		}

		public static float FractalBrownianMotion3D(float x, float y, float z, float freq, float amp,
			float oct, float pers)
		{
			float XY = MapToNewRange(0f, 1f, FractalBrownianMotion2D(x, y, freq, amp, oct, pers));
			float YZ = MapToNewRange(0f, 1f, FractalBrownianMotion2D(y, z, freq, amp, oct, pers));
			float XZ = MapToNewRange(0f, 1f, FractalBrownianMotion2D(x, z, freq, amp, oct, pers));

			float YX = MapToNewRange(0f, 1f, FractalBrownianMotion2D(y, x, freq, amp, oct, pers));
			float ZY = MapToNewRange(0f, 1f, FractalBrownianMotion2D(z, y, freq, amp, oct, pers));
			float ZX = MapToNewRange(0f, 1f, FractalBrownianMotion2D(z, x, freq, amp, oct, pers));

			return ((XY + YZ + XZ + YX + ZY + ZX) / 6);
		}
	}
}