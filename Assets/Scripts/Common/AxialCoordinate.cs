using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct AxialCoord{
	public int q;
	public int r;
	private static AxialCoord[] directions =  {		
		new AxialCoord(1,  0), new AxialCoord(1, -1), new AxialCoord( 0, -1),
		new AxialCoord(-1,  0), new AxialCoord(-1, 1),new AxialCoord( 0, 1)
	};
	public static AxialCoord origin = new AxialCoord (0, 0);

	public AxialCoord(int qCoord, int rCoord)
	{
		q = qCoord;
		r = rCoord;
	}
	public static int DistanceInHexes(Hex a, Hex b)
	{
		AxialCoord aPos = a.GetPosition();
		AxialCoord bPos = b.GetPosition();
		return DistanceInHexes (aPos, bPos);
	}
	public static int DistanceInHexes(AxialCoord a, AxialCoord b)
	{
		return (Mathf.Abs(a.q - b.q) 
		        + Mathf.Abs(a.q + a.r - b.q - b.r)
		        + Mathf.Abs(a.r - b.r)) / 2;
	}
	public static AxialCoord AxialFromWorld(Vector3 worldPosition)
	{
		float q = 0;
		float r = 0;
		q = worldPosition.x * 2.0f/3.0f / Hex.radius;
		r = -(worldPosition.x / 3.0f + Mathf.Sqrt(3)/3.0f * worldPosition.y) / Hex.radius;
		return Round (new AxialFloat (q, r));
	}
	public static AxialCoord AxialFromWorld(Vector2 worldPosition)
	{
		float q = 0;
		float r = 0;
		q = worldPosition.x * 2.0f/3.0f / Hex.radius;
		r = -(worldPosition.x / 3.0f + Mathf.Sqrt(3)/3.0f * worldPosition.y) / Hex.radius;
		return Round (new AxialFloat (q, r));
	}
	public static Vector3 WorldPositionFromAxial(AxialCoord position)
	{
		return  new Vector3 ((Hex.radius * (1.5f) * position.q)
		                     , -(Hex.radius * Mathf.Sqrt (3) * (position.r + position.q * 0.5f)));
	}
	public static Vector3 FlatWorldPositionFromAxial(AxialCoord position)
	{
		return  new Vector3 ((Hex.radius * (1.5f) * position.q)
		                     , -(Hex.radius * Mathf.Sqrt (3) * (position.r + position.q * 0.5f)));
	}
	public static Vector3 SunkenWorldPositionFromAxial(AxialCoord position)
	{
		return  new Vector3 ((Hex.radius * (1.5f) * position.q)
		                     , -(Hex.radius * Mathf.Sqrt (3) * (position.r + position.q * 0.5f)), 10.0f);
	}
	public static Vector3 PreviewWorldPositionFromAxial(AxialCoord position)
	{
		return  new Vector3 (-(Hex.radius * (1.5f) * position.q)
		                     , -(Hex.radius * Mathf.Sqrt (3) * (position.r + position.q * 0.5f)), -20.0f);
	}
	public static Vector3 FlatWorldPositionFromAxial(Hex position)
	{
		return  FlatWorldPositionFromAxial (position.GetPosition());
	}
	public static Vector2 TwoDWorldPosFromAxial(Hex h)
	{
		return TwoDWorldPosFromAxial (h.position.q, h.position.r);
		//return TwoDWorldPosFromAxial (h.position);
	}
	public static Vector2 TwoDWorldPosFromAxial(AxialCoord position)
	{
		return  new Vector2 ((Hex.radius * (1.5f) * position.q)
		                     , -(Hex.radius * Mathf.Sqrt (3) * (position.r + position.q * 0.5f)));
	}
	public static Vector2 TwoDWorldPosFromAxial(int q, int r)
	{
		return  new Vector2 ((Hex.radius * (1.5f) * q)
		                     , -(Hex.radius * Mathf.Sqrt (3) * (r + q * 0.5f)));
	}
	public static float AngleFromNorth(AxialCoord position)
	{
		Vector2 positionVector = TwoDWorldPosFromAxial (position);
		float angle = Vector2.Angle (Vector2.up, positionVector);
		if (angle > 180)
			angle = 360 - angle;
		return angle;
	}
	public static float AngleTolerance(AxialCoord a, AxialCoord b)
	{
		return 360 / (DistanceInHexes (a, b) * 6);
	}
	public static float AngleBetween(AxialCoord a, AxialCoord b, AxialCoord hinge)
	{
		Vector2 hA = TwoDWorldPosFromAxial (a - hinge);
		Vector2 hB = TwoDWorldPosFromAxial (b - hinge);
		return Mathf.Abs( Vector2.Angle (hA, hB));
	}
	public static float AngleBetween(Hex a, Hex b, Hex hinge)
	{
		Vector2 hA = a.transform.position - hinge.transform.position ;
		Vector2 hB= b.transform.position - hinge.transform.position ;
		return Mathf.Abs( Vector2.Angle (hA, hB));
	}
	public static float AngleBetween(Hex a, Hex b, Vector2 hinge)
	{
		Vector2 hA = (Vector2) a.transform.position - hinge;
		Vector2 hB= (Vector2)b.transform.position - hinge;
		return Mathf.Abs( Vector2.Angle (hA, hB));
	}
	public static float AngleBetween(Hex target, Vector2 skewerHole, Vector2 hinge)
	{
		Vector2 hA = (Vector2) target.transform.position - hinge;
		Vector2 hB= skewerHole - hinge;
		return Mathf.Abs( Vector2.Angle (hA, hB));
	}
	public static AxialCoord operator + (AxialCoord a, AxialCoord b)
	{
		return new AxialCoord (a.q + b.q, a.r + b.r);
	}
	public static AxialCoord operator - (AxialCoord a, AxialCoord b)
	{
		return new AxialCoord (a.q - b.q, a.r - b.r);
	}
	public static bool operator == (AxialCoord a, AxialCoord b)
	{
		if (System.Object.ReferenceEquals (a, null)) {
						if(System.Object.ReferenceEquals(b,null)){
							return true;
						}
			
						return false;
					}
					else if (System.Object.ReferenceEquals(b, null))
					         return false;
		return a.q == b.q && a.r == b.r;
	}
	public static bool operator != (AxialCoord a, AxialCoord b)
	{
		if (System.Object.ReferenceEquals (a, null)) {
			if(System.Object.ReferenceEquals(b,null)){
				return false;
			}
			
			return true;
		}
		else if (System.Object.ReferenceEquals(b, null))
			return true;
		return a.q != b.q || a.r != b.r;
	}
	public static AxialCoord hex_direction(int direction){
		return directions [direction];
	}
	
	public static AxialCoord hex_neighbor(AxialCoord hex, int direction){
		return Hex_Add (hex, hex_direction (direction));
	}
	public static AxialCoord Hex_Add(AxialCoord originHex, AxialCoord hexDirection)
	{
		return new AxialCoord (originHex.q + hexDirection.q, originHex.r + hexDirection.r);
	}
	public static AxialCoord Hex_Scale(AxialCoord hexDirection, int radius)
	{
		AxialCoord result = new AxialCoord(0,0);
		for (int i =0; i<radius; i++)
			result = Hex_Add (result, hexDirection);
		return result;
	}
	public override string ToString()
	{
		return q.ToString() + "," + r.ToString();
	}
	public bool Equals (AxialCoord obj)
	{
		return obj.q == this.q && obj.r == this.r;
	}

	public static AxialCoord cube_to_hex(CubeCoord h){
		int q = h.x;
		int r = h.z;;
		return new AxialCoord(q, r);
	}
	public static CubeCoord hex_to_cube(AxialCoord h){
		int x = h.q;
		int z = h.r;
		int y = -x - z;
		return new CubeCoord(x, y, z);
	}
	public static CubeCoord FloatHexToCube(AxialFloat h){
		return new CubeCoord (new Vector3 (h.q, -h.q - h.r, h.r));
	}
	public static AxialCoord Round(AxialFloat h)
	{
		return cube_to_hex (FloatHexToCube (h));
	}
	public AxialCoord FlipAboutOrigin()
	{
		return new AxialCoord (-q, q + r);
	}
	
}

