using System;
using Petera3d;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct OBB
{
    public Matrix3x3 Orientation
    {
        get;
        //private set;
        set;
    }

    public Vector3 Center;
    public Vector3 Extends;

    private Vector3 _rotation;

    //Used only for Unity's custom editor GUI for OBBList.
    public string name;

    public bool show;
    //

    #region Properties

    //It is not good idea at all to use rotation computations in properties. I did that to be able to change rotation value in Editor's inspector (There are other better solutions but this is quick and simple, good for debugging)
    public Vector3 Rotation
    {
        get => _rotation;
        set
        {
            Orientation = Matrix3x3.Identity * Matrix3x3.Rotate(value);
            _rotation = value;
            //Rotate OBB at separate axis per time
            //  if (!Mathf.Approximately(_rotation.x, value.x))
            //  {
            //      Orientation = Matrix3x3.Identity *  Matrix3x3.RotateX(value.x);
            //  }
            //  if (!Mathf.Approximately(_rotation.y, value.y))
            //  {
            //      Orientation = Matrix3x3.Identity *  Matrix3x3.RotateY(value.y);
            //  }
            //  if (!Mathf.Approximately(_rotation.z, value.z))
            //  {
            //      Orientation = Matrix3x3.Identity *  Matrix3x3.RotateZ(value.z);
            //  }
            //   _rotation = value;
            // }
        }
    }

    //We could use AxisX/Y/Z for OBB position matrix for our computations, but I prefer more simple (understandable) code, it is enough for what we do here.
    public Vector3 AxisX => Center + Extends.x * Orientation.ColumnX;
    public Vector3 AxisY => Center + Extends.y * Orientation.ColumnY;
    public Vector3 AxisZ => Center + Extends.z * Orientation.ColumnZ;

    //To avoid pivot computation for rotation matrix that is PR-P (P is position matrix for some vertex, R is rotation matrix and -P negate vertex position matrix)
    //We compute the rotation at world origin and only then add the OBB's center position at each vertex position to move vertex at OBB's world position
    public Vector3 BottomEdgeTopLeft =>
        Orientation * new Vector3(Extends.x, -Extends.y, Extends.z) + Center;

    public Vector3 BottomEdgeTopRight =>
        Orientation * new Vector3(Extends.x, -Extends.y, -Extends.z) + Center;

    public Vector3 BottomEdgeBottomleft =>
        Orientation * new Vector3(-Extends.x, -Extends.y, Extends.z) + Center;

    public Vector3 BottomEdgeBottomRight =>
        Orientation * new Vector3(-Extends.x, -Extends.y,
            -Extends.z) + Center;

    public Vector3 TopEdgeTopLeft =>
        Orientation * new Vector3(Extends.x, Extends.y, Extends.z) + Center;

    public Vector3 TopEdgeTopRight =>
        Orientation * new Vector3(Extends.x, Extends.y, -Extends.z) + Center;

    public Vector3 TopEdgeBottomleft =>
        Orientation * new Vector3(-Extends.x, Extends.y, Extends.z) + Center;

    public Vector3 TopEdgeBottomRight =>
        Orientation * new Vector3(-Extends.x, Extends.y, -Extends.z) + Center;

    #endregion

    public OBB(Vector3 center, Vector3 extends)
    {
        Center = center;
        Orientation = Matrix3x3.Identity;
        Extends = extends;
        _rotation = Vector3.zero;
        name = "default";
        show = true;
    }

    public OBB(Vector3 center, Vector3 extends, Matrix3x3 Orientation)
    {
        Center = center;
        this.Orientation = Orientation;
        Extends = extends;
        _rotation = Vector3.zero;
        name = "default";
        show = true;
    }

    public OBB(Vector3 center, Vector3 extends, Matrix3x3 Orientation, Vector3 rotation)
    {
        Center = center;
        this.Orientation = Orientation;
        Extends = extends;
        _rotation = rotation;
        name = "default";
        show = true;
    }

    //Generate minimum area OBB from  2D points. Compute center and orientation on xy plane.
    public OBB GenerateMinimumAreaOBB2D(Point[] points)
    {
        Vector2[] localBase = new Vector2[2];
        Vector2 center = Vector2.zero;
        Vector2 extends = Vector2.zero;
        int pointsCount = points.Length;
        float minArea = float.MaxValue;
        float eMax = 0, eMin = 0;
        float eNormalMax = 0, eNormalMin = 0;

        for (int i = 0, j = pointsCount - 1; i < pointsCount; j = i, i++)
        {
            //Get current edge and normalized
            Vector2 e = (points[i].ToVector2() - points[j].ToVector2()).normalized;
            //Get orthogonal axis to vector e
            Vector2 eNormal = new Vector2(-e.y, e.x);
            for (int k = 0; k < pointsCount; k++)
            {
                //Project points into vectors e and eNormal and get minumum and maximum values alogn both axis.
                Vector2 v = points[k].ToVector2() - points[j].ToVector2();
                float dot = Vector2.Dot(v, e);
                if (dot > eMax) eMax = dot;
                if (dot < eMin) eMin = dot;
                dot = Vector2.Dot(v, eNormal);
                if (dot > eNormalMax) eNormalMax = dot;
                if (dot < eNormalMin) eNormalMin = dot;
            }

            float le = eMax - eMin;
            float ln = eNormalMax - eNormalMin;
            float area = le * ln;
            if (area < minArea)
            {
                minArea = area;
                center = points[j].ToVector2() + 0.5f * ((eMax + eMin) * e + (eNormalMax + eNormalMin) * eNormal);
                extends.x = le / 2;
                extends.y = ln / 2;
                //Set orientation  
                localBase[0] = e;
                localBase[1] = eNormal;
            }
        }

        Orientation = new Matrix3x3(localBase[0], localBase[1], Vector2.zero);

        //Convert rotation (orientation) Matrix of Z Axis (2D)  to degrees for Rotation property. In this way OBB will be rotated upon creation.
        //David Eberly https://www.geometrictools.com/Documentation/EulerAngles.pdf
        float z;
        if (Orientation[2, 0] < 1) // < 90
        {
            if (Orientation[2, 0] > -1) // > -90 (-p/2)
            {
                z = Mathf.Atan2(Orientation[1, 0], Orientation[0, 0]);
            }
            else // = -90
            {
                z = -Mathf.Atan2(-Orientation[1, 2], Orientation[1, 1]);
            }
        }
        else
        {
            z = Mathf.Atan2(-Orientation[1, 2], Orientation[1, 1]);
        }

        z *= Mathf.Rad2Deg;
        
        //Other way is to compute z angle is with dot product of 2 vectors - normalized vector e and world x axis 
        //z = math.acos(Vector2.Dot(localBase[0] ,Vector2.right)) * Mathf.Rad2Deg

        return new OBB(center, extends, Orientation, new Vector3(0, 0, z));
    }

    // 15 separated axis tests need to determine OBB-OBB intersection (Separating Axis Theorem).
    //All out variables are for testing and visualization (with Handles) purpose only.
    public bool OBBCollision(OBB otherOBB, out Matrix3x3 rTest, out Vector3 translation)
    {
        Matrix3x3 absR = Matrix3x3.Zero;
        //Compute rotation matrix that will express otherOBB orientation in a's orientation frame.
        //We  need that to decrease needed computations.
        //Good article that explanes in details how and why we need that is https://ocw.mit.edu/courses/aeronautics-and-astronautics/16-07-dynamics-fall-2009/lecture-notes/MIT16_07F09_Lec03.pdf 

        #region First way, with dot product.

        //1st row
        // Matrix3x3 r = Matrix3x3.Zero;
        //   r[0, 0] = Vector3.Dot(Orientation.ColumnX, otherOBB.Orientation.ColumnX);
        //   r[0, 1] = Vector3.Dot(Orientation.ColumnX, otherOBB.Orientation.ColumnY);
        //   r[0, 2] = Vector3.Dot(Orientation.ColumnX, otherOBB.Orientation.ColumnZ);
        //   //2nd rowOrientation.
        //   r[1, 0] = Vector3.Dot(Orientation.ColumnY, otherOBB.Orientation.ColumnX);
        //   r[1, 1] = Vector3.Dot(Orientation.ColumnY, otherOBB.Orientation.ColumnY);
        //   r[1, 2] = Vector3.Dot(Orientation.ColumnY, otherOBB.Orientation.ColumnZ);
        //   //3rd rowOrientation.
        //   r[2, 0] = Vector3.Dot(Orientation.ColumnZ, otherOBB.Orientation.ColumnX);
        //   r[2, 1] = Vector3.Dot(Orientation.ColumnZ, otherOBB.Orientation.ColumnY);
        //   r[2, 2] = Vector3.Dot(Orientation.ColumnZ, otherOBB.Orientation.ColumnZ);

        #endregion

        //Second way with matrix multiplication
        Matrix3x3 r = Orientation * otherOBB.Orientation;
        float ra, rb;
        //Compute translation vector and bring it to a's coordination frame
        Vector3 t = otherOBB.Center - Center;
        t = new Vector3(Vector3.Dot(t, Orientation.ColumnX), Vector3.Dot(t, Orientation.ColumnY),
            Vector3.Dot(t, Orientation.ColumnZ));

        //Out variables for testing
        rTest = r;
        translation = t;
        //

        //Add smallest positive number to rotation matrix elements. When 2 edges are parallel their cross products is near to zero. 
        //This used avoid arithmetic errors
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                absR[i, j] = math.abs(r[i, j]) + float.Epsilon;
            }
        }

        //First we test 3 axis of a obb (The order of tests are important for better efficiency )

        //Test Axis X/Y/Z according .this OBB (Edge to face check)
        for (int i = 0; i < 3; i++)
        {
            ra = Extends[i];
            rb = otherOBB.Extends.x * absR[i, 0] + otherOBB.Extends.y * absR[i, 1] +
                 otherOBB.Extends.z * absR[i, 2];
            //Debug.Log("Axis : " + i + "   " + otherOBB.Extends.x +" * " + absR[i, 0] +" + " +  otherOBB.Extends.y + " * " +  absR[i, 1] +" + " +
            //        otherOBB.Extends.z + " * " +  absR[i, 2] + " : " + "RB = "  + rb + " : " + (ra + rb) + " : " + math.abs(t[i]));

            if (math.abs(t[i]) > ra + rb) return false;

        }

        //Test Axis X/Y/Z according otherOBB  (Edge to face check)
        for (int i = 0; i < 3; i++)
        {
            ra = Extends.x * absR[0, i] + Extends.y * absR[1, i] + Extends.z * absR[2, i];
            rb = otherOBB.Extends[i];
            //Debug.Log("Axis : " + i + "   " + Extends.x +" * " + absR[i, 0] +" + " +  Extends.y + " * " +  absR[i, 1] +" + " +
            //Extends.z + " * " +  absR[i, 2] + " : " + "RA = "  + ra + " : " + (ra + rb) + " : " + math.abs(t.x * r[0, i] +  t.y * r[1, i] +  t.z * r[2, i]));
               if (math.abs(t.x * r[0, i] + t.y * r[1, i] + t.z * r[2, i]) > ra + rb) return false;

        }

        //Test axis Lx = a.x cross Bx
        ra = Extends.y * absR[2, 0] + Extends.z * absR[1, 0];
        rb = otherOBB.Extends.y * absR[0, 2] + otherOBB.Extends.z * absR[0, 1];

        Vector3 va = new Vector3(Extends.x, Extends.z * absR[1, 0], Extends.y * absR[2, 0]);
        Vector3 vb = new Vector3(otherOBB.Extends.x, otherOBB.Extends.y * absR[0, 2], otherOBB.Extends.z * absR[0, 1]);

        Debug.DrawLine(Center, Center + va);
        Debug.DrawLine(otherOBB.Center, otherOBB.Center + vb);


       // if (math.abs(t.z * r[1, 0] - t.y * r[2, 0]) > ra + rb) return false;
