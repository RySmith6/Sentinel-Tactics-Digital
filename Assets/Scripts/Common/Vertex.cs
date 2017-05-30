using UnityEngine;
using System.Collections;

public class Vertex{
	public Vector3 location;
	public Hex[] neighbors;
	
	public Vertex(Vector3 gameLocation)
	{
		this.location = gameLocation;
		neighbors = new Hex[3];
	}
	public override string ToString()
	{
		return location.ToString ();
	}
	public int AddNewNeighbor(Hex h)
	{
		for (int i = 0; i<neighbors.Length; i++) {
			if(neighbors[i] ==h)
			{
				return i;
			}
			if(neighbors[i] ==null)
			{
				neighbors[i] = h;
				return i;
			}
		}
		return 0;
	}
	public bool AdjacentTo(Hex h)
	{
		for (int i = 0; i<neighbors.Length; i++) {
			if (neighbors [i] == h) {
				return true;
			}
		}
		return false;
	}
	
}

