using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class InferenceManager : MonoBehaviour
{
    [Header("Neural Net")]
    [SerializeField] NNModel _onnx;
    private Model _model;
    private IWorker _engine;

    private void Start()
    {
        _model = ModelLoader.Load(_onnx);
        _engine = WorkerFactory.CreateWorker(_model, WorkerFactory.Device.GPU);
    }

    public int InferNumber(Texture2D texture)
    {
        var input = new Tensor(texture, 1);
        input.Reshape(new TensorShape(1, 1, 28, 28));   // Align with the model's Tensor shape
        var output = _engine.Execute(input).PeekOutput();
        var candidates = output.ToReadOnlyArray();
        var max = candidates.Max();

        // Collect garbages first
        input.Dispose();
        output.Dispose();

        if (max < 1)    // Likely blank
            return -1;

        var result = Array.IndexOf(candidates, candidates.Max());
        
        return result;
    }

    private void OnDestroy()
    {
        _engine.Dispose();
        Resources.UnloadUnusedAssets();
    }
}
