using System;
using UnityEngine;
using TA;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;
internal static class FlankingMathExtensions
{
    internal static bool LineIntersectsCubeOppositeSides(Point3D p1, Point3D p2, Cube cube)
    {
        // Check if the line intersects opposite sides of the cube
        var intersectsFrontBack =
            LineIntersectsFace(p1, p2, cube.FrontFace) && LineIntersectsFace(p1, p2, cube.BackFace);

        if (intersectsFrontBack)
        {
            return true;
        }

        var intersectsLeftRight =
            LineIntersectsFace(p1, p2, cube.LeftFace) && LineIntersectsFace(p1, p2, cube.RightFace);

        if (intersectsLeftRight)
        {
            return true;
        }

        var intersectsTopBottom =
            LineIntersectsFace(p1, p2, cube.TopFace) && LineIntersectsFace(p1, p2, cube.BottomFace);

        return intersectsTopBottom;
    }

    internal static bool LineIntersectsFace(Point3D p1, Point3D p2, Plane face)
    {
        // Check if the line intersects the plane of the face
        if (!LineIntersectsPlane(p1, p2, face))
        {
            return false;
        }

        // Find the intersection point on the plane
        var intersection = GetIntersectionPoint(p1, p2, face);

        // Check if the intersection point is within the boundaries of the face
        return PointIsWithinFace(intersection, face);
    }

    internal static bool LineIntersectsPlane(Point3D p1, Point3D p2, Plane plane)
    {
        // Compute the direction vector of the line
        var direction = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);

        // Compute the dot product of the line direction and the normal vector of the plane
        var dotProduct = direction.DotProduct(plane.Normal);

        // If the dot product is close to zero, the line is parallel to the plane
        return !(Math.Abs(dotProduct) < double.Epsilon);
    }

    internal static Point3D GetIntersectionPoint(Point3D p1, Point3D p2, Plane plane)
    {
        // Compute the direction vector of the line
        var direction = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);

        // Compute the distance from p1 to the intersection point
        var t = (plane.D - plane.Normal.DotProduct(new Vector3D(p1.X, p1.Y, p1.Z))) /
                plane.Normal.DotProduct(direction);

        // Compute the intersection point
        var x = p1.X + (direction.X * t);
        var y = p1.Y + (direction.Y * t);
        var z = p1.Z + (direction.Z * t);

        return new Point3D(x, y, z);
    }

    internal static bool PointIsWithinFace(Point3D point, Plane face)
    {
        // Check if the point is within the boundaries of the face
        return point.X >= face.MinX && point.X <= face.MaxX &&
               point.Y >= face.MinY && point.Y <= face.MaxY &&
               point.Z >= face.MinZ && point.Z <= face.MaxZ;
    }

    internal class Point3D
    {
        public Point3D(Vector3 pt)
        {
            X = pt.x;
            Y = pt.y;
            Z = pt.z;
        }


        public Point3D(int3 pt)
        {
            X = pt.x;
            Y = pt.y;
            Z = pt.z;
        }

        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public override String ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
        }
    }

    internal class Vector3D
    {
        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public double DotProduct(Vector3D other)
        {
            return (X * other.X) + (Y * other.Y) + (Z * other.Z);
        }
    }

    internal class Plane
    {
        public Plane(double minX, double maxX, double minY, double maxY, double minZ, double maxZ, Vector3D normal,
            double d)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
            Normal = normal;
            D = d;
        }

        public double MinX { get; }
        public double MaxX { get; }
        public double MinY { get; }
        public double MaxY { get; }
        public double MinZ { get; }
        public double MaxZ { get; }
        public Vector3D Normal { get; }
        public double D { get; }
    }

    internal class Cube
    {
        private readonly Point3D max;
        private readonly Point3D min;

        public Cube(Point3D minPoint, Point3D maxPoint)
        {
            min = minPoint;
            max = maxPoint;

            // Define the six faces of the cube
            FrontFace = new Plane(minPoint.X, maxPoint.X, minPoint.Y, maxPoint.Y, maxPoint.Z, maxPoint.Z,
                new Vector3D(0, 0, 1), maxPoint.Z);
            BackFace = new Plane(minPoint.X, maxPoint.X, minPoint.Y, maxPoint.Y, minPoint.Z, minPoint.Z,
                new Vector3D(0, 0, -1), -minPoint.Z);
            LeftFace = new Plane(minPoint.X, minPoint.X, minPoint.Y, maxPoint.Y, minPoint.Z, maxPoint.Z,
                new Vector3D(-1, 0, 0), -minPoint.X);
            RightFace = new Plane(maxPoint.X, maxPoint.X, minPoint.Y, maxPoint.Y, minPoint.Z, maxPoint.Z,
                new Vector3D(1, 0, 0), maxPoint.X);
            TopFace = new Plane(minPoint.X, maxPoint.X, maxPoint.Y, maxPoint.Y, minPoint.Z, maxPoint.Z,
                new Vector3D(0, 1, 0), maxPoint.Y);
            BottomFace = new Plane(minPoint.X, maxPoint.X, minPoint.Y, minPoint.Y, minPoint.Z, maxPoint.Z,
                new Vector3D(0, -1, 0), -minPoint.Y);
        }

        public Plane FrontFace { get; }
        public Plane BackFace { get; }
        public Plane LeftFace { get; }
        public Plane RightFace { get; }
        public Plane TopFace { get; }
        public Plane BottomFace { get; }

        public override String ToString()
        {
            return "(" + min + ":" + max + ")";
        }
    }
}
