using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Unit : MonoBehaviour
{
    #region Variables

    public UnitState state;
    public Sprite Icon;
    public string DsiplayName;

    public unitSize Size;
    public float MaxSpeed;
    public float Acceleration;


    public int Armour;
    public ArmourType ArmourType;

    public int Shields;
    public int Engines;
    public float Evasion = 0.01f;

    public List<string> Weapons;
    public List<string> Actions;

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

        WeaponSpawns = new List<Transform>();
        foreach (var model in UnitModels)
        {
            if (model.WeaponSpawns.Count() > 0)
                WeaponSpawns.AddRange(model.WeaponSpawns);
        }
    }

    #region Getters & Setters

    public string faction { get { return state.Owner; } }

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

    public bool AttackWith(string _weapon, string _subSystem)
    {
        var weapon = Game.Register.GetWeapon(_weapon);
        if (weapon.accuracy >= this.state.evasion)
        {
            return true;
        }

        return false;

    }

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
            return true;
        }

        return false;



    }


    public void Fire(Unit _Target)
    {
        StartCoroutine(FireSelectedWeapon(_Target));
    }

    IEnumerator FireSelectedWeapon(Unit _Target)
    {
        var weapon = Game.Register.GetWeapon(SelectedWeapon);
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

    //public void FireSelectedWeapon(Vector3 _Target)
    //{
    //    var weapon = Game.Register.GetWeapon(SelectedWeapon);
 

    //    if (!Firing)
    //    {
    //        transform.LookAt(_Target);
    //        StartCoroutine(UnitModelsFire(_Target, weapon));
    //    }
    //}


    //IEnumerator UnitModelsFire(Unit _Target, Weapon _weapon)
    //{
    //    Firing = true;
        

    //    var targets = _Target.UnitModels;
    //    var attackers = this.UnitModels;
    //    int _TargetIndex = 0;
     
    //        for (int i = 0; i < attackers.Count(); i++)
    //        {
    //        attackers[i].FireAt(targets[_TargetIndex].transform.position,_weapon);

    //        if (_TargetIndex < targets.Count())
    //            _TargetIndex++;
    //        else
    //            _TargetIndex = 0; 
    //        }

    //    while(true)
    //    {
    //        bool unitsfinishedFireing = true;
    //        foreach (var model in attackers)
    //        {
    //            if (model.Firing)
    //                unitsfinishedFireing = false;
    //        }

    //        if (unitsfinishedFireing)
    //            break;

    //        yield return null;
    //    }

    //    Firing = false;
        
      
    // }

    //IEnumerator UnitModelsFire(Vector3 _Target, Weapon _weapon)
    //{
    //    Firing = true;
            
    //    var attackers = this.UnitModels;
    //    for (int i = 0; i < attackers.Count(); i++)
    //    {
    //        attackers[i].FireAt(_Target, _weapon);
    //    }

    //    while (true)
    //    {
    //        bool unitsfinishedFireing = true;
    //        foreach (var model in attackers)
    //        {
    //            if (model.Firing)
    //                unitsfinishedFireing = false;
    //        }

    //        if (unitsfinishedFireing)
    //            break;

    //        yield return null;
    //    }

    //    Firing = false;


    //}



    //IEnumerator Fire(Unit _Target, Weapon _weapon)
    //{
    //    Firing = true;
    //    int layerMask = 1 << 10;
    //    var ammo = _weapon.projectile;
    //    var speed = _weapon.projectileSpeed;

        
    //    int shots = Game.Random.Next(_weapon.projectile_Min_Shots, _weapon.projectile_Max_Shots);
    //    var targets = _Target.UnitModels;
    //    var attackers = this.UnitModels;
    //    int firedshots = 0;

    //    foreach(var unit in attackers)
    //    {
    //        unit.GetComponent<Collider>().enabled = false;
    //    }



    //    while (firedshots < shots)
    //    {
                         
    //        for (int t = 0; t < targets.Count(); t++)
    //        {
    //            for (int i = 0; i < attackers.Count(); i++)
    //            {
    //                var attacker = attackers[i];
    //                var targetPosition = targets[t].transform.position;
    //                Vector3 trajectory = targetPosition - attacker.transform.position;
    //                attacker.transform.LookAt(targetPosition);
    //                var weaponindex = Game.Random.Next(attacker.WeaponSpawns.Count());
    //                Vector3 spawnpoint = attacker.WeaponSpawns[weaponindex].transform.position;
    //                RaycastHit hit;

    //                if (Physics.Raycast(spawnpoint, trajectory, out hit, 10000f, layerMask))
    //                {
    //                    ProjectileScript projectile = Instantiate(ammo, spawnpoint, Quaternion.identity) as ProjectileScript;
    //                    projectile.transform.LookAt(targetPosition);
    //                    projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);
    //                    projectile.impactNormal = hit.normal;
    //                }
    //                t++;
    //                yield return new WaitForSeconds(_weapon.projectileShotDelay / 2);
    //                firedshots++;
    //            }                
    //        }
            
    //        yield return new WaitForSeconds(_weapon.projectileShotDelay);

    //    }

    //    foreach (var unit in attackers)
    //    {
    //        unit.GetComponent<Collider>().enabled = true;
    //    }

    //    Firing = false;

    //}

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

        Destroy (gameObject);
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


    //float stoppingDistance
    //{
    //    get
    //    {
    //        var acceleration = state.Acceleration;
    //        var currentVelocity = state.MaxSpeed;

    //        var stoppingDistance = (acceleration * acceleration) / (currentVelocity * 2);
    //        return stoppingDistance;


    //    }
    //}

    //IEnumerator MoveUsingRigidBody()
    //{
    //    var heading = destination - transform.position;
    //    var direction = heading.normalized;
    //    var distance = heading.magnitude;
    //    var Sqr_StoppingDistance = stoppingDistance * stoppingDistance;
    //    float maxVelocity;

    //    if((distance* distance) <= (Sqr_StoppingDistance) * 2 )
    //    {
    //        maxVelocity = Mathf.Sqrt(stoppingDistance * 2 * state.Acceleration);
    //    }
    //    else
    //    {
    //        maxVelocity = state.MaxSpeed;
    //    }
        

    //    bool outsideStoppingDistance = distance > stoppingDistance;

    //    while (true)
    //    {
    //        heading = destination - transform.position;
    //        direction = heading.normalized;
    //        distance = heading.magnitude;
    //        outsideStoppingDistance = distance > stoppingDistance;

    //        if(outsideStoppingDistance)
    //        { 
    //            while (rb.velocity.magnitude < maxVelocity)
    //            {
    //                rb.AddForce(direction * state.Acceleration, ForceMode.Acceleration);                   
    //                yield return null;
    //            }
    //            rb.velocity = direction * maxVelocity;               
    //         }
    //        else
    //        {
    //            break;
    //        }

    //        yield return null;          
    //    }

    //    heading = destination - transform.position;
    //    direction = heading.normalized;
    //    distance = heading.magnitude;
    //    var curVelocity = rb.velocity;
    //    //var deceleration = curVelocity * curVelocity / 2 * distance;

    //    rb.AddForce(-curVelocity, ForceMode.Acceleration);       
    //    Debug.Log("Move Finished ");





    //}

    #endregion
}
