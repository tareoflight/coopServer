extends CharacterBody3D

@onready var neck: Node3D = $neck
@onready var head: Node3D = $neck/head
@onready var eyes: Camera3D = $neck/head/Camera3D
@onready var crouch_collider: CollisionShape3D = $crouch_collider
@onready var standing_collider: CollisionShape3D = $standing_collider
@onready var can_stand: RayCast3D = $can_stand

const base_speed = 5.0
var current_speed = 5.0
const base_jump_velocity = 4.5
const mouse_sens = 0.4

var sprint_mod = 1.3

var crouch_mod = 0.6

var lerp_speed = 10.0
var lerp_air_speed = 0.8
var direction = Vector3.ZERO

func _ready() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _input(event):
	if event.is_action_pressed("action"):
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	if event.is_action_pressed("ui_cancel"):
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
			rotate_y(deg_to_rad(-event.relative.x*mouse_sens))
			head.rotate_x(deg_to_rad(-event.relative.y*mouse_sens))
			head.rotation.x = clamp(head.rotation.x,deg_to_rad(-89),deg_to_rad(89))



func _physics_process(delta: float) -> void:
	
	var current_jv = base_jump_velocity
	var current_lerp_speed = lerp_speed
	
	if Input.is_action_pressed("crouch"):
		current_speed = base_speed * crouch_mod
		neck.position.y = lerp(neck.position.y,crouch_collider.position.y*2,lerp_speed*delta)
		standing_collider.disabled=true
		crouch_collider.disabled=false
	elif !can_stand.is_colliding():
		standing_collider.disabled=false
		crouch_collider.disabled=true
		neck.position.y = lerp(neck.position.y,standing_collider.position.y*2,lerp_speed*delta)
		if Input.is_action_pressed("sprint"):
			current_speed = base_speed * sprint_mod
		else:
			current_speed = base_speed
	
		
	
	
	
	
	#	gravity !!
	if not is_on_floor():
		current_lerp_speed = lerp_air_speed
		velocity += get_gravity() * delta
	else:
		current_lerp_speed = lerp_speed


	# Handle jump.
	if Input.is_action_just_pressed("jump") and is_on_floor():
		velocity.y = current_jv


	var input_dir := Input.get_vector("left", "right", "forward", "backword")
	direction = lerp(direction,(transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized(),current_lerp_speed*delta)
	
	if direction:
		velocity.x = direction.x * current_speed
		velocity.z = direction.z * current_speed
	else:
		velocity.x = move_toward(velocity.x, 0, current_speed)
		velocity.z = move_toward(velocity.z, 0, current_speed)

	move_and_slide()
