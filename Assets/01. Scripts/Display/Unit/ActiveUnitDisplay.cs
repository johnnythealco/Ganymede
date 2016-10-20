using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;


public class ActiveUnitDisplay : MonoBehaviour
{
    #region Variables
 
    public Text UnitName;	
    public Text ActiveTarget;

    public Text Armour;
	public Text Shields;
    public Text Engines;
    public Text Evasion;
    public Text Speed;
    public Text Size;
    public Text ArmourType;
    public Transform weaponsPanel;
    public Transform DetailsPanel;
    public Transform weaponsList;
	public WeaponDisplay weaponDisplayPrefab;

	Unit unit;
	List<WeaponDisplay> weaponDisplays = new List<WeaponDisplay> ();
    #endregion

    #region Delegates & Events

    public delegate void UnitModelDisplayDelegate ();

	#endregion    
	public void Prime (Unit _unitModel)
	{
		unit = _unitModel;        
        clearWeaponDisplays ();

		if (unit.SelectedWeapon == null || unit.SelectedWeapon == "")
			unit.SelectedWeapon = unit.Weapons.First ();

        if (ActiveTarget != null && Game.BattleManager.ActiveUnit.ActiveTarget != null)
            ActiveTarget.text = Game.BattleManager.ActiveUnit.ActiveTarget.DsiplayName;

        #region UnitDetails
        if (UnitName != null)
            UnitName.text = unit.state.UnitType; 
        if (Armour != null)
            Armour.text = unit.state.armour.ToString();
        if (Shields != null)
            Shields.text = unit.state.shields.ToString();
        if (Engines != null)
            Engines.text = unit.state.engines.ToString();
        if (Evasion != null)
            Evasion.text = unit.state.evasion.ToString();       
        if (Size != null)
            Size.text = unit.state.Size.ToString();
        if (ArmourType != null)
            ArmourType.text = unit.state.armourType.ToString();
        #endregion

        if (weaponsList != null)
		{
            PrimeWeapons();
        }
  
    }

    public void UpdateActiveTarget()
    {
        if (ActiveTarget != null && Game.BattleManager.ActiveUnit.ActiveTarget != null)
            ActiveTarget.text = Game.BattleManager.ActiveUnit.ActiveTarget.DsiplayName;
    }

    public void NextTarget()
    {


    }

    public void PrevTarget()
    {

      
    }

    void highlightSelectedWeapon ()
	{
		foreach (var item in weaponDisplays)
		{
			if (item.weaponName.text == unit.SelectedWeapon)
				item.weaponName.fontStyle = FontStyle.Bold;
			else
				item.weaponName.fontStyle = FontStyle.Normal;
		}


	}

	void WeaponDsiplay_onClick (Weapon _weapon)
	{
		unit.SelectedWeapon = _weapon.name;
		highlightSelectedWeapon ();
     
    }

	void onDestroy ()
	{
		clearWeaponDisplays ();
	}

	void clearWeaponDisplays ()
	{
		foreach (var weaponDsiplay in weaponDisplays)
		{
			weaponDsiplay.onClick -= WeaponDsiplay_onClick;
		}

		for (int i = 0; i < weaponsList.childCount; i++)
		{
			Destroy (weaponsList.GetChild (i).gameObject);
		}

		weaponDisplays.Clear ();

	}

    void PrimeWeapons()
    {
        foreach (var item in unit.Weapons)
        {
            var weapon = Game.Register.GetWeapon(item);
            var weaponDsiplay = (WeaponDisplay)Instantiate(weaponDisplayPrefab);
            weaponDsiplay.transform.SetParent(weaponsList, false);
            weaponDsiplay.Prime(weapon);
            weaponDsiplay.onClick += WeaponDsiplay_onClick;
            weaponDisplays.Add(weaponDsiplay);
        }

        highlightSelectedWeapon();
    }





}
