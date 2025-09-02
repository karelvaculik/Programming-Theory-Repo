using UnityEngine;

// INHERITANCE
public class RedCar : Car
{
    private float _baseSpeed;
    private Coroutine _boostRoutine;
    
    private void Awake()
    {
        _baseSpeed = Speed;
    }

    // POLYMORPHISM
    protected override void Interact()
    {
        StartSpeedBoost(2f);
    }
 
    // ABSTRACTION
    private void StartSpeedBoost(float durationSeconds)
    {
        // Refresh existing boost if already active
        if (_boostRoutine != null)
        {
            StopCoroutine(_boostRoutine);
        }

        // Apply boost (preserve sign; double magnitude)
        Speed = _baseSpeed * 2f;

        _boostRoutine = StartCoroutine(SpeedBoostTimer(durationSeconds));
    }
    
    private System.Collections.IEnumerator SpeedBoostTimer(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // Restore to pre-boost value
        Speed = _baseSpeed;
        _boostRoutine = null;
    }
    
    private void OnDisable()
    {
        // Ensure speed is restored if object gets disabled during boost
        if (_boostRoutine != null)
        {
            StopCoroutine(_boostRoutine);
            _boostRoutine = null;
        }
        Speed = _baseSpeed;
    }
}
