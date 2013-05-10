/*
 * Dean Reina
 * Prims Algorithm
 * May 2013
 * 
 * Prim's MST Algorithm on Adjacency Lists representation 
 * Uses an Adjacency Linked Lists, suitable for sparse graphs
 */

using System;
using System.IO;

// Heap code to be adapted for Prim's algorithm
// on adjacency lists graph
class Heap
{
    private int[] h;       // heap array
    private int[] hPos;    // hPos[h[k]] == k
    private int[] dist;    // dist[v] = priority of v

    private int N;         // heap size

    // The heap constructor gets passed from the Graph:
    //    1. maximum heap size
    //    2. reference to the dist[] array
    //    3. reference to the hPos[] array
    public Heap(int maxSize, int[] _dist, int[] _hPos)
    {
        N = 0;
        h = new int[maxSize + 1];
        dist = _dist;
        hPos = _hPos;
    }


    //simple bool
    public bool isEmpty()
    {
        return N == 0;
    }

    // SiftUp code from Richard
    public void siftUp(int k)
    {
        int v = h[k];

        h[0] = 0; //puts dummy node at top of the heap
        dist[0] = int.MinValue;

        while (dist[v] < dist[h[k / 2]])
        {
            h[k] = h[k / 2];
            hPos[h[k]] = k;
            k = k / 2;
        }
        h[k] = v;
        hPos[v] = k;
    }

    //sort the heap nodes into the correct positions
    public void siftDown(int k)
    {
        int v, j;
        v = h[k];
        j = 2 * k;
        while (j <= N)
        {
            //Ensures there is something in the Heap, then finds the larger branch
            if (j + 1 <= N && dist[h[j]] > dist[h[j + 1]])
            {
                j++;
            }

            //comparing the position with the node being moved
            if (dist[h[j]] >= dist[v])
            {
                break;
            }

            h[k] = h[j];
            k = j;
            j = k * 2;
        }

        h[k] = v;
        hPos[v] = k;
    }

    //Adding a node, and sifting up
    public void insert(int x)
    {
        h[++N] = x;
        siftUp(N);
    }

    public int remove()
    {
        int v = h[1];
        hPos[v] = 0; //v is no longer in heap
        h[N + 1] = 0; //put null node into empty spot

        h[1] = h[N--];
        siftDown(1);

        return v;
    }
}  // end of Heap class


// Graph code to support Prim's MST Alg
class Graph
{
    // Same as in GraphLists.cs which had DF traversal
    // If you did that, just copy and paste attributes code      

    //used for traversing the graph
    private int V, E;
    private Node[] adj;

    public class Node
    {
        public int data;
        public Node next;
        public int vert;
        public int wgt;
    }

    private Node z;

    // default constructor
    public Graph(string graphFile)
    {
        int u, v;
        int e, wgt;
        Node t;

        StreamReader reader = new StreamReader(graphFile);

        char[] splits = new char[] { ' ', ',', '\t' };
        string line = reader.ReadLine();
        string[] parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);

        //find out the number of vertices and edges
        V = int.Parse(parts[0]);
        E = int.Parse(parts[1]);

        //create the sentinel node
        z = new Node();
        z.next = z;

        //Create adjacency lists, initialised to sentinel node z
        //Dynamically allocate array
        adj = new Node[V + 1];

        for (v = 1; v <= V; ++v)
        {
            adj[v] = z;
        }

        Console.WriteLine("Reading edges from text file");

        for (e = 1; e <= E; ++e)
        {
            line = reader.ReadLine();
            parts = line.Split(splits, StringSplitOptions.RemoveEmptyEntries);

            u = int.Parse(parts[0]);
            v = int.Parse(parts[1]);
            wgt = int.Parse(parts[2]);

            Console.WriteLine("Edge {0}--({1})--{2}", toChar(u), wgt, toChar(v));

            //code to put edge into adjacency lists
            t = new Node();
            t.data = wgt;
            t.vert = u;
            t.next = adj[v];

            adj[v] = t;

            t = new Node();
            t.data = wgt;
            t.vert = v;
            t.next = adj[u];

            adj[u] = t;
        }
    }

    private char toChar(int u)
    {

        return (char)(u + 64);
    }

    public void display()
    {
        int v;
        Node n;

        for (v = 1; v <= V; ++v)
        {
            Console.Write("\nAdjacency[ {0} ] -> ", toChar(v));

            for (n = adj[v]; n != z; n = n.next)
            {
                Console.Write(" |{0} | {1} | ->", toChar(n.vert), n.wgt);
            }

            Console.WriteLine("  ");
        }
    }

    // Prim's algorithm to compute MST
    int[] MST_Prim(int s)
    {
        int v;
        int wgt_sum = 0;
        int[] dist, parent, hPos;
        Node t;

        //the distance from node to node
        dist = new int[V + 1];

        //the parent node
        parent = new int[V + 1];

        //current heap position
        hPos = new int[V + 1];


        // initialising parent and position to zero, and dist to the max value
        for (v = 1; v <= V; v++)
        {
            dist[v] = int.MaxValue;
            parent[v] = 0;
            hPos[v] = 0;
        }

        Heap heap = new Heap(V + 1, dist, hPos);
        heap.insert(s);

        dist[s] = 0;


        while (!heap.isEmpty())
        {

            v = heap.remove();


            Console.Write("\nAdding edge {0}--({1})--{2}", toChar(parent[v]), dist[v], toChar(v));


            //calculates the sum of the weights
            wgt_sum += dist[v];

            //prevents duplicates
            dist[v] = -dist[v];

            for (t = adj[v]; t != z; t = t.next)
            {

                if (t.data < dist[t.vert])
                {
                    dist[t.vert] = t.data;
                    parent[t.vert] = v;

                    //If the vertex is empty, insert next vertex
                    if (hPos[t.vert] == 0)
                    {
                        heap.insert(t.vert);
                    }
                    else //Else call sift up
                    {
                        heap.siftUp(hPos[t.vert]);
                    }
                }
            }
        }






        Console.Write("\n\nWeight  = {0}\n", wgt_sum);
        return parent;


    }



    //Printing 
    public void showMST(int[] mst)
    {
        Console.Write("\n\nMinimum Spanning tree parent array is:\n");
        for (int v = 1; v <= V; ++v)
            Console.Write("{0} -> {1}\n", toChar(v), toChar(mst[v]));
        Console.WriteLine("");
    }

    public static void Main()
    {
        int s = 4;
        int[] mst;
        string fname = "wGraph3.txt";

        Graph g = new Graph(fname);

        g.display();

        mst = g.MST_Prim(s);

        g.showMST(mst);

        Console.ReadLine();
    }
} // end of Graph class