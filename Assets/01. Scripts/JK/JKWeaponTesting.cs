using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class JKWeaponTesting : MonoBehaviour
{
    public Unit ActiveUnit;
    public Unit ActiveTarget;


    RaycastHit hit;
    public GameObject ammo;
    [HideInInspector]
    public int currentProjectile = 0;
    public float speed = 1000;
    public int Maxshots = 10;
    
   

    void Update()
    {

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Time.timeScale = 0;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {

            StartCoroutine(Fire());
            
        }
    }

    IEnumerator Fire()
    {
        int layerMask = 1 << 9;
      
        System.Random rnd = new System.Random();
        int shots = rnd.Next(Maxshots);
        var targets = ActiveTarget.UnitModels;
        var attackers = ActiveUnit.UnitModels;
        int firedshots = 0;

  
            
        while (firedshots < shots)
        {
            for (int i = 0; i < attackers.Count(); i++)
            {
                var attacker = attackers[i];
                var targetindex = rnd.Next(targets.Count());
                Vector3 trajectory = targets[targetindex].transform.position;
                attacker.transform.LookAt(targets[targetindex].transform.position);
                var weaponindex = rnd.Next(attacker.WeaponSpawns.Count());
                Vector3 spawnpoint = attacker.WeaponSpawns[weaponindex].transform.position;

                if (Physics.Raycast(spawnpoint, trajectory, out hit, 1000f, layerMask))
                {
                    GameObject projectile = Instantiate(ammo, spawnpoint, Quaternion.identity) as GameObject;
                    projectile.transform.LookAt(targets[targetindex].transform.position);
                    projectile.GetComponent<Rigidbody>().AddForce(projectile.transform.forward * speed);
                    projectile.GetComponent<ProjectileScript>().impactNormal = hit.normal;
                }
            }
            firedshots++;
            yield return new WaitForSeconds(0.2f);

        }
            
               

    }
}
