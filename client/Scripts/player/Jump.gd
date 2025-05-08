extends State

func enter():
	print("Jumping")

func update(delta):
	#	air control
	var input_dir := Input.get_vector("left", "right", "forward", "backword")
	
	
	
	if user.is_on_floor():
		user.velocity.y = user.stats.dex_jumpstr
		user.stats.jumptimer = 0
		user.stats.direction = lerp(user.stats.direction ,Vector3(input_dir.x, 0, input_dir.y),delta*user.stats.lerp_speed)

	elif not user.is_on_floor():
		user.stats.jumptimer += delta
		user.velocity += user.get_gravity() * delta

	if user.stats.direction:
		user.velocity.x = user.stats.direction.x * user.stats.dex_speed
		user.velocity.z = user.stats.direction.z * user.stats.dex_speed


	user.move_and_slide()
	
	if user.is_on_floor():
		state_machine.change_state("Idle")
