using UnityEngine;
using System.Collections;

public class RingCursor : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;
	public int rotationSpeed;

	Transform cam;

	void Awake ()
	{
		cam = Camera.main.transform;
		transform.LookAt (cam.position);

	}


	public void SetCursor (float unitSize, Color colour)
	{
		transform.localScale = new Vector3 (unitSize / 10, unitSize / 10, unitSize / 10);

		spriteRenderer.color = colour;

	}


	void Update ()
	{		
		spriteRenderer.transform.Rotate (spriteRenderer.transform.up, rotationSpeed * Time.deltaTime, Space.Self);
		transform.LookAt (cam.position);

	}


	

}
