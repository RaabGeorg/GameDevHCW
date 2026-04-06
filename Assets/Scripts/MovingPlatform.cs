using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float platformSpeed;
    [SerializeField] private Vector3 start;
    [SerializeField] private Vector3 end;

    private Vector3 lastPosition;
    private Vector3 currentVelocity;
    

    void FixedUpdate()
    {
        lastPosition = transform.position;
        //calculate pos with pingpong
        float pingPong = Mathf.PingPong(Time.time * platformSpeed, 1.0f);
        Vector3 newPosition = Vector3.Lerp(start, end, pingPong);

        // velocity
        currentVelocity = (newPosition - lastPosition) / Time.fixedDeltaTime;

        // apply zhe movemnt
        transform.position = newPosition;
        lastPosition = newPosition;
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }
}