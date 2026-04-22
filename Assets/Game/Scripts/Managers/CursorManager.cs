using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
	[SerializeField] private bool _lockMouseCursor = default;

	private void OnEnable()
	{
		LockAndHideCursor();
	}

	private void LockAndHideCursor()
	{
		if (_lockMouseCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	private void OnDisable()
	{
		if (_lockMouseCursor)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
