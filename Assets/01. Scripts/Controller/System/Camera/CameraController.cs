using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    [SerializeField]
    float ScrollSpeed = 15f;
    [SerializeField]
    float ScrollEdge = 0.01f;
 
 
    public float zoomSpeed = 10f;
    public float minZoomFOV = 2;
    public float maxZoomFOV = 90;


    public float dragSpeed = 0.25f;
    public float OrbitSpeed = 5f;        
    private Vector3 dragOrigin;

    public Camera Cam { get; set; }
    public static CameraController camController;
    public Vector3 Focus { get; set; }

    public bool TopDown;
    public bool TargetCam;
    public bool ActyiveUnitCam;
    public bool screenEdgeMoveEnabled;

   Vector3 offSet;
   Quaternion InitRotation;
   float InitalFOV;
   public bool continueOrbit { get; set; }

    void Awake()
    {
        Cam = this.GetComponent<Camera>();
        offSet = transform.position;
        InitRotation = transform.rotation;
        InitalFOV = Cam.fieldOfView;

        if (camController == null)
        {
            camController = this;
        }
        Focus = new Vector3(0f,0f,0f);
    }


    void Update()
    {
        if (!Cam.enabled)
            return;

        if (Input.GetKeyDown(KeyCode.O))
        {
            continueOrbit = !continueOrbit;
        }

        if(!TargetCam && !TopDown)
        {
            if (Input.GetKeyDown(KeyCode.R))
                StartCoroutine(horizontalOrbit());
        }

        if (TargetCam)
            return;

        if (Input.GetKeyDown(KeyCode.F1))
            screenEdgeMoveEnabled = !screenEdgeMoveEnabled;

        #region Movement
        if (Input.GetKey("d"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
        }
        else if (Input.GetKey("a"))
        {
            transform.Translate(Vector3.right * Time.deltaTime * -ScrollSpeed, Space.World);
        }

        if (Input.GetKey("w") )
        {
            transform.Translate(Vector3.forward * Time.deltaTime * ScrollSpeed, Space.World);
        }
        else if (Input.GetKey("s"))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * -ScrollSpeed, Space.World);
        }

        if (screenEdgeMoveEnabled)
        {
            if (Input.mousePosition.x >= Screen.width * (1 - ScrollEdge))
            {
                transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.mousePosition.x <= Screen.width * ScrollEdge)
            {
                transform.Translate(Vector3.right * Time.deltaTime * -ScrollSpeed, Space.World);
            }

            if (Input.mousePosition.y >= Screen.height * (1 - ScrollEdge))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * ScrollSpeed, Space.World);
            }
            else if (Input.mousePosition.y <= Screen.height * ScrollEdge)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * -ScrollSpeed, Space.World);
            }
        }
        #endregion

        #region Roation

     
            if (Input.GetKey("e"))
            {
                transform.RotateAround(Focus, -transform.forward, dragSpeed + 20f * Time.deltaTime);
            }

            if (Input.GetKey("q"))
            {
                transform.RotateAround(Focus, transform.forward, dragSpeed + 20f * Time.deltaTime);
            }


            if (Input.GetMouseButtonDown(1))
            {
                dragOrigin = Input.mousePosition;
                return;
            }

            if (Input.GetKey("mouse 1"))
            {
                Vector3 pos = Input.mousePosition - dragOrigin;

                var verticalRotation = pos.y * dragSpeed;


                transform.RotateAround(Focus, -transform.right, verticalRotation * Time.deltaTime);



                var HorizontalRotation = pos.x * dragSpeed;


                transform.RotateAround(Focus, Vector3.up, HorizontalRotation * Time.deltaTime);

            }
        
        #endregion

        #region Zoom
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            ZoomIn();
        }
        else if (scroll < 0f)
        {
            ZoomOut();
        }

        #endregion




    }

    public void ZoomIn()
    {
        Cam.fieldOfView -= zoomSpeed;
        if (Cam.fieldOfView < minZoomFOV)
        {
            Cam.fieldOfView = minZoomFOV;
        }
    }

    public void ZoomOut()
    {
        Cam.fieldOfView += zoomSpeed;
        if (Cam.fieldOfView > maxZoomFOV)
        {
            Cam.fieldOfView = maxZoomFOV;
        }
    }


    public void CentreOnPoint(Vector3 point)
    {
        if (!TopDown)
            return;
     
        var currentoffSet = transform.position - Focus;
        transform.position = point + currentoffSet;
        Focus = point;
    }



    public void ResetOn(Vector3 point)
    {
        this.transform.position = point + offSet;

        transform.rotation = InitRotation;
        Cam.fieldOfView = 50.0f;
        Focus = point;
    }

    public void ResetOnUnit(Unit _unit)
    {
        if (!TopDown)
        {
            transform.position = _unit.transform.position;
            transform.forward = _unit.transform.forward;
            transform.SetParent(_unit.transform);
            transform.localPosition = offSet;
            transform.LookAt(_unit.transform.position);
        }
        else if(TopDown)
        {
            transform.position = _unit.transform.position + offSet;
            transform.rotation = InitRotation;
        }




        Cam.fieldOfView = InitalFOV;
        Focus = _unit.transform.position;

        if (TargetCam && !continueOrbit)
        {
            HorizontalOrbit();
        }
    }

    void HorizontalOrbit()
    {
        continueOrbit = true;
        StartCoroutine(horizontalOrbit()); 

    }

    IEnumerator horizontalOrbit()
    {
        while(continueOrbit)
        { 
            transform.RotateAround(Focus, Vector3.up, OrbitSpeed * Time.deltaTime);
            yield return null;
        }

    }
}
