using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleDraw : MonoBehaviour
{
    private const int SEGMENTS = 30;
    private LineRenderer lineRender;

    void Awake()
    {
        lineRender = gameObject.GetComponent<LineRenderer>();
        lineRender.useWorldSpace = false;
        lineRender.positionCount = (SEGMENTS + 1);
        lineRender.loop = true;
    }

    public void Draw(float radius, float lineWidth)
    {
        lineRender.startWidth = lineWidth;

        float x;
        float y;
        float angle = 0;

        for (int i = 0; i < (SEGMENTS + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lineRender.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / SEGMENTS);
        }
    }
}
