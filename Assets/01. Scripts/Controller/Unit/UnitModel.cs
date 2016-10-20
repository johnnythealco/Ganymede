using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UnitModel : MonoBehaviour
{
    public List<Transform> WeaponSpawns;
    public Collider ModelCollider;
    public bool Firing;

    public void Fire_Hit(Transform weaponSpawn, GameObject _Target, Weapon _weapon)
    {
        if (WeaponSpawns.Contains(weaponSpawn))
        {            
            int layerMask = 1 << 10;
            var ammo = _weapon.projectile;
            var speed = _weapon.projectileSpeed;
            var _targetPosition = _Target.transform.position;

            Vector3 trajectory = _targetPosition - weaponSpawn.position;
            RaycastHit hit;
            if (Physics.Raycast(weaponSpawn.position, trajectory, out hit, 10000f, layerMask))
            {
                ProjectileScript projectile = Instantiate(ammo, weaponSpawn.position, Quaternion.identity) as ProjectileScript;
                projectile.SetTarget(_Target, speed);
                projectile.impactNormal = hit.normal;
            }
        }
    }

    public void Fire_Miss(Transform weaponSpawn, Vector3 _targetPosition, Weapon _weapon, float _UnitSize)
    {

        if (WeaponSpawns.Contains(weaponSpawn))
        {
            var ammo = _weapon.projectile;
            var speed = _weapon.projectileSpeed;
            

            bool XDir = (Game.Random.Next(0, 10) > 5) ;
            bool YDir = (Game.Random.Next(0, 10) > 5);
            bool ZDir = (Game.Random.Next(0, 10) > 5);

            float X = _UnitSize;
            float Y = _UnitSize;
            float Z = _UnitSize;

            if (XDir)
                X = -X;
            if (YDir)
                Y = -Y;
            if (ZDir)
                Z = -Z;

            X = _UnitSize + _targetPosition.x;
            Y = _UnitSize + _targetPosition.y;
            Z = _UnitSize + _targetPosition.z;


            Vector3 ShotTarget = new Vector3(X, Y, Z);
            
            ProjectileScript projectile = Instantiate(ammo, weaponSpawn.position, Quaternion.identity) as ProjectileScript;
            projectile.transform.LookAt(ShotTarget);
            projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);              
        }
    }


    public void SetColliders(bool _value)
    {

        var colliders = gameObject.GetComponents<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = _value;
        }
    }
    
}
