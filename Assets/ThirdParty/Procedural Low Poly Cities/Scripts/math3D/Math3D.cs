using UnityEngine;
using System.Collections;

namespace SegmentMath {
    public class Math3D : MonoBehaviour {

            //Returns true if line segment made up of pointA1 and pointA2 is crossing line segment made up of
            //pointB1 and pointB2. The two lines are assumed to be in the same plane.
            public static bool AreLineSegmentsCrossing(out Vector3 intersection, Vector3 pointA1, Vector3 pointA2, Vector3 pointB1, Vector3 pointB2)
            {
                intersection = Vector3.zero;
                Vector3 closestPointA;
                Vector3 closestPointB;
                int sideA;
                int sideB;

                Vector3 lineVecA = pointA2 - pointA1;
                Vector3 lineVecB = pointB2 - pointB1;

                bool valid = ClosestPointsOnTwoLines(out closestPointA, out closestPointB, pointA1, lineVecA.normalized, pointB1, lineVecB.normalized);

                //lines are not parallel
                if (valid)
                {

                    sideA = PointOnWhichSideOfLineSegment(pointA1, pointA2, closestPointA);
                    sideB = PointOnWhichSideOfLineSegment(pointB1, pointB2, closestPointB);

                    if ((sideA == 0) && (sideB == 0))
                    {
                        intersection = closestPointA;
                        return true;
                    }

                    else
                    {

                        return false;
                    }
                }

                //lines are parallel
                else
                {

                    return false;
                }
            }

            public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point)
            {

                Vector3 lineVec = linePoint2 - linePoint1;
                Vector3 pointVec = point - linePoint1;

                float dot = Vector3.Dot(pointVec, lineVec);

                //point is on side of linePoint2, compared to linePoint1
                if (dot > 0)
                {

                    //point is on the line segment
                    if (pointVec.magnitude <= lineVec.magnitude)
                    {

                        return 0;
                    }

                    //point is not on the line segment and it is on the side of linePoint2
                    else
                    {

                        return 2;
                    }
                }

                //Point is not on side of linePoint2, compared to linePoint1.
                //Point is not on the line segment and it is on the side of linePoint1.
                else
                {

                    return 1;
                }
            }

            public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
            {

                closestPointLine1 = Vector3.zero;
                closestPointLine2 = Vector3.zero;

                float a = Vector3.Dot(lineVec1, lineVec1);
                float b = Vector3.Dot(lineVec1, lineVec2);
                float e = Vector3.Dot(lineVec2, lineVec2);

                float d = a * e - b * b;

                //lines are not parallel
                if (d != 0.0f)
                {

                    Vector3 r = linePoint1 - linePoint2;
                    float c = Vector3.Dot(lineVec1, r);
                    float f = Vector3.Dot(lineVec2, r);

                    float s = (b * f - c * e) / d;
                    float t = (a * f - c * b) / d;

                    closestPointLine1 = linePoint1 + lineVec1 * s;
                    closestPointLine2 = linePoint2 + lineVec2 * t;

                    return true;
                }

                else
                {
                    return false;
                }
            }
    }
}