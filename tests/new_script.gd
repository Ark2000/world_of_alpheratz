@tool
extends EditorScript

func _run() -> void:
	print("hello")
	var a = Vector2.ZERO
	a.x = 10
	a["x"] = 20
	print(a)
	var b = Rect2()
	b["position"] = Vector2(10,10)
	print(b)
