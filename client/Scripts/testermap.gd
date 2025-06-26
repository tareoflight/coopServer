extends Node3D

# Paths to your materials (change these to your actual files)
const MAT_WALL := preload("res://assets/textures/dev/grids/Dark/texture_07_material.tres")
const MAT_FLOOR := preload("res://assets/textures/fpass/path/forestGrass.tres")
const MAT_PLATFORM := preload("res://assets/textures/fpass/path/slate.tres")
const MAT_RAMP := preload("res://assets/textures/dev/grids/Dark/texture_07_material.tres")

func _ready():
	var stage = $stage if has_node("stage") else self

	# Base arena floor
	_add_box(stage, Vector3(80, 1, 80), Vector3(0, -0.5, 0), MAT_FLOOR)

	# Perimeter walls
	_create_walls(stage, 80, 10)

	# Central platform with stairs
	_add_box(stage, Vector3(10, 2, 10), Vector3(0, 1, 0), MAT_PLATFORM)
	_add_stairs(stage, Vector3(0, 1, -7), 5)

	# Side ledges + ramps
	_add_box(stage, Vector3(20, 1, 5), Vector3(-25, 2, 25), MAT_PLATFORM)
	_add_box(stage, Vector3(20, 1, 5), Vector3(25, 2, -25), MAT_PLATFORM)
	_add_ramp(stage, Vector3(-25, 0.5, 20), 20, -20, MAT_RAMP)
	_add_ramp(stage, Vector3(25, 0.5, -20), 20, 20, MAT_RAMP)

	# Corner towers/pillars
	for x in [-35, 35]:
		for z in [-35, 35]:
			_add_cylinder(stage, Vector3(x, 3, z), 6, MAT_RAMP)

	# Optional jump gap
	_add_box(stage, Vector3(10, 1, 4), Vector3(0, 2, 30), MAT_PLATFORM)
	_add_box(stage, Vector3(10, 1, 4), Vector3(0, 2, 40), MAT_PLATFORM)
	
	
# Floor/wall/platform
func _add_box(parent: Node, size: Vector3, position: Vector3, material = null):
	var mesh_instance = MeshInstance3D.new()
	var mesh = BoxMesh.new()
	mesh.size = size
	mesh_instance.mesh = mesh
	mesh_instance.transform.origin = position
	if material:
		mesh_instance.material_override = material

	var body = StaticBody3D.new()
	var shape = CollisionShape3D.new()
	var col = BoxShape3D.new()
	col.size = size
	shape.shape = col
	body.add_child(shape)
	mesh_instance.add_child(body)
	parent.add_child(mesh_instance)


# Vertical columns
func _add_cylinder(parent: Node, position: Vector3, height: float, material = null):
	var mesh_instance = MeshInstance3D.new()
	var mesh = CylinderMesh.new()
	mesh.height = height
	mesh.top_radius = 1.0
	mesh.bottom_radius = 1.0
	mesh_instance.mesh = mesh
	mesh_instance.transform.origin = position
	if material:
		mesh_instance.material_override = material
	var body = StaticBody3D.new()
	var shape = CollisionShape3D.new()
	var capsule = CapsuleShape3D.new()
	capsule.height = height
	capsule.radius = 1.0
	shape.shape = capsule
	body.add_child(shape)
	mesh_instance.add_child(body)
	parent.add_child(mesh_instance)


# Sloped ramp (using BoxMesh)
func _add_ramp(parent: Node, position: Vector3, width: float, direction: float, material = null):
	var mesh_instance = MeshInstance3D.new()
	var mesh = BoxMesh.new()
	mesh.size = Vector3(width, 1, 10)
	mesh_instance.mesh = mesh
	mesh_instance.transform.origin = position
	mesh_instance.rotation.x = deg_to_rad(-30 if direction < 0 else 30)
	if material:
		mesh_instance.material_override = material

	var body = StaticBody3D.new()
	var shape = CollisionShape3D.new()
	var col = BoxShape3D.new()
	col.size = Vector3(width, 1, 10)
	shape.shape = col
	body.add_child(shape)
	mesh_instance.add_child(body)
	parent.add_child(mesh_instance)


# Simple stair stack
func _add_stairs(parent: Node, start: Vector3, steps: int):
	for i in range(steps):
		var height = float(i) + 1.0
		var pos = start + Vector3(0, height - 1, float(i) * 2)
		_add_box(parent, Vector3(3, 1, 2), pos, MAT_PLATFORM)


# Arena perimeter walls
func _create_walls(parent: Node, size: float, height: float):
	var wall_thickness = 1
	# Front/back
	_add_box(parent, Vector3(size, height, wall_thickness), Vector3(0, height/2, size/2), MAT_WALL)
	_add_box(parent, Vector3(size, height, wall_thickness), Vector3(0, height/2, -size/2), MAT_WALL)
	# Left/right
	_add_box(parent, Vector3(wall_thickness, height, size), Vector3(size/2, height/2, 0), MAT_WALL)
	_add_box(parent, Vector3(wall_thickness, height, size), Vector3(-size/2, height/2, 0), MAT_WALL)
