using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Genome
{
    List<Node> nodeList = new List<Node>();
    List<Gene> geneList = new List<Gene>();
    public Genome()
    {

    }

    public void MakeRandom()
    {
        Node node1 = new Node(0, Node.TYPE_INPUT, 0);
        Node node2 = new Node(1, Node.TYPE_INPUT, 0);
        Node node3 = new Node(2, Node.TYPE_INPUT, 0);
        Node node4 = new Node(3, Node.TYPE_INPUT_BIAS, 0);
        Node node5 = new Node(4, Node.TYPE_OUTPUT, 0);
        nodeList.Add(node1);
        nodeList.Add(node2);
        nodeList.Add(node3);
        nodeList.Add(node4);
        nodeList.Add(node5);

        Mutate(); Mutate(); Mutate(); Mutate(); Mutate(); Mutate();  Mutate(); Mutate(); Mutate(); Mutate(); Mutate(); Mutate(); Mutate();
        print();
        feedforward();
    }

    public void Mutate()
    {
        /*int randomNodeIndex = Random.Range(-1, nodeList.Count);
        if(randomNodeIndex == -1)
        {
            Gene gene;
            Node node = new Node(nodeList.Count, Node.TYPE_HIDDEN, -1, 0);
            int connectToIndex = Random.Range(-1, nodeList.Count);
            float randomWeight = Random.Range(-10f,10f);
            bool enabled = Random.Range(0,2) == 0? true:false;
            

            if (connectToIndex != -1)
            {
                gene = new Gene(1, node.nodeID, connectToIndex, randomWeight, enabled);
                geneList.Add(gene);
                nodeList.Add(node);
            }
        }
        else
        {
            int randomType = Random.Range(0,2);

            if(randomType == 0)
            {
                
            }
            else
            {

            }
        }*/

        int randomNodeIndex = Random.Range(-1, nodeList.Count);
        if (randomNodeIndex == -1)
        {
            Gene gene;
            Node node = new Node(nodeList.Count, Node.TYPE_HIDDEN, 0);
            int inIndex = node.nodeID;
            int outIndex = Random.Range(0, nodeList.Count);
            float weight = Random.Range(-10f, 10f);
            bool enabled = Random.Range(0, 4) != 0 ? true : false;
            int inno = 1;

            nodeList.Add(node);

            if(!geneConnectionExists(inIndex, outIndex))
            {
                gene = new Gene(inno,inIndex,outIndex,weight,enabled);
                geneList.Add(gene);
            }
            
        }
        else
        {
            Gene gene;
            Node node = nodeList[randomNodeIndex];
            int inIndex = node.nodeID;

            int outMin = 0;
            if (node.type == Node.TYPE_INPUT || node.type == Node.TYPE_INPUT_BIAS)
                outMin = 4;

            int outIndex = Random.Range(outMin, nodeList.Count);
            float weight = Random.Range(-10f, 10f);
            bool enabled = Random.Range(0, 4) != 0 ? true : false;
            int inno = 1;

            
            if (!geneConnectionExists(inIndex, outIndex))
            {
                gene = new Gene(inno, inIndex, outIndex, weight, enabled);
                geneList.Add(gene);
            }
        }
    }

    public bool geneConnectionExists(int inIndex, int outIndex)
    {
        for(int i = 0; i < geneList.Count; i++)
        {
            if(geneList[i].inNodeID == inIndex && geneList[i].outNodeID == outIndex)
            {
                return true;
            }
        }

        return false;
    }

    public void feedforward()
    {
        Node[] tempNode = new Node[nodeList.Count];
        Gene[] tempGene = new Gene[geneList.Count];

        for(int i = 0; i < tempNode.Length; i++)
        {
            Node node = nodeList[i].Copy();
            
            tempNode[i] = node;
        }

        tempNode[0].nodeID = 1000;
        Debug.Log(nodeList[0].nodeID);
    }

    public void print()
    {
        for(int i = 0 ; i <nodeList.Count; i++)
        {
            Node node = nodeList[i];
            Debug.Log(node.nodeID+" "+node.type);
        }
        Debug.Log("-----------------------------------------------------");
        for (int i = 0; i < geneList.Count; i++)
        {
            Gene gene = geneList[i];
            Debug.Log(gene.inNodeID+" "+gene.outNodeID+" "+" "+gene.weight+" "+gene.enabled);
        }
    }
}
