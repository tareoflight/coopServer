extends Control

var ingame = false

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$VBoxContainer/StartButton.grab_focus()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func _on_start_button_pressed() -> void:
	if !ingame:
		get_tree().change_scene_to_file("res://scenes/tester.tscn")
	else:
		get_parent_control().remove_child($".")
		#close the menu
	


func _on_options_button_pressed() -> void:
	var opt = load("res://scenes/Menus/Options.tscn").instance()
	get_tree().current_scene.add_child(opt)


func _on_quit_button_pressed() -> void:
	pass # Replace with function body.
