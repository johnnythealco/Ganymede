using UnityEngine;
using System.Collections.Generic;

public class JKQuickStart : MonoBehaviour
{
    public List<Fleet> Fleets;


    void Start()
    {
        QuickStart();
    }
	

    void QuickStart()
    {
        var playerfleet = Battle.Manager.CreateFleet(Fleets[0].Units, Fleets[0].Owner, Fleets[0].Name);
        var Enemyfleet = Battle.Manager.CreateFleet(Fleets[1].Units, Fleets[1].Owner, Fleets[1].Name);

        Vector3 playerSpawn = new Vector3();
        Vector3 enemySpawn = new Vector3(0, 0, 30000);


        Battle.Manager.DeployFleet(playerfleet, playerSpawn);
        Battle.Manager.DeployFleet(Enemyfleet, enemySpawn);

    }
	

}
