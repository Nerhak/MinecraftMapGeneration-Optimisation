using UnityEngine;

public class DevControllerCamera : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private Transform _camera = default;
	[SerializeField] private Transform _target = default;
	[SerializeField] private Vector3 _offsetPosition = default;

	//Variables
	//	Exposed
	public Transform Camera => _camera;
	//	Private
	private float _xAxisRotation = default;
	private float _yAxisRotation = default;

	//LateUpdate
	#region LateUpdate
	private void LateUpdate()
	{
		FollowTarget();
		HandleRotation();
	}

	private void FollowTarget()
	{
		_camera.position = _target.position + _offsetPosition;
	}

	private void HandleRotation()
	{
		_camera.eulerAngles = new Vector3(_xAxisRotation, _yAxisRotation, 0.0f);
	}
	#endregion
	#region Setters
	//Setters
	public void SetAxisRotationValues(float xAxisRotation, float yAxisRotation)
	{
		//Set
		_xAxisRotation -= xAxisRotation;
		_yAxisRotation += yAxisRotation;
		//Clamp
		if (_xAxisRotation < -90f)
			_xAxisRotation = -90f;
		else if (_xAxisRotation > 90f)
			_xAxisRotation = 90f;
	}
	#endregion
}
