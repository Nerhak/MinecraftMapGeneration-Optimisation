using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
	where T : class
{
	private static T _i;

	public static T I
	{
		get
		{
			if (_i == null)
			{
				_i = FindObjectOfType(typeof(T)) as T;
				if (_i == null)
					Debug.LogError("MonoSingleton<Class>: Could not found GameObject of type " + typeof(T).Name);
			}
			return _i;
		}
	}
}