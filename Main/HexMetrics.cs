using Godot;
using System;
public static class HexMetrics {

	public const float outerRadius = 70f;

	public const float innerRadius = outerRadius * 0.86602540378f;

	public const float squashAmount = 0.8f;

	//inner  = 60.62

	public const float squashPoint = squashAmount * outerRadius;

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
		new Vector2(0f, squashPoint),
		new Vector2(innerRadius, (0.5f * outerRadius) *squashAmount), //front right edge
		new Vector2(innerRadius, (-0.5f * outerRadius)*squashAmount),
		new Vector2(0f, -squashPoint),
		new Vector2(-innerRadius, (-0.5f * outerRadius)*squashAmount),
		new Vector2(-innerRadius, 0.5f * outerRadius *squashAmount)
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
	public static Vector2[] cornersbase = {
		new Vector2(innerRadius, 0.5f * outerRadius),
		new Vector2(innerRadius, depth + (0.5f * outerRadius)),
		new Vector2(0f, depth + squashPoint ),
		new Vector2(-innerRadius, depth + (0.5f * outerRadius)),
		new Vector2(-innerRadius, 0.5f * outerRadius),
		new Vector2(0f, squashPoint),
	};
}