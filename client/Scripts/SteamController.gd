extends Control


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	Steam.steamInit()
	
	var steamRunning = Steam.isSteamRunning()
	if !steamRunning:
		print("steam not running")
		return
		
		
	var uid = Steam.getSteamID()
	var name = Steam.getFriendPersonaName(uid)
	
	print("hello, "+ name)
		
		
		
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
