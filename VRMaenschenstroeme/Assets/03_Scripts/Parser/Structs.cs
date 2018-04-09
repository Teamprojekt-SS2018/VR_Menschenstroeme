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

    /*public struct Triangle
    {
        public Point first, second, third;

        public Triangle(Point first, Point second, Point third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public override string ToString()
        {
            return "Dreieck: " + first + " - " + second + " - " + third;
        }

        public float getArea()
        {
            // TODO
            return 0;
        }
    }*/
}
