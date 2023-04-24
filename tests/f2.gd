extends CanvasLayer

var duration := 4.0

var frame_times:Array[float] = [0.016]
var frame_times_sum := 0.016

var line2d := Line2D.new()
var label = Label.new()

func get_avg_fps() -> float:
	return frame_times.size() / frame_times_sum

func _ready() -> void:
	line2d.width = 2.0
	line2d.default_color = Color(1.0, 0.0, 0.0, 0.8)
	add_child(line2d)
	
func _physics_process(_delta: float) -> void:
	var w = get_viewport().get_visible_rect().size.x
	var h = get_viewport().get_visible_rect().size.y
	var step_x = w / frame_times.size()
	var avg_fps = get_avg_fps()
	line2d.clear_points()
	for i in range(frame_times.size()):
		line2d.add_point(Vector2(i * step_x, h * (1.0 - 1.0 / frame_times[i] / avg_fps / 2)))

func _process(delta:float):
	frame_times.append(delta)
	frame_times_sum += delta

	while frame_times_sum > duration:
		frame_times_sum -= frame_times.pop_front()
