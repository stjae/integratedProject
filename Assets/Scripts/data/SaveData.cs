namespace Poly.Data
{
    [System.Serializable]
    public class SaveData
    {
        private int chapter;
        private int level;
        private int checkpoint;

        public int Chapter    { get { return chapter; }    set { chapter = value; } }
        public int Level      { get { return level; }      set { level = value; } }
        public int Checkpoint { get { return checkpoint; } set { checkpoint = value; } }

        public SaveData()
        {
            chapter    = 0;
            level      = 0;
            checkpoint = 0;
        }
    }
}