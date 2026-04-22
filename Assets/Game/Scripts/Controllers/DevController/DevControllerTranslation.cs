using UnityEngine;

public class DevControllerTranslation : MonoBehaviour
{
	[Header("Injections")]
	[SerializeField] private Rigidbody _rigidbody = default;

	[Header("Settings")]
	[SerializeField] private float _speed = default;
	[SerializeField] private float _sprintSpeedMultiplier = default;
	[SerializeField] private float _flySpeedMultiplier = default;
	[SerializeField] private float _jumpAmplitude = default;
	[SerializeField] private float _fallSpeed = default;

	//Variables
	//	Exposed
	[HideInInspector] public float XAxisTranslation = default;
	[HideInInspector] public float YAxisTranslation = default;
	[HideInInspector] public float ZAxisTranslation = default;
	[HideInInspector] public bool IsSprinting = default;
	[HideInInspector] public bool IsJumping = default;
	//	Private
	private Transform _transform;
	private bool _isTouchingFloor = default;
	private bool _isInFlyMode = default;

	private void Awake()
	{
		Construct();
	}

	private void Construct()
	{
		_transform = transform;
	}

	#region Setters
	public void SetIsJumpingBool(bool jumpInput)
	{
		if (jumpInput && _isTouchingFloor && !IsJumping && !_isInFlyMode)
			IsJumping = true;
	}

	public void SetFlyMode(bool flyInput)
	{
		if (flyInput && !_isInFlyMode)
			SwitchToFlyMode();
		else if (flyInput && _isInFlyMode)
			SwitchToWalkMode();
	}

	private void SwitchToFlyMode()
	{
		_isInFlyMode = true;
		_rigidbody.useGravity = false;
	}

	private void SwitchToWalkMode()
	{
		if (_isInFlyMode)
		{
			_isInFlyMode = false;
			_rigidbody.useGravity = true;
		}
	}
	#endregion
	#region FixedUpdate
	//FixedUpdate
	private void FixedUpdate()
	{
		Translate();
		Jump();
		Fall();
	}

	private void Translate()
	{
		_rigidbody.velocity = new Vector3(0f, _isInFlyMode ? 0f : _rigidbody.velocity.y, 0f);
		var direction = (_transform.right * XAxisTranslation) +
		                (_transform.up * (YAxisTranslation * (_isInFlyMode ? 1 : 0))) +
		                (_transform.forward * ZAxisTranslation);
		direction = direction.normalized;
		_rigidbody.AddForce(direction * 
		                    (_speed *
		                    (IsSprinting ? _sprintSpeedMultiplier : 1) *
		                    (_isInFlyMode ? _flySpeedMultiplier : 1 ) *
		                    Time.fixedDeltaTime),
							ForceMode.Impulse);
		//_rigidbody.MovePosition(transform.position + direction * _baseMovementSpeed * (_isSprinting ? _sprintSpeedMultiplier : 1) * Time.fixedDeltaTime);
	}

	private void Jump()
	{
		if (IsJumping && _isTouchingFloor && !_isInFlyMode)
		{
			IsJumping = false;
			_rigidbody.AddForce(transform.up * _jumpAmplitude, ForceMode.Impulse);
		}
	}

	private void Fall()
	{
		if (!_isInFlyMode && _rigidbody.velocity.y < 0f)
			_rigidbody.velocity += Vector3.up * (Physics.gravity.y * (_fallSpeed - 1) * Time.fixedDeltaTime);
	}
	#endregion
	#region OnTriggerStay
	//OnTriggerStay
	private void OnTriggerStay(Collider other)
	{
		if (!_isTouchingFloor)
			_isTouchingFloor = true;
	}
	#endregion
	#region OnTriggerExit
	//OnTriggerExit
	private void OnTriggerExit(Collider other)
	{
		if (_isTouchingFloor)
			_isTouchingFloor = false;
	}
	#endregion
}
