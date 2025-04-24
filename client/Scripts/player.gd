extends CharacterBody3D

@onready var neck: Node3D = $neck
@onready var head: Node3D = $neck/head
@onready var eyes: Camera3D = $neck/head/Camera3D
@onready var crouch_collider: CollisionShape3D = $crouch_collider
@onready var standing_collider: CollisionShape3D = $standing_collider
@onready var can_stand: RayCast3D = $can_stand

var current_speed = 5.0

var walk_speed = 5.0
var sprint_speed_mod = 1.4
var crouch_speed_mod = 0.6
var crouch_depth = -0.5

# States
enum movement_states {
	WALKING,
	SPRINTING,
	CROUCHING,
	SLIDING,
	SCUTLEING
}
var current_movement_state = movement_states.WALKING
enum looking_states {
	FREE,
	NORMAL
}
var current_looking_state = looking_states.NORMAL

var slide_timer = 0.0
var slide_timer_max = 1.0
var slide_vector = Vector2.ZERO

var mouse_sens = 0.25

var jump_velocity = 7
var direction = Vector3.ZERO

var lerp_speed = 20.0
var free_look_tilt = 3

func _ready():
	Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

func _input(event):
	if event.is_action_pressed("action"):
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	if event.is_action_pressed("ui_cancel"):
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	if event is InputEventMouseMotion:
		if Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED:
			if current_looking_state == looking_states.FREE:
				neck.rotate_y(-deg_to_rad(event.relative.x * mouse_sens))
				neck.rotation.y = clamp(neck.rotation.y,deg_to_rad(-120),deg_to_rad(120))
			else:
				rotate_y(-deg_to_rad(event.relative.x * mouse_sens))
				head.rotate_x(-deg_to_rad(event.relative.y * mouse_sens))
				head.rotation.x = clamp(head.rotation.x,deg_to_rad(-89),deg_to_rad(89))

func _physics_process(delta: float):
	var input_dir := Input.get_vector("left", "right", "forward", "backword")
	
	current_speed = walk_speed
	#state setup
	print("ms: ",current_movement_state,"\t\tls:",current_movement_state)

	if Input.is_action_pressed("crouch"):
		match current_movement_state:
			movement_states.WALKING:
				current_movement_state = movement_states.CROUCHING
			movement_states.SPRINTING:
				current_movement_state = movement_states.SLIDING
	else:
		match current_movement_state:
			movement_states.CROUCHING:
				if !can_stand.is_colliding():
					current_movement_state = movement_states.WALKING
			movement_states.SLIDING:
				current_movement_state = movement_states.SPRINTING

	if Input.is_action_pressed("sprint"):
		match current_movement_state:
			movement_states.WALKING:
				current_movement_state = movement_states.SPRINTING
			movement_states.CROUCHING:
				current_movement_state = movement_states.SCUTLEING
	else:
		match current_movement_state:
			movement_states.SPRINTING:
				current_movement_state = movement_states.WALKING
			movement_states.SCUTLEING:
				current_movement_state = movement_states.CROUCHING
				
	if Input.is_action_pressed("free_look"):
		match current_looking_state:
			looking_states.NORMAL:
				current_looking_state = looking_states.FREE
	else:
		current_looking_state = looking_states.NORMAL


			
	
	
	
	
	
	
	
	
	
	
	
	
	if Input.is_action_pressed("crouch"):
		current_speed = current_speed * crouch_speed_mod
		head.position.y = lerp(head.position.y,0.1+crouch_depth,delta*lerp_speed)
		standing_collider.disabled = true
		crouch_collider.disabled = false
		
		#walking = false
		#if sprinting && input_dir != Vector2.ZERO && !sliding:
			#sliding = true
			#free_looking = true
			#slide_timer = slide_timer_max
			#slide_vector = input_dir
			#print("sliding+")
		#else:
			#crouching = true
			#scutleing = false
		
		
	elif !can_stand.is_colliding():
		standing_collider.disabled = false
		crouch_collider.disabled = true
		head.position.y = lerp(head.position.y,0.1,delta*lerp_speed)
		
		#walking = true
		

	if Input.is_action_pressed("sprint"):
		#walking = false
		current_speed = sprint_speed_mod*current_speed
		#if crouching and !sliding:
			#crouching = false
			#scutleing = true
		#else:
			#scutleing = false
			#sprinting = true

	#free looking
	if Input.is_action_pressed("free_look"):
		
		#free_looking = true
		eyes.rotation.z = -deg_to_rad(neck.rotation.y *free_look_tilt)
	else:
		#free_looking = false
		eyes.rotation.z = lerp(eyes.rotation.z,0.0,delta*lerp_speed)
		neck.rotation.y = lerp(neck.rotation.y,0.0,delta*lerp_speed)
	
	#sliding
	#if sliding:
		#slide_timer -= delta
		#if slide_timer <= 0:
			##sliding = false
			#print("sliding-")
			##free_looking=false

	# Add the gravity.
	if not is_on_floor():
		velocity += get_gravity() * delta

	# Handle jump.
	if Input.is_action_just_pressed("ui_accept") and is_on_floor():
		velocity.y = jump_velocity

	# handle the movement/deceleration.

	direction = lerp(direction,(transform.basis * Vector3(input_dir.x, 0, input_dir.y)).normalized(),delta*lerp_speed)
	#if sliding:
		#direction = (transform.basis *Vector3(slide_vector.x,0.0,slide_vector.y)).normalized()
		
	if direction:
		velocity.x = direction.x * current_speed
		velocity.z = direction.z * current_speed
		
		#if sliding:
			#velocity.x = direction.x * slide_timer
			#velocity.z = direction.z * slide_timer
	else:
		velocity.x = move_toward(velocity.x, 0, current_speed)
		velocity.z = move_toward(velocity.z, 0, current_speed)

	move_and_slide()
