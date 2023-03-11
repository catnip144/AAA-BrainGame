using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLayout : MonoBehaviour
{
    [SerializeField] private List<RectTransform> playerPositions;
    void OnEnable()
    {
        for (int i = 0; i < PlayerConfigManager.instance.PlayerConfigs.Count; i++) {
            Transform currentPos = PlayerConfigManager.instance.PlayerConfigs[i].Input.gameObject.transform;
            currentPos.SetParent(transform);
            currentPos.localPosition = playerPositions[i].localPosition;
            currentPos.localScale += new Vector3(-0.2f, -0.2f);
        }
    }

}
