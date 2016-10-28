using UnityEngine;
using System.Collections.Generic;

using System.Linq;
using System;
using System.Collections;

[RequireComponent (typeof(BattlePlane))]
public class Battle : MonoBehaviour
{
	#region Variables

	#region public

	public RingCursor ringCursor;
	public Unit ActiveUnit;
	public ActiveUnitDisplay activeUnitDisplay;
	public TargetDisplay targetDisplay;
	public StrategicCamera strategicCamera;
	public CameraController GameCamera;
	public CameraController TargetCam;

	#endregion

	#region Properties

	public BattlePlane battlePlane { get; set; }

	public static Battle Manager { get; set; }

	public static List<Unit> AllUnits{ get; set; }

	public static List<Unit> VisibleUnits { get; set; }

	public static bool StrategicView { get; set; }

	#endregion

	#region private



	#endregion

	#endregion

	void Awake ()
	{
		if (Manager == null)
		{
			Manager = this;
		} else if (Manager != this)
			Destroy (gameObject);

		AllUnits = new List<Unit> ();
		VisibleUnits = new List<Unit> ();
		battlePlane = GetComponent<BattlePlane> ();
		Game.BattleManager = this;
		Game.PlayerName = "Player";

	}

       
	void Start ()
	{      
		//StrategyView();
		battlePlane.onClickPoint += BattlePlane_onClickPoint;
		battlePlane.onRightClickPoint += BattlePlane_onRightClickPoint;
		battlePlane.onClickUnit += BattlePlane_onClickUnit;
		battlePlane.onRightClickUnit += BattlePlane_onRightClickUnit;           

	}

	public void StartBattle ()
	{
		foreach (var unit in AllUnits)
		{
			if (unit.state.Owner == "Player")
			{
				if (ActiveUnit == null)
					selectUnit (unit);
			}
		}
		RangeFinding ();
		selectTarget (getClosestTarget (TargetType.enemy));
	}

	private void BattlePlane_onClickUnit (Unit _unit)
	{
		switch (_unit.targetType)
		{
		case TargetType.ally:
			selectUnit (_unit);
			break;
		case TargetType.enemy:
			selectTarget (_unit);                
			break;
		}
	}

	private void BattlePlane_onRightClickUnit (Unit _unit)
	{
		switch (_unit.targetType)
		{
		case TargetType.ally:                
			break;
		case TargetType.enemy:
			selectTarget (_unit);
			ActiveUnit.Fire (ActiveUnit.ActiveTarget);
			break;
		}
	}

	private void BattlePlane_onRightClickPoint (Vector3 _point)
	{
		//ActiveUnit.FireSelectedWeapon(_point);        
	}

	private void BattlePlane_onClickPoint (Vector3 _point)
	{
		if (ActiveUnit != null)
		{
			var heading = _point - ActiveUnit.transform.position;
			var distance = heading.magnitude;
			int layerMask = 1 << 9;
			RaycastHit hit;

			if (Physics.Raycast (ActiveUnit.transform.position, heading, out hit, distance, layerMask))
			{
				var _unit = hit.collider.gameObject.GetComponent<Unit> ();
				Debug.Log ("Path Blocked by " + _unit.DsiplayName);
				return;
			}
			ActiveUnit.Move (_point);
		}
	}


	void StrategyView ()
	{
		strategicCamera.Cam.enabled = true;
		StrategicView = true;
		GameCamera.Cam.enabled = false;
		TargetCam.Cam.enabled = false;
		targetDisplay.gameObject.SetActive (false);


		if (ActiveUnit != null)
		{
			strategicCamera.ResetOnUnit (ActiveUnit);
			GameCamera.ResetOnUnit (ActiveUnit);

			if (ActiveUnit.ActiveTarget != null)
			{
				TargetCam.ResetOnUnit (ActiveUnit.ActiveTarget);
			}
		}
	}

	void selectUnit (Unit _unit)
	{
		ActiveUnit = _unit;
		activeUnitDisplay.Prime (ActiveUnit);
		RangeFinding ();
		strategicCamera.ResetOnUnit (ActiveUnit);

	}

	void selectTarget (Unit _unit)
	{
		ActiveUnit.ActiveTarget = _unit;
		ActiveUnit.transform.LookAt (ActiveUnit.ActiveTarget.transform.position);
		targetDisplay.Prime (_unit);
     
	}

	void fireAtTarget ()
	{
        

     
	}

	void SortUnits_Range (List<Unit> list, int left, int right)
	{
		if (left < right)
		{
			int pivotIdx = Partition_Range (list, left, right);

			if (pivotIdx > 1)
				SortUnits_Range (list, left, pivotIdx - 1);

			if (pivotIdx + 1 < right)
				SortUnits_Range (list, pivotIdx + 1, right);
		}

	}

	int Partition_Range (List<Unit> list, int left, int right)
	{
		Unit pivot = list [left];

		while (true)
		{
			while (list [left].RangeToTarget < pivot.RangeToTarget)
				left++;

			while (list [right].RangeToTarget > pivot.RangeToTarget)
				right--;

			if (list [right].RangeToTarget == pivot.RangeToTarget && list [left].RangeToTarget == pivot.RangeToTarget)
				left++;

			if (left < right)
			{
				Unit temp = list [left];
				list [left] = list [right];
				list [right] = temp;
			} else
			{
				return right;
			}
		}
	}

