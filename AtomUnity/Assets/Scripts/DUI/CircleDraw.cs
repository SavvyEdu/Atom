using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleDraw : MonoBehaviour
{
    private const int SEGMENTS = 50;
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
        lineRender.startWidth = lineWidth; //set the line wideth 
        
        float angle = 0; //temp variables

        for (int i = 0; i < (SEGMENTS + 1); i++)
        {
            lineRender.SetPosition(i, new Vector3(Mathf.Sin(angle) * radius, 
                                                  Mathf.Cos(angle) * radius, 0));

            angle += Mathf.PI * 2 / SEGMENTS; //increase the angle
        }
        Debug.Log(lineWidth + " - " + lineRender.positionCount);
    }
}
