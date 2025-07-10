extends Node

var client := StreamPeerTCP.new()
var reponce := preload("res://Scripts/grpc/comms.gd")

func _ready():
	var err = client.connect_to_host("127.0.0.1", 25569)
	if err == OK:
		print("Connecting to server...")
	else:
		print("Connection failed: ", err)
		
	print("-----------------UNIT TESTS-------------")

	var tests = {
		"send_request": send_request,
		"get_response": get_response
	}

	for name in tests.keys():
		print("Running test:", name)
		tests[name].call()
	

func connect_on(port : int = 25569):
	
	pass


func send_request():
	while client.get_status() == StreamPeerTCP.STATUS_CONNECTING:
		client.poll()
		print("connecting")
		await get_tree().create_timer(1).timeout
		
		
	if client.get_status() == StreamPeerTCP.STATUS_CONNECTED:
		# Example: Send a message
		var message = reponce.Request.new()
		message.set_request_id(1)
		message.new_control_request()
		message.get_control_request().new_shutdown()
		message.get_control_request().get_shutdown().set_delay(1)
		var data = message.to_bytes()
		
		print("--------------------")
		print(data)
		print(message)
		print("+++++++++++++++++++")
		client.put_32(len(data))
		client.put_data(data)
		# Try to read response (non-blocking)
		if client.get_available_bytes() > 0:
			var received = client.get_utf8_string(client.get_available_bytes())
			print("Received from server: ", received)
	else:
		print("not connected",client.get_status())
		
func get_response():
	# TODO: Implement mock test
	print("get_response test skipped.")
