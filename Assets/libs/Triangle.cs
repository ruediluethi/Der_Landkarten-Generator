using UnityEngine;
using System;
using System.Collections.Generic;
//using System.Text;

public struct Triangle
{
	public int p1;
	public int p2;
	public int p3;
	
	public Vector3 v3D1;
	public Vector3 v3D2;
	public Vector3 v3D3;
	
	public Vector2 circumCirclesCenter;
	
	public List<Edge> edges;
    public List<int> points;
	
	public Triangle[] neighbours;
	public int neighbourCounter;
	
	public Triangle(int point1, int point2, int point3) {
		p1 = point1; p2 = point2; p3 = point3;

        points = new List<int>();
        points.Add(p1);
        points.Add(p2);
        points.Add(p3);

		this.v3D1 = Vector3.zero;
		this.v3D2 = Vector3.zero;
		this.v3D3 = Vector3.zero;
		
		edges = new List<Edge>();
		edges.Add (new Edge(p1, p2));
		edges.Add (new Edge(p2, p3));
		edges.Add (new Edge(p3, p1));
		
		neighbours = new Triangle[4];
		neighbourCounter = 0;
		
		circumCirclesCenter = new Vector2(0F, 0F);
	}
	
	public void addNeighbour(Triangle neighbour){
		
		neighbours[neighbourCounter] = neighbour;
		neighbourCounter++;
		
	}
	
	public Boolean checkIfNeighbour(Triangle possibleNeighbour){

		if (edges != null){
			
			if (!(this.p1 == possibleNeighbour.p1 && this.p2 == possibleNeighbour.p2 && this.p3 == possibleNeighbour.p3)){ // don't himself as neighbour
			
				foreach(Edge edgeA in edges){
					foreach(Edge edgeB in possibleNeighbour.edges){
						
						if (edgeA.p1 == edgeB.p1 && edgeA.p2 == edgeB.p2){
							return true;	
						}
						
						if (edgeA.p1 == edgeB.p2 && edgeA.p2 == edgeB.p1){
							return true;	
						}
						
					}
				}
			}
		}
		
		return false;
		
	}
	
	public void calcCircumCircles (Vector2 v1, Vector2 v2, Vector2 v3) {
		
		Vector2 v1v2Center = (v1 + v2)/2F;
		Vector2 v1v3Center = (v1 + v3)/2F;
		
		Vector2 v1v2Delta = v1 - v2;
		Vector2 v1v3Delta = v1 - v3;
		
		Vector2 v1v2Bisector = new Vector2(v1v2Delta.y, -v1v2Delta.x);
		Vector2 v1v3Bisector = new Vector2(v1v3Delta.y, -v1v3Delta.x);
		
		v1v2Bisector.Normalize();
		v1v3Bisector.Normalize();
		
		v1v2Bisector = v1v2Bisector*100F;
		v1v3Bisector = v1v3Bisector*100F;
		
		/*
		Debug.DrawLine(
			new Vector3(v1v2Center.x - v1v2Bisector.x, 0.0F, v1v2Center.y - v1v2Bisector.y),
			new Vector3(v1v2Center.x + v1v2Bisector.x, 0.0F, v1v2Center.y + v1v2Bisector.y),
			Color.blue
		);
		
		Debug.DrawLine(
			new Vector3(v1v3Center.x - v1v3Bisector.x, 0.0F, v1v3Center.y - v1v3Bisector.y),
			new Vector3(v1v3Center.x + v1v3Bisector.x, 0.0F, v1v3Center.y + v1v3Bisector.y),
			Color.blue
		);
		*/
		
		circumCirclesCenter = lineIntersectionPoint(
			v1v2Center - v1v2Bisector,
			v1v2Center + v1v2Bisector,
			v1v3Center - v1v3Bisector,
			v1v3Center + v1v3Bisector
		);
		
		this.v3D1 = new Vector3(v1.x, 0.0F, v1.y);
		this.v3D2 = new Vector3(v2.x, 0.0F, v2.y);
		this.v3D3 = new Vector3(v3.x, 0.0F, v3.y);
		
		
	}
	
	private Vector2 lineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
	{
	  // Get A,B,C of first line - points : ps1 to pe1
	  float A1 = pe1.y-ps1.y;
	  float B1 = ps1.x-pe1.x;
	  float C1 = A1*ps1.x+B1*ps1.y;
	 
	  // Get A,B,C of second line - points : ps2 to pe2
	  float A2 = pe2.y-ps2.y;
	  float B2 = ps2.x-pe2.x;
	  float C2 = A2*ps2.x+B2*ps2.y;
	 
	  // Get delta and check if the lines are parallel
	  float delta = A1*B2 - A2*B1;
	  if(delta == 0)
	     throw new System.Exception("Lines are parallel");
	 
	  // now return the Vector2 intersection point
	  return new Vector2(
	      (B2*C1 - B1*C2)/delta,
	      (A1*C2 - A2*C1)/delta
	  );
	}
	
}
