using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Camera Cam;
    public float RopeSegLenght;
    public int SegCount;

    private LineRenderer RopeRendrer;
    private Vector3[] DefaultArray;
    private bool IsTouching;
    private int CurPos;

    void Start()
    {
        RopeRendrer = GetComponent<LineRenderer>();
        DefaultArray = new Vector3[RopeRendrer.positionCount];
        RopeRendrer.GetPositions(DefaultArray);

        CurPos = 0;
        IsTouching = false;          
    }

    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            if(touch.phase == TouchPhase.Began)
            {
                IsTouching = true;
                RopeRendrer.SetPosition(0, (Vector2)Cam.ScreenToWorldPoint(touch.position));
                Debug.Log("Touch Pos = " + touch.position + " In World space : " + Cam.WorldToScreenPoint(touch.position));
            }
            else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) 
            {
                CurPos = 0;
                RopeRendrer.positionCount = DefaultArray.Length;
                RopeRendrer.SetPositions(DefaultArray);
                IsTouching = false;
            }
        }

        if (IsTouching)
        {
            Vector2 TouchPos = Cam.ScreenToWorldPoint(Input.touches[0].position);

            Vector3 Dif = RopeRendrer.GetPosition(CurPos) - (Vector3)TouchPos;

            if (Dif.magnitude > RopeSegLenght)
            {
                if(CurPos == RopeRendrer.positionCount - 1)
                {
                    RopeRendrer.positionCount++;
                }

                Vector3 NewPos = RopeRendrer.GetPosition(CurPos) + Dif.normalized * RopeSegLenght;

                CurPos++;
                RopeRendrer.SetPosition(CurPos, NewPos);
            }           
        }
    }              
}
