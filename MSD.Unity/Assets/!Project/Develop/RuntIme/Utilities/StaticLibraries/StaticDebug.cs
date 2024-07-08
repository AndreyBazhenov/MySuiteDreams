using UnityEngine;


public static class StaticDebug
{
    public static void DebugParallelipiped(Vector3[] vertices)
    {
        float time = 30f;

        for (int i = 0; i < 4; i++)
        {
            
            //De.DebugWireSphere(vertices[i], Color.green, 0.5f, time);
        }
        for (int i = 4; i < 8; i++)
        {
            //DebugExtension.DebugWireSphere(vertices[i], Color.red, 0.5f, time);
        }
        
        
        Debug.DrawLine(vertices[0],vertices[1] , Color.magenta,time);
        Debug.DrawLine(vertices[1],vertices[2] , Color.magenta,time);
        Debug.DrawLine(vertices[2],vertices[3] , Color.magenta,time);
        Debug.DrawLine(vertices[3],vertices[0] , Color.magenta,time);
        
        
        Debug.DrawLine(vertices[4],vertices[5] , Color.magenta,time);
        Debug.DrawLine(vertices[5],vertices[6] , Color.magenta,time);
        Debug.DrawLine(vertices[6],vertices[7] , Color.magenta,time);
        Debug.DrawLine(vertices[7],vertices[4] , Color.magenta,time);
        
        
        Debug.DrawLine(vertices[0],vertices[4] , Color.magenta,time);
        Debug.DrawLine(vertices[1],vertices[5] , Color.magenta,time);
        Debug.DrawLine(vertices[2],vertices[6] , Color.magenta,time);
        Debug.DrawLine(vertices[3],vertices[7] , Color.magenta,time);
    }
}
