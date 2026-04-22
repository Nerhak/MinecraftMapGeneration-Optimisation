//#if false
using UnityEngine;

namespace Legacy
{
	public class DevControllerInputs : MonoBehaviour
	{
		[Header("Injections")]
		[SerializeField] private DevControllerTranslation _translation = default;
		[SerializeField] private DevControllerRotation _rotation = default;
		[SerializeField] private DevControllerCamera _camera = default;
		[SerializeField] private DevControllerActions _actions = default;

		//Variables
		//	Keycodes
		private const KeyCode _left			= KeyCode.A;
		private const KeyCode _forward		= KeyCode.W;
		private const KeyCode _right		= KeyCode.D;
		private const KeyCode _backward		= KeyCode.S;
		private const KeyCode _down			= KeyCode.LeftAlt;
		private const KeyCode _up			= KeyCode.Space;
		private const KeyCode _sprint		= KeyCode.LeftShift;
		private const KeyCode _jump			= KeyCode.Space;
		private const KeyCode _placeBlock	= KeyCode.Mouse1;
		private const KeyCode _destroyBlock = KeyCode.Mouse0;
		private const KeyCode _fly			= KeyCode.F;
		private const KeyCode _sudo			= KeyCode.LeftShift;

		#region Update
		//Update
		private void Update()
		{
			HandleTranslationInputs();
			HandleRotationInputs();
			HandleCameraRotationInputs();
			HandleBlockManipulationInputs();
		}

		private void HandleTranslationInputs()
		{
			//XAxis
			_translation.XAxisTranslation = GetAxisTranslation(_left, _right);
			//YAxis
			_translation.YAxisTranslation = GetAxisTranslation(_down, _up);
			//ZAxis
			_translation.ZAxisTranslation = GetAxisTranslation(_backward, _forward);
			//Sprint
			_translation.IsSprinting = Input.GetKey(_sprint);
			//Jump
			_translation.SetIsJumpingBool(Input.GetKeyDown(_jump));
			//Fly
			_translation.SetFlyMode(Input.GetKeyDown(_fly));
		}

		private static float GetAxisTranslation(KeyCode negativeInput, KeyCode positiveInput)
		{
			if ((Input.GetKey(negativeInput) && !Input.GetKey(positiveInput)))
				return (-1);
			else if (Input.GetKey(positiveInput) && !Input.GetKey(negativeInput))
				return (1);
			return (0);
		}

		private void HandleRotationInputs()
		{
			_rotation.YAxisRotation += Input.GetAxis("Mouse X");
		}

		private void HandleCameraRotationInputs()
		{
			_camera.SetAxisRotationValues(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
		}

		private void HandleBlockManipulationInputs()
		{
			if (Input.GetKeyDown(_placeBlock))
				_actions.PlaceBlock();
			if (Input.GetKeyDown(_destroyBlock) && !Input.GetKey(_sudo))
				_actions.DamageBlock();
			if (Input.GetKeyDown(_destroyBlock) && Input.GetKey(_sudo))
				_actions.DestroyBlock();
		}
		#endregion
	}
}

//#endif