using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    // Floater v0.0.2
    // by Donovan Keith
    //
    // [MIT License](https://opensource.org/licenses/MIT)

    // User Inputs
    public float minDegreesPerSecond = 15.0f;
    public float maxDegreesPerSecond = 15.0f;
    public float amplitude = 0.5f;
    public float frequency = 1f;

    private float degreesPerSecond;

    // Position Storage Variables
    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    // Use this for initialization
    private void Start()
    {
        // Store the starting position & rotation of the object
        posOffset = transform.position;

        degreesPerSecond = Random.Range(minDegreesPerSecond, maxDegreesPerSecond);
    }

    // Update is called once per frame
    private void Update()
    {
        // Spin object around Y-Axis
        transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);

        // Float up/down with a Sin()
        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }
}
