extends Node

var tests = [
	preload("res://tests/test_example.gd"),
	#preload("res://tests/test_inventory.gd"),
	# Add more test files here
]

func _ready():
	print("Running tests...")
	for test in tests:
		var test_instance = test.new()
		add_child(test_instance)
