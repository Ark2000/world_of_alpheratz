extends CanvasLayer

var line2d:Line2D
var line2d2:Line2D
var fps:Label

var W = 1152
var H = 628
var STEP = 1
var MAX_POINTS = 1000

var frames := []

var avg_60 := 60.0

var removep := 0
var addp := []

func _ready() -> void:
	W = get_viewport().get_visible_rect().size.x
	H = get_viewport().get_visible_rect().size.y
	
	line2d = Line2D.new()
	line2d.width = 2
	line2d.default_color = Color.RED
	line2d2 = Line2D.new()
	line2d2.width = 1
	line2d2.default_color = Color.BLUE
	fps = Label.new()
	
	add_child(line2d)
	add_child(line2d2)
	add_child(fps)

	line2d.clear_points()
	line2d2.clear_points()
	line2d2.add_point(Vector2.ZERO)
	line2d2.add_point(Vector2.ZERO)

var i = 0
func _process(delta: float) -> void:
	var f = 1.0 / delta
	frames.push_back(1.0 / delta)

	if frames.size() > 60:
		avg_60 = frames.slice(frames.size() - 60).reduce(sum, 0) / 60

	if frames.size() > (W / STEP) or frames.size() > MAX_POINTS:
		frames.pop_front()
		removep += 1

	addp.append(Vector2(i * STEP, f / 60.0 * H / 2.0))
	i += 1

func _physics_process(delta: float) -> void:
	line2d.scale.y = lerp(line2d.scale.y, 60.0 / avg_60, 0.05)
	for i in range(removep):
		line2d.remove_point(0)
		line2d.position.x -= STEP
	removep = 0
	for p in addp:
		line2d.add_point(p)
	addp.clear()

	line2d2.scale.y = lerp(line2d2.scale.y, 60.0 / avg_60, 0.01)
	line2d2.set_point_position(0, Vector2(0, avg_60 / 60.0 * H / 2.0))
	line2d2.set_point_position(1, Vector2(W, avg_60 / 60.0 * H / 2.0))

	fps.text = "%d FPS | MAX: %d | MIN: %d | AVG(60): %d" % [int(1.0 / delta), frames.max(), frames.min(), avg_60]
	fps.position.x = W - fps.size.x
	fps.position.y = H / 2.0

func sum(accum, number):
	return accum + number
