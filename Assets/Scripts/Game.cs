using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState
{
    PlayerInput,
    PlayerPathFinding,
    PlayerMove,
    FoeMove
}

public class Game : MonoBehaviour
{
    private MapCell[,] _map;
    private List<Foe> _foes;
    private Player _player;
    private GameState _gameState;

    private ThreadedRequest<PathFindingInput, List<Vector2Int>> _pathFindingThread;
    
    [SerializeField] private Player originalPlayer;
    [SerializeField] private Foe originalFoe;

    [SerializeField] private MapCell originalCell;
    [SerializeField] private int startingHeight = 10;
    [SerializeField] private int startingLength = 10;
    [SerializeField] private int startingFoes = 10;

    [SerializeField] private Vector2 cellScale = new Vector2(1, 1);
    [SerializeField] private Camera gameCamera = new Camera();
    [SerializeField] private float renderScale = 1;
    
    private static readonly Vector2Int[] AdjacentVec =
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    // Start is called before the first frame update
    private void Start()
    {
        GenerateMap();
        CenterCamera();
        GeneratePlayer();
        if (startingFoes + 1 < _map.GetLength(0) * _map.GetLength(1))
            GenerateFoes();
        _gameState = GameState.PlayerInput;
    }

    private void CenterCamera()
    {
        var height = _map.GetLength(0);
        var width = _map.GetLength(1);
        var transform1 = gameCamera.transform;
        transform1.position = new Vector3(
            width * cellScale.x / 2,
            height * cellScale.y / 2,
            transform1.position.z
        ) + transform.position;
        gameCamera.orthographicSize = Mathf.Max(width * cellScale.x, height * cellScale.y) * renderScale;
    }

    private void GenerateMap()
    {
        _map = new MapCell[startingHeight, startingLength];

        var height = _map.GetLength(0);
        var width = _map.GetLength(1);
        
        
        for (var y = 0; y < height; ++y)
        {
            for (var x = 0; x < width; ++x)
            {
                _map[y, x] = CellInstantiate(x, y);
            }
        }
    }
    
    private MapCell CellInstantiate(int x, int y)
    {
        var cell = Instantiate(
            originalCell,
            new Vector3(
                x * cellScale.x,
                y * cellScale.y,
                0
            ),
            Quaternion.identity,
            transform
        );
        cell.Init(this, cellScale, x, y);
        return cell;
    }

    private void GeneratePlayer()
    {
        var posY = Random.Range(0, _map.GetLength(0));
        var posX = Random.Range(0, _map.GetLength(1));

            _player = Instantiate(
            originalPlayer,
            new Vector3(
                posX * cellScale.x,
                posY * cellScale.y,
                -1
            ),
            Quaternion.identity,
            transform
        );
        _player.mapPos = new Vector2Int(posX, posY);
        _player.cellScale = cellScale;
    }

    private void GenerateFoes()
    {
        _foes = new List<Foe>();

        for (var i = 0; i < startingFoes; ++i)
        {
            var height = _map.GetLength(0);
            var width = _map.GetLength(1);

            var pos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));

