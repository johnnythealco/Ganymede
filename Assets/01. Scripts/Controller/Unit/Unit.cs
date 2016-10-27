using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof (SphereCollider))]
public class Unit : MonoBehaviour
{
    #region Variables

    public UnitState state;
    public Sprite Icon;
    public string DsiplayName;    
    public float MaxSpeed;
    public float Acceleration;
    public int Armour;
    public ArmourType ArmourType;
    public int Shields;
    public int Engines;
    public float Evasion = 0.01f;

    public List<string> Weapons;
    public List<string> Actions;

    public GameObject Explosion;

    public float Size { get; set; }
    public Unit ActiveTarget { get; set; }
    public string SelectedWeapon { get; set; }
    public string SelectedAction { get; set; }
    public float RangeToTarget { get; set; }
    public List<UnitModel> UnitModels { get; set; }
    public List<Transform> WeaponSpawns { get; set; }


    bool Moving;
    bool Firing;
    Vector3 destination;

    #endregion

    void Awake()
    {
        UnitModels = transform.GetComponentsInChildren<UnitModel>().ToList();

        Size = GetComponent<SphereCollider>().radius;

        WeaponSpawns = new List<Transform>();
        foreach (var model in UnitModels)
        {
            if (model.WeaponSpawns.Count() > 0)
                WeaponSpawns.AddRange(model.WeaponSpawns);
        }
    }

    #region Getters & Setters
  
    public string Owner { get { return state.Owner; } }

    public float currentMovement { get { return state.MaxSpeed; } }

    public float currentAttackRange {
        get {
            var weapon = Game.Register.GetWeapon(SelectedWeapon);
            return weapon.range;
        }
    }

    public int currentArmour { get { return state.armour; } }

    public TargetType targetType
    {
        get
        {
            if (this == Game.BattleManager.ActiveUnit && this.state.Owner == Game.PlayerName)
                return TargetType.self;

            if (this.state.Owner == Game.PlayerName)
            {
                return TargetType.ally;
            }
            else
            {
                return TargetType.enemy;
            }
        }
    }


    public void setUnitState(UnitState _unit)
    {
        this.state = _unit;
    }

    public float HitChance
    {
        get
        {
            if (ActiveTarget == null)
                return 0f;

            var distance = (ActiveTarget.transform.position - transform.position).magnitude;
            var Weapon = Game.Register.GetWeapon(SelectedWeapon);
            var WeaponRange = Weapon.range;
            var accuracy = Weapon.accuracy;
            float TargetSize = 1;

            if (ActiveTarget.GetComponent<SphereCollider>() == null)
                Debug.Log("Target Requires Sphere Collider");
            else
                TargetSize = ActiveTarget.GetComponent<SphereCollider>().radius;

            float hitChance = (WeaponRange * accuracy) / (distance * TargetSize);

            hitChance = hitChance - ActiveTarget.Evasion;

            if (hitChance > 1)
                hitChance = 1f;

            if (hitChance < 0)
                hitChance = 0f;

            return hitChance;

        }
    }

    #endregion

    #region Combat

    public bool HitBy(string _weapon)
    {
        var weapon = Game.Register.GetWeapon(_weapon);
        var weaponDamage = weapon.damage;


        switch (weapon.damageType)
        {
            case DamageType.laser:
                {
                    takeLaserDamage(weaponDamage);

                }
                break;
            case DamageType.kinetic:
                {
                    takeKineticDamage(weaponDamage);
                }
                break;
            case DamageType.plasma:
                {
                    takePlasmaDamage(weaponDamage);

                }
                break;
        }

        if (state.armour <= 0)
        {
            DestroyUnit();
            return true;
        }

        return false;
   }
    
    public void Fire(Unit _Target)
    {
        var weapon = Game.Register.GetWeapon(SelectedWeapon);
        StartCoroutine(Attack(_Target, weapon));
    }

