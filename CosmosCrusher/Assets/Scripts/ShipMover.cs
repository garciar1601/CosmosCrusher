using UnityEngine;
using System.Collections;

public class ShipMover : MonoBehaviour {

    public GameObject front;
    public GameObject left;
    public float speed;

    private Transform frontTransform;
    private Transform leftTransform;
    private Transform currentTransform;

	// Use this for initialization
	void Start () {
        frontTransform = front.GetComponent<Transform>();
        leftTransform = left.GetComponent<Transform>();
        currentTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKey)
        {
            Vector3 frontDirection = (currentTransform.position - frontTransform.position).normalized * speed;
            Vector3 leftDirection = (currentTransform.position - leftTransform.position).normalized * speed;
            if (Input.GetKey(KeyCode.W))
            {
                currentTransform.position += frontDirection;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                currentTransform.position += leftDirection;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                currentTransform.position += -frontDirection;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                currentTransform.position += -leftDirection;
            }
        }       
	}

    public void moveForward()
    {
        Vector3 frontDirection = (currentTransform.position - frontTransform.position).normalized * speed;
        //Vector3 leftDirection = (currentTransform.position - leftTransform.position).normalized * speed;
        currentTransform.position += frontDirection;        
    }
}
