
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*public class Tree
{
    //public Creature_V2 parent;
    //public List<Tree> children = new List<Tree>();

    public int ID;
    public string name;
    public List<Tree> children = new List<Tree>();
}*/

public class AncestryTreeMaker : MonoBehaviour
{
    public GameObject namePrefab;
    public GameObject linePrefab;

    private Creature_V2 creature;
    private List<GameObject> lines;
    private List<GameObject> names;

    void Start()
    {
        creature = null;
        lines = new List<GameObject>();
        names = new List<GameObject>();
    }

    public void MakeTree(List<Creature_V2> creatures)
    {

        this.creature = creatures.OrderBy(o => o.GetID()).ToList()[0]; //order the list in acending order and get the creature at index 0
        Debug.LogError(CreatureTraverseRecursive(this.creature));

        GameObject name = Instantiate(namePrefab) as GameObject;
        names.Add(name);
        TextMesh textMesh = name.GetComponent<TextMesh>();
        textMesh.text = this.creature.GetName() + "::" + creature.GetChildCount()+"::"+creature.radius + "::" + creature.GetEnergy(); 
        textMesh.transform.position = new Vector3(0, globalY, 0);
        globalY -= 0.25f;
        textMesh.color = colors[0];
        CreatureTraverseParentTree(this.creature, 0.5f,1);

        this.creature.SetIsNode(true);

    }

    /*public void CreatureTraverseLine(Creature_V2 parent, float x, float y,float factor)
    {
        List<Creature_V2> children = parent.GetChildren();
        if (children.Count >= 0)
        {

            for (int i = 0; i < children.Count(); i++)
            {
                GameObject line = Instantiate(linePrefab) as GameObject;
                lines.Add(line);
                LineRenderer lineRen = line.GetComponent<LineRenderer>();
                lineRen.SetPosition(0,new Vector3(x,y,0));
                //int newX = (x - (factor/2)) + (i* factor);
                //float newX = (x+(-children.Count/2))+i;
                float newX = (x + ((-children.Count() / 2) * factor)) + ((float)i * factor);
                float newY = y - 1;
                lineRen.SetPosition(1, new Vector3(newX,newY,0));
                float factorDec = 2f;
                if (children.Count > 4f)
                {
                    factorDec *= (children.Count / 2f);
                }
                CreatureTraverseLine(children[i],newX,newY, factor/factorDec);
            }
        }
    }*/


    float globalY = -0.5f;
    Color[] colors = new Color[] { new Color(0.75f,1f,0f), Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow};
    public void CreatureTraverseParentTree(Creature_V2 parent, float x, int colorIndex)
    {
        List<Creature_V2> children = parent.GetChildren();


        if (children.Count > 0)
        {
            for (int i = 0; i < children.Count; i++)
            {
                GameObject name = Instantiate(namePrefab) as GameObject;
                names.Add(name);
                TextMesh textMesh = name.GetComponent<TextMesh>();
                if (children[i].IsAlive())
                    textMesh.text = children[i].GetName() + "::" + children[i].GetChildCount() + "::" + children[i].radius + "::" + children[i].GetEnergy();
                else
                    textMesh.text = children[i].GetName() + "::" + children[i].GetChildCount() + "::" + children[i].radius + "::" + children[i].GetEnergy() + "  X_X";
                textMesh.transform.position = new Vector3(x, globalY,0);
                globalY-=0.25f;

                if (colorIndex == colors.Length)
                    colorIndex = 0;
                textMesh.color = colors[colorIndex];

                CreatureTraverseParentTree(children[i],x+0.5f, colorIndex+1);
            }
        }
    }

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

        globalY = -0.5f;

        for (int i = 0; i < this.lines.Count; i++)
        {
            Destroy(lines[i]);
        }
        lines.Clear();
        lines = new List<GameObject>();

        for (int i = 0; i < this.names.Count; i++)
        {
            Destroy(names[i]);
        }
        names.Clear();
        names = new List<GameObject>();

        if(this.creature!=null)
            this.creature.SetIsNode(false);
        this.creature = null;

    }

    public Creature_V2 GetSelectedCreature()
    {
        return creature;
    }

}
