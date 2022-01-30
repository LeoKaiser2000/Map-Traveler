using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2Int mapPos;
    public MoveComponent moveComponent;
    public List<Vector2Int> moveBuffer = new List<Vector2Int>();
    [SerializeField] private float moveTime = 0.3f;
    public Vector2 cellScale;
    
    // Start is called before the first frame update
    void Start()
    {
        moveComponent = GetComponent<MoveComponent>();
    }

    public bool NextMove()
    {
        if (moveComponent.running == false)
        {
            if (moveBuffer.Count == 0)
                return false;
            var move = moveBuffer[0];
            moveComponent.MoveToPositionIn(
                new Vector3(cellScale.x * move.x, cellScale.y * move.y, transform.position.z),
                moveTime);
            moveBuffer.RemoveAt(0);
        }
        return true;
    }
}
