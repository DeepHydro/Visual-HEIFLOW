using System;

namespace Heiflow.Visualization.Geometry.Triangulation
{
	/// <summary>
	///To define the common types used in 
	///Analytical Geometry calculations.
	/// </summary>
	
	//To define some constant Values 
	//used for local judgment 
	public struct ConstantValue
	{
		internal const  double SmallValue=0.00001;
		internal const double BigValue=99999;
	}
	
	public enum VertexType
	{
		ErrorPoint,
		ConvexPoint,
		ConcavePoint		
	}

	public enum PolygonType
	{
		Unknown,
		Convex, 
		Concave	
	}

	public enum PolygonDirection
	{
		Unknown,
		Clockwise,
		Count_Clockwise
	}
}
