extends State

func enter():
	print("Running")

func update(delta):
	var input_dir := Input.get_vector("left", "right", "forward", "backword")
	user.stats.direction = lerp(user.stats.direction ,Vector3(input_dir.x, 0, input_dir.y),delta*user.stats.lerp_speed)
	if user.stats.direction and user.stats.con_stamina >0 and Input.is_action_pressed("sprint"):
		user.velocity.x = user.stats.direction.x * user.stats.dex_speed * user.stats.dex_sprintMod
		user.velocity.z = user.stats.direction.z * user.stats.dex_speed * user.stats.dex_sprintMod
		user.stats.con_stamina -= delta
	else:
		user.velocity.x = user.stats.direction.x * user.stats.dex_speed
		user.velocity.z = user.stats.direction.z * user.stats.dex_speed
		
	user.move_and_slide()
	
	if input_dir.is_zero_approx():
		state_machine.change_state("Idle")

	if Input.is_action_pressed("jump"):
		state_machine.change_state("Jump")
	
	if not user.is_on_floor():
		state_machine.change_state("Jump")
