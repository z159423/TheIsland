using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectDetector : ObjectDetector
{
    protected override void OnTriggerStay(Collider other)
    {

        if (other.TryGetComponent<WorldEnviromentObject>(out WorldEnviromentObject obj))
        {
            if (!triggeredObjects.Contains(other) && obj.CanBreak())
            {
                triggeredObjects.Add(other);
            }
        }

        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if (!triggeredObjects.Contains(other))
            {
                triggeredObjects.Add(other);
            }
        }

        if (triggeredObjects.Count > 0)
            detector.enabled = false;
    }
}
