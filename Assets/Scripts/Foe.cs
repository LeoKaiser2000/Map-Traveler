using UnityEngine;

public class Foe : MonoBehaviour
{
    public Vector2Int mapPos;
    public MoveComponent moveComponent;

    // Start is called before the first frame update
    void Start()
    {
        moveComponent = GetComponent<MoveComponent>();
    }
}