            while (CharacterInPosition(pos))
            {
                pos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            }
            AddFoe(pos);
        }
    }

    private void AddFoe(Vector2Int pos)
    {

        var foe = Instantiate(
            originalFoe,
            new Vector3(
                pos.x * cellScale.x,
                pos.y * cellScale.y,
                -1
            ),
            Quaternion.identity,
            transform
        );
        foe.mapPos = pos;
        _foes.Add(foe);
    }

    private bool CharacterInPosition(Vector2Int pos)
    {
        return PlayerInPosition(pos) || FoeInPosition(pos);
    }

    private bool PlayerInPosition(Vector2Int pos)
    {
        return pos == _player.mapPos;
    }

    private bool FoeInPosition(Vector2 pos)
    {
        return _foes.Any(foe => foe.mapPos == pos);
    }

    private void RemoveFoe(Foe foe)
    {
        Destroy(foe.gameObject);
        _foes.Remove(foe);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_gameState)
        {
            case GameState.PlayerInput:
                break;
            case GameState.PlayerPathFinding:
                PathFindingStateLoop();
                break;
            case GameState.PlayerMove:
                _player.moveComponent.UpdateByTime(Time.deltaTime);
                if (!_player.moveComponent.running)
                {
                    if (!_player.NextMove())
                    {
                        _gameState = GameState.FoeMove;
                        StartCoroutine(MoveFoes());
                    }
                }

                break;
            case GameState.FoeMove:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void PathFindingStateLoop()
    {
        if (_pathFindingThread is null)
        {
            _gameState = GameState.PlayerInput;
            return;
        }
        if (_pathFindingThread.GetState() ==
            ThreadedRequest<PathFindingInput, List<Vector2Int>>.RequestState.Failure)
        {
            _gameState = GameState.PlayerInput;
            _pathFindingThread = null;
            return;
        }
        if (_pathFindingThread.GetState() ==
            ThreadedRequest<PathFindingInput, List<Vector2Int>>.RequestState.Succeed) {
            var solution = _pathFindingThread.GetValue();
            if (solution is null)
            {
                _gameState = GameState.PlayerInput;
            }
            else
            {
                if (solution.Count != 0)
                    solution.RemoveAt(0);
                _gameState = GameState.PlayerMove;
                var destination = solution.Last();
                _player.mapPos = destination;
                _player.moveBuffer = solution;
                _player.NextMove();
            }
            _pathFindingThread = null;
        }
    }

    public void MovePlayerAt(Vector2Int mapPos)
    {
        if (_gameState != GameState.PlayerInput)
        {
            LogInvalidGameState(GameState.PlayerInput);
            return;
        }
        if (CharacterInPosition(mapPos))
        {
            Debug.Log("Impossible move: a character is already in requested position.");
            return;
        }
        var input = new PathFindingInput(_map, _player, _foes, mapPos);
        _pathFindingThread = new ThreadedRequest<PathFindingInput, List<Vector2Int>>(PathFinder.FindPlayerBestWay, input);
        _gameState = GameState.PlayerPathFinding;
    }

    public void SetFoeAt(Vector2Int mapPos)
    {
        if (_gameState != GameState.PlayerInput)
        {
            LogInvalidGameState(GameState.PlayerInput);
            return;
        }
        var foe = _foes.FirstOrDefault(foe => foe.mapPos == mapPos);
        if (foe is null)
        {
            AddFoe(mapPos);
        }
        else
        {
            RemoveFoe(foe);
        }
    }

    private void LogInvalidGameState(GameState requiredState)
    {
        Debug.Log($"Action request {requiredState} phase. Current phase: {_gameState}.");
    }
    
    IEnumerator MoveFoes()
    {
        foreach (var foe in _foes)
        {
            var directions = AdjacentVec.Where(vec => ValidPosition(vec + foe.mapPos)).ToArray();
            if (directions.Length == 0) continue;
            var destination = foe.mapPos + directions[Random.Range(0, directions.Length)];
            foe.moveComponent.MoveToPositionIn(
                new Vector3(cellScale.x * destination.x, cellScale.y * destination.y, foe.transform.position.z),
                1);
            foe.mapPos = destination;
            yield return new WaitForSeconds(.5f);

            while (true)
            {
                foe.moveComponent.UpdateByTime(0.05f);
                if (!foe.moveComponent.running)
                    break;
                yield return new WaitForSeconds(0.05f);
            }
        }
        _gameState = GameState.PlayerInput;
    }

    private bool ValidPosition(Vector2Int pos)
    {
        return !(
            pos.x < 0 ||
            pos.y < 0 ||
            pos.x >= _map.GetLength(1) ||
            pos.y >= _map.GetLength(0) ||
            CharacterInPosition(pos)
        );
    }
}
