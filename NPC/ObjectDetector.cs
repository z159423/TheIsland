using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField] protected List<Collider> triggeredObjects = new List<Collider>();
    [SerializeField] protected Collider detector;
    protected virtual void OnTriggerStay(Collider other)
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

    public virtual void ClearObjectList()
    {
        triggeredObjects.Clear();

        detector.enabled = true;
    }

    public List<Collider> GetTriggerdObjects()
    {
        return triggeredObjects;
    }
}
