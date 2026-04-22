namespace Library.SceneManagement
{
	using UnityEngine.SceneManagement;

	public class SceneManagement
	{
		public static int ReturnCurrentSceneBuildIndex()
		{
			return (SceneManager.GetActiveScene().buildIndex);
		}

		public static void LoadSceneByBuildIndex(int buildIndex)
		{
			SceneManager.LoadScene(buildIndex);
		}

		public static void LoadSceneByName(string sceneName)
		{
			SceneManager.LoadScene(sceneName);
		}

		public static void ReloadCurrentScene()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public static void LoadNextSceneInBuildIndex()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}
}