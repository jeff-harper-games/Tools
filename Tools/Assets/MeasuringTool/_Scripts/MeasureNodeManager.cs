/*
 *  Author: Jeff Harper @jeffdevsitall
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureNodeManager : MonoBehaviour
{
    public List<MeasureNode> nodes;
    public MeasureNode nodePrefab;
    public LineRenderer lr;

    public void TestingCreateNode()
    {
        Vector2 mousePos = new Vector2();

        mousePos.x = Input.mousePosition.x;
        mousePos.y = Input.mousePosition.y;

        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
        CreateNode(position);
    }

    public void CreateNode(Vector3 position)
    {
        MeasureNode node = Instantiate(nodePrefab);
        node.Create(position, this);
        nodes.Add(node);

        lr.positionCount = nodes.Count;

        lr.SetPosition(nodes.Count - 1, nodes[nodes.Count - 1].GetPosition());

        if (nodes.Count < 2)
            return;

        float distance = Vector3.Distance(node.GetPosition(), nodes[nodes.Count - 2].GetPosition());
        Debug.Log(distance);

    }

    public void RemoveNode(MeasureNode node)
    {
        nodes.Remove(node);
    }
}
