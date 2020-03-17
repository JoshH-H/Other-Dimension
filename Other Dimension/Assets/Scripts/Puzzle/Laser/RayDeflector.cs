﻿using System;
using Controllers;
using UnityEngine;

namespace Puzzle.Laser
{
    public class RayDeflector : RayMaster, IRayReceiver, IRayInteract
    {
        [SerializeField] private CubeColour _colourType;
        [SerializeField, Range(1, 10)] private int _distanceFromPlayer;
        [SerializeField] private int _followSpeed;
        [SerializeField] private int _rotateSpeed;
        [SerializeField] private AudioSource _audio;
        private float _scrollScale = 1;
        private Vector3 _targetVector3;
        public Color _laserColour;
        private bool _userLaserColourProperty;
        private PlayerController _player;
        private float _upDistance;
        private Ray _ray;
        public Color LaserColour { get; set; }
        public Transform Transform => transform;


        private void Start()
        {
            _audio.Stop();
            switch (_colourType)
            {
                case CubeColour.Blue:
                    _laserColour = Color.blue;
                    break;
                case CubeColour.Green:
                    _laserColour = Color.green;
                    break;
                case CubeColour.Red:
                    _laserColour = Color.red;
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (FollowPlayer && _player)
            {

                var transform1 = _player.transform;
                var mouseScroll = Input.GetAxis("Mouse ScrollWheel");
                if (mouseScroll > 0) _upDistance += mouseScroll;
                else if (mouseScroll < 0) _upDistance += mouseScroll;
                _targetVector3 = transform1.position + transform1.forward * _distanceFromPlayer +
                                 transform1.up * _upDistance;
                transform.position = Vector3.Lerp(transform.position, _targetVector3, _followSpeed * Time.deltaTime);
                if (Input.GetKey(KeyCode.E))
                {
                    transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
                }

                if (Input.GetKey(KeyCode.Q))
                {
                    transform.Rotate(-Vector3.up * _rotateSpeed * Time.deltaTime);
                }
            }
            
            if (Time.time > _rayRunOutTime)
            {
                _hitWithRay = false;
                NotHitWithRay();
                if (_audio.isPlaying) _audio.Stop();
            }

            var position = Transform.position;

            _transformDirection = transform.forward;
            position = Transform.position;
            _laserVisual.SetPosition(0, position);
            _laserVisual.SetPosition(1, position);

            if (!_hitWithRay)
            {
                return;
            }

            Physics.Raycast(position, _transformDirection, out _hit, Mathf.Infinity, -10);
            _laserVisual.SetPosition(1, position + _transformDirection * _distance);
            if (!_hit.collider)
            {
                if (_rayReceiver == null) return;
                _rayReceiver.LaserColour -= _laserVisual.startColor;
                _rayReceiver = null;
                _addedColour = false;
                return;
            }

            _laserVisual.SetPosition(1, _hit.point);
            _rayReceiver = _hit.collider.gameObject.GetComponent<IRayReceiver>();
            _rayReceiver?.HitWithRay(this);

            if (_rayReceiver != null && !_addedColour)
            {
                _rayReceiver.LaserColour += _laserVisual.startColor;
                _addedColour = true;
            }
        }



        public void HitWithRay(Ray ray)
        {
            if (!_audio.isPlaying) _audio.Play();
            _ray = ray;
            _hitWithRay = true;
            _rayRunOutTime = Time.time + _hitByRayRefreshTime;
            _laserVisual.startColor = _laserColour;
            _laserVisual.endColor = _laserColour;
            var laserParticleMain = _laserParticle.main;
            laserParticleMain.startColor = _laserColour;
        }

        public void NotHitWithRay()
        {
            if(_ray) _ray._addedColour = false;
            _laserVisual.startColor = Color.black;
            _laserVisual.endColor = Color.black;
            var laserParticleMain = _laserParticle.main;
            laserParticleMain.startColor = Color.black;
            _laserVisual.SetPosition(1, Transform.position);
        }

        public bool FollowPlayer { get; set; }

        

        public void RayInteraction(PlayerController player)
        {
            _player = player;
            FollowPlayer = !FollowPlayer;
        }
    }
}
