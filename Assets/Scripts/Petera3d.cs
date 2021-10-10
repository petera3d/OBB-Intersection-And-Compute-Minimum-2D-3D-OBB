using UnityEngine;
using System;
using Unity.Mathematics;
using UnityEditor;

namespace Petera3d
{

    #region Structs

    [Serializable]
    public struct Point
    {
        public float x, y, z;

        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 ToVector3() => new Vector3(x, y, z);
        public Vector2 ToVector2() => new Vector2(x, y);
        public override string ToString()
        {
            return x + " , " + y + " , " + z;
        }

        public static Point operator *(Point point, float f) => new Point(point.x * f, point.y * f, point.z * f);
        public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    [Serializable]
    public struct AABB
    {
        public Point Center;
        public Point Size;
        public Point Extents => Size * 0.5f;

        public AABB(Point center, Point size)
        {
            Center = center;
            Size = size;
        }

        public bool IsCollide(AABB b)
        {
            if (Mathf.Abs(Center.x - b.Center.x) > Extents.x + b.Extents.x) return false;
            if (Mathf.Abs(Center.y - b.Center.y) > Extents.y + b.Extents.y) return false;
            if (Mathf.Abs(Center.z - b.Center.z) > Extents.z + b.Extents.z) return false;
            return true;
        }
    }

    #endregion

    #region Extentions

    public static class Vector3Ext
    {
        public static Point ToPoint(this Vector3 v3)
        {
            return new Point(v3.x, v3.y, v3.z);
        }
    }

    #endregion

    [Serializable]
    public struct Matrix3x3
    {
        private float[,] _maxtrix3x3;
        
        public float this[int r, int c]
        {
            get => _maxtrix3x3[r, c];
            set => _maxtrix3x3[r, c] = value;
        }

        public static readonly Matrix3x3 Identity =
            new Matrix3x3(new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1));
        
        public static readonly Matrix3x3 Zero = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
        
        
        public readonly Vector3 ColumnX => new Vector3(_maxtrix3x3[0, 0], _maxtrix3x3[1, 0], _maxtrix3x3[2, 0]);
        public readonly Vector3 ColumnY => new Vector3(_maxtrix3x3[0, 1], _maxtrix3x3[1, 1], _maxtrix3x3[2, 1]);
        public readonly Vector3 ColumnZ => new Vector3(_maxtrix3x3[0, 2], _maxtrix3x3[1, 2], _maxtrix3x3[2, 2]);
        public Matrix3x3(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            _maxtrix3x3 = new float[3, 3];
            _maxtrix3x3[0, 0] = v1.x;
            _maxtrix3x3[1, 0] = v1.y;
            _maxtrix3x3[2, 0] = v1.z;

            _maxtrix3x3[0, 1] = v2.x;
            _maxtrix3x3[1, 1] = v2.y;
            _maxtrix3x3[2, 1] = v2.z;

            _maxtrix3x3[0, 2] = v3.x;
            _maxtrix3x3[1, 2] = v3.y;
            _maxtrix3x3[2, 2] = v3.z;
        }

        //Row Matrix constructor 
        public Matrix3x3(float e00, float e01, float e02,
            float e10, float e11, float e12,
            float e20, float e21, float e22)
        {
            _maxtrix3x3 = new float[3, 3];
            _maxtrix3x3[0, 0] = e00;
            _maxtrix3x3[1, 0] = e10;
            _maxtrix3x3[2, 0] = e20;
            _maxtrix3x3[0, 1] = e01;
            _maxtrix3x3[1, 1] = e11;
            _maxtrix3x3[2, 1] = e21;
            _maxtrix3x3[0, 2] = e02;
            _maxtrix3x3[1, 2] = e12;
            _maxtrix3x3[2, 2] = e22;
        }
        
        public Matrix3x3 Transpose()
        {
            Matrix3x3 m = Zero;
            m[0, 0] = _maxtrix3x3[0, 0];
            m[0, 1] = _maxtrix3x3[1, 0];
            m[0, 2] = _maxtrix3x3[2, 0];
            m[1, 0]= _maxtrix3x3[0, 1];
            m[1, 1]= _maxtrix3x3[1, 1];
            m[1, 2]= _maxtrix3x3[2, 1];
            m[2, 0]= _maxtrix3x3[0, 2];
            m[2, 1]= _maxtrix3x3[1, 2];
            m[2, 2]= _maxtrix3x3[2, 2];
            return m;
        }
        
