using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TargetDisplay : MonoBehaviour {

    public Text UnitName;
    public Text Range;
    public Text HitChance;
   
    public Text Armour;
    public Text Shields;
    public Text Engines;
    public Text Evasion;
    public Text Speed;
    public Text Size;
    public Text ArmourType;
    public GameObject DetailsPanel;

    Unit unit;
    bool DetailsPanelHidden;

    void Awake()
    {
        DetailsPanel.SetActive(false);
        DetailsPanelHidden = true;
    }

    void Update()
    {
        if(gameObject.activeSelf)
        {
            var activeUnit = Battle.Manager.ActiveUnit;
            var activeTarget = activeUnit.ActiveTarget;
            if (activeUnit != null && activeTarget != null)
            {
                var range = (int)(activeUnit.transform.position - activeUnit.ActiveTarget.transform.position).magnitude;

                if (Range != null)
                    Range.text = "Range: " + range.ToString();
                if (HitChance != null)
                    HitChance.text = "Hit :" + ((int)(activeUnit.HitChance * 100)).ToString() + "%";
            }
        }
    }


    public void Prime(Unit _unit)
    {
        unit = _unit;
     
        if (UnitName != null)
            UnitName.text = unit.state.UnitType;
        if (Range != null)
            Range.text = "Range: " + ((int)unit.RangeToTarget).ToString("N");
        if (HitChance != null)
            HitChance.text = "Hit :" + ((int)(Battle.Manager.ActiveUnit.HitChance *100)).ToString() + "%";
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
    }

   
    public void ToggleDetailsPanel()
    {
        if(DetailsPanelHidden)
        {
            DetailsPanel.SetActive(true);
            DetailsPanelHidden = false;
        }
        else
        {
            DetailsPanel.SetActive(false);
            DetailsPanelHidden = true;
        }
    }
}
