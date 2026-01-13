using UnityEngine;

public class RopeCreate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject ropePrefab;
    public int ropeCount;
    public Rigidbody2D anchorRig;
    FixedJoint2D prevJoint;
    void Start()
    {
        for (int i = 0; i < ropeCount; i++)
        {
            FixedJoint2D currentJoint = Instantiate(ropePrefab, transform).GetComponent<FixedJoint2D>();
            currentJoint.transform.localPosition = new Vector3(0, (i + 1) * -2f, 0);
            if (i == 0)
            {
                currentJoint.connectedBody = anchorRig;
            }
            else
            {
                currentJoint.connectedBody = prevJoint.GetComponent<Rigidbody2D>();
            }
            prevJoint = currentJoint;

            if(i==ropeCount - 1)
            {
                currentJoint.GetComponent<Rigidbody2D>().mass = 10;
                currentJoint.GetComponent<SpriteRenderer>().enabled = false;
            }

        }
    }
}
