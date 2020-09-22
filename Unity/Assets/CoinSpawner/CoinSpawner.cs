using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CoinSpawner
{
    public class CoinSpawner : MonoBehaviour
    {
        public GameObject spawnee;
        public float elevationAboveGround = 1.0f;
        public List<GameObject> coins;

        private Vector3 spawnPosition;
        private Bounds meshBounds;

        private void Start()
        {
            coins = new List<GameObject>();
            spawnPosition = transform.position;
            meshBounds = GetComponent<MeshRenderer>().bounds;
        }

        // Update is called once per frame
        void Update()
        {
            //generate coins while left mouse button is pressed
            if (Input.GetMouseButton(0))
            {
                SpawnCoin();
            }
        }

        public void SpawnCoin()
        {
            var position = new Vector3(
                Random.Range(spawnPosition.x - meshBounds.size.x / 2,
                    spawnPosition.x + meshBounds.size.x / 2),
                spawnPosition.y,
                Random.Range(spawnPosition.z - meshBounds.size.z / 2,
                    spawnPosition.z + meshBounds.size.z / 2));
            if (Physics.Raycast(position, Vector3.down, out var hit, 1000.0f))
            {
                coins.Add(Instantiate(spawnee, hit.point + new Vector3(0, elevationAboveGround, 0),
                    Quaternion.identity));
            }
            else
            {
                print("there seems to be no ground at this position");
            }
        }
    }
}