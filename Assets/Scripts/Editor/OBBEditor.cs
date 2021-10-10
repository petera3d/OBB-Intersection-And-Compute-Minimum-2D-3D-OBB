using System;
using Petera3d;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OrientedBoundindBox))]
public class OBBEditor : Editor
{
    private OrientedBoundindBox _orientedBoundingBox;
    private bool _isCollide;
    private int _pointsAmount = 12, _randomX = 20, _randomY = 7, _randomZ = 0;
    private void OnEnable()
    {
        _orientedBoundingBox = target as OrientedBoundindBox;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
       
        
        for (int j = 0; j < _orientedBoundingBox.ObbsList.Count; j++)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("OBB " + j);
            EditorGUILayout.Space(20);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            Vector3 c = EditorGUILayout.Vector3Field("OBB Center", _orientedBoundingBox.ObbsList[j].Center);
            _orientedBoundingBox.UpdateCenter(c, j);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            Vector3 e = EditorGUILayout.Vector3Field("OBB Extends", _orientedBoundingBox.ObbsList[j].Extends);
            _orientedBoundingBox.UpdateExtends(e, j);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            Vector3 r = EditorGUILayout.Vector3Field("OBB Rotation", _orientedBoundingBox.ObbsList[j].Rotation);
            _orientedBoundingBox.UpdateRotation(r, j);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            bool s = EditorGUILayout.Toggle("OBB Show on Editor", _orientedBoundingBox.ObbsList[j].show);
            _orientedBoundingBox.UpdateVisibility(s, j);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Delete OBB"))
            {
                _orientedBoundingBox.RemoveOBB(_orientedBoundingBox.ObbsList[j]);
                EditorUtility.SetDirty(_orientedBoundingBox);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("OBB Control");
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(50);
        if (GUILayout.Button("Add OBB"))
        {
            _orientedBoundingBox.AddOBB();
            EditorUtility.SetDirty(_orientedBoundingBox);
        }

        EditorGUILayout.EndHorizontal();

        //Generate Points for OOB Contruction
            
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Generate Points for OOB Contruction");
        EditorGUILayout.Separator();
            
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Points Amount");
        _pointsAmount =  EditorGUILayout.IntField(_pointsAmount); 
        EditorGUILayout.EndHorizontal();
            
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Random X Positions");
        _randomX =  EditorGUILayout.IntField(_randomX);
        EditorGUILayout.EndHorizontal();
            
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Random Y Positions");
        _randomY =  EditorGUILayout.IntField(_randomY);
        EditorGUILayout.EndHorizontal();
            
       // EditorGUILayout.BeginHorizontal();
       // EditorGUILayout.LabelField("Random Z Positions");
       // _randomZ =  EditorGUILayout.IntField(_randomZ);
       // EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);
            
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("GenerateRandomPoints"))
        {
            _orientedBoundingBox.GenerateRandomPoints(_pointsAmount,_randomX,_randomY,_randomZ);
        }
        EditorGUILayout.EndHorizontal();
            
        EditorGUILayout.Space(10);
            
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Minimum Area 2D OBB")) 
        {
            if (_orientedBoundingBox.Points.Length == 0)
            {
                Debug.Log("No points generated");
                return;
            }
            _orientedBoundingBox.GenerateMinimumAreaOBB2D();
        }
        EditorGUILayout.EndHorizontal();
        //
    }


    void DrawOBB(OBB obb)
    {
        if (!obb.show) return;
        Handles.color = Color.gray;
        Handles.DrawLine(obb.Center, obb.AxisX);
        Handles.DrawLine(obb.Center, obb.AxisY);
        Handles.DrawLine(obb.Center, obb.AxisZ);
        Handles.Label(obb.AxisX, "X");
        Handles.Label(obb.AxisY, "Y");
        Handles.Label(obb.AxisZ, "Z");
        Handles.Label(obb.Center, obb.name);
        Handles.color = !_isCollide ? Color.green : Color.red;
        //Bottom
        Handles.DrawLine(obb.BottomEdgeTopLeft, obb.BottomEdgeTopRight);
        Handles.DrawLine(obb.BottomEdgeTopLeft, obb.BottomEdgeBottomleft);
        Handles.DrawLine(obb.BottomEdgeTopRight, obb.BottomEdgeBottomRight);
        Handles.DrawLine(obb.BottomEdgeBottomleft, obb.BottomEdgeBottomRight);
        //Top
        Handles.DrawLine(obb.TopEdgeTopLeft, obb.TopEdgeTopRight);
        Handles.DrawLine(obb.TopEdgeTopLeft, obb.TopEdgeBottomleft);
        Handles.DrawLine(obb.TopEdgeTopRight, obb.TopEdgeBottomRight);
        Handles.DrawLine(obb.TopEdgeBottomleft, obb.TopEdgeBottomRight);
        //Sides
        Handles.DrawLine(obb.BottomEdgeTopLeft, obb.TopEdgeTopLeft);
        Handles.DrawLine(obb.BottomEdgeTopRight, obb.TopEdgeTopRight);
        Handles.DrawLine(obb.BottomEdgeBottomleft, obb.TopEdgeBottomleft);
        Handles.DrawLine(obb.BottomEdgeBottomRight, obb.TopEdgeBottomRight);
    }

    void DrawPoints()
    {
        if (_orientedBoundingBox.Points.Length == 0) return;
        Handles.color = Color.cyan;
        for (int i = 0; i <_orientedBoundingBox.Points.Length ; i++)
        {
            Vector3 pos = _orientedBoundingBox.Points[i].ToVector3();
            Handles.DrawWireCube(pos, Vector3.one * 0.2f);
            Handles.Label(pos,i.ToString());
        }
    }
    private void OnSceneGUI()
    {
        #region OOB Collision

        if (_orientedBoundingBox.ObbsList.Count == 0) return;
        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < _orientedBoundingBox.ObbsList.Count; i++)
        {
            Vector3 c = Handles.PositionHandle(_orientedBoundingBox.ObbsList[i].Center, quaternion.identity);
            _orientedBoundingBox.UpdateCenter(c, i);
        }

        if (Event.current.type == EventType.Repaint)
        {
            foreach (OBB obb in _orientedBoundingBox.ObbsList)
            {
                DrawOBB(obb);
            }

            if (_orientedBoundingBox.ObbsList.Count > 2)
            {
                if (_orientedBoundingBox.ObbsList[0].OBBCollision(_orientedBoundingBox.ObbsList[1], out Matrix3x3 rTest,
                    out Vector3 translation))
                {
                    _isCollide = true;
                }
                else
                {
                    _isCollide = false;
                }
            }
            
        }

        #endregion

        DrawPoints();
    }
}
