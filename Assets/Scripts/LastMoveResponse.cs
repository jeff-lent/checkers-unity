using System;

namespace DefaultNamespace
{
    [Serializable]
    public class LastMoveResponse
    {
       
        public int start;
        public int end;
        public int jump;
        public bool gameOver;
        public bool additionalJumps;
        public string color;
    }
}
