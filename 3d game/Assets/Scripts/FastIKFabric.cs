using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FastIKFabric : MonoBehaviour
{
    public int ChainLength = 2;

    public Transform target;
    public Transform Pole;
    [Header("Solver parameters")]
    public int Iterations = 10;

    /// <summary>
    /// distance when the solver stops
    /// </summary>
    public float Delta = 0.001f;
    /// <summary>
    ///  Strength of going back to the start position
    /// </summary>
    [Range(0, 1)]
    public float SnapBackStrength = 1f;


    protected float[] BonesLength;
    protected float CompleteLength;
    protected Transform[] Bones;
    protected Vector3[] Positions;
    protected Vector3[] StartDirectionSucc;
    protected Quaternion[] StartRotationBone;
    protected Quaternion StartRotationTarget;
    protected Quaternion StartRotationRoot;

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        Bones = new Transform[ChainLength + 1];
        Positions = new Vector3[ChainLength + 1];
        BonesLength = new float[ChainLength];
        StartDirectionSucc= new Vector3[ChainLength + 1];
        StartRotationBone = new Quaternion[ChainLength + 1];

        if (target == null)
        {
            target = new GameObject(gameObject.name + " target").transform;
            target.position = transform.position;
        }
        StartRotationTarget = target.rotation;
        CompleteLength = 0;

        //init data
        var current = transform;
        for(var i=Bones.Length-1; i >= 0; i--)
        {
            Bones[i] = current;
            StartRotationBone[i] = current.rotation;

            if (i == Bones.Length - 1)
            {
                //leaf
                StartDirectionSucc[i] = target.position - current.position;
            }
            else
            {
                //mid bone
                StartDirectionSucc[i] = Bones[i + 1].position - current.position;
                BonesLength[i] = StartDirectionSucc[i].magnitude;
                CompleteLength += BonesLength[i];
            }
            current = current.parent;

        }

    }

    private void LateUpdate()
    {
        ResolveIK();
    }
    void ResolveIK()
    {
        if (target == null)
            return;

        if (BonesLength.Length != ChainLength)
            Init();

        //get position
        for (int i = 0; i < Bones.Length; i++)
            Positions[i] = Bones[i].position;

        var RootRot = (Bones[0].parent != null) ? Bones[0].parent.rotation : Quaternion.identity;
        var RootRotDiff = RootRot * Quaternion.Inverse(StartRotationRoot);

        //calculation is it possible to reach?
        if ((target.position-Bones[0].position).sqrMagnitude>=CompleteLength*CompleteLength)
        {
            // just stretch it
            var direction = (target.position - Positions[0]).normalized;
            //set everything after root
            for(int i=1;i<Positions.Length;i++)
            {
                Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
            }

        }
        else
        {   
            for(int iteration=0; iteration<Iterations; iteration++)
            {
                //back
                for (int i=Positions.Length-1;i>0;i--)
                {
                    if (i == Positions.Length - 1)
                        Positions[i] = target.position;// set it to target
                    else
                        Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];//set in line on distance
                }

                //forward
                for (int i = 1;i< Positions.Length; i++)
                {
           
                        Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i-1];//set in line on distance
                }

                //close enough?
                if ((Positions[Positions.Length - 1] - target.position).sqrMagnitude < Delta * Delta)
                    break;
            }
        }
        //move towards pole
        if (Pole != null)
        {
            
            for (int i = 1; i < Positions.Length - 1; i++)
            {
                var plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(Pole.position);
                var projectedBone = plane.ClosestPointOnPlane(Positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - Positions[i - 1], projectedPole - Positions[i - 1], plane.normal);
                Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
            }
        }

        //set position
        for (int i = 0; i < Positions.Length; i++)
        {
            if (i == Positions.Length - 1)
                Bones[i].rotation = target.rotation * Quaternion.Inverse(StartRotationTarget) * StartRotationBone[i];
            else
                Bones[i].rotation = Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) * StartRotationBone[i];


            Bones[i].position = Positions[i];
        }
    }


    /*
    private void OnDrawGizmos()
    {
        var current = this.transform;
        for(int i=0; i<ChainLength&& current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);


            current = current.parent;
        }
    }
    */
}
