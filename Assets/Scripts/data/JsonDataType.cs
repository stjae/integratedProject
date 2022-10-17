using UnityEngine;

// serializable data types (struct) for json
namespace JsonDataType
{
    public struct ivec2
    {
        int x;
        int y;

        public int X { get { return x; } set { x = value; } }
        public int Y { get { return y; } set { y = value; } }

        public ivec2(int x, int y) { this.x = x; this.y = y; }
        public ivec2(Vector2Int v) { x = v.x; y = v.y; }
    }

    /*
    public struct vec3
    {
        float x;
        float y;
        float z;

        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public float Z { get { return z; } set { z = value; } }

        public vec3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public vec3(Vector3 v) { x = v.x; y = v.y; z = v.z; }
    }
    */
}