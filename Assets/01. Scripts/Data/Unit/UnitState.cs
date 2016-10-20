using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UnitState
{


	public string UnitType;
    public Vector3 Position;
    public string Owner;

    public unitSize Size{ get { return Game.Register.GetUnitType (UnitType).Size; } }

	public ArmourType armourType { get { return Game.Register.GetUnitType (UnitType).ArmourType; } }


    public float MaxSpeed;
    public float Acceleration;
    
	public int armour;
	public int shields;
	public int engines;
	public float evasion;


	public List<string> weapons;
	public List<string> actions;


	public UnitState (Unit _type, string _faction)
	{
		this.Owner = _faction;
		this.UnitType = _type.DsiplayName;
		this.MaxSpeed = _type.MaxSpeed;
		this.Acceleration = _type.Acceleration;
        this.armour = _type.Armour;
        this.shields = _type.Shields;
        this.engines = _type.Engines;
        this.evasion = _type.Evasion;
        this.weapons = _type.Weapons;
		this.actions = _type.Actions;
    }


}

#region Weapon
[System.Serializable]
public class Weapon
{
	public string name;
	public Sprite icon;
	public float range = 1;
	public float accuracy = 1;
	public int damage = 10;
	public DamageType damageType;
    public float coolDownTime = 5;
    public int FireRateMin = 1;
    public int FireRateMax = 5;
    public ProjectileScript projectile;
    public float projectileSpeed = 1000;
    public int projectile_Min_Shots = 10;
    public int projectile_Max_Shots = 10;
    public float projectileShotDelay = 0.2f;

}
#endregion

#region Action
[System.Serializable]
public class UnitAction
{
	public string name;
    public int range;
    public int APCost;
	public Sprite icon;
	public TargetType targetType;

}
#endregion


#region Enums
public enum unitSize
{
	tiny = 0,
	small = 1,
	medium = 2,
	large = 3,
	massive = 4
}

public enum DamageType
{
	kinetic = 0,
	laser = 1,
	plasma = 2
}

public enum ArmourType
{
	light = 0,
	medium = 1,
	heavy = 2
}

public enum TargetType
{
	empty = 0,
	enemy = 1,
	ally = 2,
	self = 3
}


[System.Serializable]
public class UnitCost
{
	public string unitType;
	public int cost;
}

#endregion