    IEnumerator Attack(Unit _Target, Weapon _Weapon)
    {
        var weapon = _Weapon;
        int weaponSpawnIndex = 0;
        int TargetIndex = 0;
        int shots = Game.Random.Next(weapon.projectile_Min_Shots, weapon.projectile_Max_Shots);
        int shotsFired = 0;
        bool hit;
        float size = _Target.GetComponent<SphereCollider>().radius;

        while(shotsFired < shots)
        {
            if (HitChance < 1)
            {
                var hitRoll = (float)Game.Random.Next(1, 99);
                hit = hitRoll < HitChance * 100;
            }
            else
            {
                hit = true;
            }

            if (weaponSpawnIndex > WeaponSpawns.Count()-1)
                weaponSpawnIndex = 0;
            var weaponSpawn = WeaponSpawns[weaponSpawnIndex];

            if (TargetIndex > _Target.UnitModels.Count()-1)
                TargetIndex = 0;

            var targetModel = _Target.UnitModels[TargetIndex].transform.position;

            var unitModel = WeaponSpawns[weaponSpawnIndex].transform.GetComponentInParent<UnitModel>();

            if (hit)
            {
                if (unitModel != null)
                    unitModel.SetColliders(false);
                    unitModel.Fire_Hit(weaponSpawn, _Target.UnitModels[TargetIndex].gameObject, weapon);

                _Target.HitBy(weapon.name);
            }
            else
            {
                if (unitModel != null)
                    unitModel.SetColliders(false);
                    unitModel.Fire_Miss(weaponSpawn, targetModel, weapon, size);
            }

            weaponSpawnIndex++;
            TargetIndex++;
            shotsFired++;
            yield return new WaitForSeconds(weapon.projectileShotDelay);
        }
        

    }
        
    #region Damage Types

    void takeLaserDamage (int _damage)
	{

		var Shields = state.shields;
		var armourType = state.armourType;
        float shieldDefense = 1f;
        float lightArmourDefense = 1f;
        float mediumArmourDefense = 0.8f;
        float heavyArmourDefense = 0.5f;

        int shieldDamage = 0;
        int armourDamage = 0;

        #region Shields Remaininag
        if (Shields > 0)
        {               
            shieldDamage = (int)(_damage * shieldDefense);
            this.Shields = this.Shields - shieldDamage;
                
            if (Shields < 0)
            {
                var extraDamage = -Shields;
                switch (armourType)
                {
                    case ArmourType.light:
                        {
                            armourDamage = (int)(extraDamage * lightArmourDefense);
                            if (armourDamage > 0)
                                state.armour = state.armour - armourDamage;
                        }
                        break;
                    case ArmourType.medium:
                        {
                            armourDamage = (int)(extraDamage * mediumArmourDefense);
                            if (armourDamage > 0)
                                state.armour = state.armour - armourDamage;
                        }
                        break;
                    case ArmourType.heavy:
                        {
                            armourDamage = (int)(extraDamage * heavyArmourDefense);
                            if (armourDamage > 0)
                                state.armour = state.armour - armourDamage;
                        }
                        break;
                }
            }            
        }
        #endregion

        #region No Shields Remaining
        else
        {
            switch (armourType)
            {
                case ArmourType.light:
                    {
                        armourDamage = (int)(_damage * lightArmourDefense);
                        if (armourDamage > 0)
                            state.armour = state.armour - armourDamage;
                    }
                    break;
                case ArmourType.medium:
                    {
                        armourDamage = (int)(_damage * mediumArmourDefense);
                        if (armourDamage > 0)
                            state.armour = state.armour - armourDamage;
                    }
                    break;
                case ArmourType.heavy:
                    {
                        armourDamage = (int)(_damage * heavyArmourDefense);
                        if (armourDamage > 0)
                            state.armour = state.armour - armourDamage;
                    }
                    break;
            }
        }
        #endregion

        BattleLog.Damage(this, "Laser", _damage, shieldDamage, armourDamage);

    }

