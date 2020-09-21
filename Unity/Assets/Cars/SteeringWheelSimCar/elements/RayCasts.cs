using System.Collections.Generic;
using UnityEngine;

public class RayCasts : MonoBehaviour
{
    // length of raycasts
    public float length = 10;

    [Range(1, 360)] public float angle = 120;

    public int numberOfRays = 10;

    private void FixedUpdate()
    {
        RaycastHit hit;
        var rayInFront = ((transform.rotation) * Vector3.forward);
        for (var j = 0 - angle / 2; j < angle / 2; j += angle / numberOfRays)
        {
            var rayVector = Quaternion.AngleAxis(j, Vector3.up) * rayInFront;
            var position = transform.position + new Vector3(0, 0.4f, 0);

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

    public HitAndDistances GetHitsAndDistances()
    {
        var distances = new List<float>();
        var hits = new List<bool>();
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
                hits.Add(true);
            }
            else
            {
                distances.Add(0);
                hits.Add(false);
            }
        }

        return new HitAndDistances(distances, hits);
    }
}

public struct HitAndDistances
{
    private List<float> _distances;
    private List<bool> _isHits;

    public HitAndDistances(List<float> distances, List<bool> isHits)
    {
        _distances = distances;
        _isHits = isHits;
    }
}