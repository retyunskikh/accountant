using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class PlayerAnimation : MonoBehaviour
{
    public Color color = Color.white;
    public Vector2 size = new Vector2(2, 1);
    public float radius = 0.25f;
    public float waveAmplitude = 5f;
    public float waveFrequency = 4;
    public float waveSpeed = 20;

    private MaterialPropertyBlock props;
    private MeshRenderer mr;

    void Awake()
    {
        mr = GetComponent<MeshRenderer>();
        props = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (mr == null)
        {
            mr = GetComponent<MeshRenderer>();
        }
        if (props==null)
        {
            props = new MaterialPropertyBlock();
        }

        props.SetColor("_Color", color);
        props.SetVector("_Size", new Vector4(size.x, size.y, 0, 0));
        props.SetFloat("_Radius", radius);
        props.SetFloat("_WaveAmplitude", waveAmplitude);
        props.SetFloat("_WaveFrequency", waveFrequency);
        props.SetFloat("_WaveSpeed", waveSpeed);
        props.SetFloat("_AnimTime", Time.time);
        mr.SetPropertyBlock(props);

        transform.localScale = new Vector3(size.x, size.y, 1);
    }
}