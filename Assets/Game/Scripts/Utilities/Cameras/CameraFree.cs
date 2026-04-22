using UnityEngine;

public class CameraFree : MonoBehaviour
{
	//Adjustables
	[SerializeField] private float _movementSpeed = 10f;
	[SerializeField] private float _movementSpeedUp = 2f;
	[SerializeField] private float _movementSpeedDown = 0.5f;
	[SerializeField] private bool _lockMouseCursor = default;

	//Variables
	//AxisMovements
	private float _xAxisMovement;
	private float _yAxisMovement;
	private float _zAxisMovement;
	//AxisRoations
	private float _xAxisRotation;
	private float _yAxisRotation;
	//Movement Speed Multiplier
	private float _baseMovementSpeedModifier = 1f;
	private float _currentMovementSpeedModifier = 1f;

	//Awake
	private void Awake()
	{
		SetRotationVariablesToCurrentObjectRotation();
	}

	private void SetRotationVariablesToCurrentObjectRotation()
	{
		_xAxisRotation = transform.eulerAngles.x;
		_yAxisRotation = transform.eulerAngles.y;
	}

	//OnEnable
	private void OnEnable()
	{
		HandleCursor();
	}

	private void HandleCursor()
	{
		if (_lockMouseCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	//Update
	private void Update()
	{
		HandleMovement();
		HandleRotation();
	}

	private void HandleMovement()
	{
		SetAxisMovements();
		SetMovementSpeedMultiplier();
		Move();
	}

	private void SetAxisMovements()
	{
		HandleXAxisInputs();
		HandleYAxisInputs();
		HandleZAxisInputs();
	}

	private void HandleXAxisInputs()
	{
		if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
			_xAxisMovement = -1;
		else if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
			_xAxisMovement = 1;
		else
			_xAxisMovement = 0;
	}

	private void HandleYAxisInputs()
	{
		if (Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.E))
			_yAxisMovement = -1;
		else if (!Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.E))
			_yAxisMovement = 1;
		else
			_yAxisMovement = 0;
	}

	private void HandleZAxisInputs()
	{
		if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
			_zAxisMovement = -1;
		else if (!Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.W))
			_zAxisMovement = 1;
		else
			_zAxisMovement = 0;
	}

	private void SetMovementSpeedMultiplier()
	{
		if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space))
			_currentMovementSpeedModifier = _movementSpeedUp;
		else if (!Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Space))
			_currentMovementSpeedModifier = _movementSpeedDown;
		else
			_currentMovementSpeedModifier = _baseMovementSpeedModifier;
	}

	private void Move()
	{
		transform.Translate(new Vector3(_xAxisMovement, _yAxisMovement, _zAxisMovement) *
										(_movementSpeed * _currentMovementSpeedModifier) *
										Time.deltaTime);
	}

	private void HandleRotation()
	{
		SetAxisRotations();
		Rotate();
	}

	private void SetAxisRotations()
	{
		_xAxisRotation -= Input.GetAxis("Mouse Y");
		_yAxisRotation += Input.GetAxis("Mouse X");
	}

	private void Rotate()
	{
		transform.eulerAngles = new Vector3(_xAxisRotation, _yAxisRotation, 0.0f);
	}
}
