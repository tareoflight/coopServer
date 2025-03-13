extends Node3D

var loc = Vector3.ZERO
var contents = {
	"air": 0.0,
	"ground": 1.0
}

func set_contents(key,value):
	contents[key]=value
	print("node[",global_position.x,",",global_position.y,",",global_position.z, "]: set ",key," to ",value)

func _init() -> void:
	loc = global_position
