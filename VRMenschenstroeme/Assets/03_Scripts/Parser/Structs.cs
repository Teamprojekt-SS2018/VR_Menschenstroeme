using UnityEngine;

public class Structs {
    /* Structs */

    public struct Entrance
    {
        Point left;
        Point right;
        float passageSpeed;
        float passageCount;

        public Entrance(float leftX, float leftY, float rightX, float rightY, float passageSpeed, float passageCount)
        {
            this.left = new Point(leftX, leftY);
            this.right = new Point(rightX, rightY);
            this.passageSpeed = passageSpeed;
            this.passageCount = passageCount;
        }

        public Entrance(Point left, Point right, float passageSpeed, float passageCount)
        {
            this.left = left;
            this.right = right;
            this.passageSpeed = passageSpeed;
            this.passageCount = passageCount;
        }

        public override string ToString()
        {
            return "Eingang: " + left + " - " + right + " mit " + passageSpeed + " P/s und " + passageCount;
        }
    }

    public struct Exit
    {
        public Point left, right;

        public Exit(Point left, Point right)
        {
            this.left = left;
            this.right = right;
        }

        public Exit(float leftX, float leftY, float rightX, float rightY)
        {
            this.left = new Point(leftX, leftY);
            this.right = new Point(rightX, rightY);
        }

        public override string ToString()
        {
            return "Ausgang: " + left + " - " + right;
        }
    }

    public struct Point
    {
        public float x, y;
        Vector2 vec2;
        Vector3 vec3;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.vec3 = new Vector3(x, 0, y);
            this.vec2 = new Vector2(x, y);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public Vector2 ToVectorTwo()
        {
            return vec2;
        }

        public Vector3 ToVectorThree()
        {
            return vec3;
        } 
    }

    public struct PersonPosition
    {
        public bool activated;
        public Vector3 position;
        public float time;
        public float density;

        public PersonPosition(float time)
        {
            activated = false;
            position = Vector3.zero;
            this.time = time;
            this.density = 0;
        }

        public PersonPosition(Vector3 pos, float time, float density)
        {
            activated = true;
            position = pos;
            this.time = time;
            this.density = density;
        }
    }
}
