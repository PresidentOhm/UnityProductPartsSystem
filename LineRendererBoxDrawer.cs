using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererBoxDrawer : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private List<LineRenderer> lineRenderers = new List<LineRenderer>();

#if UNITY_EDITOR
    [ContextMenu(nameof(InitializeLineRenderers))]
    public void InitializeLineRenderers()
    {
        LineRenderer[] childLineRenderers = GetComponentsInChildren<LineRenderer>();
        for (int i = 0; i < childLineRenderers.Length; i++)
        { 
            DestroyImmediate(childLineRenderers[i].gameObject);
        }
        lineRenderers = new List<LineRenderer>();
        int goCount = 0;
        GameObject go1 = new GameObject("line" + ++goCount);
        GameObject go2 = new GameObject("line" + ++goCount);
        GameObject go3 = new GameObject("line" + ++goCount);
        GameObject go4 = new GameObject("line" + ++goCount);
        GameObject go5 = new GameObject("line" + ++goCount);
        GameObject go6 = new GameObject("line" + ++goCount);
        GameObject go7 = new GameObject("line" + ++goCount);
        GameObject go8 = new GameObject("line" + ++goCount);
        GameObject go9 = new GameObject("line" + ++goCount);
        GameObject go10 = new GameObject("line" + ++goCount);
        GameObject go11 = new GameObject("line" + ++goCount);
        GameObject go12 = new GameObject("line" + ++goCount);

        go1.transform.parent = transform;
        go2.transform.parent = transform;
        go3.transform.parent = transform;
        go4.transform.parent = transform;
        go5.transform.parent = transform;
        go6.transform.parent = transform;
        go7.transform.parent = transform;
        go8.transform.parent = transform;
        go9.transform.parent = transform;
        go10.transform.parent = transform;
        go11.transform.parent = transform;
        go12.transform.parent = transform;

        lineRenderers.Add(go1.AddComponent<LineRenderer>());
        lineRenderers.Add(go2.AddComponent<LineRenderer>());
        lineRenderers.Add(go3.AddComponent<LineRenderer>());
        lineRenderers.Add(go4.AddComponent<LineRenderer>());
        lineRenderers.Add(go5.AddComponent<LineRenderer>());
        lineRenderers.Add(go6.AddComponent<LineRenderer>());
        lineRenderers.Add(go7.AddComponent<LineRenderer>());
        lineRenderers.Add(go8.AddComponent<LineRenderer>());
        lineRenderers.Add(go9.AddComponent<LineRenderer>());
        lineRenderers.Add(go10.AddComponent<LineRenderer>());
        lineRenderers.Add(go11.AddComponent<LineRenderer>());
        lineRenderers.Add(go12.AddComponent<LineRenderer>());

        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Debug.Log("Updated shadowcasting mode");
            lineRenderers[i].useWorldSpace = false;
            lineRenderers[i].loop = false;
            lineRenderers[i].sharedMaterial = material;
            lineRenderers[i].startWidth = 0.01f;
            lineRenderers[i].endWidth = 0.01f;
            lineRenderers[i].numCapVertices = 5;
            lineRenderers[i].allowOcclusionWhenDynamic = false;
        }

        //DrawTwelveLineBox(boxCollider.bounds);
    }
#endif

    public void DrawTwelveLineBox(Bounds boxBounds)
    {
        float halfSizeX = boxBounds.size.x * 0.5f;
        float halfSizeY = boxBounds.size.y * 0.5f;
        float halfSizeZ = boxBounds.size.z * 0.5f;

        //Get all points describing the 8 corners of the box 
        Vector3 point1 = new Vector3(boxBounds.center.x + halfSizeX, boxBounds.center.y + halfSizeY, boxBounds.center.z + halfSizeZ);
        Vector3 point2 = new Vector3(boxBounds.center.x - halfSizeX, boxBounds.center.y + halfSizeY, boxBounds.center.z + halfSizeZ);
        Vector3 point3 = new Vector3(boxBounds.center.x - halfSizeX, boxBounds.center.y + halfSizeY, boxBounds.center.z - halfSizeZ);
        Vector3 point4 = new Vector3(boxBounds.center.x - halfSizeX, boxBounds.center.y - halfSizeY, boxBounds.center.z - halfSizeZ);
        Vector3 point5 = new Vector3(boxBounds.center.x - halfSizeX, boxBounds.center.y - halfSizeY, boxBounds.center.z + halfSizeZ);
        Vector3 point6 = new Vector3(boxBounds.center.x + halfSizeX, boxBounds.center.y - halfSizeY, boxBounds.center.z - halfSizeZ);
        Vector3 point7 = new Vector3(boxBounds.center.x + halfSizeX, boxBounds.center.y - halfSizeY, boxBounds.center.z + halfSizeZ);
        Vector3 point8 = new Vector3(boxBounds.center.x + halfSizeX, boxBounds.center.y + halfSizeY, boxBounds.center.z - halfSizeZ);


        Vector3[] line1 = { point1, point2 };
        lineRenderers[0].positionCount = 2;
        lineRenderers[0].SetPositions(line1);
        Vector3[] line2 = { point1, point8 };
        lineRenderers[1].positionCount = 2;
        lineRenderers[1].SetPositions(line2);
        Vector3[] line3 = { point1, point7 };
        lineRenderers[2].positionCount = 2;
        lineRenderers[2].SetPositions(line3);
        Vector3[] line4 = { point3, point8 };
        lineRenderers[3].positionCount = 2;
        lineRenderers[3].SetPositions(line4);
        Vector3[] line5 = { point3, point2 };
        lineRenderers[4].positionCount = 2;
        lineRenderers[4].SetPositions(line5);
        Vector3[] line6 = { point3, point4 };
        lineRenderers[5].positionCount = 2;
        lineRenderers[5].SetPositions(line6);
        Vector3[] line7 = { point5, point2 };
        lineRenderers[6].positionCount = 2;
        lineRenderers[6].SetPositions(line7);
        Vector3[] line8 = { point5, point4 };
        lineRenderers[7].positionCount = 2;
        lineRenderers[7].SetPositions(line8);
        Vector3[] line9 = { point5, point7 };
        lineRenderers[8].positionCount = 2;
        lineRenderers[8].SetPositions(line9);
        Vector3[] line10 = { point6, point4 };
        lineRenderers[9].positionCount = 2;
        lineRenderers[9].SetPositions(line10);
        Vector3[] line11 = { point6, point7 };
        lineRenderers[10].positionCount = 2;
        lineRenderers[10].SetPositions(line11);
        Vector3[] line12 = { point6, point8 };
        lineRenderers[11].positionCount = 2;
        lineRenderers[11].SetPositions(line12);
    }   
 }
