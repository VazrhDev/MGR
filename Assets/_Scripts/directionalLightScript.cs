using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directionalLightScript : MonoBehaviour
{
    [SerializeField] float speedOfOffset = 0.1f;
    [SerializeField] Material directionMaterial;

    //private void Update()
    //{
    //    float offset = speedOfOffset * Time.deltaTime;
    //    directionMaterial.mainTextureOffset += new Vector2(0, -offset);
    //}

    private void OnDestroy()
    {
        directionMaterial.mainTextureOffset = Vector2.zero;
    }
}
