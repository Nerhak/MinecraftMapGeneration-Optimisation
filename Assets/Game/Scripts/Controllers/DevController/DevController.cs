using UnityEngine;

public class DevController : MonoBehaviour
{
	[Header("Injections")]
	[SerializeField] private DevControllerCamera _camera = default;

	#region Awake
	//Awake
	private void Awake()
	{
		Construct();
	}

	private void Construct()
	{
		UnlinkCamera();
	}

	private void UnlinkCamera()
	{
		_camera.Camera.parent = default;
	}
	#endregion
}