	public void RangeFinding ()
	{
		VisibleUnits = new List<Unit> ();
		foreach (var _target in AllUnits)
		{
			var distance = Vector3.Distance (ActiveUnit.transform.position, _target.transform.position);
			_target.RangeToTarget = distance;
			VisibleUnits.Add (_target);
		}
		SortUnits_Range (VisibleUnits, 0, VisibleUnits.Count () - 1);
	}

	public Unit getClosestTarget (TargetType _targetType)
	{
		for (int i = 0; i < VisibleUnits.Count (); i++)
		{
			var _Target = VisibleUnits [i];

			if (_Target.targetType == _targetType)
				return _Target;
		}
		return null;
	}

	public Unit NextTarget ()
	{

		var index = VisibleUnits.IndexOf (ActiveUnit.ActiveTarget);
		var _targetType = ActiveUnit.ActiveTarget.targetType;

		if (index < VisibleUnits.Count () - 1)
		{
			for (int i = index + 1; i < VisibleUnits.Count (); i++)
			{
				var _Target = VisibleUnits [i];

				if (_Target.targetType == _targetType)
					return _Target;
			}
		}

		return getClosestTarget (_targetType);

	}

	public Unit PrevTarget ()
	{

		var index = VisibleUnits.IndexOf (ActiveUnit.ActiveTarget);
		var _targetType = ActiveUnit.ActiveTarget.targetType;

		if (index > 0)
		{
			for (int i = index - 1; i >= 0; i--)
			{
				var _Target = VisibleUnits [i];

				if (_Target.targetType == _targetType)
					return _Target;
			}

			for (int i = VisibleUnits.Count () - 1; i > index; i--)
			{
				var _Target = VisibleUnits [i];

				if (_Target.targetType == _targetType)
					return _Target;
			}
		}

		return getClosestTarget (_targetType);
	}

	public Fleet CreateFleet (List<Unit> _Units, string _Owner, string _Name)
	{
		var obj = new GameObject ();
		var _fleet = obj.AddComponent<Fleet> ();
		_fleet.Owner = _Owner;
		_fleet.Name = _Name;
		_fleet.gameObject.name = "[Fleet] " + _Name;


		foreach (var unit in _Units)
		{
			var newUnit = Instantiate (unit);
			newUnit.gameObject.SetActive (false);
			newUnit.gameObject.transform.SetParent (_fleet.transform);
			newUnit.state = new UnitState (newUnit, _Owner);
			_fleet.Units.Add (newUnit);
			AllUnits.Add (newUnit);
			VisibleUnits.Add (newUnit);
		}
		_fleet.SortBySize ();

		return _fleet;
	}


	public void DeployFleet (Fleet _fleet, Vector3 _point)
	{

		int unitcount = _fleet.Units.Count ();
		float Buffer = 0f;
		Vector3 DeploymentPosition = _point;

		for (int i = unitcount - 1; i > -1; i--)
		{
			var _unit = _fleet.Units [i];
			_unit.transform.position = DeploymentPosition;
			AddRingCursor (_unit);
			Buffer = Buffer + (_unit.Size * 4f);
			var nexpPosition = RandomDirection_XZ (_point, Buffer);
			DeploymentPosition = nexpPosition;
			_unit.gameObject.SetActive (true);
		}
	}

	public Waypoint AddWaypoint (Vector3 _point, float _size)
	{
		
		var cursor = Instantiate (ringCursor);
		var waypoint = cursor.gameObject.AddComponent<Waypoint> ();
		cursor.SetCursor (_size, Color.yellow);
		waypoint.transform.position = _point;

		return waypoint;

	}

	void AddRingCursor (Unit unit)
	{
		if (unit.targetType == TargetType.ally || unit.targetType == TargetType.self)
		{
			var cursor = Instantiate (ringCursor);
			cursor.transform.position = unit.transform.position;
			cursor.transform.SetParent (unit.transform, true);
			cursor.SetCursor (unit.Size, Color.green);
		}


		if (unit.targetType == TargetType.enemy)
		{
			var cursor = Instantiate (ringCursor);
			cursor.transform.position = unit.transform.position;
			cursor.transform.SetParent (unit.transform, true);
			cursor.SetCursor (unit.Size, Color.red);
		}
	}


	public Vector3 RandomDirection_XZ (Vector3 Origin, float magnitude)
	{

		var v2 = UnityEngine.Random.onUnitSphere;
		float XDir = v2.x * magnitude;
		float ZDir = v2.y * magnitude;
		Vector3 Offset = new Vector3 (XDir, 0, ZDir);

		Vector3 result = Origin + Offset;

		return result;

	}


	void Destroy ()
	{

		battlePlane.onClickPoint -= BattlePlane_onClickPoint;
		battlePlane.onRightClickPoint -= BattlePlane_onRightClickPoint;
	}
}


