using System;

namespace DefaultNamespace
{
    [Serializable]
    public class MoveResponse
    {
        public string gameOver;
        public string legalMove;
        public string additionalJumps;
        public string jump;
        public string setTurn;
    }
}