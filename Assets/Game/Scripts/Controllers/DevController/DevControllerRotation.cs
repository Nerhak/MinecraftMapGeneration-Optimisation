using UnityEngine;

public class DevControllerRotation : MonoBehaviour
{
	[Header("Injections")]
	[SerializeField] private Rigidbody _rigidbody = default;

	//Variables
	[HideInInspector] public float YAxisRotation = default;

	#region FixedUpdate
	//FixedUpdate
	private void FixedUpdate()
	{
		Rotate();
	}

	private void Rotate()
	{
		_rigidbody.MoveRotation(Quaternion.Euler(0f, YAxisRotation, 0f));
	}
	#endregion
}
