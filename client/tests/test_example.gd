extends Node

var rpc := preload("res://Scripts/grpc/common.gd")
var stream = rpc.new()

func _ready():
	
	await stream.new_connection()

	var tests = {
		"send_request": send_request,
		"get_response": get_response
	}

	for name in tests.keys():
		print("Running test:", name)
		tests[name].call()
	

func _process(delta):
	if stream.recever.get_available_bytes() > 0:
		var data = stream.recever.get_utf8_string(stream.recever.get_available_bytes())
		print("Got stream data:", data)

func send_request():
	print("sending shutdown")
	stream.shutdown(15).send()
	print("sent shutdown")
	
		
func get_response():
	# TODO: Implement mock test
	print("get_response test skipped.")
