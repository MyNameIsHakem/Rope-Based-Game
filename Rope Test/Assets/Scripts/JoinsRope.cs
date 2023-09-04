using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinsRope : MonoBehaviour
{
    public Camera Cam;
    public GameObject RopeSeg;
    [Tooltip("The Smallest Possible Number Of Segments to make a closed loop-Rope")]
    public int MinSegsNum;
    [Tooltip("The Closing Speed of the Created Rope ")] [Range(0 , 1f)]
    public float CloseSpeed;

    private List<HingeJoint2D> Segs;
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
                        Transform seg = Instantiate(RopeSeg, LastPos, new Quaternion()).transform;

                        //Converting the Dif vector to an angle
                        float angle = Mathf.Atan2(Dif.y, Dif.x) * Mathf.Rad2Deg - 90;
                        seg.Rotate(0 , 0 , angle);

                        //Debug.Log($"seg Rotation Before :{Mathf.Atan2(Dif.y, Dif.x)}*{Mathf.Rad2Deg}+ 90 = {angle}/ Dif = {Dif}");

                        Segs.Add(seg.GetComponent<HingeJoint2D>());
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

                        Segs.Add(seg);

                        if (Segs.Count >= MinSegsNum)
                        {
                            Vector2 dis = Segs[0].transform.position - seg.transform.position;

                            if (dis.magnitude <= SegLenght)
                            {
                                ChainComplet = true;
                                Segs[0].autoConfigureConnectedAnchor = false;
                            }
                        }
                    }
                }

            }
        }
        else
        {
            if(CurSeg != 0)
            {
                //Debug.Log("Going to the Number :" + CurTarget);
                Transform target = Segs[CurSeg].transform;

                Segs[0].transform.position = Vector2.MoveTowards(Segs[0].transform.position, target.position, CloseSpeed);

                if ((Segs[0].transform.position - target.position).magnitude <= SegLenght / 5)
                {
                    Destroy(Segs[CurSeg].gameObject);
                    CurSeg--;                    
                }
            }
            
        }
       
    }    
}
