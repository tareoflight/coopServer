extends Node3D

#pk
var loc = Vector3i.ZERO
var chunckID = Vector2i.ZERO

var render_loc = Vector3.ZERO
var neighbors: Array = []

var contents: Array = [ ### this will change to a byte array of 255 long
####
## Bool magic magic stuff
##	Packed Array?
##  the array will contain 0-255 for a key and value
## the key will state the mat type
## the val will be the volume of the mat
## the key index will state the denstity of the mat
##
## IE
## 0:255 ##will be a node that contains a vacume
## 200:200,240:55 ## will be a node that contains 200 units of stone and 55 units of iron
#######
	
]

var statesvol = {
	"gas":1, #0-63
	"liquid":0, #64-127
	"sand":0, #128-191
	"solid":0 #192-255
}

func _ready() -> void:
	loc = Vector3i(roundi(global_position.x),
				   roundi(global_position.y),
				   roundi(global_position.z))
	name = loc

func step() -> void:
	# math the new render y for this node
	render_loc = Vector3(loc.x+0.0,loc.y+0.0,loc.z+0.0)
	# for content in contents:
	# 	statesvol[content.type] += content.vol

func pass_content(content) -> Array:
	var results: Array = []
	contents[content["ID"]] = content
	var volToRemove = content.vol
	var i  = 0
	var tempcontet = {}
	while volToRemove > 0:
		tempcontet = contents[i].duplicate()
		if volToRemove <= contents[i].vol:
			tempcontet.vol = volToRemove
			results.append(tempcontet)
			contents[i].vol -= volToRemove
			volToRemove = 0
		else:
			results.append(tempcontet)
			volToRemove -= contents[i].vol
			contents[i].vol = 0
			
		i += 1
	

	return results
######
# 10
# 


# func set_contents(key,value):
# 	contents[key]=value
# 	print("node[",global_position.x,",",global_position.y,",",global_position.z, "]: set ",key," to ",value)

# func _ready() -> void:
# 	loc = global_position
	
# 	plane = MeshInstance3D.new()
# 	plane.mesh = PlaneMesh.new()  # Or your own plane mesh
# 	plane.scale = Vector3(10, 0.1, 10)  # Set scale to make it a 1x1 plane
# 	plane.position = position  # Set it in the correct spot	
# 	material.albedo_color = Color.WHITE

# 	plane.material_override = material

# 	add_child(plane)

# func update_draw():
	
# 	var c = Color.BLACK
# 	var g = contents["ground"]
# 	if g == 1:
# 		c = Color.DARK_GRAY
# 	elif g <= 0.5 and g > 0:
# 		c = Color.BROWN
# 	elif g > 0.5:
# 		c = Color.GREEN
# 	else:
# 		c = Color.WHITE
	 
# 	if contents["air"] == 1:
# 		material.albedo_color = Color.TRANSPARENT
# 		#delete the plain
# 		remove_child(plane)
# 		pass

# 	if contents["air"] > 0:
# 		plane.position = Vector3(position.x,position.y-(contents["air"]*10),position.z)
# 	else:
# 		plane.position = Vector3(position.x,position.y,position.z)

# 	material.albedo_color = c

# 	print("updated:",plane.to_string()," to color:", c.to_html())

	
