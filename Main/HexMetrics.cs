using Godot;
using System;
public static class HexMetrics {

	public const float outerRadius = 140f;

	public const float innerRadius = outerRadius * 0.86602540378f;

	public const float baaseOffsetY = 40f;

	//public const float squashAmount = 0.8f;

	//top radius = 86.4
	//side radius  = 140
	//inner  = 60.62
	/*

	637 *205
	427 * 291.4

	637 * x = 427
	x = 0.67032967033

	427 * y = 291.4
	y = 0.68243559719

	
	
	
	121.243556529
	60.62
	*/

	//public const float squashPoint = squashAmount * outerRadius;

	/// <summary>
	/// 0
	/// 70
	/// 54
	/// 35
	/// 54
	/// -35
	/// 0
	/// -70
	/// -54
	/// -35
	/// -54
	/// 35
	/// 
	/// </summary>
	public const float depth = 25f;

    public static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
	};




	public static Vector2[] corners2 = {
		new Vector2(-outerRadius,0f),
		new Vector2((-0.5f * outerRadius),-innerRadius ), //front right edge
		new Vector2((0.5f * outerRadius),-innerRadius),
		new Vector2(outerRadius,0f),
		new Vector2((0.5f * outerRadius), innerRadius),
		new Vector2((-0.5f * outerRadius),innerRadius )
	};


	public static Vector2[] values = {
		new Vector2(-140f,0f),
		new Vector2(-70,-121.243556529f), //front right edge
		new Vector2(70, -121.243556529f),
		new Vector2(140f, 0f),
		new Vector2(70, 121.243556529f),
		new Vector2(-70,121.243556529f )
	};






	// \public const float outerRadius = 70f;

	// public const float innerRadius = outerRadius * 0.86602540378f; 60.62
	public static Vector2[] corners3 = {
		new Vector2(outerRadius, 0f),
		new Vector2((0.5f * outerRadius), innerRadius *0.7f), //front right edge
		new Vector2((-0.5f * outerRadius),innerRadius*0.7f),
		new Vector2(-outerRadius,0f ),
		new Vector2((-0.5f * outerRadius),-innerRadius*0.7f),
		new Vector2(0.5f * outerRadius,-innerRadius*0.7f)
	};
	
}