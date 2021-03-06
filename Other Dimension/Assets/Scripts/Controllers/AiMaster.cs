﻿using System;
using System.Collections;
using System.Collections.Generic;
using Controllers.States;
using UnityEngine;

namespace Controllers
{
    public abstract class AiMaster : Controller
    {
        [Header("AI Master")]
        [SerializeField] protected float _attackCooldownTime;
        public AiState State = AiState.Idle;
        protected StateChange StateChange => new StateChange(this);
        protected bool _isFindingPath;
        protected Coroutine _gdi;
        protected Vector3 _goalPosition;
        protected bool _usingPath;
        protected bool _attackCooldown;
        
        protected float _timer;

        public virtual void FixedUpdate()
        {
            if (_attackCooldown)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    _attackCooldown = false;
                }
            }
            switch (State)
            {
                case AiState.Idle:
                    IdleAction();
                    break;
                case AiState.Moving:
                    MoveCharacter();
                    break;
                case AiState.FindingTarget:
                    DetermineGoalPosition();
                    break;
                case AiState.Alert:
                case AiState.Attack:
                    Attack();
                    break;
                case AiState.Block:
                    Block();
                    break;
                case AiState.Capture:
                case AiState.Chase:
                case AiState.Maneuver:
                case AiState.Strike:
                case AiState.FindingPath:
                    if (_gdi != null && !_isFindingPath)
                    {
                        StopCoroutine(_gdi);
                    }
                    if (!_isFindingPath)
                    {
                        _gdi = StartCoroutine(VisualisePath());
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected abstract void IdleAction();
        protected abstract void DetermineGoalPosition();
        protected abstract void MoveCharacter();
        protected abstract IEnumerator VisualisePath();
        protected abstract void Attack();
        protected abstract void Block();
    }
}
