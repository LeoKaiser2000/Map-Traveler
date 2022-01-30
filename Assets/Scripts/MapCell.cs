using UnityEngine;


public class MapCell : MonoBehaviour
{
    private Game _game;
    private SpriteRenderer _spriteRenderer;
    private Vector2Int _mapPos = new Vector2Int(10, 10);
    [SerializeField] private Color hoverColor;
    private Color _baseColor;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseColor = _spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(Game game, Vector2 scale, int startX, int startY)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _baseColor = _spriteRenderer.color;
        transform.localScale = scale;
        _mapPos.x = startX;
        _mapPos.y = startY;
        _game = game;
        _spriteRenderer.color = _baseColor;
    }

    /*
    public void OnMouseDown()
    {
        _game.OnCellPressed(this);
    }*/
    
    public void OnMouseOver()
    {
        _spriteRenderer.color = hoverColor;
        if (Input.GetMouseButtonDown(0))
        {
            _game.MovePlayerAt(_mapPos);
        }
        //Pressed right click
        else if (Input.GetMouseButtonDown(1))
        {
            _game.SetFoeAt(_mapPos);
        }
    }

    public void OnMouseExit()
    {
        _spriteRenderer.color = _baseColor;
    }
}
