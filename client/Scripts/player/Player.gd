extends CharacterBody3D


@export var stats: Stats

@onready var standing_collider: CollisionShape3D = $standing_collider
@onready var crouch_collider: CollisionShape3D = $crouch_collider
@onready var can_stand: RayCast3D = $can_stand
@onready var neck: Node3D = $neck
@onready var head: Node3D = $neck/head
@onready var camera_3d: Camera3D = $neck/head/Camera3D


var state_machine

func _ready():
	state_machine = $StateMachine

	state_machine.init(self)

func _physics_process(delta):
	state_machine.update(delta)
	
	
	
