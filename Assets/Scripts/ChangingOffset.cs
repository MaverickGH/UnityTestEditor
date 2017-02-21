using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChangingOffset : MonoBehaviour {

	[SerializeField]
	public Vector2 screenPos;

	[SerializeField]
	public bool isScreenPoint;

	private Vector3 tempPosition;

	void Start(){

	}

	void Update () {

	}

	public void ConvertScreenToWorldPoint(Vector2 screenPosition)
	{
		Vector3 tempPosition = new Vector3 (Camera.main.ScreenToWorldPoint (screenPosition).x,
			Camera.main.ScreenToWorldPoint(screenPosition).y, 0);

		this.transform.position = tempPosition;
	
	}

	public void ConvertWorldToScreenPoint()
	{
		tempPosition = new Vector3 (Camera.main.WorldToScreenPoint(this.transform.position).x,
			Camera.main.WorldToScreenPoint(this.transform.position).y, 0);

		screenPos.x = tempPosition.x;
		screenPos.y = tempPosition.y;
	}
}
