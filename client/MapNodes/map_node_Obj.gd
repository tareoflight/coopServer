extends Node3D
var gridsize = 10
#pk
var loc = Vector3i.ZERO
var chunckID = Vector2i.ZERO

var render_loc = Vector3.ZERO
var neighbors: Array = []
var contents = []

var state = 0
var statename = ["vac","solid","sand","ground","liquid","carved river","beach","lake","gass","stone face","dry sand","dry ground","fluid","stream","mobile","mix"]
var gass = 0
var liquid = 0
var sand = 0
var solid = 0

func init() -> void:
	loc = Vector3i(roundi(global_position.x),
				roundi(global_position.y),
				roundi(global_position.z))
	name = str("(",loc.x,",",loc.y,",",loc.z,")")
	contents.resize(256)
	for i in range(256):
		contents[i] = 0  
	contents[0]= 255

func step() -> void:
	update_state()
	render_loc = Vector3(loc.x+0.0,loc.y+get_air_offset()*gridsize,loc.z+0.0)
	


func pass_content(content) -> Array:
	var results: Array = []
	var volToRemove = content.vol
	var i  = 0
	var tempcontet = {}
	while volToRemove > 0:
		tempcontet = contents[i].duplicate()
		if volToRemove <= contents[i].vol:
			tempcontet.vol = volToRemove
			results.append(tempcontet)
			contents[i].vol -= volToRemove
			volToRemove = 0
		else:
			results.append(tempcontet)
			volToRemove -= contents[i].vol
			contents[i].vol = 0
		i += 1
	return results

func set_contents(new_contents) -> void:
	contents = new_contents

func get_air_offset() -> float:
	var results = 0.0
	var clear = (gass+liquid+0.0)
	var ground = (gass+liquid+sand+solid+0.0)
	if ground == 0:
		results = 0.0
	else:
		results = clear/ground
	return -results

func set_nab(nabs) -> void:
	neighbors = nabs

func update_state() -> void:
	gass = 0
	liquid = 0
	sand = 0
	solid = 0
	for i in range(256):
		if i < 64:
			gass += contents[i]
		elif i < 128:
			liquid += contents[i]
		elif i < 192:
			sand += contents[i]
		else:
			solid += contents[i]

	#----15 states------------------
	# g l s S o
	# 0 0 0 0 vac
	# 0 0 0 1 solid
	# 0 0 1 0 sand
	# 0 0 1 1 ground
	# 0 1 0 0 liquid
	# 0 1 0 1 carved river
	# 0 1 1 0 beach
	# 0 1 1 1 lake
	# 1 0 0 0 gass
	# 1 0 0 1 stone face
	# 1 0 1 0 dry sand
	# 1 0 1 1 dry ground
	# 1 1 0 0 fluid
	# 1 1 0 1 stream
	# 1 1 1 0 mobile
	# 1 1 1 1 mix

	# if state >4 && !8 then node for render
	# if state <4 && nab.UP.state == 0,4,8,12 then node for render

	var resuting_state = 0
	if gass > 0:
		resuting_state += 8
	if liquid > 0:
		resuting_state += 4
	if sand > 0: 
		resuting_state += 2
	if solid > 0:
		resuting_state += 1
	state = resuting_state


func update_draw() -> bool:
	step()

	return true