    void takeKineticDamage (int _damage)
	{
        var Shields = state.shields;
        var armourType = state.armourType;
        float shieldDefense = 0.5f;
        float lightArmourDefense = 1f;
        float mediumArmourDefense = 0.9f;
        float heavyArmourDefense = 0.6f;

        int shieldDamage = 0;
        int armourDamage = 0;

        #region Shields Remaininag
        if (Shields > 0)
        {

        shieldDamage = (int)(_damage * shieldDefense);
        this.Shields = this.Shields - shieldDamage;

        }
        #endregion

        #region No Shields Remaining
        else
        {
            switch (armourType)
            {
                case ArmourType.light:
                    {
                        armourDamage = (int)(_damage * lightArmourDefense);
                        if (armourDamage > 0)
                            state.armour = state.armour - armourDamage;
                    }
                    break;
                case ArmourType.medium:
                    {
                        armourDamage = (int)(_damage * mediumArmourDefense);
                        if (armourDamage > 0)
                            state.armour = state.armour - armourDamage;
                    }
                    break;
                case ArmourType.heavy:
                    {
                        armourDamage = (int)(_damage * heavyArmourDefense);
                        if (armourDamage > 0)
                            state.armour = state.armour - armourDamage;
                    }
                    break;
            }
        }
        #endregion

        BattleLog.Damage(this, "Kinetic", _damage, shieldDamage, armourDamage);

    }

	void takePlasmaDamage (int _damage)
	{
        var Shields = state.shields;
        var armourType = state.armourType;

        float shieldDefense = 0.8f;
        float lightArmourDefense = 1f;
        float mediumArmourDefense = 1f;
        float heavyArmourDefense = 0.8f;

        int shieldDamage = 0;
        int armourDamage = 0;

        #region Shields Remaininag
        if (Shields > 0)
        {
           
            shieldDamage = (int)(_damage * shieldDefense);
            this.Shields = this.Shields - shieldDamage;
                
            if (Shields < 0)
            {
                var extraDamage = -Shields;
                switch (armourType)
                {
                    case ArmourType.light:
                        {
                            armourDamage = (int)(extraDamage * lightArmourDefense);
                            if (armourDamage > 0)
                                state.armour = state.armour - armourDamage;
                        }
                        break;
                    case ArmourType.medium:
                        {
                            armourDamage = (int)(extraDamage * mediumArmourDefense);
                            if (armourDamage > 0)
                                state.armour = state.armour - armourDamage;
                        }
                        break;
                    case ArmourType.heavy:
                        {
                            armourDamage = (int)(extraDamage * heavyArmourDefense);
                            if (armourDamage > 0)
                                state.armour = state.armour - armourDamage;
                        }
                        break;
                }
            }
            
        }
        #endregion

        #region No Shields Remaining
        else
        {
            switch (armourType)
            {
                case ArmourType.light:
                    {
                        armourDamage = (int)(_damage * lightArmourDefense);
                        if (armourDamage > 0)
                            state.armour = state.armour - armourDamage;
                    }
                    break;
                case ArmourType.medium:
                    {
                        armourDamage = (int)(_damage * mediumArmourDefense);
                        if (armourDamage > 0)
                            state.armour = state.armour - armourDamage;
                    }
                    break;
                case ArmourType.heavy:
                    {
                        armourDamage = (int)(_damage * heavyArmourDefense);
                        if (armourDamage > 0)
                            state.armour = state.armour - armourDamage;
                    }
                    break;
            }
        }
        #endregion

        BattleLog.Damage(this, "Plasma", _damage, shieldDamage, armourDamage);


    }

    #endregion

    public void DestroyUnit ()
	{
        if (Battle.AllUnits.Contains(this))
            Battle.AllUnits.Remove(this);

        if (Battle.VisibleUnits.Contains(this))
            Battle.VisibleUnits.Remove(this);

        gameObject.SetActive(false);

        if(Explosion != null)
        {
            Instantiate(Explosion, transform.position, Quaternion.identity);
        }

        Destroy (gameObject, 3f);
	}

    #endregion


    #region Movement

    public void Move(Vector3 _Destination)
    {        
        transform.LookAt(_Destination);      
        destination = _Destination;
        if(!Moving)
            StartCoroutine(smooth_move());
    }

    IEnumerator smooth_move()
    {
        Moving = true;

        float sqrRemainingDistance = (transform.position - destination).sqrMagnitude; //sqrMagnitude is cheaper on the CPU than Magnitude

        while (sqrRemainingDistance > float.Epsilon && Moving)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, destination, currentMovement * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - destination).sqrMagnitude;
            yield return null;
        }
        Moving = false;
    }

    #endregion
}
