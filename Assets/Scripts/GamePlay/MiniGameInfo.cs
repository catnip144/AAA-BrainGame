using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MiniGameInfo", menuName = "MiniGameInfo")]
public class MiniGameInfo : ScriptableObject
{
    public MiniGameName miniGameName;
    public List<OptionBundle> possibleOptions = new List<OptionBundle>();
    public GameObject MiniGamePrefab;
}
