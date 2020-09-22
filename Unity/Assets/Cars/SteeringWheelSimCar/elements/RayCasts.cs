using System.Collections.Generic;
using UnityEngine;

public class RayCasts : MonoBehaviour
{
    public float length = 10;

    [Range(1, 360)] public float angle = 120;

    public int numberOfRays = 10;

    public bool drawRays = false;

    public float rayCastsElevation = 0.4f;
    private void FixedUpdate()
    {
        if(drawRays)
            DrawRayCasts();
    }

    private void DrawRayCasts()
    {
        RaycastHit hit;
        var rayInFront = ((transform.rotation) * Vector3.forward);
        for (var j = 0 - angle / 2; j < angle / 2; j += angle / numberOfRays)
        {
            var rayVector = Quaternion.AngleAxis(j, Vector3.up) * rayInFront;
            var position = transform.position + new Vector3(0, rayCastsElevation, 0);

            var ray = new Ray(position, rayVector);
            if (Physics.Raycast(ray, out hit, length))
            {
                Debug.DrawRay(position, rayVector * hit.distance, Color.yellow, 0, true);
                //print("distance: " + hit.distance + "angle: " + j);
            }
            else
            {
                Debug.DrawRay(position, rayVector * length, Color.white, 0, true);
            }
        }
    }

    public RayCastObservations GetHitsAndDistances()
    {
        var distances = new List<float>();
        var hits = new List<int>();
        RaycastHit hit;
        var rayInFront = ((transform.rotation) * Vector3.forward);
        for (var j = 0 - angle / 2; j < angle / 2; j += angle / numberOfRays)
        {
            var rayVector = Quaternion.AngleAxis(j, Vector3.up) * rayInFront;
            var position = transform.position;
            var ray = new Ray(position, rayVector);
            if (Physics.Raycast(ray, out hit, length))
            {
                distances.Add(hit.distance);
                hits.Add(1);
            }
            else
            {
                distances.Add(0);
                hits.Add(0);
            }
        }

        return new RayCastObservations(distances, hits);
    }
}

public struct RayCastObservations
{
    private List<float> _distances;
    private List<int> _hits;

    public List<float> Distances => _distances;
    public List<int> Hits => _hits;

    public RayCastObservations(List<float> distances, List<int> hits)
    {
        _distances = distances;
        _hits = hits;
    }
}