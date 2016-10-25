using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class JKWeaponTesting : MonoBehaviour
{
	public string FleetOwner;
	public string FleetName;
	public List<Unit> Units;

	public RingCursor ringCursor;

	public Fleet fleet;

	void Update ()
	{
     


	}

	void Start ()
	{

		fleet = CreateFleet (Units, FleetOwner, FleetName);  
		fleet.SortBySize ();
		DeployFleet (fleet, Vector3.zero);
        
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
		}

		return _fleet;
	}


	public void DeployFleet (Fleet _fleet, Vector3 _point)
	{

		int unitcount = _fleet.Units.Count ();
		float Buffer = 0f;
		Vector3 DeploymentPosition = _point;

		for (int i = unitcount - 1; i > 0; i--)
		{
			var _unit = _fleet.Units [i];
			_unit.transform.position = DeploymentPosition;
			var cursor = Instantiate (ringCursor);
			cursor.transform.position = DeploymentPosition;
			cursor.transform.SetParent (_unit.transform, false);
			cursor.SetCursor (_unit.Size, Color.green);

			Buffer = Buffer + (_unit.Size * 2f);
			var nexpPosition = RandomDirection_XZ (_point, Buffer);
			DeploymentPosition = nexpPosition;
			_unit.gameObject.SetActive (true);
		}
	}


	public Vector3 RandomDirection_XZ (Vector3 Origin, float magnitude)
	{
		var dir = Game.Random.Next (-1000, 1000);
		float XDir = (dir / 1000f) * magnitude;
		float ZDir = (dir / 1000f) * magnitude;

		Vector3 Offset = new Vector3 (XDir, 0, ZDir);    

		return Offset; 

	}



}
