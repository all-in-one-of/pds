using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culling : MonoBehaviour {

	public Vector3 _normal = new Vector3();
	public LevelManager LM;

	private MeshRenderer mr;

	private void Start()
	{
		mr = GetComponent<MeshRenderer>();
	}

	// Update is called once per frame
	void Update() {
		//float dotResult = Vector3.Dot(_normal - transform.position, LM.MainPlayerCamera.transform.position + transform.position);
		if (transform.position.x > LM.MainPlayerCamera.transform.position.x)
		{
			mr.enabled = false;
		}
		else
		{
			mr.enabled = true;
		}
	}
}
