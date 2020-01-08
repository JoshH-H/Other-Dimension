﻿using GamePhysics;
using UnityEngine;

namespace Controllers
{
    public class GravityController : MonoBehaviour
    {
        [SerializeField] private WhiteHole _gravityCentre;
        [SerializeField] private BouncyBall playerTransform;

        void FixedUpdate()
        {
            if (_gravityCentre)
            {
                _gravityCentre.Repel(playerTransform);
            }
        }
    }
}