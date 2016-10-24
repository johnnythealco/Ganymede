using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Fleet : MonoBehaviour
{
    public string Owner;
    public string Name;
    public List<Unit> Units = new List<Unit>();



    void SortUnits_Size(List<Unit> list, int left, int right)
    {
        if (left < right)
        {
            int pivotIdx = Partition_Size(list, left, right);

            if (pivotIdx > 1)
                SortUnits_Size(list, left, pivotIdx - 1);

            if (pivotIdx + 1 < right)
                SortUnits_Size(list, pivotIdx + 1, right);
        }

    }

    int Partition_Size(List<Unit> list, int left, int right)
    {
        Unit pivot = list[left];

        while (true)
        {
            while (list[left].Size < pivot.Size)
                left++;

            while (list[right].Size > pivot.Size)
                right--;

            if (list[right].Size == pivot.Size && list[left].Size == pivot.Size)
                left++;

            if (left < right)
            {
                Unit temp = list[left];
                list[left] = list[right];
                list[right] = temp;
            }
            else
            {
                return right;
            }
        }
    }


    public void SortBySize()
    {
        SortUnits_Size(Units, 0, Units.Count() - 1);
    }


    public void InitalizeUnits()
    {
        foreach(var unit in Units)
        {
            unit.state = new UnitState(unit, Owner);
        }
    }
       

}
