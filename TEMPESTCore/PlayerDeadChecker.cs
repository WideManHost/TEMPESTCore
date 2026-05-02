namespace TEMPESTCore
{
    public class PlayerDeadChecker 
    {
        public UltrakillEvent onPlayerDead;
        private bool _activated;
        private NewMovement newMovement;
        private PlatformerMovement platformerMovement;
        private bool ready => this.newMovement != null && this.platformerMovement != null;
        private bool alive => !newMovement.dead || !platformerMovement.dead;

        public void Initialize()
        {
            this.newMovement = MonoSingleton<NewMovement>.Instance;
            this.platformerMovement = MonoSingleton<PlatformerMovement>.Instance;
        }

        public void Tick()
        {
            if (!this.ready || this.alive) return;
            if (newMovement.dead || _activated)
            {
                _activated = true;
                onPlayerDead.Invoke();
            }
        }
    }
}
