using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;

public class TabletManager : MonoBehaviour
{
    [SerializeField] NNModel onnx;


    private void OnDestroy()
    {
        //input.Dispose();
        //engine.Dispose();
        Resources.UnloadUnusedAssets();
    }
}
