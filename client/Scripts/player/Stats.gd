extends Resource
class_name Stats
@export_group("Char Stats")
@export var str:float=8.0
@export_subgroup("str","str_")
@export var str_maxLoad: float = 0.0
@export var str_minLoad: float = 0.0
@export var str_baseMinDamg: float = 0.0

@export var dex:float=8.0
@export_subgroup("dex","dex_")
@export var dex_speed: float = 0.0
@export var dex_sprintMod: float = 0.0
@export var dex_jumpstr: float = 0.0
@export var dex_aircontrol:float = 0.5


@export var con:float=8.0
@export_subgroup("con","con_")
@export var con_maxHealth: float = 0.0
@export var con_maxStamina: float = 0.0
@export var con_stamina: float = 0.0
@export var con_regen: float = 0.0

@export var intl:float=8.0
@export var wis:float=8.0
@export var cha:float=8.0



@export_group("Char vars")
@export var current_health: float = 0.0
@export var current_stamina: float = 0.0
@export var current_speed: float = 0.0
@export var direction = Vector3.ZERO
@export var velocity = Vector3.ZERO
@export var gravity: float = -5
@export var jumpMaxtimer: float = 1
@export var jumptimer: float = 0

@export var load:float = 5.0

@export_group("Char others")
@export var lerp_speed:float = 15.0
