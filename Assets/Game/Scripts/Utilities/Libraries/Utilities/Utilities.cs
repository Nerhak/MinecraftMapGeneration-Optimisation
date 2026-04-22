namespace Library.Utilities
{
	using UnityEngine;

	public class Utilities
	{
		public static void SwitchGameObjectActiveState(GameObject gameObject)
		{
			gameObject.SetActive(!gameObject.activeSelf);
		}

		public static void UpdateGameObjectActivationState(GameObject gameObject, bool shouldBeActive)
		{
			if (shouldBeActive && !gameObject.activeSelf)
				SwitchGameObjectActiveState(gameObject);
			else if (!shouldBeActive && gameObject.activeSelf)
				SwitchGameObjectActiveState(gameObject);
		}

		public static void QuitApplication()
		{
			#if (UNITY_EDITOR)
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}
	}
}