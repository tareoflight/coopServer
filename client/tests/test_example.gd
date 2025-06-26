extends Node

func _ready():
	print("-----------------UNIT TESTS-------------")

	var tests = {
		"send_request": send_request,
		"get_response": get_response
	}

	for name in tests.keys():
		print("Running test:", name)
		tests[name].call()
	

func send_request():
	# TODO: Implement mock test
	print("send_request test skipped.")

func get_response():
	# TODO: Implement mock test
	print("get_response test skipped.")
