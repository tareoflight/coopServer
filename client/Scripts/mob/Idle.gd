extends State

func enter():
	print("Entered Idle state")
	calculate_stats()
	
func calculate_stats():
	#str
	var str = user.stats.str
	user.stats.str_maxLoad =  str * 3
	user.stats.str_minLoad = str * 1.5
	user.stats.str_baseMinDamg = minf(((str - 10) / 2),0.0)
	
	#dex
	var dex = user.stats.dex
	user.stats.dex_speed = dex
	user.stats.dex_sprintMod = maxf((1+((dex - 10) / 2)/5),1.2)
	user.stats.dex_jumpstr = user.stats.dex
	
	#con
	var con = user.stats.con
	user.stats.con_maxHealth = ((con - 10) / 2) + 5
	user.stats.con_maxStamina = ((con - 10) / 2) + 5
	user.stats.con_stamina = con
	user.stats.con_regen = minf(((dex - 10) / 2)/5,0.0)

	

func update(delta):
	user.velocity = lerp(user.velocity,Vector3.ZERO,user.stats.lerp_speed*delta)
	if user.stats.current_health < user.stats.con_maxHealth:
		user.stats.current_health = user.stats.current_health + (user.stats.con_regen*delta)
	elif user.stats.current_health > user.stats.con_maxHealth:
		user.stats.current_health = user.stats.con_maxHealth
	
	if user.stats.current_stamina < user.stats.con_maxStamina:
		user.stats.current_stamina = user.stats.current_stamina + (user.stats.con_regen*delta)
	elif user.stats.current_stamina > user.stats.con_maxStamina:
		user.stats.current_stamina = user.stats.con_maxStamina
	
	var input_dir := Input.get_vector("left", "right", "forward", "backword")
	if !input_dir.is_zero_approx():
		state_machine.change_state("Run")
	
	if Input.is_action_pressed("jump") and user.is_on_floor():
		state_machine.change_state("Jump")
	