public class CubeCoord{
	public int x;
	public int y;
	public int z;
	public CubeCoord()
	{
		x = y = z = 0;
	}
	public CubeCoord(int a, int b, int c)
	{
		x = a;
		y = b;
		z = c;
	}
	public CubeCoord(Vector3 h)
	{
		int rx = Mathf.RoundToInt (h.x);
		int ry = Mathf.RoundToInt (h.y);
		int rz = Mathf.RoundToInt (h.z);
		
		float x_diff = Mathf.Abs (rx - h.x);
		float y_diff = Mathf.Abs (ry - h.y);
		float z_diff = Mathf.Abs (rz - h.z);
		
		if (x_diff > y_diff && x_diff > z_diff) {
			rx = -ry - rz;
		} else if (y_diff > z_diff)
			ry = -rx - rz;
		else
			rz = -rx - ry;
		x = rx;
		y = ry;
		z = rz;					
	}
	public static CubeCoord operator +(CubeCoord a, CubeCoord b)
	{
		return new CubeCoord (a.x + b.x, a.y + b.y, a.z + b.z);
	}
	public static CubeCoord RotateCounter(CubeCoord a)
	{
		return new CubeCoord (-a.y, -a.z, -a.x);
	}
}
public struct AxialFloat
{
	public float q;
	public float r;
	public AxialFloat(float x, float y)
	{
		this.q = x;
		this.r = y;
	}
}
