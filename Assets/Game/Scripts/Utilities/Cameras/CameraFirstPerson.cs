using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFirstPerson : MonoBehaviour
{
	//Injections
	[Header("Injections")]
	[SerializeField] private Transform _follow = default;

	//Adjustables
	[Header("Settings")]
	[SerializeField] private Vector3 _offsetPosition = new Vector3(0f, 0.8f, 0f);

	//Variables
	private float _xAxisRotation = default;
	private float _yAxisRotation = default;

	//Update
	#region Update
	private void Update()
	{
		HandleRotationInputs();
		HandleRotationClamping();
	}

	private void HandleRotationInputs()
	{
		_xAxisRotation -= Input.GetAxis("Mouse Y");
		_yAxisRotation += Input.GetAxis("Mouse X");
	}

	private void HandleRotationClamping()
	{
		if (_xAxisRotation < -90f)
			_xAxisRotation = -90f;
		else if (_xAxisRotation > 90f)
			_xAxisRotation = 90f;
	}
	#endregion
	//LateUpdate
	#region LateUpdate
	private void LateUpdate()
	{
		FollowTarget();
		HandleRotation();
	}
	private void FollowTarget()
	{
		transform.position = _follow.position + _offsetPosition;
	}

	private void HandleRotation()
	{
		transform.eulerAngles = new Vector3(_xAxisRotation, _yAxisRotation, 0.0f);
	}
	#endregion
}
