
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct TreeData
{
    public string name;
    public Color color;
}

public class AncestryTreeMaker : MonoBehaviour
{
    private Creature_V2 creature;
    private Color[] colors = new Color[] { new Color(0.75f, 1f, 0f), Color.cyan, Color.green, Color.magenta, Color.red, Color.white, Color.yellow };

    private List<TreeData> treeDataList;

    void Start()
    {
        creature = null;
    }

    public void MakeTree(List<Creature_V2> creatures)
    {

        this.creature = creatures.OrderBy(o => o.GetID()).ToList()[0]; //order the list in acending order and get the creature at index 0

        //Debug.LogError(CreatureTraverseRecursive(this.creature));

        TreeData treeData = new TreeData();
        treeData.color = colors[0];
        treeData.name = this.creature.GetName();
        treeDataList.Add(treeData);

        CreatureTraverseParentTree(this.creature, 1,"  ");

        this.creature.SetIsNode(true);

    }
   
    public void CreatureTraverseParentTree(Creature_V2 parent, int colorIndex, string indent)
    {
        List<Creature_V2> children = parent.GetChildren();


        if (children.Count > 0)
        {
            for (int i = 0; i < children.Count; i++)
            {
                TreeData treeData = new TreeData();

                if (children[i].IsAlive())
                    treeData.name = indent + children[i].GetName();
                else
                    treeData.name = indent + children[i].GetName() + "          DEAD";

                if (colorIndex == colors.Length)
                    colorIndex = 0;

                treeData.color = colors[colorIndex];
                
                treeDataList.Add(treeData);

                CreatureTraverseParentTree(children[i], colorIndex+1, indent+"  ");
            }
        }
    }

    public List<TreeData> GetTreeDataList()
    {
        return treeDataList;
    }

    //DEBUG ONLY
    public string CreatureTraverseRecursive(Creature_V2 parent)
    {
        string add = "";
        List<Creature_V2> children = parent.GetChildren();

        if (children.Count == 0)
        {
            add = parent.GetID() + "::" + parent.GetName() + "__";
            return add;
        }

        for (int i = 0; i < children.Count; i++)
        {
            add += CreatureTraverseRecursive(children[i]);
        }

        return parent.GetID() + "::" + parent.GetName() + "==>" + add;
    }

    public void ResetAllNodes()
    {
        if(this.creature!=null)
            this.creature.SetIsNode(false);

        this.creature = null;
        this.treeDataList = new List<TreeData>();

    }

    public Creature_V2 GetSelectedCreature()
    {
        return creature;
    }

}