        #region Rotation Matrices

        public static Matrix3x3 RotateX(float theta)
        {
            theta *= Mathf.Deg2Rad;
            float sin = math.sin(theta);
            float cos = math.cos(theta);
            return new Matrix3x3(new Vector3(1, 0, 0), new Vector3(0, cos, sin), new Vector3(0, -sin, cos));
        }
        public static Matrix3x3 RotateY(float theta)
        {
            theta *= Mathf.Deg2Rad;
            float sin = math.sin(theta);
            float cos = math.cos(theta);
            return new Matrix3x3(new Vector3(cos, 0, -sin), new Vector3(0, 1, 0),new Vector3(sin, 0, cos));
        }
        public static Matrix3x3 RotateZ(float theta)
        {
            theta *= Mathf.Deg2Rad;
            float sin = math.sin(theta);
            float cos = math.cos(theta);
            return new Matrix3x3(new Vector3(cos, sin, 0),new Vector3( -sin, cos,0),new Vector3( 0, 0, 1));
        }

        public static Matrix3x3 Rotate(Vector3 rotation)
        {
            rotation *= Mathf.Deg2Rad;
            float sinx = math.sin(rotation.x);
            float cosx = math.cos(rotation.x);
            float siny = math.sin(rotation.y);
            float cosy = math.cos(rotation.y);
            float sinz = math.sin(rotation.z);
            float cosz = math.cos(rotation.z);
            Matrix3x3 X = new Matrix3x3(new Vector3(1, 0, 0), new Vector3(0, cosx, sinx), new Vector3(0, -sinx, cosx));
            Matrix3x3 Y = new Matrix3x3(new Vector3(cosy, 0, -siny), new Vector3(0, 1, 0), new Vector3(siny, 0, cosy));
            Matrix3x3 Z = new Matrix3x3(new Vector3(cosz, sinz, 0), new Vector3(-sinz, cosz, 0), new Vector3(0, 0, 1));
            return Z * Y * X;
        }

