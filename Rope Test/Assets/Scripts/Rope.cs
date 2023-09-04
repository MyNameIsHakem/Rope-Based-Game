using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Camera Cam;
    public GameObject RopeSeg , RopeTipe;
    [Tooltip("The Smallest Possible Number Of Segments to make a closed loop-Rope")]
    public int MinSegsNum;
    [Tooltip("The Closing Speed of the Created Rope")]
    [Range(0, 3f)]
    public float CloseSpeed;

    private LineRenderer RopeRendrer;
    private List<HingeJoint2D> Segs;
    private DistanceJoint2D tipe;
    private float SegLenght;
    private bool IsTouching;
    private Vector2 LastPos;
    private int CurSeg;

    private bool ChainComplet;

    void Start()
    {
        ChainComplet = false;
        CurSeg = 0;
        Segs = new List<HingeJoint2D>(10);

        SegLenght = RopeSeg.transform.localScale.y;

        RopeRendrer = GetComponent<LineRenderer>();
        RopeRendrer.startWidth = RopeSeg.transform.localScale.x; 
        RopeRendrer.endWidth = RopeSeg.transform.localScale.x; 
    }

    void Update()
    {
        if (!ChainComplet)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    IsTouching = true;
                    LastPos = Cam.ScreenToWorldPoint(touch.position);
                    RopeRendrer.SetPosition(0, LastPos);
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    IsTouching = false;
                }
            }

            if (IsTouching)
            {
                Touch touch = Input.touches[0];

                Vector2 Dif = (Vector2)Cam.ScreenToWorldPoint(touch.position) - LastPos;

                if (Segs.Count == 0)
                {
                    if (Dif.magnitude >= SegLenght / 2)
                    {
                        HingeJoint2D seg = Instantiate(RopeSeg, LastPos, new Quaternion()).GetComponent<HingeJoint2D>();
                        tipe = Instantiate(RopeTipe, LastPos, new Quaternion()).GetComponent<DistanceJoint2D>();

                        //Converting the Dif vector to an angle
                        float angle = Mathf.Atan2(Dif.y, Dif.x) * Mathf.Rad2Deg - 90;
                        seg.transform.Rotate(0, 0, angle);

                        Segs.Add(seg);
                    }
                }
                else
                {
                    if (Dif.magnitude >= SegLenght)
                    {
                        Vector2 NewPos = LastPos + Dif.normalized * SegLenght;
                        HingeJoint2D seg = Instantiate(RopeSeg, NewPos, new Quaternion()).GetComponent<HingeJoint2D>();

                        //Converting the Dif vector to an angle
                        float angle = Mathf.Atan2(Dif.y, Dif.x) * Mathf.Rad2Deg - 90;
                        seg.transform.Rotate(0, 0, angle);

                        LastPos = NewPos;

                        Segs[CurSeg].connectedBody = seg.GetComponent<Rigidbody2D>();                       

                        CurSeg++;

                        RopeRendrer.positionCount++;
                        RopeRendrer.SetPosition(CurSeg, NewPos);

                        Segs.Add(seg);

                        if (Segs.Count >= MinSegsNum)
                        {
                            Vector2 dis = Segs[0].transform.position - seg.transform.position;

                            if (dis.magnitude <= SegLenght)
                            {
                                ChainComplet = true;
                                Segs[0].autoConfigureConnectedAnchor = false;
                                tipe.connectedBody = Segs[0].GetComponent<Rigidbody2D>();
                                tipe.enabled = true;
                            }
                        }
                    }
                }

            }
        }
        else
        {            
            if (CurSeg != 0)
            {
                //Debug.Log("Going to the Number :" + CurTarget);
                Transform target = Segs[CurSeg].transform;

                tipe.transform.position = Vector2.MoveTowards(tipe.transform.position, target.position, CloseSpeed);

                if ((tipe.transform.position - target.position).magnitude <= SegLenght / 5)
                {
                    Destroy(Segs[CurSeg].gameObject);
                    CurSeg--;
                }
            }

            UpdateRopeRendrer();
        }

    }

    void UpdateRopeRendrer()
    {
        RopeRendrer.positionCount = CurSeg + 1;
        Vector3[] NewPoses = new Vector3[CurSeg + 1];

        NewPoses[0] = Segs[NewPoses.Length - 1].transform.position;

        for (int i = 1; i < NewPoses.Length; i++)
        {
            NewPoses[i] = Segs[i].transform.position;
        }      

        RopeRendrer.SetPositions(NewPoses);
    }
}
