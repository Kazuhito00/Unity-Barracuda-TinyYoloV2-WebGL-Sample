using UnityEngine;
using System.Collections;
using Unity.Barracuda;

public class NNHandler : System.IDisposable
{
    public Model model;
    public IWorker worker;

    public NNHandler(NNModel nnmodel)
    {
        model = ModelLoader.Load(nnmodel);
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("Worker:CPU");
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, model); // CPU
#else
        Debug.Log("Worker:GPU");
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model); // GPU
#endif
    }

    public void Dispose()
    {
        worker.Dispose();
    }
}
