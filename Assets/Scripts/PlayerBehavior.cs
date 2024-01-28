using UnityEngine;

namespace Matchbox
{
    public class PlayerBehavior : MonoBehaviour
    {
        public static PlayerBehavior Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            // DontDestroyOnLoad(this);
        }
        public CharacterController controller;


        private Bomb _caryyingBomb;


        public float BombsTimer => _caryyingBomb.TimerCountdown;
        public float BombsMaxTime => _caryyingBomb.LifeTime;
        
        
        private void SetBomb()
        {
            _caryyingBomb = controller.bombAttackTransform.GetComponent<Bomb>();
        }
        
        
        
    }
}