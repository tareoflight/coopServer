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
	

func _process(_delta):
	if stream.con.get_available_bytes() > 0:
		var size = stream.con.get_32()
		var data = stream.con.get_data(size)[1]
		var event = stream.event(data)
		if event.has_heartbeat_event():
			print("heartbeat ",data)
				

func send_request():
	#print("sending shutdown")
	#stream.shutdown(15).send()
	#print("sent shutdown")
	pass
	
		
func get_response():
	# TODO: Implement mock test
	print("get_response test skipped.")
