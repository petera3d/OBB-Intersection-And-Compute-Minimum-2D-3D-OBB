using System.Collections.Generic;
using Petera3d;
using UnityEngine;

public class OrientedBoundindBox : MonoBehaviour
{
    public List<OBB> ObbsList;
    public Point[] Points;

    #region Editor's Functions

    public void AddOBB()
    {
        ObbsList.Add(new OBB(Vector3.zero, Vector3.one));
    }

    public void RemoveOBB(OBB obb)
    {
        ObbsList.Remove(obb);
    }

    public void UpdateCenter(Vector3 c, int listIndex)
    {
        OBB obb = ObbsList[listIndex];
        obb.Center = c;
        obb.name = "OBB " + listIndex; //Update index.
        ObbsList[listIndex] = obb;
    }

    public void UpdateRotation(Vector3 r, int listIndex)
    {
        OBB obb = ObbsList[listIndex];
        obb.Rotation = r;
        ObbsList[listIndex] = obb;
    }

    public void UpdateExtends(Vector3 e, int listIndex)
    {
        OBB obb = ObbsList[listIndex];
        obb.Extends = e;
        ObbsList[listIndex] = obb;
    }

    public void UpdateVisibility(bool canShow, int listIndex)
    {
        OBB obb = ObbsList[listIndex];
        obb.show = canShow;
        obb.name = "OBB " + listIndex; //Update index.
        ObbsList[listIndex] = obb;
    }

    public void GenerateRandomPoints(int pointsAmount, int rX, int rY, int rZ)
    {
        Points = new Point[pointsAmount];
        for (int i = 0; i < Points.Length; i++)
        {
            float x = UnityEngine.Random.Range(-rX, rX);
            float y = UnityEngine.Random.Range(-rY, rY);
            float z = UnityEngine.Random.Range(-rZ, rZ);
            Points[i] = new Point(x, y, z);
        }
    }

    public void GenerateMinimumAreaOBB2D()
    {
        ObbsList.Add(new OBB().GenerateMinimumAreaOBB2D(Points));
    }

    #endregion

}
