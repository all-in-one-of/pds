using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Pickable : MonoBehaviour {

	[SerializeField]
	[Range(0,5)]
	protected float _pickRadius = 0.5f;
	public float PickRadius { get { return _pickRadius; } }

	virtual public void OnPick()
	{
		gameObject.SetActive(false);
	}

	private void OnDrawGizmos()
	{
		Handles.DrawWireDisc(transform.position, Vector3.up, _pickRadius);
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
			OnPick();
	}
}
