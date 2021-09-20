using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    public float radius;
    [Range(0,360)]
    public float angle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public float meshResolution;
    public int edgeIterations;
    public float edgeDistanceThreshold;

    public MeshFilter viewMeshFilter;
    Mesh viewMesh;
    PlayerController player;

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine("FindTargetWithDelay", .1f);

        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void LateUpdate()
    {
        DrawFieldOfView();
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, radius, targetMask);

        for (int i=0; i < targets.Length;i++) {
            Transform target = targets[i].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(transform.forward,directionToTarget) < angle/2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if(!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    Debug.Log("DETECTED!");
                    player.SendMessage("Die");
                }
            }
        }
    }
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(angle * meshResolution);
        float stepAngleSize = angle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for(int i=0;i<=stepCount;i++)
        {
            float drawAngle = transform.eulerAngles.y - angle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(drawAngle);
            
            if(i > 0)
            {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if(oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDistanceThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if(edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for(int i=0;i<vertexCount-1;i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 direction = directionFromAngle(globalAngle, true);
        RaycastHit hit;

        if(Physics.Raycast(transform.position, direction, out hit, radius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + direction * radius, radius, globalAngle);
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for(int i=0;i<edgeIterations;i++)
        {
            float edgeAngle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(edgeAngle);

            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDistanceThresholdExceeded)
            {
                minAngle = edgeAngle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = edgeAngle;
                maxPoint = newViewCast.point;
            }

        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public Vector3 directionFromAngle(float angle, bool angleIsGlobal)
    {
        if(!angleIsGlobal)
        {
            angle += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
        {
            this.hit = hit;
            this.point = point;
            this.distance = distance;
            this.angle = angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }
}
