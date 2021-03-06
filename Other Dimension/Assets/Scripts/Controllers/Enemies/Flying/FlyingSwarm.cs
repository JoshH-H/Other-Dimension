﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers.Enemies.Flying
{
    [RequireComponent(typeof(SphereCollider))]
    public class FlyingSwarm : BaseFlyingAi
    {
        [Header("Swarm")]
        [SerializeField] private GameObject _boid;
        [SerializeField, Range(1, 50)] private int _flockTotal;
        [SerializeField] private int _spawnRadius;
        [SerializeField] private SphereCollider _sphere;

        private BoidRules boidRule = new BoidRules();
        private List<FlyingBoid> _boidSwarm = new List<FlyingBoid>();

        protected override void IdleAction()
        {
            //if (Math.Abs(_sphere.radius - moveableRadius) > float.Epsilon) _sphere.radius = moveableRadius;
            for (int i = 0; i < _flockTotal; i++)
            {
                GameObject clone = Instantiate(_boid);
                clone.transform.position = transform.position + Random.insideUnitSphere * _spawnRadius;
                _boidSwarm.Add(clone.GetComponent<FlyingBoid>());
            }
            foreach (var boid in _boidSwarm)
            {
                foreach (var t in _boidSwarm.Where(t => !boid.NeighboursRigidbodies.Contains(t.BoidRigidbody) && t != boid))
                {
                    boid.AddNeighbour(t.BoidRigidbody);
                    
                }
                boid.AddLeader(_rb);
                boid.AddNeighbour(_rb);
            }
            
            if (_boidSwarm.Count == _flockTotal) StateChange.ToFindTargetState();
        }
        
    }
}
