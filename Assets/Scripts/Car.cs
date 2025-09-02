using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float radius;

    // Parameters computed at runtime to eliminate drift
    private Vector3 _center;
    private Vector3 _radiusVector;
    private float _angleDeg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InstantiateMovementParameters();
        SetInitialDirection();
    }


    private void InstantiateMovementParameters()
    {
        // Compute radius from world origin (XZ plane) using the car's initial position
        // This ignores Y and uses distance from (0,0,0) to (x,0,z)
        radius = new Vector2(transform.position.x, transform.position.z).magnitude;

        // Define the circle center to the LEFT of the car at the desired radius.
        // This guarantees a consistent circle regardless of current orientation/position.
        float r = Mathf.Abs(radius);
        _center = transform.position + (-transform.right) * r;

        // Vector from center to current position (ensure exact radius length)
        _radiusVector = transform.position - _center;
        if (_radiusVector.sqrMagnitude > 0f)
        {
            _radiusVector = _radiusVector.normalized * r;
        }

        _angleDeg = 0f;
    }

    private void SetInitialDirection()
    {
        // If speed is negative, flip the car by 180° so it visually "goes forward"
        if (speed < 0f)
        {
            transform.Rotate(Vector3.up, 180f, Space.Self);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float r = Mathf.Max(0.0001f, Mathf.Abs(radius));
        float angularSpeedDeg = (Mathf.Abs(speed) / r) * Mathf.Rad2Deg;

        // Positive speed => clockwise; Negative speed => counter-clockwise
        float
            dir = (speed >= 0f)
                ? -1f
                : 1f; // negative angle change = clockwise in Unity (Y-up)
        _angleDeg += dir * angularSpeedDeg * Time.deltaTime;

        // Exact position on the circle (no accumulation drift)
        Vector3 newPos =
            _center + Quaternion.AngleAxis(_angleDeg, Vector3.up) * _radiusVector;
        transform.position = newPos;

        // Face along the actual motion direction, independent of speed sign.
        // Sample a tiny step ahead along the path to get the correct forward vector.
        float epsDeg = Mathf.Max(0.01f, angularSpeedDeg * Time.deltaTime * 0.5f);
        float lookAheadAngle = _angleDeg + dir * epsDeg;
        Vector3 nextPos = _center +
                          Quaternion.AngleAxis(lookAheadAngle, Vector3.up) *
                          _radiusVector;
        Vector3 forward = (nextPos - newPos);
        if (forward.sqrMagnitude > 0.0000001f)
        {
            transform.rotation =
                Quaternion.LookRotation(forward.normalized, Vector3.up);
        }
    }
}