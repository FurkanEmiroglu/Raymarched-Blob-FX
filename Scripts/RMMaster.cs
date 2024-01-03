using System.Collections.Generic;
using UnityEngine;

// ReSharper disable NotAccessedField.Local
// ReSharper disable InconsistentNaming

[DefaultExecutionOrder(-10)]
public class RMMaster : MonoBehaviour
{
    private static RMMaster _singletonMember;
    public ComputeShader raymarching;

    private List<ComputeBuffer> buffersToDispose;
    private Camera cam;
    private Light lightSource;

    private RenderTexture target;

    public static RMMaster Instance
    {
        get
        {
            if (_singletonMember != null) return _singletonMember;

            _singletonMember = FindObjectOfType<RMMaster>();
            return _singletonMember;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Init()
    {
        cam = Camera.main;
        lightSource = FindObjectOfType<Light>();
    }

    public RenderTexture ApplyRaymarching(RenderTexture source)
    {
        Init();
        buffersToDispose = new List<ComputeBuffer>();

        InitRenderTexture();
        CreateScene();
        SetParameters();

        raymarching.SetTexture(0, Source, source);
        raymarching.SetTexture(0, Destination, target);

        int threadGroupsX = Mathf.CeilToInt(cam.pixelWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(cam.pixelHeight / 8.0f);

        raymarching.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        foreach (ComputeBuffer buffer in buffersToDispose) buffer.Dispose();

        return target;
    }

    private void CreateScene()
    {
        List<RaymarchShape> allShapes = new List<RaymarchShape>(FindObjectsOfType<RaymarchShape>());
        List<RaymarchShape> orderedShapes = new List<RaymarchShape>();

        foreach (RaymarchShape shape in allShapes)
        {
            // if (shape.transform.parent != null) continue;

            Transform parentShape = shape.transform;
            orderedShapes.Add(shape);
            shape.numChildren = parentShape.childCount;
            for (int j = 0; j < parentShape.childCount; j++)
                if (parentShape.GetChild(j).TryGetComponent(out RaymarchShape s))
                {
                    orderedShapes.Add(s);
                    orderedShapes[^1].numChildren = 0;
                }
        }

        ShapeData[] shapeData = new ShapeData[orderedShapes.Count];
        for (int i = 0; i < orderedShapes.Count; i++)
        {
            RaymarchShape s = orderedShapes[i];
            Vector3 col = new(s.colour.r, s.colour.g, s.colour.b);
            shapeData[i] = new ShapeData
            {
                position = s.Position,
                scale = s.Scale,
                colour = col,
                blendStrength = s.blendStrength * 3,
                numChildren = s.numChildren,
                shapeType = s.shapeType,
                angleZ = s.transform.eulerAngles.z
            };
        }

        ComputeBuffer shapeBuffer = new(shapeData.Length, ShapeData.GetSize());
        shapeBuffer.SetData(shapeData);
        raymarching.SetBuffer(0, Shapes, shapeBuffer);
        raymarching.SetInt(NumShapes, shapeData.Length);

        buffersToDispose.Add(shapeBuffer);
    }

    private void SetParameters()
    {
        raymarching.SetFloat("_AspectRatio", (float)Screen.width / Screen.height);
        raymarching.SetVector("_WorldSpaceCameraPos", cam.transform.position);
        raymarching.SetMatrix(CameraToWorld, cam.cameraToWorldMatrix);
        raymarching.SetMatrix(CameraInverseProjection, cam.projectionMatrix.inverse);
        raymarching.SetVector(Light1, lightSource.transform.forward);
    }

    private void InitRenderTexture()
    {
        if (target != null && target.width == cam.pixelWidth && target.height == cam.pixelHeight) return;

        if (target != null) target.Release();

        target = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
        {
            enableRandomWrite = true
        };
        target.Create();
    }

    private struct ShapeData
    {
        public Vector3 position;
        public Vector3 scale;
        public Vector3 colour;
        public float blendStrength;
        public float angleZ;
        public int numChildren;
        public int shapeType;

        public static int GetSize()
        {
            return sizeof(float) * 11 + sizeof(int) * 2;
        }
    }

    #region ShaderPropertyIDs

    private static readonly int Source = Shader.PropertyToID("Source");
    private static readonly int Destination = Shader.PropertyToID("Destination");
    private static readonly int Shapes = Shader.PropertyToID("shapes");
    private static readonly int NumShapes = Shader.PropertyToID("numShapes");
    private static readonly int CameraToWorld = Shader.PropertyToID("_CameraToWorld");
    private static readonly int CameraInverseProjection = Shader.PropertyToID("_CameraInverseProjection");
    private static readonly int Light1 = Shader.PropertyToID("_Light");

    #endregion
}