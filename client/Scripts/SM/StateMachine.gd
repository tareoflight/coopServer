extends Node

var current_state: State
var states = {}

func init(user):
	# Register states
	for child in get_children():
		if child is State:
			child.user = user
			child.state_machine = self
			states[child.name] = child

	change_state("Idle")
	
func change_state(new_state_name):
	if current_state:
		current_state.exit()
	current_state = states.get(new_state_name)
	current_state.enter()

func update(delta):
	if current_state:
		current_state.update(delta)
 
