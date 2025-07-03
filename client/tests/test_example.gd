extends Node

var client := StreamPeerTCP.new()
var reponce := preload("res://Scripts/grpc/response.gd")

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
	if client.get_status() == StreamPeerTCP.STATUS_CONNECTED:
		# Example: Send a message
		var message = reponce.new()
		
		var data = message.send_request()
		client.put_data(data)

		# Try to read response (non-blocking)
		if client.get_available_bytes() > 0:
			var received = client.get_utf8_string(client.get_available_bytes())
			print("Received from server: ", received)

func get_response():
	# TODO: Implement mock test
	print("get_response test skipped.")
