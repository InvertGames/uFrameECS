using uFrame.ECS;
using UnityEngine;

namespace Assets.uFrameECS.Tests.Basics
{
    public class ConstantRotation : EcsComponent
    {
        [SerializeField]
        private int _speed;

        public int Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
    }
}