/*
 *  Author: Jeff Harper @jeffdevsitall
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureNode : MonoBehaviour
{
    private MeasureNodeManager manager;

    public void Create(Vector3 _position, MeasureNodeManager _manager)
    {
        manager = _manager;
        transform.position = _position;
    }

    public void Delete()
    {
        manager.RemoveNode(this);
        Destroy(gameObject);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
