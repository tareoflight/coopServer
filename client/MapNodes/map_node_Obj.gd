extends Node3D

var loc = Vector3.ZERO
var contents = {
	"air": 0.0,
	"ground": 1.0
}
var neighbors: Array = []  # Neighbors list
var plane: MeshInstance3D = null 
var plane_texture: Texture2D
var material = StandardMaterial3D.new() 

func set_contents(key,value):
	contents[key]=value
	print("node[",global_position.x,",",global_position.y,",",global_position.z, "]: set ",key," to ",value)

func _ready() -> void:
	loc = global_position
	
	plane = MeshInstance3D.new()
	plane.mesh = PlaneMesh.new()  # Or your own plane mesh
	plane.scale = Vector3(10, 0.1, 10)  # Set scale to make it a 1x1 plane
	plane.position = position  # Set it in the correct spot	
	material.albedo_color = Color.WHITE

	plane.material_override = material

	add_child(plane)

func update_draw():
	
	var c = Color.BLACK
	var g = contents["ground"]
	if g == 1:
		c = Color.DARK_GRAY
	elif g <= 0.5 and g > 0:
		c = Color.BROWN
	elif g > 0.5:
		c = Color.GREEN
	else:
		c = Color.WHITE
	 
	if contents["air"] == 1:
		material.albedo_color = Color.TRANSPARENT
		#delete the plain
		remove_child(plane)
		pass

	if contents["air"] > 0:
		plane.position = Vector3(position.x,position.y-(contents["air"]*10),position.z)
	else:
		plane.position = Vector3(position.x,position.y,position.z)

	material.albedo_color = c

	print("updated:",plane.to_string()," to color:", c.to_html())

	
