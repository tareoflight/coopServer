extends Node
var url = "127.0.0.1"
var port = 25569
var sender := StreamPeerTCP.new()
var recever := StreamPeerTCP.new()
var rpc := preload("res://Scripts/grpc/comms.gd")
var rb = preload("res://Scripts/grpc/RequestBuilder.gd")

### a pause
func wait_seconds(seconds: float) -> void:
	var start_time = Time.get_ticks_msec()
	while Time.get_ticks_msec() - start_time < seconds * 1000:
		await Engine.get_main_loop().process_frame  # yields per frame

#### sub to to event stream
func new_connection():
	var err = sender.connect_to_host(url, port)
	if err == OK:
		while sender.get_status() == StreamPeerTCP.STATUS_CONNECTING:
			sender.poll()
			await wait_seconds(1.0)
	else:
		print("Connection failed: ", err)
	# err = recever.connect_to_host(url, port+1)
	# if err == OK:
	# 	while recever.get_status() == StreamPeerTCP.STATUS_CONNECTING:
	# 		recever.poll()
	# else:
	# 	print("Connection failed: ", err)
	print("Connected to ",sender.get_connected_host())
	

#### send request
func send(data: PackedByteArray):
	sender.put_32(data.size())
	sender.put_data(data)

func request_builder() -> RequestBuilder:
	return RequestBuilder.new(self)

func shutdown(delay=1) -> RequestBuilder:
	var r = rpc.Request.new()
	r.new_control_request()
	r.get_control_request().new_shutdown()
	r.get_control_request().get_shutdown().set_delay(delay)
	return request_builder().with_data(r.to_bytes())

func ping() -> RequestBuilder:
	var r = rpc.Request.new()
	r.new_control_request()
	r.get_control_request().new_ping()
	return request_builder().with_data(r.to_bytes())

func reboot() -> RequestBuilder:
	var r = rpc.Request.new()
	r.new_control_request()
	r.get_control_request().new_reboot()
	return request_builder().with_data(r.to_bytes())
