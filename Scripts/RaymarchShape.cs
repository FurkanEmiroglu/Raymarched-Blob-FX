using DG.Tweening;
using UnityEngine;

[SelectionBase]
public class RaymarchShape : MonoBehaviour
{
    [Range(0, 1)]
    public float blendStrength;

    [HideInInspector]
    public int numChildren;

    public int shapeType;

    public int cachedShapeType;

    public Color colour = Color.white;

    public Vector3 Position
    {
        get { return transform.position; }
    }

    public Vector3 Scale
    {
        get { return transform.localScale; }
    }

    private void Awake()
    {
        if (transform.parent != null)
        {
            GameObject go = gameObject;
            go.name = transform.parent.name + " " + go.name;
        }
    }

    private void Start()
    {
        cachedShapeType = shapeType;
    }

    public void TweenBlendValue(float blend, float d)
    {
        float current = blendStrength;
        DOVirtual.Float(current, blend, d, UpdateBlendValue).Play();
    }

    public void SetActive(bool isActive)
    {
        shapeType = isActive ? cachedShapeType : -1;
    }

    private void UpdateBlendValue(float b)
    {
        blendStrength = b;
    }
}