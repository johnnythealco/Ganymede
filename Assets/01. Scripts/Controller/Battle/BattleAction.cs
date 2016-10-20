using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleAction : MonoBehaviour
{


    public static void DeployFleet(FleetState _fleet)
    {  
             
        var register = Game.Register;

        foreach (var _unit in _fleet.Units)
        {
            var unitType = register.GetUnitType(_unit.UnitType);

            var unit = (Unit)Instantiate(unitType);
            unit.transform.position = _unit.Position;
            unit.setUnitState(_unit);
            unit.SelectedWeapon = unit.Weapons.First();
            unit.SelectedAction = unit.Actions.First();
            Battle.AllUnits.Add(unit);
            Battle.VisibleUnits.Add(unit);

            unit.gameObject.name = unit.DsiplayName + "(" + unit.state.Owner + ")";

        }
    }


    public static List<UnitState> CreateUnits(List<string> _UnitTypes, string _Owner)
    {
        var result = new List<UnitState>();

        foreach (var _unit in _UnitTypes)
        {
            var newUnitType = Game.Register.GetUnitType(_unit);

            var newUnit = new UnitState(newUnitType, _Owner);

            result.Add(newUnit);
        }
        return result;
    }

    public static FleetState AddBasicFleet(string _player)
    {


        var _units = CreateUnits(Game.Manager.BasicFleet, _player);

        var _fleet = new FleetState(_player, "Basic Fleet", _units);

        foreach (var unit in _fleet.Units)
        {
            var X_position = (float)Game.Random.Next(-200, 200);
            var Y_position = (float)Game.Random.Next(-200, 200);
            var position = new Vector3(X_position, 0, Y_position);
            unit.Position = position;
        }

        return _fleet;

    }

    
}
