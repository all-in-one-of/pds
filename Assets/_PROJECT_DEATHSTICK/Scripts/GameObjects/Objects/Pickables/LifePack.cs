using UnityEngine;
using System.Collections;

public class LifePack : Pickable
{
	public override void OnPick()
	{
		Debug.Log("Pick Child");

		base.OnPick();
	}
}
