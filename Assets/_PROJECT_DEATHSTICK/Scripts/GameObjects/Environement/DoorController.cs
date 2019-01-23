using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DoorController : MonoBehaviour {

	/**********************/
	/* VISIBLE PROPERTIES */
	/**********************/

	[Header("Debug")]
	[SerializeField]
	private bool _drawDebug = true;

	[Header("Gameplay")]
	[SerializeField]
	private float _activationDistance = 1;
	public float ActivationDistance { get { return _activationDistance; } }
	[SerializeField]
	private Vector3 _centerOverride = new Vector3();

	[Header("Animations")]
	[SerializeField]
	private AnimationClip _openAnimation;
	[SerializeField]
	private AnimationClip _unlockAnimation;

	/**************************/
	/* NON VISIBLE PROPERTIES */
	/**************************/

	private Animation _animComponent;

	private float animTimer = -1.0f;

	private bool _isOpen = false;

	private void Awake()
	{
		_animComponent = GetComponent<Animation>();
	}

	public void OpenDoor()
	{
		animTimer = _openAnimation.length;
		_animComponent.Play(_openAnimation.name);
		_isOpen = true;
	}

	public void UnlockDoor()
	{
		StartCoroutine(Unlock());
	}

	private void OnDrawGizmos()
	{
		if (_drawDebug)
		{
			Handles.DrawWireDisc(transform.position + _centerOverride, Vector3.up, _activationDistance);
		}
	}

	private IEnumerator Open()
	{
		Debug.Log(_animComponent);

		if (_isOpen) yield return null;
		_animComponent.Play(_openAnimation.name);
	}

	private IEnumerator Unlock()
	{
		_animComponent.Play(_unlockAnimation.name);
		if (_animComponent.isPlaying)
		{
			yield return new WaitForEndOfFrame();
		}
	}
}