        #endregion
        public void MaxOffDiagonialElementValueAndPosition(out int p, out int q)
        {
            p = 0;
            q = 1; //if all off diagonial elements are Zero then assume that value on [0,1] position is the max (Zero). This privents to assign value on diagonial pos 0.0  
            // in the case math.abs(_maxtrix3x3[i, j]) > math.abs(maxValue) is never true
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == j) continue;
                    if (math.abs(_maxtrix3x3[i, j]) > math.abs(_maxtrix3x3[p, q]))
                    {
                        p = i;
                        q = j;
                        //Debug.Log("Max " +  _maxtrix3x3[p, q]);
                    }
                }
            }
        }

        public override string ToString()
        {
            return Math.Round(_maxtrix3x3[0, 0], 2) + " " + Math.Round(_maxtrix3x3[0, 1], 2) + " " +
                   Math.Round(_maxtrix3x3[0, 2], 2) + "\n" +
                   Math.Round(_maxtrix3x3[1, 0], 2) + " " + Math.Round(_maxtrix3x3[1, 1], 2) + " " +
                   Math.Round(_maxtrix3x3[1, 2], 2) + "\n" +
                   Math.Round(_maxtrix3x3[2, 0], 2) + " " + Math.Round(_maxtrix3x3[2, 1], 2) +
                   " " + Math.Round(_maxtrix3x3[2, 2], 2);
        }


        #region Operators

        public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
        {
            Matrix3x3 m = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        m[r, c] += a[r, k] * b[k, c];
                    }
                }
            }
            return m;
        }

        public static Matrix3x3 operator *(Matrix3x3 a, Quaternion q)
        {
           Vector3 X =  q * a.ColumnX;
           Vector3 Y =  q * a.ColumnY;
           Vector3 Z =  q * a.ColumnZ;
           return new Matrix3x3(X, Y, Z);
        }

        public static Matrix3x3 operator *(float f, Matrix3x3 a)
        {
            Matrix3x3 m = Zero;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    m[i, j] = a[i, j] * f;
                }
            }

            return m;
        }
        public static Matrix3x3 operator *(Matrix3x3 a,float f)
        {
            Matrix3x3 m = Zero;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    m[i, j] = a[i, j] * f;
                }
            }

            return m;
        }
        public static Vector3 operator *(Matrix3x3 a, Vector3 v)
        {
            float x = a[0, 0] * v.x + a[0, 1] * v.y + a[0, 2] * v.z;
            float y = a[1, 0] * v.x + a[1, 1] * v.y + a[1, 2] * v.z;
            float z = a[2, 0] * v.x + a[2, 1] * v.y + a[2, 2] * v.z;
            return new Vector3(x, y, z);
        }
        
        #endregion
    }

    public class Plane
    {
        public Vector3 Normal;
        public float Distance;
        
        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            Normal = Vector3.Normalize(Vector3.Cross(a - b, a - c));
            Distance = Vector3.Dot(a, Normal);
        }

        public float DistancePointFromPlane(Vector3 point)
        {
            return 0; // Vector3.Dot(v, plane.Normal);
        }
        
        public Vector3 ProjectPointOnPlane()
        {
            return Vector3.back;
        }
    }

    public class BalancedSearchTree
    {
        public class Node
        {
            public float Data;
            public int Level;
            public Node LeftNode;
            public Node RightNode;
            public Node Parent;
            public Vector2 pos; // used for Handles display           

            public Node(float data)
            {
                Data = data;
            }
        }

        public Node rootNode { get; private set; }

        public void Add(float data)
        {
            Node newNode = new Node(data);
            if (rootNode == null)
            {
                rootNode = newNode;
            }
            else
            {
                rootNode = Insert(rootNode, newNode, null);
                RecalculateParent(rootNode);
                RecalculateLevels(rootNode, 0);
            }
        }

        Node Insert(Node currentNode, Node newNode, Node parent)
        {
            if (currentNode == null)
            {
                currentNode = newNode;
                currentNode.Parent = parent;
                currentNode.Level = parent.Level + 1;
                return currentNode;
            }

            if (newNode.Data < currentNode.Data)
            {
                currentNode.LeftNode = Insert(currentNode.LeftNode, newNode, currentNode);
                currentNode = BalanceTree(currentNode);
            }
            else if (newNode.Data > currentNode.Data)
            {
                currentNode.RightNode = Insert(currentNode.RightNode, newNode, currentNode);
                currentNode = BalanceTree(currentNode);
            }

            return currentNode;
        }

        Node BalanceTree(Node node)
        {
            int bFactor = BalanceFactor(node);
            if (bFactor > 1)
            {
                if (BalanceFactor(node.LeftNode) > 0)
                {
                    node = RotateLL(node);
                }
                else
                {
                    node = RotateLR(node);
                }
            }

            if (bFactor < -1)
            {
                if (BalanceFactor(node.RightNode) > 0)
                {
                    node = RotateRL(node);
                }
                else
                {
                    node = RotateRR(node);
                }
            }

            return node;
        }

        public bool Find(float value)
        {
            if (Find(rootNode, value)?.Data == value)
            {
                print("Found " + value);
                return true;
            }
            else
            {
                print("Not Found " + value);
                return false;
            }
        }

        Node Find(Node node, float value)
        {
            if (node == null) return null;
            if (node.Data == value)
            {
                return node;
            }
            else if (value < node.Data)
            {
                return Find(node.LeftNode, value);
            }
            else if (value > node.Data)
            {
                return Find(node.RightNode, value);
            }
            else
            {
                return null;
            }
        }

        public void Delete(int value)
        {
            rootNode = Delete(rootNode, value);
        }

        Node Delete(Node node, float value)
        {
            if (node == null)
            {
                return node;
            }

            if (value < node.Data) //Left Tree
            {
                node.LeftNode = Delete(node.LeftNode, value);
                if (BalanceFactor(node) < -1)
                {
                    if (BalanceFactor(node.RightNode) <= 0)
                    {
                        node = RotateRR(node);
                    }
                    else
                    {
                        node = RotateRL(node);
                    }
                }
            }
            else if (value > node.Data) // Right Tree
            {
                node.RightNode = Delete(node.RightNode, value);
                if (BalanceFactor(node) > 1)
                {
                    if (BalanceFactor(node.LeftNode) >= 0)
                    {
                        node = RotateLL(node);
                    }
                    else
                    {
                        node = RotateLR(node);
                    }
                }
            }
            else if (value == node.Data)
            {
                if (node.LeftNode == null && node.RightNode == null) //leaf
                {
                    return null;
                }
                else if (node.LeftNode != null && node.RightNode != null) //two nodes
                {
                    Node temp = node.RightNode;
                    while (temp.LeftNode != null)
                    {
                        temp = temp.LeftNode;
                    }

                    node.Data = temp.Data;
                    node.RightNode = Delete(node.RightNode, temp.Data);
                }
                else // one node
                {
                    node = node.LeftNode != null ? node.LeftNode : node.RightNode;
                }
            }

            RecalculateParent(rootNode);
            RecalculateLevels(rootNode, 0);
            return node;
        }

        Node RotateLL(Node parent, bool dbg = false)
        {
            if (dbg) print(string.Format("LL Parent {0} Pivot {1}", parent.Data, parent.LeftNode.Data));
            Node pivot = parent.LeftNode;
            parent.LeftNode = pivot.RightNode;
            pivot.RightNode = parent;
            pivot.pos = Vector2.zero;
            return pivot;
        }

        Node RotateLR(Node parent, bool dbg = false)
        {
            if (dbg) print(string.Format("LR Parent {0} Pivot {1}", parent.Data, parent.LeftNode.Data));
            Node pivot = parent.LeftNode;
            parent.LeftNode = RotateRR(pivot);
            return RotateLL(parent);
        }

        Node RotateRR(Node parent, bool dbg = false)
        {
            if (dbg) print(string.Format("RR Parent {0} Pivot {1}", parent.Data, parent.RightNode.Data));
            Node pivot = parent.RightNode;
            parent.RightNode = pivot.LeftNode;
            pivot.LeftNode = parent;
            pivot.pos = Vector2.zero;
            return pivot;
        }

        Node RotateRL(Node parent, bool dbg = false)
        {
            if (dbg) print(string.Format("RL Parent {0} Pivot {1}", parent.Data, parent.RightNode.Data));
            Node pivot = parent.RightNode;
            parent.RightNode = RotateLL(pivot);
            return RotateRR(parent);
        }

        public int BalanceFactor(Node node, bool dbg = false)
        {
            int l = GetHeight(node.LeftNode);
            int r = GetHeight(node.RightNode);
            if (dbg) print(string.Format("BalanceFactor - Node {0} Left {1} Right {2}", node.Data, l, r));
            return l - r;
        }

        int GetHeight(Node node, bool dbg = false)
        {
            int height = 0;
            if (node != null)
            {
                int l = GetHeight(node.LeftNode);
                int r = GetHeight(node.RightNode);
                height = 1 + Mathf.Max(l, r);
                if (dbg)
                    print(string.Format("GetHeight - Node {0} Height {1} Left {2} Right {3}", node.Data, height, l, r));
            }

            return height;
        }

        void RecalculateLevels(Node node, int parentLevel)
        {
            if (node == null) return;
            node.Level = parentLevel++;
            RecalculateLevels(node.LeftNode, parentLevel);
            RecalculateLevels(node.RightNode, parentLevel);
        }

        void RecalculateParent(Node node, Node parent = null)
        {
            if (node == null) return;
            node.Parent = parent;
            RecalculateParent(node.LeftNode, node);
            RecalculateParent(node.RightNode, node);
        }

        public void DisplayTree(Node node)
        {
            if (node == null)
            {
                return;
            }

            print("Start ==============================");
            if (node != null)
            {
                if (node.LeftNode == null && node.RightNode == null)
                {
                    print(node.Data + " is leaf" + " Parent " + node.Parent?.Data);
                    return;
                }

                if (node.LeftNode != null)
                {
                    print("Parent " + node.Data + " Left " + node.LeftNode.Data + " Parent " + node.Parent?.Data);
                    DisplayTree(node.LeftNode);
                }

                if (node.RightNode != null)
                {
                    print("Parent " + node.Data + " Right " + node.RightNode.Data + " Parent " + node.Parent?.Data);
                    DisplayTree(node.RightNode);
                }
            }
        }

        void print(object o)
        {
            Debug.Log(o.ToString());
        }

        //Function to visualize BST with Unity Handles
        public void HandlesDisplayTree(BalancedSearchTree.Node node, Vector2 v = default)
        {
            if (node == null)
            {
                return;
            }

            if (node.LeftNode != null && node.RightNode != null && node.Parent == null)
            {
                Handles.color = Color.magenta;
                Handles.SphereHandleCap(1, node.pos, Quaternion.identity, 0.1f, EventType.Repaint);
                Handles.Label(node.pos, node.Data.ToString() + " : " + BalanceFactor(node));
            }

            if (node.LeftNode != null && node.LeftNode.Parent != null)
            {
                Node _node = node.LeftNode;
                float offsetX = 0;
                if (_node.Level == 1) offsetX = 3;
                else offsetX = 0f;
                _node.pos = new Vector2(_node.Parent.pos.x - 1 - offsetX, _node.Parent.pos.y - 1);
                Handles.color = _node.RightNode == null && _node.LeftNode == null ? Color.green : Color.blue;
                Handles.SphereHandleCap(1, _node.pos, Quaternion.identity, 0.1f, EventType.Repaint);
                //Handles.Label(_node.pos, _node.Data.ToString() + " Parent " + _node.Parent?.Data + " BF " + bst.BalanceFactor(_node) + " Level " + _node.Level);
                Handles.Label(_node.pos, _node.Data.ToString() + " BF " + BalanceFactor(_node));
                //Handles.Label(new Vector2(_node.pos.x - 0.1f, _node.pos.y), _node.Data.ToString());
                Handles.color = Color.yellow;
                Handles.DrawLine(_node.pos, _node.Parent.pos);
                if (_node.RightNode != null || _node.LeftNode != null)
                {
                    HandlesDisplayTree(_node);
                }
            }

            if (node.RightNode != null && node.RightNode.Parent != null)
            {
                Node _node = node.RightNode;
                float offsetX = 0;
                if (_node.Level == 1) offsetX = 3;
                else offsetX = 0f;
                _node.pos = new Vector2(_node.Parent.pos.x + 1 + offsetX, _node.Parent.pos.y - 1f);
                Handles.color = _node.RightNode == null && _node.LeftNode == null ? Color.green : Color.blue;
                Handles.SphereHandleCap(1, _node.pos, Quaternion.identity, 0.1f, EventType.Repaint);
                //Handles.Label(_node.pos, _node.Data.ToString() + " Parent " + _node.Parent?.Data + " BF " + bst.BalanceFactor(_node) + " Level " + _node.Level);
                Handles.Label(_node.pos, _node.Data.ToString() + " BF " + BalanceFactor(_node));
                //Handles.Label(new Vector2(_node.pos.x + 0.1f, _node.pos.y), _node.Data.ToString());
                Handles.color = Color.white;
                Handles.DrawLine(_node.pos, _node.Parent.pos);
                if (_node.RightNode != null || _node.LeftNode != null)
                {
                    HandlesDisplayTree(_node);
                }
            }
        }
    }

    public static class Probability
    {
        public static float Variance(float[] points)
        {
            float u = 0;
            int l = points.Length;
            for (int i = 0; i < l; i++)
            {
                u += points[i];
            }

            u /= l;
            float s2 = 0;
            for (int i = 0; i < l; i++)
            {
                s2 += (points[i] - u) * (points[i] - u);
            }

            return s2 / l;
        }

        public static Matrix3x3 CovarianceMatrix(Point[] points)
        {
            Point meanPoint = new Point(0, 0, 0);
            float l = points.Length;
            float n = 1f / l;
            float e00 = 0, e11 = 0, e22 = 0, e01 = 0, e02 = 0, e12 = 0;
            for (int i = 0; i < l; i++)
            {
                meanPoint += points[i];
            }

            meanPoint *= n;
            for (int i = 0; i < l; i++)
            {
                Point p = points[i] - meanPoint;
                e00 += p.x * p.x;
                e11 += p.y * p.y;
                e22 += p.z * p.z;
                e01 += p.x * p.y;
                e02 += p.x * p.z;
                e12 += p.y * p.z;
            }
            
            return new Matrix3x3(e00 * n, e01 * n, e02 * n, e01 * n, e11 * n, e12 * n, e02 * n, e12 * n, e22 * n);
        }
    }
    
}
        
