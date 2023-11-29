using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 *   CameraScript was used for cinematics                    *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

public class CameraScript : MonoBehaviour
{
    public GameObject pivot;
    private List<GameObject> building = new List<GameObject>();
    private bool finished = false;
    private Vector3 start;
    // private float xAmplitude = 100f;
    // private float zAmplitude = 85f;
    // private float phase = 0;
    // Start is called before the first frame update
    void Start()
    {
        // start = gameObject.transform.position;
        start = new Vector3(0f, 30f, -35f);
        building.AddRange(GameObject.FindGameObjectsWithTag("Floor"));
        building.AddRange(GameObject.FindGameObjectsWithTag("Wall"));
        building.AddRange(GameObject.FindGameObjectsWithTag("Door"));
        building.AddRange(GameObject.FindGameObjectsWithTag("Exit"));
        building = building.OrderBy(x => Vector3.Distance(x.transform.position, pivot.transform.position)).ToList();

        foreach(GameObject piece in building) {
            piece.SetActive(false);
        }
        
        StartCoroutine(Build(0.0175f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!finished) {
            pivot.transform.Rotate(0.0f, 0.4f, 0.0f, Space.World);
        }

        // float x = xAmplitude * Mathf.Cos (Time.time + phase);
        // float z = zAmplitude * Mathf.Sin (Time.time + phase);
        // transform.localPosition = new Vector3(x, 40, z);
        // transform.LookAt(pivot.transform.position);

        if (start == gameObject.transform.position) {
            finished = true;
        }
    }

    private IEnumerator Build(float delay)
    {
        foreach(GameObject piece in building) {
            piece.SetActive(true);
            yield return new WaitForSeconds(delay);
        }
    }
}
