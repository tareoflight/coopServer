extends Node3D


var pipe_path = "\\\\.\\pipe\\sql_pipe"
var map_node_scene = preload("res://MapNodes/MapNode.tscn")

func send_sql_query(query: String) -> String:
	var result = []
	var exit_code = OS.execute("cmd", ["/c", "echo " + query + " > " + pipe_path], result, true)
	
	if exit_code != 0:
		push_error("Failed to send SQL query")
		return ""

	var response = []
	exit_code = OS.execute("cmd", ["/c", "type " + pipe_path], response, true)
	
	return response[0] if response.size() > 0 else ""

func _ready():
	spawn_map_nodes()

func spawn_map_nodes():
	var result = send_sql_query("SELECT x, y, z FROM map;")
	
	if result.is_empty():
		push_error("No data returned from SQL query.")
		return ""
	
	var locations = result.split("\n")  # In case multiple rows are returned
	
	for loc in locations:
		var coords = loc.split(",")
		if coords.size() == 3:
			var x = coords[0].to_float()
			var y = coords[1].to_float()
			var z = coords[2].to_float()
			
			var map_node = map_node_scene.instantiate()
			map_node.position = Vector3(x, y, z)  # Adjust according to your node type
			map_node.set_contents("air",0.5)
			map_node.set_contents("ground",0.5)
			
			add_child(map_node)  # Add to the current scene
			
			print("Spawned MapNode at: ", Vector3(x, y, z))