//
        //    // Test axis L = A0 x B1
        //    ra = Extends.y * absR[2, 1] + Extends.z * absR[1, 1];
        //    rb = otherOBB.Extends.x * absR[0, 2] + otherOBB.Extends.z * absR[0, 0];
        //    if (math.abs(t[2] * r[1, 1] - t[1] * r[2, 1]) > ra + rb) return false;
        //    
        //    // Test axis L = A0 x B2
        //    ra = Extends.y * absR[2, 2] + Extends.z * absR[1, 2];
        //    rb = otherOBB.Extends.x * absR[0, 1] + otherOBB.Extends.y * absR[0, 0];
        //    if (math.abs(t[2] * r[1, 2] - t[1] * r[2, 2]) > ra + rb) return false;
        //    
        //    // Test axis L = A1 x B0
        //    ra = Extends.x * absR[2, 0] + Extends.z * absR[0, 0];
        //    rb = otherOBB.Extends.y * absR[1, 2] + otherOBB.Extends.z * absR[1, 1];
//
        //    if (math.abs(t[0] * r[2, 0] - t[2] * r[0, 0]) > ra + rb) return false;
        //    
        //    // Test axis L = A1 x B1
        //    ra = Extends.x * absR[2, 1] + Extends.z * absR[0, 1];
        //    rb = otherOBB.Extends.x * absR[1, 2] + otherOBB.Extends.z * absR[1, 0];
        //    if (math.abs(t[0] * r[2, 1] - t[2] * r[0, 1]) > ra + rb) return false;
        //    
        //    // Test axis L = A1 x B2
        //    ra = Extends.x * absR[2, 2] + Extends.z * absR[0, 2];
        //    rb = otherOBB.Extends.x * absR[1, 1] + otherOBB.Extends.y * absR[1, 0];
        //    if (math.abs(t[0] * r[2, 2] - t[2] * r[0, 2]) > ra + rb) return false;
        //    
        //    // Test axis L = A2 x B0
        //    ra = Extends.x * absR[1, 0] + Extends.y * absR[0, 0];
        //    rb = otherOBB.Extends.y * absR[2, 2] + otherOBB.Extends.z * absR[2, 1];
        //    if (math.abs(t[1] * r[0, 0] - t[0] * r[1, 0]) > ra + rb) return false;
        //    
        //    // Test axis L = A2 x B1
        //    ra = Extends.x * absR[1, 1] + Extends.y * absR[0, 1];
        //    rb = otherOBB.Extends.x * absR[2, 2] + otherOBB.Extends.z * absR[2, 0];
        //    if (math.abs(t[1] * r[0, 1] - t[0] * r[1, 1]) > ra + rb) return false;
        //    
        //    // Test axis L = A2 x B2
        //    ra = Extends.x * absR[1, 2] + Extends.y * absR[0, 2];
        //    rb = otherOBB.Extends.x * absR[2, 1] + otherOBB.Extends.y * absR[2, 0];
        //    if (math.abs(t[1] * r[0, 2] - t[0] * r[1, 2]) > ra + rb) return false;

        return true;
    }
}

