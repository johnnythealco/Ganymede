using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NetworkController :  NetworkBehaviour
{
    [SyncVar]
    public bool BattleReady;

    [SyncVar]
    public bool DeploymentComplete;


    #region Setup and Network Management

    void Awake ()
	{
        DontDestroyOnLoad (this.gameObject);
        Game.NetworkManager.NetworkPlayers.Add(this);
     }

    void Start()
    {
        Setup();
   }


    public void Setup ()
	{
        if (hasAuthority)
		{
            Game.NetworkController = this;

            var _Scene = SceneManager.GetActiveScene().name;

            if (_Scene == "Lobby")
            {


            }             


        }

		if (!hasAuthority)
		{
			this.enabled = false;
		}



    }

	public void Disconnect ()
	{

		if (isServer)
		{
			NetworkManager.singleton.StopHost ();
			NetworkServer.Reset ();
		}

		if (isClient)
		{
			NetworkManager.singleton.StopClient ();
		}
	}

    [Command]
    public void Cmd_SetBattleReady(uint _id, bool _readyStatus)
    {
        if (this.netId.Value == _id)
            this.BattleReady = _readyStatus;
    }

  
    [Command]
    public void Cmd_SetDeploymentComplete(uint _id, bool _value)
    {
        if (this.netId.Value == _id)
            this.DeploymentComplete = _value;
    }

    #endregion



}
