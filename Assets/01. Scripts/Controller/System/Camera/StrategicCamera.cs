using UnityEngine;
using System.Collections;

public class StrategicCamera : MonoBehaviour
{

	public Unit Unit_Focus;

	[SerializeField]
	float ScrollSpeed = 200f;
	[SerializeField]
	float ScrollEdge = 0.01f;
	public float zoomSpeed = 10f;
	public float minZoom = 50;
	public float maxZoom = 1000;
	public float dragSpeed = 0.8f;
	private Vector3 dragOrigin;

	public Camera Cam { get; set; }

	public static StrategicCamera camController;

	public Vector3 Focus { get; set; }

	public bool screenEdgeMoveEnabled;

	Vector3 offSet;
	Quaternion InitRotation;
	float InitalFOV;


	void Awake ()
	{
		Cam = this.GetComponent<Camera> ();
		offSet = transform.position;
		InitRotation = transform.rotation;
		InitalFOV = Cam.fieldOfView;

		if (camController == null)
		{
			camController = this;
		}
	}

	void Update ()
	{
		if (!Cam.enabled)
			return;
		updateFocus ();

       
		if (Input.GetKeyDown (KeyCode.F1))
			screenEdgeMoveEnabled = !screenEdgeMoveEnabled;

	
       

		#region Movement
		if (Input.GetKey ("d"))
		{
			transform.Translate (transform.right * Time.deltaTime * ScrollSpeed, Space.World);

		} else if (Input.GetKey ("a"))
		{
			transform.Translate (transform.right * Time.deltaTime * -ScrollSpeed, Space.World);

		}

		if (Input.GetKey ("w"))
		{
			transform.Translate (transform.up * Time.deltaTime * ScrollSpeed, Space.World);

		} else if (Input.GetKey ("s"))
		{
			transform.Translate (transform.up * Time.deltaTime * -ScrollSpeed, Space.World);

		}

		if (screenEdgeMoveEnabled)
		{
			if (Input.mousePosition.x >= Screen.width * (1 - ScrollEdge))
			{
				transform.Translate (transform.right * Time.deltaTime * ScrollSpeed, Space.World);

			} else if (Input.mousePosition.x <= Screen.width * ScrollEdge)
			{
				transform.Translate (transform.right * Time.deltaTime * -ScrollSpeed, Space.World);

			}

			if (Input.mousePosition.y >= Screen.height * (1 - ScrollEdge))
			{
				transform.Translate (transform.up * Time.deltaTime * ScrollSpeed, Space.World);

			} else if (Input.mousePosition.y <= Screen.height * ScrollEdge)
			{
				transform.Translate (transform.up * Time.deltaTime * -ScrollSpeed, Space.World);

			}
		}
		#endregion

		#region Roation


		if (Input.GetKey ("e"))
		{
			transform.RotateAround (Focus, -transform.forward, dragSpeed + 20f * Time.deltaTime);
		}

		if (Input.GetKey ("q"))
		{
			transform.RotateAround (Focus, transform.forward, dragSpeed + 20f * Time.deltaTime);
		}


		if (Input.GetMouseButtonDown (1))
		{
			dragOrigin = Input.mousePosition;
			return;
		}

		if (Input.GetKey ("mouse 1"))
		{
			Vector3 pos = Input.mousePosition - dragOrigin;
			var verticalRotation = pos.y * dragSpeed;
			transform.RotateAround (Focus, -transform.right, verticalRotation * Time.deltaTime);
			var HorizontalRotation = pos.x * dragSpeed;
			transform.RotateAround (Focus, Vector3.up, HorizontalRotation * Time.deltaTime);

		}

		#endregion

		#region Zoom
		var scroll = Input.GetAxis ("Mouse ScrollWheel");
		if (scroll > 0f)
		{
			var heading = transform.position - Focus;
			var distance = heading.magnitude;
			var direction = heading / distance;
			var zoomTarget = Focus + (direction * minZoom);
			Vector3 newPosition = Vector3.MoveTowards (transform.position, zoomTarget, zoomSpeed);
			transform.position = newPosition;
		} else if (scroll < 0f)
		{
			var heading = transform.position - Focus;
			var distance = heading.magnitude;
			var direction = heading / distance;
			var zoomTarget = Focus + (direction * maxZoom);
			Vector3 newPosition = Vector3.MoveTowards (transform.position, zoomTarget, zoomSpeed);
			transform.position = newPosition;
		}

		#endregion




	}

	void updateFocus ()
	{
		var ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, 10000))
		{
			var layer = hit.collider.gameObject.layer;
			switch (layer)
			{
			case 8:
				Focus = hit.point;                    
				break;
			case 9:
				Unit_Focus = hit.transform.gameObject.GetComponent<Unit> ();
				Focus = hit.transform.position;                    
				break;
			}
		}
	}

	public void CentreOnPoint (Vector3 point)
	{       
		var currentoffSet = transform.position - Focus;
		transform.position = point + currentoffSet;
		Focus = point;
	}

	public void ResetOn (Vector3 point)
	{
		this.transform.position = point + offSet;

		transform.rotation = InitRotation;
		Cam.fieldOfView = 50.0f;
		Focus = point;
	}

	public void ResetOnUnit (Unit _unit)
	{
		Unit_Focus = _unit;

		float offset = _unit.Size * 10f;
		Vector3 posOffset = new Vector3 (0, offset, -offset);
     
		transform.position = Unit_Focus.transform.position + posOffset;
		transform.LookAt (Unit_Focus.transform.position);      

		Cam.fieldOfView = InitalFOV;

      
	}

	public void CenterOnUnitFocus ()
	{ 
		float offset = Unit_Focus.Size * 2f;
		Vector3 posOffset = new Vector3 (0, offset, -offset);

		transform.position = Unit_Focus.transform.position + posOffset;
		transform.LookAt (Unit_Focus.transform.position);

		Cam.fieldOfView = InitalFOV;


	}


}
