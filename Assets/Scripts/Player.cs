using UnityEngine;

namespace Matchbox
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

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

        public TIMERBOMB BOMB;
        public float BombsTimer => _caryyingBomb.TimerCountdown;
        public float BombsMaxTime => _caryyingBomb.LifeTime;
        
        
        private void SetBomb()
        {
            _caryyingBomb = controller.bombAttackTransform.GetComponent<Bomb>();
        }

        public void Update()
        {
            BOMB.currentTime = BombsTimer;
            BOMB.countdownDuration = BombsMaxTime;
        }

    }
}