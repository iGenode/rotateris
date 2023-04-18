using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Constant List Of Floats", menuName = "ScriptableObjects/ConstantListOfFloats", order = 1)]
public class ConstantListOfFloats : ScriptableObject
{
    [SerializeField]
    private List<float> _list;

    public List<float> List
    {
        get
        {
            return _list;
        }
    }
}
