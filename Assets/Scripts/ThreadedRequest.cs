using System;
using System.Threading;
using UnityEngine;

public class ThreadedRequest<TArg,TResult>
{
    public enum RequestState
    {
        Running,
        Succeed,
        Failure
    }

    private RequestState _state;
    private readonly Thread _thread;
    private TResult _value;
    private readonly TArg _args;
    private readonly Func<TArg, TResult> _func;

    public ThreadedRequest(Func<TArg, TResult> func, TArg args)
    {
        _state = RequestState.Running;
        _func = func;
        _args = args;
        _thread = new Thread(Exec)
        {
            Name = "Pathfinding Secondary Thread"
        };
        _thread.Start();
    }

    private void Exec()
    {
        Debug.Log($"Thread {Thread.CurrentThread.Name} started.");
        try
        {
            _value = _func.Invoke(_args);
            _state = RequestState.Succeed;
            Debug.Log($"Thread {Thread.CurrentThread.Name} succeed.");
        }
        catch (Exception e)
        {
            _state = RequestState.Failure;
            Debug.Log($"Thread {Thread.CurrentThread.Name} failed.{e}");
        }
    }

    public RequestState GetState()
    {
        if (!_thread.IsAlive && _state == RequestState.Running)
            _state = RequestState.Failure;
        return _state;
    }
    
    public TResult GetValue()
    {
        if (GetState() != RequestState.Succeed)
            throw new ThreadStateException($"Thread {_thread.Name} did not end process");
        _thread.Join();
        return _value;
    }
}