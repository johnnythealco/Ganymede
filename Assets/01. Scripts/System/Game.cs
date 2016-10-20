using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{

    #region Properties

    #region References
    [SerializeField]
    Register register;

    [SerializeField]
    NetManager networkManager;
    #endregion

    public static Game Manager = null;

	public static Register Register{ get; set; }

    public static NetManager NetworkManager { get; set; }

	public static string PlayerName{ get; set; }

    public static NetworkController NetworkController { get; set; }

	public static Battle BattleManager{ get; set; }

	public static List<Vector3> GridPoints{ get; set; }

    public List<Player> Players = new List<Player>();

    public static bool isServer { get; set; }

    public static bool isMultiplayer { get; set; }

    public List<string> BasicFleet;

    public static System.Random Random { get; set; }

    #endregion

    void Awake ()
	{
		if (Manager == null)
		{
			Manager = this;
			Register = register;
            NetworkManager = networkManager;


        } else if (Manager != this)
			Destroy (gameObject);

        Random = new System.Random();

        DontDestroyOnLoad (gameObject);
	}
    
    public static Player GetPlayer(string _playerName)
    {
        foreach(var _player in Game.Manager.Players)
        {
            if (_player.Name == _playerName)
                return _player;
        }
        return null;

    
    }

    public  void PlayerReady(uint _id)
    {
        foreach (var Player in Game.Manager.Players)
        {
            if (Player.ConnectionID == _id)
            {
                Player.ReadyStatus = true;
            }
        }       
  
    }

    public static Unit CreateUnit(UnitState _state)
    {
       
        var _Template = Register.GetUnitType(_state.UnitType);

        var _unit = (Unit)Instantiate(_Template);
        _unit.setUnitState(_state);
        _unit.SelectedWeapon = _unit.Weapons.First();
        _unit.SelectedAction = _unit.Actions.First();

        return _unit;
    }

    #region Testing

    #endregion

}
