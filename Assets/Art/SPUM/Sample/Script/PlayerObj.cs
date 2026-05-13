using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObj : MonoBehaviour
{
    public SPUM_Prefabs _prefabs;
    public float _charMS;

    public Vector3 _goalPos;
    public bool isAction;
    private PlayerState _currentState;
    public Dictionary<PlayerState, int> IndexPair = new();

    private void Start()
    {
        if (_prefabs == null)
        {
            _prefabs = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
            if (!_prefabs.allListsHaveItemsExist()) _prefabs.PopulateAnimationLists();
        }

        _prefabs.OverrideControllerInit();
        foreach (PlayerState state in Enum.GetValues(typeof(PlayerState))) IndexPair[state] = 0;
    }

    private void Update()
    {
        if (isAction) return;

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.localPosition.y * 0.01f);
        switch (_currentState)
        {
            case PlayerState.IDLE:

                break;

            case PlayerState.MOVE:
                DoMove();
                break;
        }

        PlayStateAnimation(_currentState);
    }

    public void SetStateAnimationIndex(PlayerState state, int index = 0)
    {
        IndexPair[state] = index;
    }

    public void PlayStateAnimation(PlayerState state)
    {
        _prefabs.PlayAnimation(state, IndexPair[state]);
    }

    private void DoMove()
    {
        var _dirVec = _goalPos - transform.position;
        Vector3 _disVec = (Vector2)_goalPos - (Vector2)transform.position;
        if (_disVec.sqrMagnitude < 0.1f)
        {
            _currentState = PlayerState.IDLE;
            return;
        }

        var _dirMVec = _dirVec.normalized;
        transform.position += _dirMVec * _charMS * Time.deltaTime;


        if (_dirMVec.x > 0) _prefabs.transform.localScale = new Vector3(-1, 1, 1);
        else if (_dirMVec.x < 0) _prefabs.transform.localScale = new Vector3(1, 1, 1);
    }

    public void SetMovePos(Vector2 pos)
    {
        isAction = false;
        _goalPos = pos;
        _currentState = PlayerState.MOVE;
    }
}