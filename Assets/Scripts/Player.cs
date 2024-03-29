﻿using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public class Player : MonoBehaviour
{
    [SerializeField] private DijkstraAI _dijkstraAI;
    [SerializeField] private AStarAI _aStarAI;
    [SerializeField] private float _speed = 2f;
    private float _rotationSpeed = 1.5f;
    private float _rotation = 0f;
    private Vector3 _distance;
    private float _rotationmultiplier;

    private Animator _anim;
    private IPathFinding _iPath;
    public PathFindingWays pathFindingChoice;


    private void Awake()
    {
        _anim = GetComponent<Animator>();
        Assert.IsNotNull(_anim, "Failed to find Animator component.");

        Assert.IsNotNull(_dijkstraAI, "No reference to DijkstraAI script.");

        Assert.IsNotNull(_aStarAI, "No reference to AStarAI script.");


    }

    public void SetAIPathSource(PathFindingWays choice)
    {
        pathFindingChoice = choice;
        if (choice == PathFindingWays.DijkstraAI)
            _iPath = _dijkstraAI;
        else
            _iPath = _aStarAI;
    }

    private void Update()
    {
        if (_iPath == null || _iPath.Path == null || !_iPath.PathCompleted)
            return;

        if (_iPath.Path.Any())
        {
            _anim.SetBool("Move_Forward", true);

            transform.position = Vector3.MoveTowards(transform.position, _iPath.Path.Last(), _speed);

            if (transform.position == _iPath.Path.Last())
            {
                _iPath.Path.Remove(_iPath.Path.Last());

                if (_iPath.Path.Any())
                {
                    _distance = _iPath.Path.Last() - transform.position;
                    _rotationmultiplier = _distance.x * _distance.y;

                    if (_rotationmultiplier == 0f)
                    {
                        if (_distance.x == 1f)
                            _rotationmultiplier += 4f * _distance.x;

                        if (_distance.y != 0f)
                            _rotationmultiplier -= 2f * _distance.y;
                    }
                    transform.eulerAngles = new Vector3(0, 0, 45f * _rotationmultiplier);
                    Debug.Log(_distance);
                }
            }

        }
    }

    /// <summary>
    /// This is prepared for further game dev.
    /// </summary>
    //private void MovementController()
    //{
    //    float rotateInput = Input.GetAxis("Horizontal");

    //    _rotation += rotateInput * _rotationSpeed;

    //    transform.eulerAngles = new Vector3(0f, 0f, -_rotation);


    //    if (Input.GetKey(KeyCode.UpArrow))
    //    {
    //        _anim.SetBool("Move_Forward", true);
    //        transform.Translate(-transform.right * _speed * Time.smoothDeltaTime, Space.World);
    //    }
    //    else
    //        _anim.SetBool("Move_Forward", false);

    //}
}
