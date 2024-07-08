using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticMath
{
    public static bool IsClickedObject(Camera camera, Transform reference)
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = camera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.Equals(reference))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsPointInsideEightCornerMesh(Vector3[] vertices, Vector3 point)
    {
        Plane[] planes = new Plane[]
        {
            new Plane(vertices[0], vertices[1], vertices[2]),
            new Plane(vertices[2], vertices[1], vertices[5]),
            new Plane(vertices[0], vertices[4], vertices[3]),
            new Plane(vertices[4], vertices[5], vertices[6]),
            new Plane(vertices[3], vertices[2], vertices[6]),
            new Plane(vertices[0], vertices[1], vertices[5])
        };

        foreach (var plane in planes)
        {
            if (!IsPointOnCenterSide(plane, point, vertices))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsPointOnCenterSide(Plane plane, Vector3 point, Vector3[] vertices)
    {
        Vector3 center = FindCenterMesh(vertices);

        return plane.GetSide(center) == plane.GetSide(point);
    }

    public static Vector3 FindCenterMesh(Vector3[] vertices)
    {
        Vector3 center = Vector3.zero;
        foreach (var vertex in vertices)
        {
            center += vertex;
        }
        center /= vertices.Length;
        return center;
    }
    
    public static Vector2[] CreateCorners(Vector2 startPos, Vector2 endPose, int minSize)
    {
        var bottomLeft = Vector2.Min(startPos, endPose);
        var topRight = Vector2.Max(startPos, endPose);;

        Vector2[] corners = new Vector2[]
        {
            new Vector2(bottomLeft.x, topRight.y), // Top-left
            new Vector2(topRight.x, topRight.y), // Top-right
            new Vector2(topRight.x, bottomLeft.y), // Bottom-right
            new Vector2(bottomLeft.x, bottomLeft.y) // Bottom-left
        };

        float width = (corners[0] - corners[1]).magnitude;
        float height = (corners[0] - corners[2]).magnitude;

        if (width < minSize)
        {
            float diff = minSize - width;

            corners[0].x -= diff * 0.5f;
            corners[1].x += diff * 0.5f;
            corners[2].x -= diff * 0.5f;
            corners[3].x += diff * 0.5f;
        }
        
        if (height < minSize)
        {
            float diff = minSize - height;

            corners[0].y += diff * 0.5f;
            corners[1].y += diff * 0.5f;
            corners[2].y -= diff * 0.5f;
            corners[3].y -= diff * 0.5f;
        }
        return corners;
    }

    public static Vector3[] GetSelectionMesh(Camera camera,Vector2 initialMousePosition, Vector2 currentMousePosition, 
        int minSelectionSize, float raycastMaxDistance, LayerMask raycastLayerMask)
    {
        Vector3[] vertices = new Vector3[8];
        Vector2[] corners = CreateCorners(initialMousePosition, currentMousePosition, minSelectionSize);

        Ray ray = new Ray();
        RaycastHit hit;
        
        int index = 0;
        for (int i = 0; i < corners.Length; ++i)
        {
            ray = camera.ScreenPointToRay(corners[i]);

            bool raycast = Physics.Raycast(ray, out hit, raycastMaxDistance,raycastLayerMask);

            vertices[index] = ray.origin;
            vertices[index + 4] = raycast ? hit.point : ray.GetPoint(raycastMaxDistance);;

            index++;
        }

        return vertices;
    }
    
    public static Vector3 CalculatePositionOnSemiSphere(float phi, float theta, float radius)
    {
        float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = radius * Mathf.Cos(phi);
        float z = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
        return new Vector3(x, y, z);
    }
}
