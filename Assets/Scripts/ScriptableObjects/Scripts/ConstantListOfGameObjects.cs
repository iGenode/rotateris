using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Constant List Of Game Objects", menuName = "ScriptableObjects/ConstantListOfGameObjects", order = 1)]
public class ConstantListOfGameObjects : ScriptableObject
{
    [SerializeField]
    private List<GameObject> _list;

    public List<GameObject> List
    {
        get
        {
            return _list;
        }
    }
}
