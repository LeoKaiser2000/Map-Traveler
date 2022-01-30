using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    // Start is called before the first frame update
    private float _seconds;
    private float _timer;
    private Vector3 _destination;
    private Vector3 _difference;
    private Vector3 _start;
    private float _percent;
    public bool running;
    
    void Start()
    {
        running = false;
    }

    public void MoveToPositionIn(Vector3 destination, float seconds)
    {
        running = true;
        _destination = destination;
        _start = transform.position;
        _difference = _destination - _start;
        _seconds = seconds;
        _timer = 0;
        _percent = 0;
    }

    public void CancelMove()
    {
        running = false;
        transform.position = _start;
    }

    public void CancelAnimation()
    {
        running = false;
        transform.position = _destination;
    }
    
    // Update is called once per frame
    public void UpdateByTime(float deltaTime)
    {
        if (!running) return;
        _timer += deltaTime;
        if (_timer < _seconds)
        {
            _percent = _timer / _seconds;
            transform.position = _start + _difference * _percent;
        }
        else
        {
            running = false;
            transform.position = _destination;
        }
    }
}
