extends Node3D


var map_node_scene = preload("res://MapNodes/MapNode.tscn")

#db stuff
var host = "127.0.0.1"
var port = 25569 #add logic later
var client = StreamPeerTCP.new()

func	connect_to_server() -> bool:
	var e = client.connect_to_host(host,port)
	if e != OK:
		push_error("NO db to connect too")
		return false
	return true

func send_sql_query(query: String) -> String:
	if not connect_to_server():
		return ""
	client.put_data(query.to_utf8_buffer())
	var r = ""
	while client.get_available_bytes() >0:
		r += client.get_string(client.get_available_bytes())
	client.disconnect_from_host()
	return r

func _ready():
	spawn_map_nodes()

func spawn_map_nodes():
	load_test_data()


func spawn_map_node(x: int, y: int, z: int, content: Array):
	var map_node = map_node_scene.instantiate()
	add_child(map_node)

	map_node.position = Vector3i(x*map_node.gridsize, y*map_node.gridsize, z*map_node.gridsize)
	map_node.init()
	map_node.set_contents(content)

	map_node.update_draw()
	print("Spawned a ",map_node.statename[map_node.state]," MapNode at: ",map_node.render_loc)






func load_test_data():

	var test_data = [
		{"pos": Vector3i(0, 0, 0), "content":[]},
		{"pos": Vector3i(0, 0, 1), "content":[]},
		{"pos": Vector3i(0, 0, 2), "content":[]},
		{"pos": Vector3i(1, 0, 0), "content":[]},
		{"pos": Vector3i(1, 0, 1), "content":[]},
		{"pos": Vector3i(1, 0, 2), "content":[]},
		{"pos": Vector3i(2, 0, 0), "content":[]},
		{"pos": Vector3i(2, 0, 1), "content":[]},
		{"pos": Vector3i(2, 0, 2), "content":[]},

		{"pos": Vector3i(0, 1, 0), "content":[]},
		{"pos": Vector3i(0, 1, 1), "content":[]},
		{"pos": Vector3i(0, 1, 2), "content":[]},
		{"pos": Vector3i(1, 1, 0), "content":[]},
		{"pos": Vector3i(1, 1, 1), "content":[]},
		{"pos": Vector3i(1, 1, 2), "content":[]},
		{"pos": Vector3i(2, 1, 0), "content":[]},
		{"pos": Vector3i(2, 1, 1), "content":[]},
		{"pos": Vector3i(2, 1, 2), "content":[]},

		{"pos": Vector3i(0, 2, 0), "content":[]},
		{"pos": Vector3i(0, 2, 1), "content":[]},
		{"pos": Vector3i(0, 2, 2), "content":[]},
		{"pos": Vector3i(1, 2, 0), "content":[]},
		{"pos": Vector3i(1, 2, 1), "content":[]},
		{"pos": Vector3i(1, 2, 2), "content":[]},
		{"pos": Vector3i(2, 2, 0), "content":[]},
		{"pos": Vector3i(2, 2, 1), "content":[]},
		{"pos": Vector3i(2, 2, 2), "content":[]},
	]
	var arr = []
	arr.resize(256)
	for i in range(256):
		arr[i]=0
	for i in range(test_data.size()):
		var b = arr.duplicate()
		if i < 8:
			b[0] = 0
			b[200] = 255
			test_data[i]["content"] = b
		elif i > 15:
			b[0] = 255
			test_data[i]["content"] = b
		else:
			var l = i - 7
			b[0] = ((l+0.0) / 8) * 255
			b[150] = 255 - b[0]
			test_data[i]["content"] = b

	for data in test_data:
		spawn_map_node(data["pos"].x, data["pos"].y, data["pos"].z, data["content"])
