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
	# Check if the database exists (this is just a placeholder; replace it with actual DB existence check)
	if not connect_to_server():  
		print("Database not found. Loading test data...")
		load_test_data()
		return
	
	var result = send_sql_query("SELECT x, y, z FROM map;")
	
	if result.is_empty():
		print("No data in the database. Loading test data...")
		load_test_data()
		return
	
	var locations = result.split("\n")  # In case multiple rows are returned
	
	for loc in locations:
		var coords = loc.split(",")
		if coords.size() == 3:
			pass #spawn_map_node(coords[0].to_float(), coords[1].to_float(), coords[2].to_float())


func spawn_map_node(x: float, y: float, z: float, air: float, ground: float):
	var map_node = map_node_scene.instantiate()
	add_child(map_node)

	map_node.position = Vector3(x, y, z)
	map_node.set_contents("air", air)
	map_node.set_contents("ground", ground)

	map_node.update_draw()
	print("Spawned MapNode at: ", Vector3(x, y, z))

func load_test_data():
	var test_data = [
		{"pos": Vector3(0, 0, 0), "air":0, "ground":1},
		{"pos": Vector3(0, 0, 1), "air":0, "ground":1},
		{"pos": Vector3(0, 1, 0), "air":0.9, "ground":0.1},
		{"pos": Vector3(0, 1, 1), "air":0.5, "ground":0.5},
		{"pos": Vector3(1, 0, 0), "air":0.5, "ground":0.5},
		{"pos": Vector3(1, 0, 1), "air":0.3, "ground":0.7},
		{"pos": Vector3(1, 1, 0), "air":1, "ground":0},
		{"pos": Vector3(1, 1, 1), "air":1, "ground":0},

	]
	for data in test_data:
		spawn_map_node(data["pos"].x*10, data["pos"].y*10, data["pos"].z*10, data["air"], data["ground"])
