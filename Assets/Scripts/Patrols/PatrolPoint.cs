using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolPoint : MonoBehaviour
{
    private Vector3 point;
    private PatrolPoint next;
    private PatrolPoint previous;
    private Patrol patrol;
    [SerializeField]
    private LineRenderer lineRenderer;
    private bool redoLineRenderer;
    private AssignedPointPanel panel;

    private void Start()
    {
        point = transform.position;
        redoLineRenderer = false;
    }

    private void Update()
    {
        if (redoLineRenderer)
        {
            if(!next.GetPoint().Equals(new Vector3()))
            {
                redoLineRenderer = false;
                NavMeshPath path = new NavMeshPath();
                NavMesh.SamplePosition(point, out NavMeshHit hit1, 5, -1);
                NavMesh.SamplePosition(next.GetPoint(), out NavMeshHit hit2, 5, -1);
                NavMesh.CalculatePath(hit1.position, hit2.position, -1, path);
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
            }
        }
    }

    public void Setup(Patrol patrol)
    {
        this.patrol = patrol;
    }

    public void SetNext(PatrolPoint toSet)
    {
        next = toSet;
        if (next.GetPoint().Equals(new Vector3())) redoLineRenderer = true;
        else
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.SamplePosition(point, out NavMeshHit hit1, 5, -1);
            NavMesh.SamplePosition(toSet.GetPoint(), out NavMeshHit hit2, 5, -1);
            NavMesh.CalculatePath(hit1.position, hit2.position, -1, path);
            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }
    }

    public bool ContainsPoint(Vector3 toCheck, out PatrolPoint point)
    {
        if (Vector3.Distance(toCheck, this.point) < 1)
        {
            point = this;
            return true;
        }
        point = null;
        if (next == null) return false;
        if (next == patrol.GetOrigin()) return false;
        return next.ContainsPoint(toCheck, out point);
    }

    public bool IsPoint(Vector3 toCheck)
    {
        return toCheck == point;
    }

    public PatrolPoint GetNext()
    {
        return next;
    }

    public void SetPrevious(PatrolPoint toSet)
    {
        previous = toSet;
    }

    public PatrolPoint GetPrevious()
    {
        return previous;
    }

    public Vector3 GetPoint()
    {
        return point;
    }

    public Patrol GetPatrol()
    {
        return patrol;
    }

    public void ToggleVisible(bool toToggle) //I hope it's as simple as this, though I do have my doubts
    {
        if (toToggle)
        {
            gameObject.SetActive(true);
            lineRenderer.enabled = true;
        }
        else
        {
            gameObject.SetActive(false);
            lineRenderer.enabled = false;
        }
    }

    public void SetPanel(AssignedPointPanel toSet)
    {
        panel = toSet;
    }

    public void Select()
    {
        panel.AssignPoint(this);
    }

    public void Deselect()
    {
        panel.Deactivate();
    }
}
