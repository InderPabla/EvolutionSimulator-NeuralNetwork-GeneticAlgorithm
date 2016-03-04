using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Node  {

    public static int TYPE_INPUT = 1;
    public static int TYPE_INPUT_BIAS = 3;
    public static int TYPE_HIDDEN = 0;
    public static int TYPE_OUTPUT = 2;

    public int nodeID = 0;
    public int type = 0; // 1 input, 3 = bian input, 0 = hidden node
    public float nodeValue = 0;
    

	public Node(int nodeID, int type, float nodeValue)
    {
        this.type = type;
        this.nodeValue = nodeValue;
        this.nodeID = nodeID;
    }

    public Node Copy()
    {
        return new Node(nodeID,type,nodeValue);
    }
}
