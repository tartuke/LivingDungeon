//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Experimental.Rendering.Universal;

//[ExecuteInEditMode]
//public class TileMapShadowCaster : MonoBehaviour
//{

//    internal CompositeCollider2D compositeCollider;
//    internal uint shapeHash;

//    // Start is called before the first frame update
//    void Start()
//    {
//        compositeCollider = GetComponent<CompositeCollider2D>();
//    }
//    private void OnEnable()
//    {
//        compositeCollider = GetComponent<CompositeCollider2D>();
//        Update();
//    }

//    private void OnDisable()
//    {
//        DestroyShadowCasters(transform.GetComponentsInChildren<ShadowCaster2D>());
//    }
//    // Update is called once per frame
//    void Update()
//    {
//        if (shapeHash != compositeCollider.GetShapeHash())
//        {
//            ShadowCaster2D[] old_list = transform.GetComponentsInChildren<ShadowCaster2D>();

//            if (compositeCollider != null)
//            {
//                int i = 0;
//                while( i < compositeCollider.pathCount)
//                {
//                    int pc = compositeCollider.GetPathPointCount(i);

//                    Vector2[] points = new Vector2[pc];
//                    compositeCollider.GetPath(i, points);

//                    Vector3[] path = new Vector3[pc];
//                    for (int j = 0; j < pc; j++)
//                        path[j] = points[j];

//                    if (i < old_list.Length)
//                    {
//                        old_list[i].setPath(path);
//                        old_list[i].shapePathHash += 1;
//                    } else 
//                        GenerateShadowCaster(path);
//                    i++;
//                }
//                while (i < old_list.Length)
//                {
//                    DestroyImmediate(old_list[i].gameObject);
//                    i++;
//                }
//            }
//            shapeHash = compositeCollider.GetShapeHash();
//        }
//    }

//    void GenerateShadowCaster(Vector3[] path)
//    {
//        GameObject newCaster = new GameObject();
//        newCaster.transform.parent = transform;

//        ShadowCaster2D sc = newCaster.AddComponent<ShadowCaster2D>();
//        if (sc != null)
//        {
//            sc.setPath(path);
//            sc.shapePathHash += 1;
//            sc.selfShadows = false;
//        }
//    }

//    void DestroyShadowCasters(ShadowCaster2D[] list)
//    {
//        foreach (ShadowCaster2D sc in list)
//            DestroyImmediate(sc.gameObject);
//    }
//}
