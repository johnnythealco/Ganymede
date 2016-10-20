using UnityEngine;
using System.Collections;

public class BattlePlane : MonoBehaviour {

    public delegate void BattlePlane_point(Vector3 _point);
    public delegate void BattlePlane_Unit(Unit _unit);


    public event BattlePlane_point onClickPoint;
    public event BattlePlane_point onRightClickPoint;
    public event BattlePlane_Unit onClickUnit;
    public event BattlePlane_Unit onRightClickUnit;



    Vector3 worldPosition;


    void Update()
    {
        if (Battle.StrategicView)
        {
            getStrategicMouseInput();
        }
        else
        {
            getGameMouseInput();
        }

    }

    void getStrategicMouseInput()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())              
            return;

        if (Input.GetMouseButtonDown(0))
        {
            var ray = Battle.Manager.strategicCamera.Cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var layer = hit.collider.gameObject.layer;
                switch(layer)
                {
                    case 8:
                        worldPosition = this.transform.InverseTransformPoint(hit.point);
                        if (onClickPoint != null)
                            onClickPoint.Invoke(worldPosition);                        
                        break;
                    case 9:
                        var unit = hit.collider.gameObject.GetComponent<Unit>();
                        if (onClickUnit!= null)
                            onClickUnit.Invoke(unit);
                        break;
                }              
            }           
        }

        if (Input.GetMouseButtonDown(1))
        {
            var ray = Battle.Manager.strategicCamera.Cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var layer = hit.collider.gameObject.layer;
                switch (layer)
                {
                    case 8:
                        worldPosition = this.transform.InverseTransformPoint(hit.point);
                        if (onRightClickPoint != null)
                            onRightClickPoint.Invoke(worldPosition);
                        break;
                    case 9:
                        var unit = hit.collider.gameObject.GetComponent<Unit>();
                        if (onRightClickUnit != null)
                            onRightClickUnit.Invoke(unit);
                        break;
                }
            }

        }
    }

    void getGameMouseInput()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;
        int layerMask = 1 << 9;


        if (Input.GetMouseButtonDown(0))
        {
            var ray = Battle.Manager.GameCamera.Cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000f, layerMask))
            {
                var layer = hit.collider.gameObject.layer;
                switch (layer)
                {                    
                    case 9:
                        var unit = hit.collider.gameObject.GetComponent<Unit>();
                        if (onClickUnit != null)
                            onClickUnit.Invoke(unit);
                        break;
                }
            }
        }


        if (Input.GetMouseButtonDown(1))
        {
            var ray = Battle.Manager.GameCamera.Cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000f, layerMask))
            {
                var layer = hit.collider.gameObject.layer;
                switch (layer)
                {
                    case 9:
                        var unit = hit.collider.gameObject.GetComponent<Unit>();
                        if (onRightClickUnit != null)
                            onRightClickUnit.Invoke(unit);
                        break;
                }
            }
        }
    }


}
