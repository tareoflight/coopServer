extends State

func enter():
	print("Running")

func update(delta):
	var input_dir := Input.get_vector("left", "right", "forward", "backword")
	print(input_dir)
	user.stats.direction = lerp(user.stats.direction ,Vector3(input_dir.x, 0, input_dir.y),delta*user.stats.lerp_speed)
	if user.stats.direction:
		user.velocity.x = user.stats.direction.x * user.stats.dex
		user.velocity.z = user.stats.direction.z * user.stats.dex
	user.move_and_slide()
	
	if input_dir.is_zero_approx():
		state_machine.change_state("Idle")
