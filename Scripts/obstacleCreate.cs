using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleCreate : MonoBehaviour {

	public float distance = 51f;//distance between camera and the plane
	public Vector3 m_Size = new Vector3(2,2,2);
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
        {
			CastRayToWorld();
			
		}
	}
	
	void CastRayToWorld() 
	{
		float duration = 10f;
		
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   		
		Vector3 clickPt = ray.origin + (ray.direction * distance);    
		
		
		Debug.Log( "World point " + clickPt );//debug feature
		
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(clickPt.x, 1, clickPt.z);
		cube.transform.localScale = m_Size;
		GameObject.Destroy(cube, duration);
		

	}
	
	
	
}
