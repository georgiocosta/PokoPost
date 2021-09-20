using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Sentry))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        Sentry sentry = (Sentry)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(sentry.transform.position, Vector3.up, Vector3.forward, 360, sentry.radius);
        Vector3 viewAngleA = sentry.directionFromAngle(-sentry.angle / 2, false);
        Vector3 viewAngleB = sentry.directionFromAngle(sentry.angle / 2, false);
        Handles.DrawLine(sentry.transform.position, sentry.transform.position + viewAngleA * sentry.radius);
        Handles.DrawLine(sentry.transform.position, sentry.transform.position + viewAngleB * sentry.radius);
    }

}
