extends Node3D
@onready var label: Label3D = $speechbox

var textlist = [
	"...",
	"I'm a Cool Rock",
	"no I dont talk, that would be silly",
	"still just a Cool Rock"
]
var current_text = 0
var hp = 100
	
func on_hit(dmg) -> bool:
	if hp < 0:
		label.text = "I'm dead"
		return false
	else:
		hp -= dmg
		if current_text < textlist.size():
			label.text = textlist[current_text]
			current_text += 1
		else:
			label.text = textlist[0]
		return true

func _process(delta):
	var camera = get_viewport().get_camera_3d()
	if camera:
		label.look_at(camera.global_transform.origin, Vector3.UP)
		label.rotate_y(deg_to_rad(180))
		
